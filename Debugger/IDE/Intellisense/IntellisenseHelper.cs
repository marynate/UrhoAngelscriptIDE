using ICSharpCode.AvalonEdit.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace Debugger.IDE.Intellisense {

    /// <summary>
    /// Utility functions for making assessments of the current text state in AvalonEdit
    /// </summary>
    public static class IntellisenseHelper {
        
        /// <summary>
        /// is the current caret to the right of specific character, = or . typically
        /// </summary>
        /// <param name="aCharCode">Character we wnat to know if it's on the left</param>
        /// <param name="doc">the textdocument being scanned</param>
        /// <param name="offset">caret offset</param>
        /// <param name="line">current line</param>
        public static bool OnRightSideOf(char aCharCode, TextDocument doc, int offset, int line) {
            int StartLine = doc.Lines[line-1].Offset;
            for (int i = offset; i > StartLine; --i) {
                if (doc.Text[i] == aCharCode)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// is the current care to the right of a specific word, such as "new"
        /// </summary>
        /// <param name="aWord">the word that we're checking to see if is to the left</param>
        public static bool OnRighSideOf(string aWord, TextDocument doc, int offset, int line) {
            int StartLine = doc.Lines[line-1].Offset;
            string str = doc.Text.Substring(StartLine, offset);
            string[] parts = str.Split(' ', '.', '=', '*', '/', '%', '+', '-', '(');
            if (parts.Length > 1) {
                if (parts[parts.Length - 1].ToLower().Equals(aWord))
                    return true;
            }
            return false;
        }

        public static bool ResemblesDotPath(TextDocument doc, int offset, int line) {
            int StartLine = doc.Lines[line].Offset;
            int dotsHit = 0;
            for (int i = offset; i > StartLine; --i) {
                char c = doc.Text[i];
                if (c == '.')
                    ++dotsHit;
                else if (c == ':')
                    ++dotsHit;
                else if (!Char.IsLetterOrDigit(c))
                    break;
            }
            return dotsHit > 0;
        }

        public static string[] ExtractPath(TextDocument doc, int offset, int line, out bool isFunction) {
            isFunction = false;

            int StartLine = doc.Lines[line-1].Offset;
            int Endline = doc.Lines[line-1].EndOffset;
            int pt = offset;
            int dotsHit = 0;
            for (; pt > StartLine; --pt) {
                if (doc.Text[pt] == '.')
                    ++dotsHit;
                else if (!Char.IsLetter(doc.Text[pt])) {
                    break;
                }
            }
            for (; offset < Endline; ++offset) {
                if (!Char.IsLetter(doc.Text[offset])) {
                    if (doc.Text[offset] == '(')
                        isFunction = true;
                    break;
                }
            }
            try {
                string path = doc.Text.Substring(pt + 1, offset - pt - 1);
                return path.Split('.');
            }
            catch (Exception ex) {
                return null;
            }
        }

        //get the object.property.something path if possible
        public static string[] DotPath(TextDocument doc, int offset, int line) {
            int StartLine = doc.Lines[line].Offset;
            int pt = offset;
            int dotsHit = 0;
            int braceDepth = 0;
            int funcDepth = 0;
            for (; pt > StartLine; --pt) {
                if (doc.Text[pt] == '.' || doc.Text[pt] == ':')
                    ++dotsHit;
                else if (doc.Text[pt] == ')')
                    ++funcDepth;
                else if (doc.Text[pt] == '(' && funcDepth > 0)
                    --funcDepth;
                else if (doc.Text[pt] == ']')
                    ++braceDepth;
                else if (doc.Text[pt] == '[' && braceDepth > 0)
                    --braceDepth;
                else if (!Char.IsLetterOrDigit(doc.Text[pt]) && braceDepth == 0 && funcDepth == 0)
                    break;
            }
            
            string path = doc.Text.Substring(pt + 1, offset - pt).Replace("::",".");
            return path.Split('.');
        }

        public static int OffsetWordLeft(TextDocument doc, int offset, int line) {
            return TextUtilities.GetNextCaretPosition(doc, offset, LogicalDirection.Backward, CaretPositioningMode.WordStart);
        }

        //get the word to the left
        public static string WordLeft(TextDocument doc, int offset, int line) {
            int start = TextUtilities.GetNextCaretPosition(doc, offset, LogicalDirection.Backward, CaretPositioningMode.WordStart);
            if (start < offset)
                return doc.Text.Substring(start, offset - start);
            return null;
        }
        //is the caret in the first word on the line
        public static bool IsFirstWord(TextDocument doc, int offset, int line) {
            int StartLine = doc.Lines[line].Offset;
            bool hitSplitter = false;
            bool hitLetter = false;
            for (int i = offset; i > StartLine; --i) {
                if (hitLetter && !Char.IsLetterOrDigit(doc.Text[i])) {
                    hitSplitter = true;
                } else if (hitSplitter && hitLetter)
                    return false;
                else if (Char.IsLetterOrDigit(doc.Text[i]))
                    hitLetter = true;
            }
            return hitLetter;
        }
    }
}

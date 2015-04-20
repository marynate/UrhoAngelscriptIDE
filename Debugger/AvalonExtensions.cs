using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Debugger {

    /// <summary>
    /// Extension methods for AvalonEdit
    /// </summary>
    public static class AvalonExtensions {
        static readonly char[] BREAKCHARS_LEFT = { '(', ')', ':', ' ', ',', '!' };
        static readonly char[] BREAKCHARS_RIGHT = { '(', ')', ':', ' ', ',', '!', '.' };

        static bool StringContainsBreaks(string str, bool leftward)
        {
            for (int i = 0; i < (leftward ? BREAKCHARS_LEFT.Length : BREAKCHARS_RIGHT.Length); ++i)
                if (str.Contains((leftward ? BREAKCHARS_LEFT[i] : BREAKCHARS_RIGHT[i])))
                    return true;
            return false;
        }

        public static string GetWordUnderMouse(this TextDocument document, TextViewPosition position, bool cancelDot) {
            string wordHovered = string.Empty;
            int line = position.Line;
            int column = position.Column;
            int offset = document.GetOffset(line, column);
            if (offset >= document.TextLength)
                offset--;
            string textAtOffset = document.GetText(offset, 1);

            // Get text backward of the mouse position, until the first space
            while (!string.IsNullOrWhiteSpace(textAtOffset) && !StringContainsBreaks(textAtOffset, true))
            {
                wordHovered = textAtOffset + wordHovered;
                offset--;
                if (offset < 0)
                    break;
                textAtOffset = document.GetText(offset, 1);
            }

            // Get text forward the mouse position, until the first space
            offset = document.GetOffset(line, column);
            if (offset < document.TextLength - 1) {
                offset++;

                textAtOffset = document.GetText(offset, 1);
                while (!string.IsNullOrWhiteSpace(textAtOffset) && !StringContainsBreaks(textAtOffset, false)) {
                    wordHovered = wordHovered + textAtOffset;
                    offset++;
                    if (offset >= document.TextLength)
                        break;
                    textAtOffset = document.GetText(offset, 1);
                }
            }
            return wordHovered;
        }

        public static string GetWordBeforeDot(this TextEditor textEditor) {
            var wordBeforeDot = string.Empty;
            var caretPosition = textEditor.CaretOffset - 2;
            var lineOffset = textEditor.Document.GetOffset(textEditor.Document.GetLocation(caretPosition));
            string text = textEditor.Document.GetText(lineOffset, 1);

            // Get text backward of the mouse position, until the first space
            while (!string.IsNullOrWhiteSpace(text) && text.CompareTo(".") > 0) {
                wordBeforeDot = text + wordBeforeDot;
                if (caretPosition == 0)
                    break;
                lineOffset = textEditor.Document.GetOffset(textEditor.Document.GetLocation(--caretPosition));
                text = textEditor.Document.GetText(lineOffset, 1);
            }
            return wordBeforeDot;
        }
    }
}

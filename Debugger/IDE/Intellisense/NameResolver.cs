using ICSharpCode.AvalonEdit.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Debugger.IDE.Intellisense {

    //Works with DepthScanner data to start from current position,
    //scan lines upwards looking for lines containing the term
    public class NameResolver {
        static readonly char[] BREAKCHARS = { ' ', '.', ',', '*','/','%','(','{','}',')',';','=','[',']','\t','\r','\n'};
        DepthScanner scanner_;
        Globals globals_;

        public NameResolver(Globals globals, DepthScanner aScanner) {
            scanner_ = aScanner;
            globals_ = globals;
        }

        public void GetNameMatch(TextDocument aDoc, int line, string text, ref List<string> suggestions) {
            int depth = scanner_.GetBraceDepth(line);
            do {
                string[] lineCodes = aDoc.GetText(aDoc.Lines[line]).Split(BREAKCHARS);
                foreach (string lineCode in lineCodes) {
                    if (lineCode.Length > 3 && lineCode.StartsWith(text)) { //contains our current text
                        int startidx = lineCode.IndexOf(text);
                        if (!suggestions.Contains(lineCode))
                            suggestions.Add(lineCode);
                        //unroll to the start of the word
                        //for (; startidx > 0; --startidx) {
                        //    if (BREAKCHARS.Contains(lineCode[startidx])) {
                        //        ++startidx; //move forward 1
                        //        break;
                        //    }
                        //}
                        //int endidx = startidx;
                        //for (; endidx < lineCode.Length; ++endidx) {
                        //    if (BREAKCHARS.Contains(lineCode[endidx]))
                        //        break;
                        //    if (Char.IsLetter(lineCode[endidx]) || Char.IsDigit(lineCode[endidx])) {
                        //        ++endidx;
                        //    } else
                        //        break;
                        //}
                        //if (startidx < endidx && endidx < lineCode.Length) {
                        //    string word = lineCode.Substring(startidx, endidx - startidx);
                        //    if (!word.Equals(text)) {
                        //        if (!suggestions.Contains(word.Trim()))
                        //            suggestions.Add(word.Trim());
                        //    }
                        //}
                    }
                }
                --line;
            } while (depth > 0 && line > 0); //function depth may be on the first 0 scanning up, same with class def
        }

        public TypeInfo GetClassType(TextDocument aDoc, int line, string text) {
            if (globals_ == null)
                return null;
            --line; //subtract one
            int startLine = line;
            if (text.Equals("this")) { //easy case
                int depth = scanner_.GetBraceDepth(line);
                do {
                    string lineCode = aDoc.GetText(aDoc.Lines[line]);
                    if (lineCode.Contains("class ")) {
                        string[] parts = lineCode.Trim().Split(' ');
                        if (parts[0].Equals("shared") && globals_.Classes.ContainsKey(parts[2]))
                            return globals_.Classes[parts[2]];
                        else if (globals_.Classes.ContainsKey(parts[1]))
                            return globals_.Classes[parts[1]];
                        else
                            break;
                    }
                    depth = scanner_.GetBraceDepth(line);
                    --line;
                } while (depth > 0 && line > 0); //class def may be on last line

                //unkonwn class
                int curDepth = depth;
                string[] nameparts = aDoc.GetText(aDoc.Lines[line]).Trim().Split(' ');
                string className = "";
                if (nameparts[0].Equals("shared"))
                    className = nameparts[2];
                else
                    className = nameparts[3];
                //TODO get baseclasses
                TypeInfo tempType = new TypeInfo() { Name = className };
                ++line;
                do {
                    depth = scanner_.GetBraceDepth(line);
                    if (depth == curDepth+1) {
                        string lineCode = aDoc.GetText(aDoc.Lines[line]);
                        string[] words = aDoc.GetText(aDoc.Lines[line]).Trim().Split(' ');
                        if (words != null && words.Length > 1) {
                            if (words[1].Contains("(")) { //function
                                if (globals_.Classes.ContainsKey(words[0])) {

                                }
                            } else {
                                string rettype = FilterTypeName(words[0]);
                                string propname = FilterTypeName(words[1]);
                                if (globals_.Classes.ContainsKey(rettype)) {
                                    tempType.Properties[propname] = globals_.Classes[rettype];
                                }
                            }
                        }
                    }
                    ++line;
                } while (line < startLine);
                return tempType;
            }

            //SCOPE block for depth
            {
                int depth = scanner_.GetBraceDepth(line);
                bool indexType = false;
                if (text.Contains('[')) {
                    indexType = true;
                    text = text.Substring(0, text.IndexOf('['));
                }
                do {
                    string lineCode = aDoc.GetText(aDoc.Lines[line]).Trim();   
                    if (lineCode.Contains(text)) {
                        int endidx = lineCode.IndexOf(text);
                        int idx = endidx;
                        bool okay = true;
                        bool hitSpace = false;
                        while (idx > 0) { //scan backwards to find the typename
                            if (!char.IsLetter(lineCode[idx]) && lineCode[idx] != ' ' && lineCode[idx] != '@' && lineCode[idx] != '&') {
                                okay = false;
                                ++idx;
                                break;
                            }
                            if (!hitSpace && lineCode[idx] == ' ')
                                hitSpace = true;
                            else if (lineCode[idx] == ' ')
                            {
                                break;
                            }
                            --idx;
                        }
                        if (idx < 0) idx = 0;
                        string substr = endidx - idx > 0 ? FilterTypeName(lineCode.Substring(idx, endidx - idx).Trim()) : "";
                        if (substr.Length > 0) { //not empty
                            if (substr.StartsWith(">")) {//TEMPLATE DEFINITION
                                if (!indexType)
                                    substr = lineCode.Substring(0, lineCode.IndexOf('<'));
                                else {
                                    int start = lineCode.IndexOf('<');
                                    int end = lineCode.IndexOf('>');
                                    substr = lineCode.Substring(start+1, end - start - 1);
                                    substr = substr.Replace("@", "");
                                }
                            }
                            if (globals_.Classes.ContainsKey(substr)) {
                                //Found a class
                                return globals_.Classes[substr];
                            }
                        }
                    }
                    --line;
                    depth = scanner_.GetBraceDepth(line);
                } while (depth > 0 && line > 0);
                return null;
            }
        }

        string FilterTypeName(string input)
        {
            return input.Replace("@", "").Replace("&in", "").Replace("&", "").Replace(";","");
        }
    }
}

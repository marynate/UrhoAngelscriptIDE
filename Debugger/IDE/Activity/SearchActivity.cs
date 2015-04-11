using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Debugger.IDE.Activity {
    public class SearchActivity {
        public static void doSearch(string text, string path) {
            IDEView.inst().SearchResults.Clear();
            if (text.Trim().Length == 0)
                return;
            if (text.Contains(' '))
                text = "\"" + text + "\"";

            Thread thread = new Thread(delegate() {
                string dir = System.Reflection.Assembly.GetEntryAssembly().Location.Replace("\\Debugger.exe","");
                dir += "\\bin\\ts.exe";

                Process pi = new Process();
                pi.StartInfo.FileName = dir;
                pi.StartInfo.Arguments = string.Format("\"{0}\" /s /T {1}", path, text);
                pi.StartInfo.UseShellExecute = false;
                pi.StartInfo.RedirectStandardOutput = true;
                pi.StartInfo.CreateNoWindow = true;
                pi.Start();

                string str = "";

                while ((str = pi.StandardOutput.ReadLine()) != null) {
                    if (str.Contains("scanned") && str.Contains("files"))
                        continue;
                    if (str.Contains("(") && str.Contains(")")) {
                        string search = str.Substring(0, str.IndexOf('(') - 1);

                        int start = str.IndexOf('(');
                        int end = str.IndexOf(')');
                        string ct = str.Substring(start + 1, end - start);

                        IDEView.inst().Dispatcher.Invoke(delegate() {
                            IDEView.inst().SearchResults.Add(new SearchResult {
                                File = search.Replace(path, ""),
                                Text = str.Replace(path, "")
                            });
                        });
                    }
                }
            });
            thread.Start();
        }
    }
}

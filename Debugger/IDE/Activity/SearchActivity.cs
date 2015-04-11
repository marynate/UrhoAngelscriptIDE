using Debugger.Search;
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
                FileSearch search = new FileSearch(IDEView.inst().Dispatcher, IDEView.inst().SearchResults);
                search.ScanFolder(path, new string[] { text }, true);
            });
            thread.Start();
        }
    }
}

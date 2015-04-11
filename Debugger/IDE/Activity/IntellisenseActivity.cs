using Debugger.IDE.Intellisense;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Debugger.IDE.Activity {
    public class IntellisenseActivity {
        FileSystemWatcher watcher_;

        public IntellisenseActivity(string file) {
            watcher_ = new FileSystemWatcher(file);
            watcher_.NotifyFilter = NotifyFilters.LastWrite;
            watcher_.Changed += watcher__Changed;
            watcher_.Deleted += watcher__Deleted;
            watcher_.Created += watcher__Created;
            watcher_.EnableRaisingEvents = true;
        }

        void watcher__Created(object sender, FileSystemEventArgs e) {
            buildIDB();
        }

        void watcher__Deleted(object sender, FileSystemEventArgs e) {
            buildIDB();
        }

        void watcher__Changed(object sender, FileSystemEventArgs e) {
            buildIDB();
        }

        void buildIDB() {
            Thread thread = new Thread(delegate() {
                ASParser parser = new ASParser();
                Globals globals = new Globals();
                //??
                parser.ParseDumpFile(null, globals);
            });
            thread.Start();
        }
    }
}

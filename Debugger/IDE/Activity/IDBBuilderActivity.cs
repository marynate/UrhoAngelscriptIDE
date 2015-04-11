using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace Debugger.IDE.Activity {
    public class IDBBuilderActivity {
        static FileSystemWatcher watcher_;

        public static void BuildIntellisenseDatabase() {
            
            string dir = System.Reflection.Assembly.GetEntryAssembly().Location;
            dir = System.IO.Path.GetDirectoryName(dir);
            dir = System.IO.Path.Combine(dir, "bin");
            string file = System.IO.Path.Combine(dir, "dump.h");
            //dir += "\\bin\\dump.h";
            if (watcher_ == null) {
                watcher_ = new FileSystemWatcher(dir);
                watcher_.Changed += watcher__Changed;
                watcher_.EnableRaisingEvents = true;
            }
            Thread thread = new Thread(delegate() {
                Globals globs = new Globals();
                ASParser parser = new ASParser();
                StringReader rdr = new StringReader(File.ReadAllText(file));
                parser.ParseDumpFile(rdr, globs);
                MainWindow.inst().Dispatcher.Invoke(delegate() {
                    IDEProject.inst().GlobalTypes = globs;
                });
            });
            thread.Start();

        }

        static void watcher__Changed(object sender, FileSystemEventArgs e) {
            BuildIntellisenseDatabase();
        }
    }
}

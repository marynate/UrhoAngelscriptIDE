using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace Debugger.IDE.Activity {

    /// <summary>
    /// Builds the TypeInfo database either when started
    /// or when the filesystem watcher detects a change
    /// 
    /// The typesystem is considered to be volatile, and may be there one minute, and missing the next
    /// </summary>
    public class IDBBuilderActivity {
        static FileSystemWatcher watcher_;

        public static void BuildIntellisenseDatabase() {
            
            string dir = System.Reflection.Assembly.GetEntryAssembly().Location;
            dir = System.IO.Path.GetDirectoryName(dir);
            dir = System.IO.Path.Combine(dir, "bin");
            string file = System.IO.Path.Combine(dir, "dump.h");
            
            if (watcher_ == null) {
                watcher_ = new FileSystemWatcher(dir);
                watcher_.Changed += watcher__Changed;
                watcher_.EnableRaisingEvents = true;
            }
            Thread thread = new Thread(delegate() {
                Globals globs = new Globals();
                ASParser parser = new ASParser();
                try
                {
                    StringReader rdr = new StringReader(File.ReadAllText(file));
                    parser.ParseDumpFile(rdr, globs);
                    MainWindow.inst().Dispatcher.Invoke(delegate()
                    {
                        IDEProject.inst().GlobalTypes = globs;
                    });
                } 
                catch (Exception ex)
                {
                    // swallow all exceptions
                }
            });
            thread.Start();

        }

        static void watcher__Changed(object sender, FileSystemEventArgs e) {
            BuildIntellisenseDatabase();
        }
    }
}

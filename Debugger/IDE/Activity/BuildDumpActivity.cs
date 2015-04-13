using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Debugger.IDE.Activity
{
    public class BuildDumpActivity {
        public static void CreateDumps()
        {

            string dir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            dir = System.IO.Path.Combine(dir, "bin");
            string parentDir = System.IO.Directory.GetParent(IDEProject.inst().ProjectDir).ToString();
            Thread thread = new Thread(delegate()
            {
                //Thread thread = new Thread(delegate() {
                Process pi = new Process();
                pi.StartInfo.FileName = System.IO.Path.Combine(dir, "ScriptCompiler.exe");
                pi.StartInfo.Arguments = " -dumpapi " + parentDir + " ScriptAPI.dox dump.h";
                pi.StartInfo.UseShellExecute = false;
                pi.StartInfo.CreateNoWindow = true;
                pi.StartInfo.WorkingDirectory = dir;
                pi.Start();
                pi.WaitForExit();
            });
            thread.Start();
        }
    }
}

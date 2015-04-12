using FirstFloor.ModernUI.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Debugger.IDE.Activity {
    public class CompilerActivity {
        public static void Compile(string aToCompile) {
            
            string dir = System.Reflection.Assembly.GetEntryAssembly().Location.Replace("\\asDevelop.exe", "");
            dir += "\\bin\\ScriptCompiler.exe";

            //Thread thread = new Thread(delegate() {
                Process pi = new Process();
                pi.StartInfo.FileName = dir;
                pi.StartInfo.Arguments = aToCompile;
                pi.EnableRaisingEvents = true;
                pi.StartInfo.UseShellExecute = false;
                pi.StartInfo.CreateNoWindow = true;
                pi.StartInfo.RedirectStandardOutput = true;
                pi.Start();
                pi.WaitForExit();

                string str = "";
                while ((str = pi.StandardOutput.ReadLine()) != null) {
                    IDEProject.inst().CompilerOutput += str + "\r\n";

                    if (str.Contains(',')) {
                        int firstColon = 0;
                        bool colonFond = false;
                        for (; firstColon < str.Length; ++firstColon) {
                            if (str[firstColon] == ':' && colonFond)
                                break;
                            else if (str[firstColon] == ':')
                                colonFond = true;
                        }
                        string fileName = str.Substring(0, firstColon);

                        string part = "";
                        int line = -1;
                        int column = -1;
                        //move to first number
                        ++firstColon;
                        for (; firstColon < str.Length; ++firstColon) {
                            if (str[firstColon] == ',') {
                                if (line == -1)
                                    line = int.Parse(part);
                                else
                                    column = int.Parse(part);
                            }
                            if (str[firstColon] == ' ')
                                break;
                            part += str[firstColon];
                        }
                        string msg = str.Substring(firstColon);
                        CompileError error = new CompileError {
                            File = fileName,
                            Line = line,
                            Message = msg
                        };
                        MainWindow.inst().Dispatcher.Invoke(delegate() {
                            IDEProject.inst().CompileErrors.Add(error);
                        });
                    }
                }
            //});
            //thread.Start();
        }

        static void pi_OutputDataReceived(object sender, DataReceivedEventArgs e) {
            IDEProject.inst().CompilerOutput += e.Data + "\r\n";
        }

        static void pi_ErrorDataReceived(object sender, DataReceivedEventArgs e) {
            IDEProject.inst().CompilerOutput += e.Data + "\r\n";
            if (e.Data == null)
                return;
            string str = e.Data;
            if (str.Contains("ERROR:")) {
                str = str.Replace("ERROR: ", "");
                int firstColon = str.IndexOf(':');
                string fileName = str.Substring(0, firstColon);

                string part = "";
                int line = -1;
                int column = -1;
                //move to first number
                ++firstColon;
                for (; firstColon < str.Length; ++firstColon) {
                    if (str[firstColon] == ',') {
                        if (line == -1)
                            line = int.Parse(part);
                        else
                            column = int.Parse(part);
                    }
                    if (str[firstColon] == ' ')
                        break;
                    part += str[firstColon];
                }
                string msg = str.Substring(firstColon);
                CompileError error = new CompileError {
                        File = IDEProject.inst().ProjectDir + fileName,
                        Line = line,
                        Message = msg
                };
                MainWindow.inst().Dispatcher.Invoke(delegate() {
                    IDEProject.inst().CompileErrors.Add(error);
                });
            }
        }

        public static int Run(Action<string> output, string exe, string args) {
            if (String.IsNullOrEmpty(exe))
                throw new FileNotFoundException();
            if (output == null)
                throw new ArgumentNullException("output");

            ProcessStartInfo psi = new ProcessStartInfo();
            psi.UseShellExecute = false;
            psi.RedirectStandardError = true;
            psi.RedirectStandardOutput = true;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.CreateNoWindow = true;
            psi.ErrorDialog = false;
            psi.WorkingDirectory = Environment.CurrentDirectory;
            psi.FileName = exe; //see http://csharptest.net/?p=526
            psi.Arguments = args; // see http://csharptest.net/?p=529

            using (Process process = Process.Start(psi))
            using (ManualResetEvent mreOut = new ManualResetEvent(false),
            mreErr = new ManualResetEvent(false)) {
                process.OutputDataReceived += (o, e) => { if (e.Data == null) mreOut.Set(); else output(e.Data); };
                process.BeginOutputReadLine();
                process.ErrorDataReceived += (o, e) => { if (e.Data == null) mreErr.Set(); else output(e.Data); };
                process.BeginErrorReadLine();

                //string line;
                //while (input != null && null != (line = input.ReadLine()))
                //    process.StandardInput.WriteLine(line);

                process.WaitForExit();

                mreOut.WaitOne();
                mreErr.WaitOne();
                return process.ExitCode;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrhoCompilerPlugin
{
    public class UrhoCompiler : PluginLib.ICompilerService
    {
        public string Name { get { return "Urho3D Single File"; } }

        public void CompileFile(string file, PluginLib.ICompileErrorPublisher compileErrorPublisher, PluginLib.IErrorPublisher errorPublisher)
        {
            try
            {
                string path = System.Reflection.Assembly.GetEntryAssembly().Location;
                path = Path.Combine(path, "bin");
                path = Path.Combine(path, "ScriptCompiler.exe");

                Process pi = new Process();
                pi.StartInfo.FileName = path;
                pi.StartInfo.Arguments = file;
                pi.EnableRaisingEvents = true;
                pi.StartInfo.UseShellExecute = false;
                pi.StartInfo.CreateNoWindow = true;
                pi.StartInfo.RedirectStandardOutput = true;
                pi.Start();
                pi.WaitForExit();

                string str = "";
                while ((str = pi.StandardOutput.ReadLine()) != null)
                {
                    compileErrorPublisher.PushOutput(str + "\r\n");

                    if (str.Contains(','))
                    {
                        int firstColon = 0;
                        bool colonFond = false;
                        for (; firstColon < str.Length; ++firstColon)
                        {
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
                        for (; firstColon < str.Length; ++firstColon)
                        {
                            if (str[firstColon] == ',')
                            {
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
                        PluginLib.CompileError error = new PluginLib.CompileError
                        {
                            File = fileName,
                            Line = line,
                            Message = msg
                        };
                        compileErrorPublisher.PublishError(error);
                        //MainWindow.inst().Dispatcher.Invoke(delegate()
                        //{
                        //    IDEProject.inst().CompileErrors.Add(error);
                        //});
                    }
                }

            } catch (Exception ex)
            {
                //\todo feed an error service
            }
        }

        public void PostCompile(string file)
        {
            try
            {
                string path = System.Reflection.Assembly.GetEntryAssembly().Location;
                path = Path.Combine(path, "bin");
                path = Path.Combine(path, "ScriptCompiler.exe");
            } 
            catch (Exception ex)
            {

            }

        }
    }
}

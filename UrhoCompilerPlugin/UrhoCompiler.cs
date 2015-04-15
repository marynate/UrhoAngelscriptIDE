using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UrhoCompilerPlugin
{
    /// <summary>
    /// Compiler for Urho3D
    /// 
    /// Uses ScriptCompiler.exe in the bin folder to compile a specific file
    /// </summary>
    public class UrhoCompiler : PluginLib.ICompilerService
    {
        public string Name { get { return "Urho3D Single File"; } }
        
        [DllImport("kernel32.dll")]
        static extern bool ReadConsoleW(IntPtr hConsoleInput, [Out] byte[]
           lpBuffer, uint nNumberOfCharsToRead, out uint lpNumberOfCharsRead,
           IntPtr lpReserved);

        static string ReadLine(IntPtr handle)
        {
            const int bufferSize = 1024;
            var buffer = new byte[bufferSize];

            uint charsRead = 0;

            ReadConsoleW(handle, buffer, bufferSize, out charsRead, (IntPtr)0);
            if (charsRead <= 2)
                return "";
            // -2 to remove ending \n\r
            int nc = ((int)charsRead - 2) * 2;
            var b = new byte[nc];
            for (var i = 0; i < nc; i++)
                b[i] = buffer[i];

            var utf8enc = Encoding.UTF8;
            var unicodeenc = Encoding.Unicode;
            return utf8enc.GetString(Encoding.Convert(unicodeenc, utf8enc, b));
        }

        public void CompileFile(string file, PluginLib.ICompileErrorPublisher compileErrorPublisher, PluginLib.IErrorPublisher errorPublisher)
        {
            try
            {
                string path = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
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
                    if (str.Contains("ERROR: "))
                        str = str.Replace("ERROR: ", "");
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
                    }
                }
            } catch (Exception ex)
            {
                errorPublisher.PublishError(ex);
            }
        }

        public void PostCompile(string file, string sourceTree, PluginLib.IErrorPublisher errorPublisher)
        {
            try
            {
                string path = System.Reflection.Assembly.GetEntryAssembly().Location;
                path = Path.Combine(path, "bin");
                path = Path.Combine(path, "ScriptCompiler.exe");
            } 
            catch (Exception ex)
            {
                errorPublisher.PublishError(ex);
            }

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginLib
{
    /// <summary>
    /// Wraps the task of compiling angelscript code
    /// </summary>
    public interface ICompilerService
    {
        string Name { get; }

        void CompileFile(string file, ICompileErrorPublisher compileErrorPublisher, IErrorPublisher errorPublisher);

        /// <summary>
        /// Called after all compilation has completed
        /// </summary>
        void PostCompile(string file);
    }
}

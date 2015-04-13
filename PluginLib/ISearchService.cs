using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginLib
{
    /// <summary>
    /// Invoked for performing 'search' operations
    /// 
    /// If multiple search services are installed then each will be presented as an option
    /// </summary>
    public interface ISearchService
    {
        string Name { get; }
        void Search(string projectPath, string[] searchTerms, ISearchPublisher publishTo);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginLib
{
    /// <summary>
    /// Not to be implemented by plugins
    /// 
    /// Thread-safe method for ISearchService's to publish their results to
    /// </summary>
    public interface ISearchPublisher
    {
        void PublishSearchResult(SearchResult result);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginLib
{
    /// <summary>
    /// Interface for content that goes in a tab to the UI's right
    /// </summary>
    public interface IInfoTab
    {
        string GetTabName();
        object CreateTabContent(string projectPath);
    }
}

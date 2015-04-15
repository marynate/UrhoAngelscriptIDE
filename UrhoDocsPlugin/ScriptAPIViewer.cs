using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrhoDocsPlugin
{
    public class ScriptAPIViewer : PluginLib.IInfoTab
    {
        public string GetTabName()
        {
            return "Script API";
        }

        public object CreateTabContent(string projectPath)
        {
            throw new NotImplementedException();
        }
    }
}

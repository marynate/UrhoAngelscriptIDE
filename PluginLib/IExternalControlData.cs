using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginLib
{
    /// <summary>
    /// Interface that returned controls from IFileEditor plugins must return
    /// 
    /// The concrete implementation may contain any information useful to the IFileEditor
    /// </summary>
    public interface IExternalControlData
    {
        /// <summary>
        /// The IFileEditor interface this control data came from
        /// </summary>
        IFileEditor SourceEditor { get; set; }

        /// <summary>
        /// Constructed user control that is to be presented in the tab
        /// </summary>
        object Control { get; set; }

        /// <summary>
        /// Will be queried for the Tab UI and for deciding whether to prompt or not on tab closure
        /// </summary>
        bool IsDirty { get; }

        /// <summary>
        /// Called as a result of a "Save All" command
        /// 
        /// To handle general "CTRL+S" saving use the appropriate WPF commands in your control
        /// </summary>
        void SaveData();
    }
}

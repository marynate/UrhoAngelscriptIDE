using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginLib
{
    /// <summary>
    /// Interface for content that is editable
    /// 
    /// Discovered IFileEditor classes will be queried in first-come-first-serve order
    /// to ask if they can edit/view a file
    /// 
    /// An example use case would be to provide a DataGrid for editing a CSV file
    /// 
    /// They are NOT given the opportunity to handle Angelscript (.as) files
    /// </summary>
    public interface IFileEditor
    {
        bool CanEditFile(string filePath, string fileExtension);
        object CreateEditorContent(string filePath, out object userData);
        void SaveContent(object contentGiven, object userData, string filePath);
    }
}

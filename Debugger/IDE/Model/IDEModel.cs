using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Debugger.IDE {
    public class IDEModel {
        ObservableCollection<IDEFile> openFiles_ = new ObservableCollection<IDEFile>();
        ObservableCollection<IDEFile> knownFiles_ = new ObservableCollection<IDEFile>();
        public ObservableCollection<IDEFile> OpenFiles { get { return openFiles_; } }
        public ObservableCollection<IDEFile> AllFiles { get { return knownFiles_; } }

        public ObservableCollection<PluginLib.CompileError> CompilerErrors { get; set; }
        public ObservableCollection<CompileLog> CompilerLog { get; set; }
    }


    [Serializable]
    public class IDEProject : NamedBaseClass, PluginLib.ICompileErrorPublisher {
        static IDEProject inst_;

        public static IDEProject inst() { return inst_; }

        public IDEProject() {
            inst_ = this;
            documentation_ = new API.APIDocumentation();
            docDatabase_ = new Docs.DocDatabase();
        }

        string filePath_ = "";
        string projectDir_ = "";
        List<string> openFiles_ = new List<string>();
        List<int> fileLines_ = new List<int>(); //subscript matches subscript of openfiles
        List<string> files_ = new List<string>();
        IDESettings settings_;

        API.APIDocumentation documentation_;
        Globals intellisenseGlobals_;
        ObservableCollection<PluginLib.CompileError> errors_ = new ObservableCollection<PluginLib.CompileError>();
        string compileOutput_;
        Docs.DocDatabase docDatabase_;

        [XmlIgnore]
        public Docs.DocDatabase DocDatabase { get { return docDatabase_; } }

        [XmlIgnore]
        public API.APIDocumentation Documentation { get { return documentation_; } set { documentation_ = value; OnPropertyChanged("Documentation"); } }
        [XmlIgnore]
        public Globals GlobalTypes { get { return intellisenseGlobals_; } set { intellisenseGlobals_ = value; OnPropertyChanged("GlobalTypes"); } }
        [XmlIgnore]
        public string CompilerOutput { get { return compileOutput_; } set { compileOutput_ = value; OnPropertyChanged("CompilerOutput"); } }
        [XmlIgnore]
        public ObservableCollection<PluginLib.CompileError> CompileErrors { get { return errors_; } }


        public string ProjectDir { get { return projectDir_; } set { projectDir_ = value; OnPropertyChanged("ProjectDir"); } }
        public string FilePath { get { return filePath_; } set { filePath_ = value; OnPropertyChanged("FilePath"); } }
        public List<string> OpenFiles { get { return openFiles_; } }
        public List<int> FileLines { get { return fileLines_; } }
        public List<string> Files { get { return files_; } }

        public IDESettings Settings { get { return settings_; } set { settings_ = value; } }

        public bool NewProject() {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.DefaultExt = "prj";
            dlg.Filter = "Project files (*.prj)|*.prj";
            if (dlg.ShowDialog() == true) {
                IDEProject prj = new IDEProject() { FilePath = dlg.FileName, ProjectDir = Path.GetDirectoryName(dlg.FileName) };
                prj.Settings = IDESettings.GetOrCreate(dlg.FileName);
                prj.Save();
                UserData.inst().AddRecentFile(dlg.FileName);
            } return false;
        }

        public void Save() {
            try {
                System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(IDEProject));
                using (System.IO.FileStream file = new System.IO.FileStream(FilePath, FileMode.Create, FileAccess.Write, FileShare.None)) {
                    writer.Serialize(file, this);
                    file.Flush();
                    file.Close();
                }
            }
            catch (Exception ex) {
                ErrorHandler.inst().Error(ex);
            }
        }

        public static void open(string aPath = "") {
            if (aPath == string.Empty) {
                inst_ = new IDEProject();
            } else {
                try {
                    IDEProject.open(); //Interesting?
                }
                catch (Exception ex) {
                    ErrorHandler.inst().Error(ex);
                }
            }
        }

        public void destroy() {
            inst_ = null;
        }

        public void PublishError(PluginLib.CompileError error)
        {
            IDEView.inst().Dispatcher.Invoke(delegate()
            {
                CompileErrors.Add(error);
            });
        }

        public void PushOutput(string text)
        {
            IDEView.inst().Dispatcher.Invoke(delegate()
            {
                CompilerOutput += text;
            });
        }
    }

}

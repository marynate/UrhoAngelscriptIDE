using ICSharpCode.AvalonEdit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Debugger.Debug {

    public class FileData : BaseClass {
        int id_;
        string mod_;
        string section_;
        string code_;
        List<TextEditor> attachedEditor = new List<TextEditor>();
        public void AttachEditor(TextEditor e) { attachedEditor.Add(e); }
        ObservableCollection<Breakpoint> breakPoints_;

        public FileData() {
            breakPoints_ = new ObservableCollection<Breakpoint>();
            breakPoints_.CollectionChanged += breakPoints__CollectionChanged;
        }

        void breakPoints__CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
            foreach (Breakpoint bp in this.BreakPoints) {
                if (!SessionData.inst().AllBreakpoints.Contains(bp)) {
                    SessionData.inst().AllBreakpoints.Add(bp);
                }
            }
        }

        public int SectionID { 
            get { return id_; } 
            set {id_ = value; OnPropertyChanged("SectionID");}
        }
        public string Module { 
            get { return mod_; }
            set { mod_ = value; OnPropertyChanged("Module"); }
        }
        public string SectionName { 
            get { return section_; }
            set { section_ = value; OnPropertyChanged("SectionName"); }
        }

        public bool NoPost { get; set; }

        public string Code {
            get {
                if (code_ == null) {
                    Net.DebugClient.inst().GetFile(this);
                    return "";
                }
                return code_; }
            set { 
                code_ = value; OnPropertyChanged("Code");
                MainWindow.inst().Dispatcher.Invoke(delegate() {
                    foreach (TextEditor e in attachedEditor) {
                        if (e != null)
                            e.Text = code_;
                    }
                });
            }
        }

        public ObservableCollection<Breakpoint> BreakPoints { get { return breakPoints_; } }
    }
}

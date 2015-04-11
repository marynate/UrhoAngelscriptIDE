using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Debugger.Debug {

    public class Callstack : BaseClass {
        int stackLevel_;
        public int StackLevel { get { return stackLevel_;}
            set {
                stackLevel_ = value;
                OnPropertyChanged("StackLevel");
            }
        }

        string func_;
        string file_;
        int line_;
        int sectionID_;

        public int Line {
            get { return line_; }
            set {
                line_ = value;
                OnPropertyChanged("Line");
            }
        }

        public int SectionID {
            get { return sectionID_; }
            set { sectionID_ = value; OnPropertyChanged("SectionID");  }
        }

        public string StackFunction {
            get { return func_; }
            set {
                func_ = value;
                OnPropertyChanged("StackFunction");
            }
        }

        public string StackFile {
            get {
                if (file_ == null) {
                    //TODO get filename from FileData
                }
                return file_; 
            }
            set {
                file_ = value;
                OnPropertyChanged("StackFile");
            }
        }
    }
}

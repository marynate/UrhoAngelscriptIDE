using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Debugger.Debug {

    /// <summary>
    /// Manages a breakpoint
    /// 
    /// To the asPEEK daemon, active_ == false is equivalent to 'removed'
    /// The 'inactive' breakpoint is kept in the debugger until deleted so that it can
    /// be quickly turned on/off while hiding the 'breakpoint set' and 'breakpoint removed'
    /// </summary>
    public class Breakpoint : BaseClass {
        int line_;
        int sectionID_;
        bool active_;
        string file_;
        string snip_;

        public int LineNumber { 
            get { return line_; } 
            set { line_ = value; OnPropertyChanged("LineNumber"); } 
        }

        public int SectionID { get { return sectionID_; } set { sectionID_ = value; OnPropertyChanged("LineNumber"); } }

        public bool Active { 
            get { return active_; } 
            set { 
                active_ = value; 
                OnPropertyChanged("Active");
                if (LineNumber != 0) {
                    FileData fd = SessionData.inst().Files.FirstOrDefault(file => file.SectionID == SectionID);
                    if (fd != null) {
                        Net.DebugClient.inst().SetBreakpoint(fd.SectionID, LineNumber, Active);
                    }
                }
            } 
        }
        public string File { 
            get { return file_; }
            set {
                file_ = value;
                OnPropertyChanged("File");
            }
        }
        public string LineSnippet { 
            get { return snip_; }
            set {
                snip_ = value;
                OnPropertyChanged("LineSnippet");
            }
        }
    }
}

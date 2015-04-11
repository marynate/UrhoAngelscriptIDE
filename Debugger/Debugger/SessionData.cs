using Debugger.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Debugger.Debug {
    public class SessionData : BaseClass {
        string connection_;
        bool isConnected_;
        ObservableCollection<Callstack> callStack_;
        ObservableCollection<FileData> files_;
        ObservableCollection<LogMessage> log_;
        ObservableCollection<Module> modules_;
        ObservableCollection<Breakpoint> breakpoints_;
        ObservableCollection<WatchValue> watches_;
        JWrapper localJSON_;
        JWrapper localThis_;
        JWrapper globals_;

        static SessionData inst_;

        public static SessionData inst() { return inst_; }
        public SessionData(string aCon) {
            inst_ = this;
            connection_ = aCon;
            isConnected_ = false;
            callStack_ = new ObservableCollection<Callstack>();
            files_ = new ObservableCollection<FileData>();
            log_ = new ObservableCollection<LogMessage>();
            modules_ = new ObservableCollection<Module>();
            breakpoints_ = new ObservableCollection<Breakpoint>();
            watches_ = new ObservableCollection<WatchValue>();
            currentLine_ = -1;
            currentSection_ = -1;
            isDebugging_ = false;
        }

        bool isDebugging_;
        public bool IsDebugging {
            get {
                return isDebugging_;
            }
            set {
                isDebugging_ = value;
                OnPropertyChanged("IsDebugging");
            }
        }

        public JWrapper LocalData { 
            get { return localJSON_; } 
            set { 
                localJSON_ = value;
                foreach (WatchValue wv in Watches)
                    wv.Update();
                OnPropertyChanged("LocalData");
            } 
        }
        public JWrapper ThisData { 
            get { return localThis_; } 
            set { 
                localThis_ = value; 
                OnPropertyChanged("ThisData");
                foreach (WatchValue wv in Watches)
                    wv.Update();
            } 
        }
        public JWrapper GlobalData {
            get { return globals_; }
            set {
                globals_ = value; 
                OnPropertyChanged("GlobalData"); 
                foreach (WatchValue wv in Watches)
                    wv.Update();
            }
        }

        public string Connection {
            get { return connection_; }
            set { connection_ = value; OnPropertyChanged("Connection"); }
        }

        public bool IsConnected {
            get { return isConnected_; }
            set { isConnected_ = value; OnPropertyChanged("IsConnected"); }
        }

        public ObservableCollection<Module> Modules { get { return modules_; } }
        public ObservableCollection<LogMessage> Log { get { return log_; } }
        public ObservableCollection<Callstack> CallStack { get { return callStack_; } }
        public ObservableCollection<FileData> Files { get { return files_; } }
        public ObservableCollection<WatchValue> Watches { get { return watches_; } }

        public void AddWatch(WatchValue aWatch) {
            foreach (WatchValue watch in Watches) {
                if (watch.Variable.Equals(aWatch.Variable))
                    return;
            }
            if (aWatch.Evaluate(LocalData)) {
            }
            Watches.Add(aWatch);
        }

        public ObservableCollection<Breakpoint> AllBreakpoints { get { return breakpoints_; } }

        int currentSection_;
        int currentLine_;

        public int CurrentSection {
            get { return currentSection_; }
            set {
                currentSection_ = value;
                OnPropertyChanged("CurrentSection");
                OnPropertyChanged("IsDebugging");
            }
        }

        public int CurrentLine {
            get { return currentLine_; }
            set {
                currentLine_ = value;
                OnPropertyChanged("CurrentLine");
                OnPropertyChanged("IsDebugging");
            }
        }
    }
}

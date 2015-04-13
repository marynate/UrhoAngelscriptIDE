using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Debugger.IDE
{
    public enum CompilerMsgType
    {
        Warning,
        Error
    }

    public class CompileLog : BaseClass
    {
        string msg_ = "";
        CompilerMsgType msgType_;

        public CompilerMsgType MsgType
        {
            get { return msgType_; }
            set { msgType_ = value; OnPropertyChanged("MsgType"); }
        }

        public string Message
        {
            get { return msg_; }
            set { msg_ = value; OnPropertyChanged("Message"); }
        }
    }

    public class CompileError : BaseClass
    {
        int line_ = 0;
        string msg_ = "";
        string file_ = "";

        public string File
        {
            get { return file_; }
            set { file_ = value; OnPropertyChanged("File"); }
        }

        public int Line
        {
            get { return line_; }
            set { line_ = value; OnPropertyChanged("Line"); }
        }

        public string Message
        {
            get { return msg_; }
            set { msg_ = value; OnPropertyChanged("Message"); }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginLib
{
    public class CompileError : BasePropertyBound
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

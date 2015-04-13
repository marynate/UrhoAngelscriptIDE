using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Debugger.IDE
{
    public class IDEFile : BaseClass
    {
        public static readonly int SAVE_DELTA = 50; //we'll trigger an auto-save at

        string code_ = "";
        string fileName_ = "";

        DateTime lastSaveTime_ = DateTime.Now;

        public string Code
        {
            get { return code_; }
            set
            {
                code_ = value;
                OnPropertyChanged("Code");
            }
        }

        public string FileName
        {
            get { return fileName_; }
            set
            {
                fileName_ = value;
                OnPropertyChanged("FileName");
            }
        }
    }
}

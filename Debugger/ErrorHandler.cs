using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Debugger {

    /// <summary>
    /// Receives errors and is periodically checked for a need to display an error dialog
    /// Use to prevent exceptions from bringing down the program, but still provide a notice that
    /// things have not gone according to plan
    /// 
    /// \todo Errors should probably be written to a log
    /// </summary>
    public class ErrorHandler {
        static ErrorHandler inst_;
        List<string> messages_ = new List<string>();

        public ErrorHandler()
        {
            inst_ = this;
        }

        public static ErrorHandler inst() {
            return inst_;
        }

        public bool Check() {
            if (messages_.Count > 0) {
                return true;
            } return false;
        }

        public string GetMessage() {
            string msg = messages_[0];
            messages_.RemoveAt(0);
            return msg;
        }

        public void Error(Exception ex) {
            messages_.Add(ex.Message);
        }
    }
}

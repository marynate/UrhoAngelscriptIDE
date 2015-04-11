using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Debugger {
    public class ErrorHandler {
        static ErrorHandler inst_;
        List<string> messages_ = new List<string>();

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Debugger.Debug {
    public enum MessageType {
        Info,
        Warning,
        Error,
        Data
    }

    public class LogMessage {
        public MessageType MsgType { get; set; }
        public string Message { get; set; }
    }
}

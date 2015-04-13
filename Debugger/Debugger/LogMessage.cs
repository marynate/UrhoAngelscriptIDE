using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Debugger.Debug {

    /// <summary>
    /// Type of message received from asPEEK daemon
    /// </summary>
    public enum MessageType {
        Info,
        Warning,
        Error,
        Data
    }

    /// <summary>
    /// A log message from the asPEEK daemon
    /// Is not reused for any other purposes
    /// </summary>
    public class LogMessage {
        public MessageType MsgType { get; set; }
        public string Message { get; set; }
    }
}

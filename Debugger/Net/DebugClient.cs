using Debugger.Debug;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using WebSocket4Net;

namespace Debugger.Net {

    /// <summary>
    /// Client for the asPEEK embedded debugger, main workhorse of the Debugger
    /// 
    /// Sends/receives messages to/from an asPEEK daemon
    /// </summary>
    public class DebugClient {
        SessionData session_;
        WebSocket socket_;
        static DebugClient inst_;

        public bool VerboseLog { get; set; }

        public static DebugClient inst() { return inst_; }

        public DebugClient(SessionData session) {
            inst_ = this;
            session_ = session;
            VerboseLog = false;
        }

        public void Connect(string url) {
            if (!url.Contains("ws://"))
                url = "ws://" + url;
            if (socket_ != null) {
                if (session_ != null)
                    session_.IsConnected = false;
                socket_.Close();
                socket_ = null;
            }
            if (socket_ == null) {
                session_.Log.Add(new LogMessage {
                    MsgType = MessageType.Info,
                    Message = "Attempting to connect"
                });
                string res = new Uri(url).AbsoluteUri;
                socket_ = new WebSocket(res);
                socket_.EnableAutoSendPing = true;
                socket_.AutoSendPingInterval = 50;
                socket_.Opened += _Opened;
                socket_.Closed += _Closed;
                socket_.MessageReceived += _ReceiveMsg;
                socket_.DataReceived += _ReceiveData;
                socket_.Error += _Error;
                socket_.Open();
            }
        }

        string[] GetBrackedParts(string input) {
            int stack = 0;
            List<string> strings = new List<string>();
            string current = "";
            for (int i = 0; i < input.Length; ++i) {
                char c = input[i];
                if (c == '[') {
                    ++stack;
                } else if (c == ']') {
                    --stack;
                    if (stack == 0) {
                        strings.Add(current);
                        current = "";
                    } else
                        current += c;
                } else
                    current += c;
            }
            return strings.ToArray();
        }

        void _Opened(object o, EventArgs args) {
            session_.IsConnected = true;
            MainWindow.inst().Dispatcher.Invoke(delegate() {
                session_.Log.Add(new LogMessage {
                    MsgType = MessageType.Info,
                    Message = "Connection established"
                });
            });
        }

        void _Error(object o, SuperSocket.ClientEngine.ErrorEventArgs args) {
            MainWindow.inst().Dispatcher.Invoke(delegate() {
                session_.Log.Add(new LogMessage {
                    MsgType = MessageType.Error,
                    Message = args.Exception.Message
                });
            });
        }

        void _Closed(object o, EventArgs args) {
            session_.IsConnected = false;
            MainWindow.inst().Dispatcher.Invoke(delegate() {
                session_.Log.Add(new LogMessage {
                    MsgType = MessageType.Info,
                    Message = "Connection Closed"
                });
            });
        }

        //reconstruct strings
        string JoinStringsWith(string[] parts, int startSub, string joinString) {
            string ret = "";
            for (int i = startSub; i < parts.Length; ++i) {
                if (i > startSub)
                    ret += joinString;
                ret += parts[i];
            }
            return ret;
        }

        void _ReceiveMsg(object o, MessageReceivedEventArgs args) {
            try {
                string msg = args.Message.Substring(0, 4).ToLower();
                string[] parts = args.Message.Split(' ');/*Regex.Matches(msg, @"[\""].+?[\""]|[^ ]+")
                    .Cast<Match>()
                    .Select(m => m.Value)
                    .ToArray();*/

                if (VerboseLog) { //if vebose logging then send all messages
                    MainWindow.inst().Dispatcher.Invoke(delegate() {
                        session_.Log.Add(new LogMessage { Message = args.Message, MsgType = MessageType.Data });
                    });
                }

                if (msg.Equals("varv")) {
                    //TODO
                    MainWindow.inst().Dispatcher.Invoke(delegate() {
                        session_.Log.Add(new LogMessage { Message = args.Message, MsgType = MessageType.Data });
                    });
                } else if (msg.Equals("reqv")) {
                    string varName = parts[1];
                    string varValue = parts[2];
                    //TODO
                } else if (msg.Equals("locv")) {
                    string code = args.Message.Substring(5);
                    JArray array = JArray.Parse(code);
                    MainWindow.inst().Dispatcher.Invoke(delegate() {
                        SessionData.inst().LocalData = new Json.JWrapper("Stack Level ", array);
                        //Screens.DebugScreen.inst().LocalsTree.DataContext = null;
                        Screens.DebugScreen.inst().LocalsTree.DataContext = SessionData.inst().LocalData;
                    });
                } else if (msg.Equals("glov")) {
                    string code = args.Message.Substring(5).Replace("1.#INF","\"1.#INF\"");
                    JArray array = JArray.Parse(code);
                    MainWindow.inst().Dispatcher.Invoke(delegate() {
                        SessionData.inst().GlobalData = new Json.JWrapper("Globals", array);
                        //Screens.DebugScreen.inst().GlobalsTree.DataContext = null;
                        Screens.DebugScreen.inst().GlobalTree.DataContext = SessionData.inst().GlobalData;
                    });
                } else if (msg.Equals("this")) {
                    int stackDepth = int.Parse(parts[1]);
                    string code = JoinStringsWith(parts, 2, " ");
                    JObject array = JObject.Parse(code);
                    MainWindow.inst().Dispatcher.Invoke(delegate() {
                        SessionData.inst().ThisData = new Json.JWrapper("This", array);
                        //Screens.DebugScreen.inst().ThisTree.DataContext = null;
                        Screens.DebugScreen.inst().ThisTree.DataContext = SessionData.inst().ThisData;
                    });
                    //TODO
                } else if (msg.Equals("modl")) {
                    JArray array = JArray.Parse(args.Message.Substring(5));
                    for (int i = 0; i < array.Count; ++i) {
                        if (array[i].Type == JTokenType.String) {
                            string str = array[i].ToString();
                            MainWindow.inst().Dispatcher.Invoke(delegate() {
                                if (session_.Modules.FirstOrDefault(m => m.Name.Equals(str)) == null) {
                                    session_.Modules.Add(new Module {
                                        Name = str
                                    });
                                }
                            });
                        }
                    }
                } else if (msg.Equals("ctxl")) {

                } else if (msg.Equals("stck")) {
                    if (parts[1].StartsWith("[")) {
                        //TODO
                        string code = args.Message.Substring(5);
                        JArray array = JArray.Parse(code);
                        MainWindow.inst().Dispatcher.Invoke(delegate() {
                            SessionData.inst().CallStack.Clear();
                            //STCK [{"l":47,"c":5,"s":18,"f":"UIElement@ SetValue(LineEdit@, const String&in, bool)"},{"l":471,"c":9,"s":18,"f":"void LoadAttributeEditor(UIElement@, const Variant&in, const AttributeInfo&in, bool, bool, const Variant[]&in)"},{"l":444,"c":9,"s":18,"f":"void LoadAttributeEditor(ListView@, Serializable@[]@, const AttributeInfo&in, uint)"},{"l":785,"c":9,"s":18,"f":"void UpdateAttributes(Serializable@[]@, ListView@, bool&inout)"},{"l":226,"c":9,"s":17,"f":"void UpdateAttributeInspector(bool = true)"},{"l":738,"c":5,"s":2,"f":"void HandleHierarchyListSelectionChange()"}]
                            int i = 0;
                            foreach (JObject obj in array.Children<JObject>()) {
                                session_.CallStack.Add(new Callstack {
                                    Line = obj.Property("l").Value.ToObject<int>(),
                                    StackFunction = obj.Property("f").Value.ToString(),
                                    StackLevel = i,
                                    SectionID = obj.Property("s").Value.ToObject<int>()
                                });
                                ++i;
                            }
                        });

                    } else {
                        string ctx = parts[1];
                        int depth = int.Parse(parts[2]);
                        int line = int.Parse(parts[3]);
                        int sectionid = int.Parse(parts[4]);
                        MainWindow.inst().Dispatcher.Invoke(delegate() {
                            session_.CallStack.Add(new Callstack {
                                StackLevel = depth,
                                StackFunction = ctx,
                                SectionID = sectionid,
                                Line = line
                            });
                        });
                    }
                } else if (msg.Equals("scls")) {
                    JArray array = JArray.Parse(args.Message.Substring(5));

                    foreach (JObject obj in array.Children<JObject>()) {
                        int id = obj.Property("id").Value.ToObject<int>();
                        string module = obj.Property("mod").Value.ToString();
                        string name = obj.Property("name").Value.ToString();
                        MainWindow.inst().Dispatcher.Invoke(delegate() {
                            FileData fd = session_.Files.FirstOrDefault(file => file.SectionName.Equals(name));
                            if (fd == null) {
                                session_.Files.Add(new FileData {
                                    SectionName = name,
                                    Module = module,
                                    SectionID = id
                                });
                            } else {
                                fd.SectionID = id;
                            }
                        });
                    }

                } else if (msg.Equals("file")) {
                    int sectionID = int.Parse(parts[1]);
                    int len = sectionID.ToString().Length + 6;
                    string data = args.Message.Substring(len);

                    Debugger.Debug.FileData file = session_.Files.FirstOrDefault(f => f.SectionID == sectionID);
                    if (file != null)
                        file.Code = data;
                    //??is there an else case? should presumably know the section ahead of time

                } else if (msg.Equals("bset")) {
                    int sectionID = int.Parse(parts[1]);
                    int line = int.Parse(parts[2]);

                    Debugger.Debug.FileData file = session_.Files.FirstOrDefault(f => f.SectionID == sectionID);
                    if (file != null) {
                        Debugger.Debug.Breakpoint breakpoint = file.BreakPoints.FirstOrDefault(bp => bp.LineNumber == line);
                        if (breakpoint == null) {
                            MainWindow.inst().Dispatcher.Invoke(delegate() {
                                file.BreakPoints.Add(new Breakpoint { LineNumber = line, Active = true, SectionID = sectionID, File = file.SectionName });
                            });
                        } else {
                            breakpoint.Active = true;
                        }
                    } else {
                        Debugger.Debug.FileData fd = new FileData { SectionID = sectionID };
                    }
                } else if (msg.Equals("brem")) {
                    int sectionID = int.Parse(parts[1]);
                    int line = int.Parse(parts[2]);
                    Debugger.Debug.FileData file = session_.Files.FirstOrDefault(f => f.SectionID == sectionID);
                    if (file != null) {
                        Debugger.Debug.Breakpoint breakpoint = file.BreakPoints.FirstOrDefault(bp => bp.LineNumber == line);
                        if (breakpoint != null) {
                            breakpoint.Active = false;
                        }
                    }
                } else if (msg.Equals("hitl")) {
                    session_.IsDebugging = true;
                    int sectionId = int.Parse(parts[1]);
                    int line = int.Parse(parts[2]);
                    session_.CurrentSection = sectionId;
                    session_.CurrentLine = line;
                    MainWindow.inst().Dispatcher.Invoke(delegate() {
                        Screens.DebugScreen.inst().EditorTabs.OpenFile(sectionId, line);
                    });

                } else if (msg.Equals("cont")) {
                } else if (msg.Equals("secm")) { //A file has been changed remotely, send a request for it
                    int sectionid = int.Parse(parts[1]);
                    socket_.Send(string.Format("GETF {0}", sectionid));
                } else if (msg.Equals("logw")) {
                    string m = args.Message.Substring(5);
                    MainWindow.inst().Dispatcher.Invoke(delegate() {
                        session_.Log.Add(new LogMessage {
                            MsgType = MessageType.Warning,
                            Message = JoinStringsWith(parts, 1, " ")
                        });
                    });
                } else if (msg.Equals("loge")) {
                    string m = args.Message.Substring(5);
                    MainWindow.inst().Dispatcher.Invoke(delegate() {
                        session_.Log.Add(new LogMessage {
                            MsgType = MessageType.Error,
                            Message = JoinStringsWith(parts, 1, " ")
                        });
                    });
                } else if (msg.Equals("logi")) {
                    string m = args.Message.Substring(5);
                    MainWindow.inst().Dispatcher.Invoke(delegate() {
                        session_.Log.Add(new LogMessage {
                            MsgType = MessageType.Info,
                            Message = JoinStringsWith(parts, 1, " ")
                        });
                    });
                } else if (msg.Equals("rstr")) {
                    //TODO
                } else {
                    //Unknown msg
                    MainWindow.inst().Dispatcher.Invoke(delegate() {
                        session_.Log.Add(new LogMessage {
                            MsgType = MessageType.Data,
                            Message = args.Message
                        });
                    });
                }
            }
            catch (Exception ex) {
                MainWindow.inst().Dispatcher.Invoke(delegate() {
                    session_.Log.Add(new LogMessage {
                        MsgType = MessageType.Error,
                        Message = ex.Message
                    });
                    session_.Log.Add(new LogMessage {
                        MsgType = MessageType.Info,
                        Message = args.Message
                    });
                });
            }
        }

        void _ReceiveData(object o, DataReceivedEventArgs args) {
            
        }

        public void Continue() {
            session_.CurrentLine = -1;
            session_.CurrentSection = -1;
            session_.CallStack.Clear();
            session_.IsDebugging = false;
            socket_.Send("CONT");
        }

        public void StepOver() {
            session_.CallStack.Clear();
            socket_.Send("STOV");
        }

        public void StepOut() {
            session_.CallStack.Clear();
            socket_.Send("STOU");
        }

        public void StepIn() {
            session_.CallStack.Clear();
            socket_.Send("STIN");
        }

        public void Restart() {
            socket_.Send("RSTR");
        }

        public void SetBreakpoint(int sectionID, int line, bool active) {
            if (active)
                socket_.Send(string.Format("BRKS {0} {1}", sectionID, line));
            else
                socket_.Send(string.Format("BRKR {0} {1}", sectionID, line));
        }

        public void Save(FileData aData) {
            socket_.Send(string.Format("SAVE {0} {1}", aData.SectionID, aData.Code));
        }

        public void RawMsg(string msg) {
            socket_.Send(msg);
        }

        public void GetFile(FileData aData) {
            socket_.Send(string.Format("GETF {0}", aData.SectionID));
        }
    }
}

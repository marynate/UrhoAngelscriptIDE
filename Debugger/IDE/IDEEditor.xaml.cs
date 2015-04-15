using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Debugger.Editor;
using ICSharpCode.AvalonEdit;
using Debugger.IDE.Intellisense;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.CodeCompletion;
using FirstFloor.ModernUI.Presentation;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using System.Xml;
using System.Reflection;
using System.Collections.ObjectModel;

namespace Debugger.IDE {
    /// <summary>
    /// Interaction logic for IDEEditor.xaml
    /// </summary>
    /// 


    public partial class IDEEditor : UserControl {
        FileBaseItem item;
        DepthScanner scanner;

        public IDEEditor(FileBaseItem aItem) {
            InitializeComponent();
            item = aItem;
            changeChecker = new DataChanged { editor = editor };
            SetCode(aItem);
            editor.ShowLineNumbers = true;
            if (aItem.Name.EndsWith(".as")) {
                editor.SyntaxHighlighting = LoadHighlightingDefinition("Debugger.Resources.Angelscript.xshd");
            } else if (aItem.Name.EndsWith(".xml")) {
                editor.SyntaxHighlighting = LoadHighlightingDefinition("Debugger.Resources.XML.xshd");
            } else {
                editor.SyntaxHighlighting = ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance.GetDefinitionByExtension(item.Path.Substring(item.Path.LastIndexOf('.')));
            }

            editor.FontFamily = new FontFamily("Consolas");
            editor.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFDCDCCC"));
            editor.TextArea.TextView.BackgroundRenderers.Add(new LineHighlighter());
            editor.TextArea.TextView.BackgroundRenderers.Add(new ErrorLineHighlighter(aItem));
            Debugger.Editor.SearchPanel.Install(editor);
            editor.TextChanged += editor_TextChanged;
            scanner = new DepthScanner();
            scanner.Process(editor.Text);
            editor.MouseHover += editor_MouseHover;
            editor.KeyUp += editor_KeyUp;

            editor.ContextMenu = new ContextMenu();
            editor.ContextMenu.Items.Add(new MenuItem {
                Header = "Compile",
                Command = new RelayCommand(p =>
                {
                    IDEView.Compile();
                })
            });
            editor.ContextMenu.Items.Add(new Separator());
            editor.ContextMenu.Items.Add(new MenuItem {
                Header = "New ScriptObject",
                Command = new RelayCommand(p => {
                    editor.Document.BeginUpdate();
                    editor.Document.Insert(editor.CaretOffset, SOScript);
                    editor.Document.EndUpdate();
                })
            });
            editor.ContextMenu.Items.Add(new MenuItem {
                Header = "Insert #include",
                Command = new RelayCommand(p => {
                    Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                    dlg.DefaultExt = "*";
                    dlg.Filter = "All files (*.*)|*.*";
                    if (dlg.ShowDialog() == true) {
                        editor.Document.BeginUpdate();
                        editor.Document.Insert(editor.CaretOffset, string.Format("#include \"{0}\"", dlg.FileName));
                        editor.Document.EndUpdate();
                    }
                })
            });
            editor.ContextMenu.Items.Add(new MenuItem {
                Header = "Doxygen Comment",
                Command = new RelayCommand(p => {
                    editor.Document.BeginUpdate();
                    editor.Document.Insert(editor.CaretOffset,
@"/////////////////////////////////////////////////
/// DOCUMENT_HERE
/////////////////////////////////////////////////", AnchorMovementType.AfterInsertion);
                    editor.Document.EndUpdate();
                })
            });
            editor.ContextMenu.Items.Add(new MenuItem {
                Header = "Property Comment",
                Command = new RelayCommand(p => {
                    editor.Document.BeginUpdate();
                    editor.Document.Insert(editor.CaretOffset, "///< DOCUMENT", AnchorMovementType.AfterInsertion);
                    editor.Document.EndUpdate();
                })
            });
            editor.ContextMenu.Items.Add(new MenuItem {
                Header = "Snippet",
                Command = new RelayCommand(p =>
                {
                    ObservableCollection<Snippets.CodeSnippet> snips = new ObservableCollection<Snippets.CodeSnippet>();
                    string ext = System.IO.Path.GetExtension(item.Path);
                    foreach (Snippets.CodeSnippet snip in Snippets.SnippetData.inst().Snippets) {
                        if (snip.Extension.Equals(ext))
                            snips.Add(snip);
                    }
                    if (snips.Count > 0) {
                        Snippets.SnippetDlg dlg = new Snippets.SnippetDlg(editor, snips);
                        dlg.ShowDialog();
                    }
                }, sp => {
                    ObservableCollection<Snippets.CodeSnippet> snips = new ObservableCollection<Snippets.CodeSnippet>();
                    string exte = System.IO.Path.GetExtension(item.Path);
                    foreach (Snippets.CodeSnippet snip in Snippets.SnippetData.inst().Snippets) {
                        if (snip.Extension.Equals(exte))
                            return true;
                    }
                    return false;
                })
            });
            editor.ContextMenu.Items.Add(new Separator());
            editor.ContextMenu.Items.Add(new MenuItem {
                Header = "Cut",
                Command = ApplicationCommands.Cut
            });
            editor.ContextMenu.Items.Add(new MenuItem {
                Header = "Copy",
                Command = ApplicationCommands.Copy
            });
            editor.ContextMenu.Items.Add(new MenuItem {
                Header = "Paste",
                Command = ApplicationCommands.Paste
            });
            editor.ContextMenu.Items.Add(new Separator());
            editor.ContextMenu.Items.Add(new MenuItem {
                Header = "Undo",
                Command = ApplicationCommands.Undo
            });
            editor.ContextMenu.Items.Add(new MenuItem {
                Header = "Redo",
                Command = ApplicationCommands.Redo
            });
            editor.ContextMenu.Items.Add(new Separator());
            editor.ContextMenu.Items.Add(new MenuItem {
                Header = "Save",
                Command = ApplicationCommands.Save
            });
            editor.ContextMenu.Items.Add(new Separator());
            editor.ContextMenu.Items.Add(new MenuItem {
                Header = "Save As",
                Command = ApplicationCommands.SaveAs
            });
            editor.ContextMenu.Items.Add(new Separator());
            editor.ContextMenu.Items.Add(new MenuItem {
                Header = "Open",
                Command = ApplicationCommands.Open
            });
        }

        void editor_MouseHover(object sender, MouseEventArgs e) {
            TextViewPosition? pos = editor.GetPositionFromPoint(e.GetPosition(editor));
            if (pos != null) {
                try {
                    int line = pos.Value.Line;
                    int offset = editor.Document.GetOffset(pos.Value.Location);
                    Globals globs = IDEProject.inst().GlobalTypes;
                    if (globs != null) {
                        bool isFunc = false;
                        string[] words = IntellisenseHelper.ExtractPath(editor.Document, offset, pos.Value.Location.Line, out isFunc);
                        if (words != null && words.Length > 0) {
                            TypeInfo info = null;
                            FunctionInfo func = null;
                            NameResolver reso = new NameResolver(globs, scanner);
                            if (words.Length > 1) {
                                for (int i = 0; i < words.Length; ++i) {
                                    if (i == words.Length - 1 && info != null && isFunc) {
                                        func = info.Functions.FirstOrDefault(f => f.Name.Equals(words[i]));
                                    } else {
                                        if (info == null) {
                                            info = reso.GetClassType(editor.Document, editor.TextArea.Caret.Line, words[i]);
                                        } else if (info != null) {
                                            if (info.Properties.ContainsKey(words[i]))
                                                info = info.Properties[words[i]];
                                        }
                                    }
                                }
                            } else if (isFunc && words.Length == 1) {
                                func = globs.Functions.FirstOrDefault(f => f.Name.Equals(words[0]));
                            } else if (!isFunc && words.Length == 1) {
                                info = reso.GetClassType(editor.Document, line, words[0]);
                                if (info == null) {
                                    KeyValuePair<string, TypeInfo> ty = globs.Classes.FirstOrDefault(p => p.Value.Equals(words[0]));
                                    if (ty.Value != null)
                                        info = ty.Value;
                                }
                            }

                            string msg = "";
                            // Ask documentation for the information
                            if (info != null && func != null) { //member function
                                msg = func.ReturnType.Name + " " + func.Name;
                                string m = IDEProject.inst().DocDatabase.GetDocumentationFor(info.Name + "::" + func.Name + func.Inner);
                                if (m != null)
                                    msg += "\r\n" + m;
                            } else if (func != null) { //global function
                                msg = func.ReturnType.Name + " " + func.Name;
                                string m = IDEProject.inst().DocDatabase.GetDocumentationFor(func.Name + func.Inner);
                                if (m != null)
                                    msg += "\r\n" + m;
                            } else if (info != null) { //global or member type
                                msg = info.Name;
                                string m = IDEProject.inst().DocDatabase.GetDocumentationFor(info.Name);
                                if (m != null)
                                    msg += "\r\n" + m;
                            }

                            if (msg.Length > 0) {
                                InsightWindow window = new InsightWindow(editor.TextArea);
                                window.Content = msg;
                                window.Show();
                            }
                        }
                    }
                }
                catch (Exception ex) { }
            }
        }

        void editor_KeyUp(object sender, KeyEventArgs e) {
            // These keys halt and terminate intellisense
            switch (e.Key) {
                case Key.Home:
                case Key.End:
                case Key.Left:
                case Key.Right:
                case Key.Escape:
                case Key.LWin:
                case Key.RWin:
                case Key.Space:
                    if (currentComp != null)
                        currentComp.Close();
                    return;
            }
            // These keys halt further checks
            switch (e.Key) {
                case Key.Up:
                case Key.Down:
                case Key.PageDown:
                case Key.PageUp:
                case Key.LeftShift:
                case Key.RightShift:
                case Key.LeftAlt:
                case Key.RightAlt:
                case Key.LeftCtrl:
                case Key.RightCtrl:
                case Key.Scroll:
                case Key.Capital:
                case Key.CrSel:
                case Key.Clear:
                case Key.Insert:
                case Key.PrintScreen:
                case Key.Print:
                    return;
            }

            char KEY = KeyHelpers.GetCharFromKey(e.Key);
            if (KEY == ')' || KEY == ';') {
                if (currentComp != null)
                    currentComp.Close();
                return;
            }
            int curOfs = editor.TextArea.Caret.Offset;
            int line = editor.TextArea.Caret.Line;

            // Do not attempt intellisense inside of comments
            string txt = editor.Document.GetText(editor.Document.Lines[editor.TextArea.Caret.Line - 1]);
            if (txt.Trim().StartsWith("//"))
                return;

            if (e.Key == Key.OemPeriod || KEY == ':') {
                //IntellisenseHelper.ResemblesDotPath(editor.Document, curOfs-1, line-1)) {
                int ofs = TextUtilities.GetNextCaretPosition(editor.Document, curOfs, LogicalDirection.Backward, CaretPositioningMode.WordStart);
                ofs = TextUtilities.GetNextCaretPosition(editor.Document, ofs, LogicalDirection.Backward, CaretPositioningMode.WordStart);
                string word = "";
                for (; ofs < curOfs; ++ofs) {
                    if (editor.Document.Text[ofs] != '.')
                        word += editor.Document.Text[ofs];
                }

                NameResolver reso = new NameResolver(IDEProject.inst().GlobalTypes, scanner);
                BaseTypeInfo info = null;
                string[] words = IntellisenseHelper.DotPath(editor.Document, curOfs - 1, line - 1);
                if (words.Length > 1) {
                    for (int i = 0; i < words.Length - 1; ++i) {
                        if (info == null) {
                            info = reso.GetClassType(editor.Document, editor.TextArea.Caret.Line, words[i]);
                        } else if (info != null) {
                            info = info.ResolvePropertyPath(IDEProject.inst().GlobalTypes, words[i]);
                            //if (info.Properties.ContainsKey(words[i]))
                            //    info = info.Properties[words[i]];
                        }
                    }
                }

                bool functionsOnly = false;
                //attempt to resolve it locally
                if (info == null)
                    info = reso.GetClassType(editor.Document, editor.TextArea.Caret.Line, word);
                //attempt to resolve it from globals
                if (info == null && IDEProject.inst().GlobalTypes != null && IDEProject.inst().GlobalTypes.Properties.ContainsKey(word))
                    info = IDEProject.inst().GlobalTypes.Properties[word];
                if (info == null && word.Contains("::")) {
                    if (IDEProject.inst().GlobalTypes == null)
                        return;
                    if (word.Length > 2) {
                        if (IDEProject.inst().GlobalTypes.Classes.ContainsKey(word.Replace("::", ""))) {
                            EnumInfo ti = IDEProject.inst().GlobalTypes.Classes[word.Replace("::", "")] as EnumInfo;
                            if (ti != null) {
                                currentComp = new CompletionWindow(editor.TextArea);
                                IList<ICompletionData> data = currentComp.CompletionList.CompletionData;
                                foreach (string str in ti.Values)
                                    data.Add(new BaseCompletionData(null, str));
                                currentComp.Show();
                                currentComp.Closed += comp_Closed;
                                return;
                            } else {
                                TypeInfo ty = IDEProject.inst().GlobalTypes.Classes.FirstOrDefault(p => p.Key.Equals(word.Replace("::", ""))).Value;
                                if (ty != null) {
                                    info = ty;
                                    functionsOnly = true;
                                }
                            }
                        } else { //list global functions
                            Globals globs = IDEProject.inst().GlobalTypes;
                            currentComp = new CompletionWindow(editor.TextArea);
                            IList<ICompletionData> data = currentComp.CompletionList.CompletionData;
                            foreach (string str in globs.Properties.Keys)
                                data.Add(new PropertyCompletionData(globs.Properties[str], str));
                            foreach (FunctionInfo fi in globs.Functions)
                                data.Add(new FunctionCompletionData(fi));
                            currentComp.Show();
                            currentComp.Closed += comp_Closed;
                            return;
                        }
                    }
                }

                //build the list
                if (info != null && info is TypeInfo) {
                    TypeInfo ti = info as TypeInfo;
                    currentComp = new CompletionWindow(editor.TextArea);
                    IList<ICompletionData> data = currentComp.CompletionList.CompletionData;
                    if (!functionsOnly) {
                        foreach (string str in ti.Properties.Keys)
                            data.Add(new PropertyCompletionData(ti.Properties[str], str, ti.ReadonlyProperties.Contains(str)));
                    }
                    foreach (FunctionInfo fi in ti.Functions)
                        data.Add(new FunctionCompletionData(fi));
                    currentComp.Show();
                    currentComp.Closed += comp_Closed;
                }
            } else if (KEY == '(' && IntellisenseHelper.ResemblesDotPath(editor.Document, curOfs - 2, line - 1)) {

                NameResolver reso = new NameResolver(IDEProject.inst().GlobalTypes, scanner);
                TypeInfo info = null;
                FunctionInfo func = null;
                string[] words = IntellisenseHelper.DotPath(editor.Document, curOfs - 2, line - 1);
                if (words.Length > 1) {
                    for (int i = 0; i < words.Length; ++i) {
                        if (i == words.Length - 1 && info != null) {
                            func = info.Functions.FirstOrDefault(f => f.Name.Equals(words[i]));
                        } else {
                            if (info == null) {
                                info = reso.GetClassType(editor.Document, editor.TextArea.Caret.Line, words[i]);
                            } else if (info != null) {
                                if (info.Properties.ContainsKey(words[i]))
                                    info = info.Properties[words[i]];
                            }
                        }
                    }
                }
                if (func != null) {
                    List<FunctionInfo> data = new List<FunctionInfo>();
                    foreach (FunctionInfo fi in info.Functions.Where(f => { return f.Name.Equals(func.Name); }))
                        data.Add(fi);
                    if (data.Count > 0) {
                        OverloadInsightWindow window = new OverloadInsightWindow(editor.TextArea);
                        window.Provider = new OverloadProvider(info, data.ToArray());
                        window.Show();
                        //compWindow.Closed += comp_Closed;
                    }
                } 
                else if (func == null && info == null) // Found nothing
                {
                    List<FunctionInfo> data = new List<FunctionInfo>();
                    foreach (FunctionInfo fi in IDEProject.inst().GlobalTypes.Functions.Where(f => { return f.Name.Equals(words[1]); }))
                        data.Add(fi);
                    if (data.Count > 0)
                    {
                        OverloadInsightWindow window = new OverloadInsightWindow(editor.TextArea);
                        window.Provider = new OverloadProvider(info, data.ToArray());
                        window.Show();
                        //compWindow.Closed += comp_Closed;
                    }
                }
            } else if (Char.IsLetter(KEY)) {
                if (currentComp != null || editor.TextArea.Caret.Line == 1)
                    return;

                int ofs = TextUtilities.GetNextCaretPosition(editor.Document, curOfs, LogicalDirection.Backward, CaretPositioningMode.WordStart);
                int nextOfs = TextUtilities.GetNextCaretPosition(editor.Document, ofs, LogicalDirection.Backward, CaretPositioningMode.WordStart);
                if (nextOfs > 0) {
                    if (editor.Document.Text[nextOfs] == '.')
                        return;
                }
                string word = "";
                if (ofs < 0)
                    return;
                for (; ofs < curOfs; ++ofs) {
                    if (editor.Document.Text[ofs] != '.')
                        word += editor.Document.Text[ofs];
                }
                if (word.Contains(".")) {
                    if (currentComp != null)
                        currentComp.Close();
                    //editor_KeyUp(sender, e);
                    return;
                }

                NameResolver reso = new NameResolver(IDEProject.inst().GlobalTypes, scanner);
                List<string> suggestions = new List<string>();
                reso.GetNameMatch(editor.Document, editor.TextArea.Caret.Line - 1, word, ref suggestions);

                CompletionWindow compWindow = new CompletionWindow(editor.TextArea);
                compWindow.StartOffset = TextUtilities.GetNextCaretPosition(editor.Document, curOfs, LogicalDirection.Backward, CaretPositioningMode.WordStart);
                IList<ICompletionData> data = compWindow.CompletionList.CompletionData;
                //attempt local name resolution first
                if (suggestions.Count > 0) {
                    foreach (string str in suggestions) //text suggestions are of lower priority
                        data.Add(new BaseCompletionData(null, str) { Priority = 0.5 });
                }
                //Scal globals
                if (IDEProject.inst().GlobalTypes != null) {
                    foreach (string str in IDEProject.inst().GlobalTypes.Classes.Keys) {
                        if (str.StartsWith(word))
                            data.Add(new ClassCompletionData(IDEProject.inst().GlobalTypes.Classes[str]));
                    }
                    foreach (FunctionInfo fun in IDEProject.inst().GlobalTypes.Functions) {
                        if (fun.Name.StartsWith(word))
                            data.Add(new FunctionCompletionData(fun));
                    }
                }
                if (data.Count > 0) {
                    currentComp = compWindow;
                    currentComp.Show();
                    currentComp.Closed += comp_Closed;
                }
            }
        }

        void comp_Closed(object sender, EventArgs e) {
            currentComp = null;
        }
        CompletionWindow currentComp = null;

        public DataChanged changeChecker { get; set; }

        public class DataChanged : BaseClass {
            public string code;
            public TextEditor editor;

            public void Recheck() {
                backing_ = !code.Equals(editor.Text);
                OnPropertyChanged("IsDirty");
            }

            bool backing_;
            public bool IsDirty {
                get { return backing_; }
                set {
                    backing_ = value;
                    OnPropertyChanged("IsDirty");
                }
            }
        }

        public bool IsDirty {
            get {
                if (item is FileLeafItem) {
                    return changeChecker.IsDirty;
                }
                return false;
            }
        }

        void editor_TextChanged(object sender, EventArgs e) {
            MainWindow.inst().Dispatcher.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate() {
                changeChecker.Recheck();
                DepthScanner newScan = new DepthScanner();
                newScan.Process(editor.Text);
                scanner = newScan;
            });
        }

        public TextEditor Editor { get { return editor; } }

        public void SetCode(FileBaseItem item) {
            try {
                this.item = item;
                editor.Text = File.ReadAllText(item.Path);
                changeChecker.code = editor.Text;
                changeChecker.IsDirty = false;
            }
            catch (Exception ex) {
                ErrorHandler.inst().Error(ex);
            }
        }

        public void Save() {
            File.WriteAllText(item.Path, editor.Text);
            changeChecker.code = editor.Text;
            MainWindow.inst().Dispatcher.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate() {
                changeChecker.Recheck();
            });
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e) {
            Save();
        }

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = true;
        }

        static string SOScript = @"class MY_SCRIPTOBJECT : ScriptObject
{
    //UNADVISED
    void Start() //called when we're made
    {
    }
    
    void Stop() //called right before we die
    {
    }
    
    //FAVOR THIS INSTEAD OF START
    void DelayedStart() //called after we've been fully init'd (after Start)
    {
    }
    //UNADVISED
    void Update(float td) //ad-hoc update method
    {
    }
    //UNADVISED
    void PostUpdate(float td) //ad-hoc post update method, called after everyone has Update'd
    {
    }
    
    void FixedUpdate(float td) //fixed interval update, use this preferably
    {
    }
    
    void FixedPostUpdate(float td) //See PostUpdate, only for FixedUpdate
    {
    }
    //UNADVISED
    void Save(Serializer& serializer) //Save to disk
    {
    }
    //UNADVISED
    void Load(Deserializer& deserializer) //Read from disk
    {
    }
    //UNADVISED
    void WriteNetworkUpdate(Serializer& serializer) //Save to network packet
    {
    }
    //UNADVISED
    void ReadNetworkUpdate(Deserializer& deserializer) //Read from network packet
    {
    }
    //UNADVISED
    void ApplyAttributes() //Called after are attributes have been set during load
    {
    }

    void TransformChanged() //Called whenever our node's position/rotation/scale changes
    {
    }
}";

        public static IHighlightingDefinition LoadHighlightingDefinition(string resourceName) {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            using (var reader = new XmlTextReader(stream))
                return HighlightingLoader.Load(reader, HighlightingManager.Instance);
        }
    }
}

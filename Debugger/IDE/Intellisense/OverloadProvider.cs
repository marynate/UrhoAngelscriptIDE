using ICSharpCode.AvalonEdit.CodeCompletion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace Debugger.IDE.Intellisense {

    // Used by AvalonEdit for handling function overloads

    public class OverloadProvider : IOverloadProvider {
        TypeInfo source;
        List<FunctionInfo> functions_;
        int index_ = 0;

        public OverloadProvider(TypeInfo source, params FunctionInfo[] funcs) {
            functions_ = new List<FunctionInfo>(funcs);
            this.source = source;
        }

        public int Count {
            get { return functions_.Count; }
        }

        public object CurrentContent {
            get {
                if (source != null)
                {
                    string docName = source.Name + "::" + functions_[SelectedIndex].Name + functions_[SelectedIndex].Inner;
                    string docText = IDEProject.inst().DocDatabase.GetDocumentationFor(docName);
                    if (docText != null)
                        return new TextBlock { Text = docText };
                    StackPanel stck = new StackPanel { Orientation = Orientation.Horizontal };
                    stck.Children.Add(new TextBlock { Text = "Undocumented" });
                    Button docBtn = new Button { Content = "Document" };
                    docBtn.Click += docBtn_Click;
                    stck.Children.Add(docBtn);
                    return stck;
                }
                else
                {
                    string docName = functions_[SelectedIndex].Name + functions_[SelectedIndex].Inner;
                    string docText = IDEProject.inst().DocDatabase.GetDocumentationFor(docName);
                    if (docText != null)
                        return new TextBlock { Text = docText };
                    StackPanel stck = new StackPanel { Orientation = Orientation.Horizontal };
                    stck.Children.Add(new TextBlock { Text = "Undocumented" });
                    Button docBtn = new Button { Content = "Document" };
                    docBtn.Click += docBtn_Click;
                    stck.Children.Add(docBtn);
                    return stck;
                }
            }
        }

        void docBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (source != null)
            {
                string docName = source.Name + "::" + functions_[SelectedIndex].Name + functions_[SelectedIndex].Inner;
                IDEProject.inst().DocDatabase.Document(docName);
            }
            else
            {
                string docName = functions_[SelectedIndex].Name + functions_[SelectedIndex].Inner;
                IDEProject.inst().DocDatabase.Document(docName);
            }
        }

        // Header appears to the right, show number of overloads
        public object CurrentHeader {
            get { return (SelectedIndex+1) + " of " + functions_.Count; }
        }

        public string CurrentIndexText {
            get { return string.Format("{0}{1}\r\n", functions_[SelectedIndex].Name, functions_[SelectedIndex].Inner); }
        }

        public int SelectedIndex {
            get {
                return index_;
            }
            set {
                index_ = value;
                PropertyChanged.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs("CurrentContent"));
                PropertyChanged.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs("CurrentHeader"));
                PropertyChanged.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs("CurrentIndexText"));
                PropertyChanged.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs("SelectedIndex"));
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
    }
}

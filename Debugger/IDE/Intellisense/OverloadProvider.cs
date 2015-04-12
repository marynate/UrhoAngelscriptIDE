using ICSharpCode.AvalonEdit.CodeCompletion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Debugger.IDE.Intellisense {

    // Used by AvalonEdit for handling function overloads

    public class OverloadProvider : IOverloadProvider {
        List<FunctionInfo> functions_;
        int index_ = 0;
        System.Windows.Controls.TextBlock currentText_;

        void UpdateText()
        {
            if (currentText_ != null)
                currentText_.Text = "Todo: Get function documentation"; 
        }

        public OverloadProvider(params FunctionInfo[] funcs) {
            functions_ = new List<FunctionInfo>(funcs);
        }

        public int Count {
            get { return functions_.Count; }
        }

        public object CurrentContent {
            get { 
                if (currentText_ == null) {
                    currentText_ = new System.Windows.Controls.TextBlock();
                }
                UpdateText();
                return currentText_;
                //return string.Format("{0}{1}\r\n", functions_[SelectedIndex].Name, functions_[SelectedIndex].Inner); 
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
                UpdateText();
                PropertyChanged.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs("CurrentHeader"));
                PropertyChanged.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs("CurrentIndexText"));
                PropertyChanged.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs("SelectedIndex"));
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
    }
}

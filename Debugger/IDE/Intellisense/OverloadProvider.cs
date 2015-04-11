using ICSharpCode.AvalonEdit.CodeCompletion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Debugger.IDE.Intellisense {
    public class OverloadProvider : IOverloadProvider {
        List<FunctionInfo> functions_;
        int index_ = 0;

        public OverloadProvider(params FunctionInfo[] funcs) {
            functions_ = new List<FunctionInfo>(funcs);
        }

        public int Count {
            get { return functions_.Count; }
        }

        public object CurrentContent {
            get { return string.Format("{0}{1}", functions_[SelectedIndex].Name, functions_[SelectedIndex].Inner); }
        }

        public object CurrentHeader {
            get { return functions_[0].Name; }
        }

        public string CurrentIndexText {
            get { return string.Format("{0}{1}", functions_[SelectedIndex].Name, functions_[SelectedIndex].Inner); }
        }

        public int SelectedIndex {
            get {
                return index_;
            }
            set {
                index_ = value;
                PropertyChanged.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs("SelectedIndex"));
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
    }
}

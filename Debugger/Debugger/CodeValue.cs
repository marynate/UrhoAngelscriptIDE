using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Debugger.Debug {
    [Serializable]
    public class CodeValue : BaseClass {
        string key_;
        string value_;
        ObservableCollection<CodeValue> children_;

        public string Key {
            get { return key_; }
            set { key_ = value; OnPropertyChanged("Key"); }
        }

        public string Value {
            get { return value_; }
            set { value_ = value; OnPropertyChanged("Value"); }
        }

        public ObservableCollection<CodeValue> Children {
            get { return children_; }
            set {
                children_ = value;
                OnPropertyChanged("Children");
            }
        }
    }
}

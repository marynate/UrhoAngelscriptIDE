using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Debugger {

    [Serializable]
    public class BaseClass : INotifyPropertyChanged {
        [XmlIgnore]
        public BaseClass Parent { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event 
        public virtual void OnPropertyChanged() {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(string.Empty));
            if (Parent != null)
                Parent.OnPropertyChanged();
        }

        // Create the OnPropertyChanged method to raise the event 
        public virtual void OnPropertyChanged(string name) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));
            if (Parent != null)
                Parent.OnPropertyChanged();
        }
    }

    public class NamedBaseClass : BaseClass {
        protected string name_;

        public string Name { get { return name_; } set { name_ = value; OnPropertyChanged("Name"); } }
    }
}

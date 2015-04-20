using PluginLib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace CSVEditorPlugin
{
    public class CSVControlData : BasePropertyBound, IExternalControlData
    {
        IFileEditor sourceEditor_;
        object control_;
        DataTable table_;
        bool dirty_ = false;
        string file_;

        public CSVControlData(string fileName, object control, DataTable table)
        {
            file_ = fileName;
            control_ = control;
            table_ = table;
            table_.RowChanged += table__RowChanged;
            table_.RowDeleted += table__RowDeleted;
            table_.TableNewRow += table__TableNewRow;
        }

        void table__TableNewRow(object sender, DataTableNewRowEventArgs e)
        {
            dirty_ = true;
            OnPropertyChanged("IsDirty");
        }

        void table__RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            dirty_ = true;
            OnPropertyChanged("IsDirty");
        }

        void table__RowChanged(object sender, DataRowChangeEventArgs e)
        {
            dirty_ = true;
            OnPropertyChanged("IsDirty");
        }

        public IFileEditor SourceEditor {
            get {
                return sourceEditor_;
            }
            set {
                sourceEditor_ = value; OnPropertyChanged("SourceEditor");
            }
        }

        public object Control {
            get {
                return control_;
            }
            set {
                control_ = value; OnPropertyChanged("Control");
            }
        }

        public bool IsDirty
        {
            get { return dirty_; }
            set { dirty_ = false; OnPropertyChanged("IsDirty"); }
        }

        public void SaveData()
        {
            IsDirty = false;
            DataTableBuilder.Write(table_, file_);
        }
    }
}

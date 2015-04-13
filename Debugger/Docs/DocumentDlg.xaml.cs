using FirstFloor.ModernUI.Windows.Controls;
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
using System.Windows.Shapes;

namespace Debugger.Docs
{
    public partial class DocumentDlg : ModernDialog
    {
        class DataItem : BaseClass
        {
            string docText_;

            public DataItem(string txt)
            {
                docText_ = txt;
            }

            public string Text { get { return docText_; } set { docText_ = value; OnPropertyChanged("Text"); } }
        }

        DataItem data;
        public DocumentDlg(string name, string curDoc)
        {
            data = new DataItem(curDoc);
            InitializeComponent();
            txtDocumenting.Text = name;
            txtContent.DataContext = data;

            Buttons = new Button[] {
                OkButton,
                CancelButton
            };
        }

        public string DocText { get { return data.Text; } }
    }
}

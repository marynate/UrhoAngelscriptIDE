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

namespace Debugger.Dlg {
    /// <summary>
    /// Dialog to be used for all (basic) cases of renaming something, such as files and folders
    /// </summary>
    public partial class RenameDlg : ModernDialog {
        public static string Show(string name) {
            RenameDlg dlg = new RenameDlg(name);
            if (dlg.ShowDialog() == true)
                return dlg.txtValue.Text.Trim();
            return "";
        }

        RenameDlg(string startname) {
            InitializeComponent();
            txtMsg.Text = string.Format("Rename {0} to:", startname);
            txtValue.Text = startname;
            txtValue.Focus();
            txtValue.SelectAll();
            Buttons = new Button[] {
                OkButton,
                CancelButton
            };
            foreach (Button btn in Buttons)
                btn.Style = FindResource("StyledButton") as Style;
        }
    }
}

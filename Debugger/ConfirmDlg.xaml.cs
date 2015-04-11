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

namespace Debugger {
    /// <summary>
    /// Interaction logic for DeleteDlg.xaml
    /// </summary>
    public partial class ConfirmDlg : ModernDialog {
        public static bool? Show(string msg) {
            ConfirmDlg dlg = new ConfirmDlg(msg);
            if (dlg.ShowDialog() == true)
                return true;
            return false;
        }

        public ConfirmDlg(string msg) {
            InitializeComponent();
            txtMsg.Text = msg;
            Buttons = new Button[] {
                new Button {
                    Content = "Confirm",
                    Style = FindResource("StyledButton") as Style
                },
                CancelButton
            };
            Buttons.Last().Style = FindResource("StyledButton") as Style;
            ((Button)Buttons.First()).Click += onOK;
        }

        void onOK(object sender, EventArgs e) {
            DialogResult = true;
            Close();
        }
    }
}

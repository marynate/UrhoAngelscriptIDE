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
    /// Interaction logic for InputDlg.xaml
    /// </summary>
    public partial class InputDlg : ModernDialog {

        public static string Show(string title, string prompt, string input = null) {
            InputDlg dlg = new InputDlg();
            dlg.Title = title;
            dlg.lblPrompt.Content = prompt;
            if (input != null) {
                dlg.txtInput.Text = input;
            }
            if (dlg.ShowDialog() == true) {
                return dlg.GetText();
            }
            return null;
        }

        public InputDlg() {
            InitializeComponent();
            Buttons = new Button[] {
                OkButton,
                CancelButton
            };
        }

        public string GetText() {return txtInput.Text;}
    }
}

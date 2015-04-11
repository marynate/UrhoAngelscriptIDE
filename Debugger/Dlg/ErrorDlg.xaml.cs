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
    /// Interaction logic for ErrorDlg.xaml
    /// </summary>
    public partial class ErrorDlg : ModernDialog {
        public static void Show(string aText, string aURL = "") {
            //MainWindow.Blur();
            ErrorDlg dlg = new ErrorDlg(aText, aURL);
            //dlg.MinWidth = MainWindow.GetSize().Width;
            dlg.ShowDialog();
            //MainWindow.UnBlur();
        }

        public ErrorDlg(string aErrorText, string aReportTo = "") {
            InitializeComponent();

            errorText.TextWrapping = TextWrapping.Wrap;
            errorText.Text = aErrorText;
            rptServerURL.Text = aReportTo;

            Button rptButton = new Button { Content = "Report" };
            rptButton.Click += onReportClick;
            Button closeButton = new Button { Content = "Close" };
            closeButton.Click += onClose;

            Buttons = new Button[] { 
                rptButton,
                closeButton
            };
            foreach (Button bt in Buttons)
                bt.Style = FindResource("StyledButton") as Style;
        }

        void onReportClick(object sender, EventArgs e) {
            Close();
        }

        void onClose(object sender, EventArgs e) {
            Close();
        }
    }
}

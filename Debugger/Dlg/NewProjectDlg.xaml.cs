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
    /// Interaction logic for NewProjectDlg.xaml
    /// </summary>
    public partial class NewProjectDlg : ModernDialog {
        public NewProjectDlg() {
            InitializeComponent();

            Buttons = new Button[] {
                OkButton,
                CancelButton
            };
            foreach (Button btn in Buttons)
                btn.Style = FindResource("StyledButton") as Style;
        }
    }
}

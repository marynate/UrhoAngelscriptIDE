using FirstFloor.ModernUI.Presentation;
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

namespace Debugger.IDE.Dlg {
    /// <summary>
    /// Interaction logic for NewFileDlg.xaml
    /// </summary>
    public partial class NewFileDlg : ModernDialog {
        public NewFileDlg() {
            InitializeComponent();
            Buttons = new Button[] {
                new Button {
                    Name = "btnCreate",
                    Content = "Create",
                    Style = FindResource("StyledButton") as Style,
                },
                CancelButton
            };
        }

        public string FileName { get { return txtFileName.Text; } }
        public string Type { get { return comboType.SelectedValue.ToString(); } }
    }
}

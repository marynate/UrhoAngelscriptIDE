using Debugger.IDE.Activity;
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

namespace Debugger.IDE.Dlg
{
    /// <summary>
    /// Interaction logic for CompSuccessDlg.xaml
    /// </summary>
    public partial class CompSuccessDlg : ModernDialog
    {
        public CompSuccessDlg()
        {
            InitializeComponent();
            BuildDumpActivity.CreateDumps();
        }
    }
}

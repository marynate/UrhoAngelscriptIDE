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

namespace Debugger.IDE {
    /// <summary>
    /// Interaction logic for IDESettingsDlg.xaml
    /// </summary>
    public partial class IDESettingsDlg : ModernDialog {
        public IDESettingsDlg() {
            InitializeComponent();
            txtCompileFile.DataContext = IDEProject.inst().Settings;
            txtDebugExe.DataContext = IDEProject.inst().Settings;
            txtDebugParams.DataContext = IDEProject.inst().Settings;
            txtRunExe.DataContext = IDEProject.inst().Settings;
            txtRunParams.DataContext = IDEProject.inst().Settings;
            txtSourceTree.DataContext = IDEProject.inst().Settings;
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Button btn = sender as Button;
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            if (dlg.ShowDialog() == true) {
                if (btn.Tag.Equals("run"))
                    IDEProject.inst().Settings.RunExe = dlg.FileName;
                else if (btn.Tag.Equals("debug"))
                    IDEProject.inst().Settings.DebugExe = dlg.FileName;
                else if (btn.Tag.Equals("compile"))
                    IDEProject.inst().Settings.CompilerPath = dlg.FileName;
                else if (btn.Tag.Equals("sourcetree"))
                    IDEProject.inst().Settings.SourceTree = dlg.FileName;
            }
        }
    }
}

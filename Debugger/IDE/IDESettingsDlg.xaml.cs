using FirstFloor.ModernUI.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public class Compiler : BaseClass {
            string name_;
            string value_;

            public string Name { get { return name_; } set { name_= value; OnPropertyChanged("Name"); } }
            public string Value { get { return value_; } set { value_ = value; OnPropertyChanged("Value"); } }
        }

        ObservableCollection<Compiler> compilers_ = new ObservableCollection<Compiler>();

        public IDESettingsDlg() {
            foreach (PluginLib.ICompilerService compiler in PluginManager.inst().Compilers)
                compilers_.Add(new Compiler { Name = compiler.Name, Value = compiler.Name });

            InitializeComponent();
            txtCompileFile.DataContext = IDEProject.inst().Settings;
            txtDebugExe.DataContext = IDEProject.inst().Settings;
            txtDebugParams.DataContext = IDEProject.inst().Settings;
            txtRunExe.DataContext = IDEProject.inst().Settings;
            txtRunParams.DataContext = IDEProject.inst().Settings;
            txtSourceTree.DataContext = IDEProject.inst().Settings;
            comboCompile.ItemsSource = compilers_;
            comboCompile.DataContext = IDEProject.inst().Settings;
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

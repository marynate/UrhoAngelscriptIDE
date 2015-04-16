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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Debugger.Screens {
    /// <summary>
    /// Interaction logic for LaunchScreen.xaml
    /// </summary>
    public partial class LaunchScreen : UserControl {
        public LaunchScreen() {
            InitializeComponent();
            Net.DebugClient client = new Net.DebugClient(new Debugger.Debug.SessionData(txtAddr.Text));
            foreach (string str in UserData.inst().RecentFiles) {
                Button tb = new Button { Content = str, Tag = str };
                tb.Style = this.FindResource("StyledButton") as Style;
                recentFiles.Children.Add(tb);
                tb.Click += tb_Click;
            }
        }

        void tb_Click(object sender, RoutedEventArgs e) {
            Button btn = sender as Button;
            //IDE.IDEProject.open(btn.Tag.ToString());
            IDE.IDEProject.open();
            IDE.IDEProject.inst().ProjectDir = btn.Tag.ToString();
            IDE.IDEProject.inst().Settings = IDE.IDESettings.GetOrCreate(btn.Tag.ToString());
            MainWindow.inst().ContentSource = new Uri("IDE/IDEView.xaml", UriKind.Relative);
        }

        void onConnect(object sender, EventArgs args) {
            MainWindow.inst().ContentSource = new Uri("Screens/DebugScreen.xaml", UriKind.Relative);
            Net.DebugClient.inst().Connect(txtAddr.Text);
        }

        private void btnNewProject_Click(object sender, RoutedEventArgs e) {
            MainWindow.inst().ContentSource = new Uri("IDE/IDEView.xaml", UriKind.Relative);
        }

        private void btnOpenProject_Click(object sender, RoutedEventArgs e) {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = "prj";
            dlg.Filter = "Project files (*.prj)|*.prj";
            if (dlg.ShowDialog() == true) {
                IDE.IDEProject.open(dlg.FileName);
                MainWindow.inst().Content = new Uri("IDE/IDEView.xaml", UriKind.Relative);
            }
        }
    }
}

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

namespace Debugger.Controls {
    /// <summary>
    /// Interaction logic for JWrapView.xaml
    /// </summary>
    public partial class JWrapView : UserControl {
        public JWrapView() {
            InitializeComponent();
            PrefixWatches = "";
        }

        public string PrefixWatches { get; set; }
        public TreeView View { get { return globalTree; } }

        private void MenuItem_Click(object sender, RoutedEventArgs e) {
            MenuItem item = sender as MenuItem;
            ContextMenu menu = item.CommandParameter as ContextMenu;
            Json.JLeaf target = (menu.PlacementTarget as StackPanel).Tag as Json.JLeaf;
            if (target != null) {
                Debugger.Debug.WatchValue watch = new Debugger.Debug.WatchValue { Variable = PrefixWatches + target.GetDotPath() };
                Debugger.Debug.SessionData.inst().AddWatch(watch);
            }
        }
    }
}

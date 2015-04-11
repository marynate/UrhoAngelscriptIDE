using FirstFloor.ModernUI.Presentation;
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
    /// Interaction logic for AltDocViewer.xaml
    /// </summary>
    public partial class AltDocViewer : UserControl {
        public AltDocViewer() {
            InitializeComponent();
            tree.PreviewMouseRightButtonDown += tree_PreviewMouseRightButtonDown;
        }

        public string[] CommandText { get; set; }
        public string[] LowerText { get; set; }

        public string[] CommandFormats { get; set; }
        public string[] LowerCommands { get; set; }

        private void tree_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e) {
            TreeViewItem treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);
            if (treeViewItem != null) {
                treeViewItem.Focus();
                //e.Handled = true;
                Debugger.IDE.API.APINode nd = treeViewItem.DataContext as Debugger.IDE.API.APINode;
                if (nd.Context.Count > 0) {
                    ContextMenu cmenu = new ContextMenu();
                    TextBlock os = e.OriginalSource as TextBlock;
                    for (int i = 0; i < nd.Context.Count; ++i) {
                        string f = nd.Context[i].Value;
                        cmenu.Items.Add(new MenuItem {
                            Header = nd.Context[i].Key,
                            Command = new RelayCommand(p =>
                            {
                                if (os.Text.Contains(":"))
                                    System.Windows.Clipboard.SetText(string.Format(f, os.Text.Substring(0, os.Text.IndexOf(":")).Trim()));
                                else
                                    System.Windows.Clipboard.SetText(string.Format(f, os.Text.Trim()));
                            })
                        });
                    }
                    treeViewItem.ContextMenu = cmenu;
                }
            }
        }

        static TreeViewItem VisualUpwardSearch(DependencyObject source) {
            while (source != null && !(source is TreeViewItem))
                source = VisualTreeHelper.GetParent(source);

            return source as TreeViewItem;
        }


        public TreeView Tree { get { return tree; } }
    }
}

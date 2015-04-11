using FirstFloor.ModernUI.Windows;
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
using System.IO;
using System.Collections.ObjectModel;
using FirstFloor.ModernUI.Windows.Controls;
using System.Diagnostics;

namespace Debugger.IDE {
    /// <summary>
    /// Interaction logic for IDEView.xaml
    /// </summary>
    public partial class IDEView : UserControl, IContent {
        IDEProject project_;
        Folder folder_;
        ObservableCollection<SearchResult> searchResults_ = new ObservableCollection<SearchResult>();

        public ObservableCollection<SearchResult> SearchResults { get { return searchResults_; } }

        static IDEView inst_;
        public static IDEView inst() { return inst_; }

        public IDEView() {
            InitializeComponent();
            inst_ = this;
            gridSearch.DataContext = this;
        }

        //Compile the current file only
        void onCompileFile(object sender, EventArgs e) {
        }

        //Compile everything
        void onCompile(object sender, EventArgs e) {
        }

        public void OnFragmentNavigation(FirstFloor.ModernUI.Windows.Navigation.FragmentNavigationEventArgs e) {
        }

        public void OnNavigatedFrom(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e) {
        }

        public void OnNavigatedTo(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e) {
            if (IDEProject.inst() == null) {
                IDEProject.open();
                System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog();
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                    IDEProject.inst().Settings = IDESettings.GetOrCreate(dlg.SelectedPath);
                    IDEProject.inst().ProjectDir = dlg.SelectedPath;
                    UserData.inst().AddRecentFile(dlg.SelectedPath);
                } else {
                    MainWindow.inst().ContentSource = new Uri("Screens/LaunchScreen.xaml", UriKind.Relative);
                    IDEProject.inst().destroy();
                    return;
                }
            }

            if (IDEProject.inst() == null) {
                project_ = new IDEProject();
            }  else
                project_ = IDEProject.inst();
            if (folder_ == null)
                folder_ = new Folder { Path = project_.ProjectDir };
            fileTree.DataContext = folder_;
            objectTree.DataContext = IDEProject.inst();
            txtConsole.DataContext = IDEProject.inst();
            gridErrors.DataContext = IDEProject.inst();
            errorTabCount.DataContext = IDEProject.inst();
            stackErrorHeader.DataContext = IDEProject.inst();

            eventsDoc.Tree.DataContext = IDEProject.inst().Documentation.DocumentNode.Children[0];
            eventsDoc.CommandText = new string[] { "Copy Subscription", "Copy Unsubscription" };
            eventsDoc.CommandFormats = new string[] {"SubscribeToEvent(\"{0}\",\"Handle{0}\");", "UnsubscribeFromEvent(\"Handle{0}\");" };
            eventsDoc.LowerText = new string[] { "Copy event getter" };
            eventsDoc.LowerCommands = new string[] { "eventData[\"{0}\"];" };

            attrDoc.Tree.DataContext = IDEProject.inst().Documentation.DocumentNode.Children[1];
            scriptDoc.Tree.DataContext = IDEProject.inst().Documentation.DocumentNode.Children[2];
            //eventsDoc.MiddleMenuBuilder = new Controls.EventSubscriptionDocMenu();
            //eventsDoc.BottomMenuBuilder = new Controls.EventDataDocMenu();
            //eventsDoc.Root = IDEProject.inst().Documentation.DocumentNode.Children[0];
            //attrDoc.Shallow = true;
            //attrDoc.MiddleMenuBuilder = new Controls.AttrDocMenu();
            //attrDoc.Root = IDEProject.inst().Documentation.DocumentNode.Children[1];
            //scriptDoc.Shallow = true;
            //scriptDoc.Root = IDEProject.inst().Documentation.DocumentNode.Children[2];
        }

        public void OnNavigatingFrom(FirstFloor.ModernUI.Windows.Navigation.NavigatingCancelEventArgs e) {
        }


        private void fileTree_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FileBaseItem item = fileTree.SelectedItem as FileBaseItem;
            if (item is FileLeafItem) {
                if (item.Path.EndsWith(".as") || item.Path.EndsWith(".xml") || item.Path.EndsWith(".csv") || item.Path.EndsWith(".txt"))
                    ideTabs.OpenFile(item);
            }
        }

        void onNewFolder(object sender, EventArgs e) {
            MenuItem item = sender as MenuItem;
            ContextMenu menu = item.CommandParameter as ContextMenu;
            FileBaseItem target = (menu.PlacementTarget as StackPanel).Tag as FileBaseItem;

            string r = Debugger.Dlg.InputDlg.Show("Create Folder", "Name of new folder:");
            if (r != null) {
                System.IO.Directory.CreateDirectory(System.IO.Path.Combine(target.Path, r));
            }
        }

        void onNewFile(object sender, EventArgs e) {
            MenuItem item = sender as MenuItem;
            ContextMenu menu = item.CommandParameter as ContextMenu;
            FileBaseItem target = (menu.PlacementTarget as StackPanel).Tag as FileBaseItem;

            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.InitialDirectory = target.Path;
            dlg.DefaultExt = "as";
            dlg.Filter = "Script (*.as)|*.as|Material (*.xml)|*.xml";
            if (dlg.ShowDialog() == true) {
                File.WriteAllText(dlg.FileName, "");
                ideTabs.OpenFile(new FileBaseItem { Path = dlg.FileName, Name = dlg.FileName });
            }
        }

        void onEditFile(object sender, EventArgs e) {
            MenuItem item = sender as MenuItem;
            ContextMenu menu = item.CommandParameter as ContextMenu;
            FileBaseItem target = (menu.PlacementTarget as StackPanel).Tag as FileBaseItem;
            if (target is FileLeafItem)
                if (target.Path.EndsWith(".as") || target.Path.EndsWith(".xml") || target.Path.EndsWith(".csv") || target.Path.EndsWith(".txt"))
                    ideTabs.OpenFile(target);
        }

        void onRenameFolder(object sender, EventArgs e) {
            MenuItem item = sender as MenuItem;
            ContextMenu menu = item.CommandParameter as ContextMenu;
            FileBaseItem target = (menu.PlacementTarget as StackPanel).Tag as FileBaseItem;
            string newName = RenameDlg.Show(target.Name);
            if (newName.Length > 0) {
                try {
                    string dir = System.IO.Path.GetDirectoryName(target.Path);
                    Directory.Move(target.Path, System.IO.Path.Combine(dir, newName));
                } catch (Exception ex) {
                    ErrorHandler.inst().Error(ex);
                }
            }
        }
        void onRenameFile(object sender, EventArgs e) {
            MenuItem item = sender as MenuItem;
            ContextMenu menu = item.CommandParameter as ContextMenu;
            FileBaseItem target = (menu.PlacementTarget as StackPanel).Tag as FileBaseItem;
            string newName = RenameDlg.Show(target.Name);
            if (newName.Length > 0) {
                try {
                    string dir = System.IO.Path.GetDirectoryName(target.Path);
                    File.Move(target.Path, System.IO.Path.Combine(dir, newName));
                } catch (Exception ex) {
                    ErrorHandler.inst().Error(ex);
                }
            }
        }
        void onDeleteFile(object sender, EventArgs e) {
            MenuItem item = sender as MenuItem;
            ContextMenu menu = item.CommandParameter as ContextMenu;
            FileBaseItem target = (menu.PlacementTarget as StackPanel).Tag as FileBaseItem;
            if (ConfirmDlg.Show(string.Format("Delete {1} '{0}'?", target.Name, (target is Folder) ? "folder" : "file")) == true) {
                try {
                    if (target.Parent is Folder)
                        ((Folder)target.Parent).Children.Remove(target);
                    FileOperationAPIWrapper.MoveToRecycleBin(target.Path);
                } catch (Exception ex) {
                    ErrorHandler.inst().Error(ex);
                }
            }
        }

        private void txtSearchString_PreviewKeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                Activity.SearchActivity.doSearch(txtSearchString.Text, IDEProject.inst().ProjectDir);
                Activity.IDBBuilderActivity.BuildIntellisenseDatabase();
            }
        }

        void searchDoubleClick(object sender, MouseEventArgs args) {
            DataGridRow row = sender as DataGridRow;
            SearchResult result = row.DataContext as SearchResult;
            ideTabs.OpenFile(new FileLeafItem {
                Path = result.File,
                Name = System.IO.Path.GetFileName(result.File)
            }, result.Line);
        }

        private void txtSearchClasses_PreviewKeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                if (txtSearchClasses.Text.Trim().Length > 0) {
                    string searchString = txtSearchClasses.Text.Trim().ToLower();

                    foreach (object o in objectTree.Items) {
                        if (o is TypeInfo) {
                            if (((TypeInfo)o).Name.ToLower().Equals(searchString)) {
                                TreeViewItem item = ((TreeViewItem)objectTree.ItemContainerGenerator.ContainerFromItem(o));
                                item.BringIntoView();
                                item.IsSelected = true;
                                return;
                            }
                        }
                    }
                }
            }
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e) {
            IDESettingsDlg dlg = new IDESettingsDlg();
            dlg.ShowDialog();
        }

        private void txtSearchClasses_GotFocus(object sender, RoutedEventArgs e) {
            txtSearchClasses.SelectAll();
        }

        private void errorDoubleClick(object sender, MouseEventArgs args) {
            DataGridRow row = sender as DataGridRow;
            CompileError result = row.DataContext as CompileError;
            IDEEditor editor = ideTabs.OpenFile(new FileLeafItem {
                Path = result.File,
                Name = result.File.Replace(IDEProject.inst().ProjectDir, "")
            });
            if (result.Line != -1) {
                editor.Editor.TextArea.Caret.Line = result.Line;
                editor.Editor.ScrollToLine(result.Line);
            }
        }

        private void btnCompile_Click(object sender, RoutedEventArgs e) {
            Compile();
        }

        public static void Compile() {
            if (IDEProject.inst().Settings.CompilerPath == null || IDEProject.inst().Settings.CompilerPath.Trim().Length == 0) {
                if (ModernDialog.ShowMessage("You need to set a compile file in settings", "No princess here", System.Windows.MessageBoxButton.OKCancel) == MessageBoxResult.OK) {
                    IDESettingsDlg dlg = new IDESettingsDlg();
                    dlg.ShowDialog();
                }
                return;
            }
            IDEProject.inst().CompilerOutput = "";
            IDEProject.inst().CompileErrors.Clear();
            Activity.CompilerActivity.Compile(IDEProject.inst().Settings.CompilerPath);
            if (IDEProject.inst().CompileErrors.Count != 0) {
                Dlg.CompErrDlg dlg = new Dlg.CompErrDlg();
                dlg.ShowDialog();
            }
            else
            {
                Dlg.CompSuccessDlg dlg = new Dlg.CompSuccessDlg();
                dlg.ShowDialog();
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e) {

        }

        private void btnRun_Click(object sender, RoutedEventArgs e) {
            if (IDEProject.inst().Settings.RunExe == null || IDEProject.inst().Settings.RunExe.Trim().Length == 0) {
                if (ModernDialog.ShowMessage("You need to set a 'run' target", "Run what?", MessageBoxButton.OKCancel) == MessageBoxResult.OK) {
                    btnSettings_Click(null, null);
                }
                return;
            }
            Process pi = new Process();
            pi.StartInfo.FileName = IDEProject.inst().Settings.RunExe;
            pi.StartInfo.Arguments = IDEProject.inst().Settings.CompilerPath + " " + IDEProject.inst().Settings.RunParams;
            pi.EnableRaisingEvents = true;
            pi.StartInfo.UseShellExecute = false;
            pi.StartInfo.CreateNoWindow = false;
            pi.StartInfo.RedirectStandardOutput = false;
            pi.Start();
        }

        private void btnDebug_Click(object sender, RoutedEventArgs e) {
            if (IDEProject.inst().Settings.RunExe == null || IDEProject.inst().Settings.RunExe.Trim().Length == 0) {
                if (ModernDialog.ShowMessage("You need to set a 'debug' target", "Debug what?", MessageBoxButton.OKCancel) == MessageBoxResult.OK) {
                    btnSettings_Click(null, null);
                }
                return;
            }
            Process pi = new Process();
            pi.StartInfo.FileName = IDEProject.inst().Settings.DebugExe;
            pi.StartInfo.Arguments = IDEProject.inst().Settings.CompilerPath + " " + IDEProject.inst().Settings.DebugParams;
            pi.EnableRaisingEvents = true;
            pi.StartInfo.UseShellExecute = false;
            pi.StartInfo.CreateNoWindow = false;
            pi.StartInfo.RedirectStandardOutput = false;
            pi.Start();
        }
    }
}
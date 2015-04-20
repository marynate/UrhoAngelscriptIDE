using FirstFloor.ModernUI.Windows.Controls;
using ICSharpCode.AvalonEdit;
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

namespace Debugger.IDE {
    /// <summary>
    /// Interaction logic for IDETabs.xaml
    /// </summary>
    public partial class IDETabs : UserControl {
        public IDETabs() {
            InitializeComponent();
        }

        public IDEEditor OpenFile(FileBaseItem aFile, int aLine)
        {
            if (aFile.Name.Contains(".as"))
            {
                IDEEditor ret = OpenFile(aFile);
                ret.Editor.TextArea.Caret.Line = aLine;
                ret.Editor.ScrollToLine(aLine);
                ret.InvalidateArrange();
                return ret;
            }
            return null;
        }

        public IDEEditor OpenFile(FileBaseItem aFile) {
            if (aFile.Name.Contains(".as") || aFile.Name.Contains(".txt"))
            {
                foreach (TabItem item in tabs.Items)
                {
                    if (item.Tag.Equals(aFile.Path))
                    {
                        tabs.SelectedItem = item;
                        ((IDEEditor)((TabItem)tabs.SelectedItem).Content).SetCode(aFile);
                        return ((IDEEditor)((TabItem)tabs.SelectedItem).Content);
                    }
                }
                Grid grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition());

                IDEEditor ideEditor = new IDEEditor(aFile);

                TextBlock txt = new TextBlock { Text = aFile.Name };
                txt.DataContext = ideEditor.changeChecker;
                txt.Foreground = FindResource("ButtonText") as Brush;
                txt.Style = FindResource("IDETabHeader") as Style;

                grid.Children.Add(txt);
                Button close = new Button { Content = "X", Padding = new Thickness(0), Foreground = new SolidColorBrush(Colors.LightGray), FontWeight = FontWeights.Bold, VerticalAlignment = System.Windows.VerticalAlignment.Top, HorizontalAlignment = System.Windows.HorizontalAlignment.Right };
                close.MinHeight = close.MinWidth = 18;
                close.MaxHeight = close.MaxWidth = 18;
                close.Background = close.BorderBrush = null;
                close.Click += onCloseTab;
                Grid.SetColumn(txt, 0);
                Grid.SetColumn(close, 1);
                grid.Children.Add(close);

                tabs.Items.Add(new TabItem
                {
                    Tag = aFile.Path,
                    Header = grid,
                    Content = ideEditor,
                });
                ((TabItem)tabs.Items[tabs.Items.Count - 1]).MouseUp += EditorTabs_MouseUp;
                tabs.SelectedItem = tabs.Items[tabs.Items.Count - 1];
                return ((IDEEditor)((TabItem)tabs.SelectedItem).Content);
            }
            else
            {
                foreach (PluginLib.IFileEditor editor in PluginManager.inst().FileEditors)
                {
                    if (editor.CanEditFile(aFile.Path, System.IO.Path.GetExtension(aFile.Name)))
                    {
                        object ud = null;
                        PluginLib.IExternalControlData externalControl = editor.CreateEditorContent(aFile.Path);

                        Grid grid = new Grid();
                        grid.ColumnDefinitions.Add(new ColumnDefinition());
                        grid.ColumnDefinitions.Add(new ColumnDefinition());

                        TextBlock txt = new TextBlock { Text = aFile.Name };
                        txt.DataContext = externalControl;
                        txt.Foreground = FindResource("ButtonText") as Brush;
                        txt.Style = FindResource("IDETabHeader") as Style;

                        grid.Children.Add(txt);
                        Button close = new Button { Content = "X", Padding = new Thickness(0), Foreground = new SolidColorBrush(Colors.LightGray), FontWeight = FontWeights.Bold, VerticalAlignment = System.Windows.VerticalAlignment.Top, HorizontalAlignment = System.Windows.HorizontalAlignment.Right };
                        close.MinHeight = close.MinWidth = 18;
                        close.MaxHeight = close.MaxWidth = 18;
                        close.Background = close.BorderBrush = null;
                        close.Click += onCloseTab;
                        Grid.SetColumn(txt, 0);
                        Grid.SetColumn(close, 1);
                        grid.Children.Add(close);

                        tabs.Items.Add(new TabItem
                        {
                            Tag = aFile.Path,
                            Header = grid,
                            Content = externalControl.Control,
                        });
                        ((TabItem)tabs.Items[tabs.Items.Count - 1]).MouseUp += EditorTabs_MouseUp;
                        tabs.SelectedItem = tabs.Items[tabs.Items.Count - 1];
                        return null;
                    }
                }
                return null;
            }
        }

        void onCloseTab(object sender, EventArgs e) {
            TabItem item = ((Grid)((Button)sender).Parent).Parent as TabItem;
            if (item != null) {
                IDEEditor editor = item.Content as IDEEditor;
                if (editor != null)
                {
                    if (editor.IsDirty)
                    {
                        MessageBoxResult res = ModernDialog.ShowMessage("Save file changes before closing?", "Close?", MessageBoxButton.YesNoCancel);
                        if (res == MessageBoxResult.Yes)
                        {
                            editor.Save();
                        }
                        else if (res == MessageBoxResult.Cancel)
                            return;
                    }
                }
                else
                {
                    if (item.Content is Control && ((Control)item.Content).Tag is PluginLib.IExternalControlData) {
                        PluginLib.IExternalControlData data = ((Control)item.Content).Tag as PluginLib.IExternalControlData;
                        if (data.IsDirty)
                        {
                            MessageBoxResult res = ModernDialog.ShowMessage("Save file changes before closing?", "Close?", MessageBoxButton.YesNoCancel);
                            if (res == MessageBoxResult.Yes)
                            {
                                data.SaveData();
                            }
                            else if (res == MessageBoxResult.Cancel)
                                return;
                        }
                    }
                }
                tabs.Items.Remove(item);
            }
        }

        void EditorTabs_MouseUp(object sender, MouseButtonEventArgs e) {
            if (e.ChangedButton == MouseButton.Middle) {
                if (sender is TabItem) {
                    IDEEditor editor = (sender as TabItem).Content as IDEEditor;
                    TabItem item = sender as TabItem;
                    if (editor != null)
                    {
                        if (editor.IsDirty)
                        {
                            MessageBoxResult res = ModernDialog.ShowMessage("Save file changes before closing?", "Close?", MessageBoxButton.YesNoCancel);
                            if (res == MessageBoxResult.OK)
                            {
                                editor.Save();
                            }
                            else if (res == MessageBoxResult.Cancel)
                                return;
                        }
                    }
                    else
                    {
                        if (item.Content is Control && ((Control)item.Content).Tag is PluginLib.IExternalControlData)
                        {
                            PluginLib.IExternalControlData data = ((Control)item.Content).Tag as PluginLib.IExternalControlData;
                            if (data.IsDirty)
                            {
                                MessageBoxResult res = ModernDialog.ShowMessage("Save file changes before closing?", "Close?", MessageBoxButton.YesNoCancel);
                                if (res == MessageBoxResult.Yes)
                                {
                                    data.SaveData();
                                }
                                else if (res == MessageBoxResult.Cancel)
                                    return;
                            }
                        }
                    }
                    tabs.Items.Remove(sender);
                }
            }
        }
    }
}

using Debugger.Debug;
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

namespace Debugger.Editor {
    /// <summary>
    /// Interaction logic for EditorTabs.xaml
    /// </summary>
    public partial class EditorTabs : UserControl {
        public EditorTabs() {
            InitializeComponent();
        }

        public CodeEditor OpenFile(FileData aData) {
            foreach (TabItem item in tabs.Items) {
                if (item.Tag.Equals(aData.SectionName)) {
                    tabs.SelectedItem = item;
                    ((CodeEditor)((TabItem)tabs.SelectedItem).Content).SetCode(aData.Code);
                    return ((CodeEditor)((TabItem)tabs.SelectedItem).Content);
                }
            }
            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());

            TextBlock txt = new TextBlock { Text = aData.SectionName };
            txt.Foreground = FindResource("ButtonText") as Brush;
            txt.Style = FindResource("IDETabHeader") as Style;

            grid.Children.Add(txt);
            Button close = new Button { Content = "X", Padding = new Thickness(0), Foreground = new SolidColorBrush(Colors.LightGray), FontWeight = FontWeights.Bold, VerticalAlignment = System.Windows.VerticalAlignment.Top };
            close.MinHeight = close.MinWidth = 18;
            close.MaxHeight = close.MaxWidth = 18;
            close.Background = close.BorderBrush = null;
            close.Click += onCloseTab;
            Grid.SetColumn(txt, 0);
            Grid.SetColumn(close, 1);
            grid.Children.Add(close);

            tabs.Items.Add(new TabItem {
                 Tag = aData.SectionName,
                 Header = grid,
                 Content = new CodeEditor(aData),
            });
            ((TabItem)tabs.Items[tabs.Items.Count - 1]).MouseUp += EditorTabs_MouseUp;
            tabs.SelectedItem = tabs.Items[tabs.Items.Count - 1];
            return ((CodeEditor)((TabItem)tabs.SelectedItem).Content);
        }

        void EditorTabs_MouseUp(object sender, MouseButtonEventArgs e) {
            if (e.ChangedButton == MouseButton.Middle) {
                if (sender is TabItem) {
                    tabs.Items.Remove(sender);
                }
            }
        }

        public void OpenFile(int aSectionID, int aLine) {
            Debugger.Debug.FileData fileData = Debugger.Debug.SessionData.inst().Files.FirstOrDefault(file => file.SectionID == aSectionID);
            if (fileData != null) {
                CodeEditor editor = OpenFile(fileData);
                editor.Editor.TextArea.Caret.Line = aLine;
                editor.Editor.ScrollToLine(aLine);
                editor.InvalidateVisual();
            }
        }

        void onCloseTab(object sender, EventArgs e) {
            TabItem item = ((Grid)((Button)sender).Parent).Parent as TabItem;
            if (item != null) {
                tabs.Items.Remove(item);
            }
        }
    }
}

using FirstFloor.ModernUI.Windows.Controls;
using ICSharpCode.AvalonEdit;
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

namespace Debugger.IDE.Snippets {
    /// <summary>
    /// Interaction logic for SnippetDlg.xaml
    /// </summary>
    public partial class SnippetDlg : ModernDialog {
        Dictionary<string, string> inputs_ = new Dictionary<string, string>();

        public ObservableCollection<CodeSnippet> Snippets { get; set; }

        TextEditor editor_;
        public SnippetDlg(TextEditor aEditor, ObservableCollection<CodeSnippet> snippets) {
            InitializeComponent();
            Snippets = snippets;
            editor_ = aEditor;

            Buttons = new Button[] {
                new Button {
                    Name = "btnCreate",
                    Content = "Insert",
                    Style = FindResource("StyledButton") as Style,
                },
                CancelButton
            };
            Buttons.First().Click += SnippetDlg_Click;
            snippetCombo.DataContext = this;

        }

        void SnippetDlg_Click(object sender, RoutedEventArgs e) {
            string code = (snippetCombo.SelectedItem as CodeSnippet).CreateCode(inputs_);
            editor_.Document.BeginUpdate();
            editor_.Document.Insert(editor_.CaretOffset, code, ICSharpCode.AvalonEdit.Document.AnchorMovementType.AfterInsertion);
            editor_.Document.EndUpdate();
            this.Close();
        }


        private void snippetCombo_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            snippetInputs.Children.Clear();
            inputs_.Clear();
            CodeSnippet snip = snippetCombo.SelectedItem as CodeSnippet;
            foreach (CodeSnippetInput input in snip.Inputs) {
                StackPanel pnl = new StackPanel { Orientation = Orientation.Horizontal };
                if (input is CodeSnippetOption)
                {
                    CheckBox cb = new CheckBox { Content = input.Name, Margin = new Thickness(5.0), Tag = input.Key };
                    cb.Checked += cb_Checked;
                    pnl.Children.Add(cb);
                }
                else
                {
                    Label lbl = new Label { Content = input.Name, Margin = new Thickness(5.0) };
                    TextBox tb = new TextBox { Tag = input.Name, MinWidth = 160 };
                    tb.TextChanged += tb_TextChanged;
                    pnl.Children.Add(lbl);
                    pnl.Children.Add(tb);
                }
                snippetInputs.Children.Add(pnl);
            }
        }

        void cb_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox src = sender as CheckBox;
            inputs_[src.Tag.ToString()] = src.IsChecked.ToString().ToLower();
        }

        void tb_TextChanged(object sender, TextChangedEventArgs e) {
            TextBox src = sender as TextBox;
            inputs_[src.Tag.ToString()] = src.Text.Trim();
        }
    }
}

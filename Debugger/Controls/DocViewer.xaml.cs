using Debugger.IDE.API;
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
    /// Interaction logic for DocViewer.xaml
    /// </summary>
    public partial class DocViewer : UserControl {
        public DocViewer() {
            InitializeComponent();
        }

        public IDocMenuBuilder TopMenuBuilder { get; set; }
        public IDocMenuBuilder MiddleMenuBuilder { get; set; }
        public IDocMenuBuilder BottomMenuBuilder { get; set; }

        public bool Shallow { get; set; }
        List<TextBlock> searchables_ = new List<TextBlock>();
        IDE.API.APINode node_;
        public IDE.API.APINode Root { get { return node_; } set { node_ = value; Rebuild(); } }

        void Rebuild() {
            searchables_.Clear();
            contentStack.Children.Clear();
            foreach (APINode section in node_.Children) {

                if (section.Children.Count == 0) {
                    TextBlock header = new TextBlock { Text = section.Name };
                    if (TopMenuBuilder != null)
                        header.ContextMenu = TopMenuBuilder.BuildContextMenuFor(section);
                    header.Margin = new Thickness(25, 2, 2, 0);
                    contentStack.Children.Add(header);
                    searchables_.Add(header);
                } else {
                    Expander exp = new Expander();
                    TextBlock header = new TextBlock {
                        Text = section.Name,
                        FontWeight = section.Children.Count > 0 ? FontWeights.Bold : FontWeights.Normal
                    };
                    if (TopMenuBuilder != null)
                        header.ContextMenu = TopMenuBuilder.BuildContextMenuFor(section);
                    searchables_.Add(header);
                    exp.Header = header;
                    StackPanel topStack = new StackPanel();
                    exp.Content = topStack;
                    topStack.Margin = new Thickness(20, 0.0, 0.0, 0);
                    contentStack.Children.Add(exp);

                    if (!Shallow) { //events and such
                        foreach (APINode subsection in section.Children) {
                            if (subsection.Children.Count == 0) {
                                TextBlock tb = new TextBlock { Text = subsection.Name };
                                if (MiddleMenuBuilder != null)
                                    tb.ContextMenu = MiddleMenuBuilder.BuildContextMenuFor(subsection);
                                searchables_.Add(tb);
                                tb.Margin = new Thickness(25, 2, 2, 0);
                                topStack.Children.Add(tb);
                            } else {
                                Expander subExp = new Expander();
                                TextBlock hd = new TextBlock { Text = subsection.Name };
                                if (MiddleMenuBuilder != null)
                                    hd.ContextMenu = MiddleMenuBuilder.BuildContextMenuFor(subsection);
                                subExp.Header = hd;
                                searchables_.Add(hd);
                                StackPanel subStack = new StackPanel();
                                subStack.Margin = new Thickness(20.0, 0.0, 0.0, 0);
                                subExp.Content = subStack;
                                topStack.Children.Add(subExp);

                                foreach (APINode item in subsection.Children) {
                                    if (item is APILeaf) {
                                        APILeaf leaf = item as APILeaf;
                                        TextBlock tb = new TextBlock { Text = leaf.Name };
                                        if (BottomMenuBuilder != null)
                                            tb.ContextMenu = BottomMenuBuilder.BuildContextMenuFor(leaf);
                                        tb.Margin = new Thickness(20, 0, 0, 0);
                                        subStack.Children.Add(tb);
                                    }
                                }
                            }
                        }
                    } else {
                        foreach (APINode item in section.Children) {
                            if (item is APILeaf) {
                                APILeaf leaf = item as APILeaf;
                                TextBlock tb = new TextBlock { Text = leaf.Name };
                                if (MiddleMenuBuilder != null)
                                    tb.ContextMenu = MiddleMenuBuilder.BuildContextMenuFor(leaf);
                                tb.Margin = new Thickness(20, 0, 0, 0);
                                topStack.Children.Add(tb);
                            }
                        }
                    }
                }
            }
        }

        private void txtSearch_KeyUp(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter && txtSearch.Text.Trim().Length > 0) {
                foreach (TextBlock tb in searchables_) {
                    if (tb.Text.ToLower().Contains(txtSearch.Text.Trim().ToLower())) {
                        tb.BringIntoView();
                        if (tb.Parent != null)
                            ((Expander)tb.Parent).Focus();
                        else
                            tb.Focus();
                        return;
                    }
                }
            }
        }

        private void txtSearch_GotFocus(object sender, RoutedEventArgs e) {
            txtSearch.Text = "";
        }
    }
}

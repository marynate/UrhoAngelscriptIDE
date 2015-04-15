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

namespace Debugger.Dlg
{
    /// <summary>
    /// Interaction logic for PluginDlg.xaml
    /// </summary>
    public partial class PluginDlg : ModernDialog
    {
        public PluginDlg()
        {
            InitializeComponent();

            foreach (PluginInfo info in PluginManager.inst().InstalledPlugins)
            {
                StackPanel content = new StackPanel();
                content.Margin = new Thickness(10, 0, 10, 0);

                content.Children.Add(new Label() { Content = info.Name, FontWeight = FontWeights.Bold });
                foreach (string s in info.Parts)
                {
                    if (s.Length > 0)
                    {
                        content.Children.Add(new Label() { Content = s });
                    }
                }


                if (info.Components.Count > 0)
                {
                    content.Children.Add(new Separator());
                    content.Children.Add(new Label { Content = "Components", FontWeight = FontWeights.Bold });
                    WrapPanel subStack = new WrapPanel();
                    subStack.MaxWidth = 160;
                    subStack.Orientation = Orientation.Horizontal;
                    content.Children.Add(subStack);
                    foreach (string s in info.Components)
                    {
                        subStack.Children.Add(new Label() { Content = s, HorizontalAlignment = System.Windows.HorizontalAlignment.Left, Margin = new Thickness(4) });
                    }
                }

                pluginStack.Children.Add(content);
            }
        }
    }
}

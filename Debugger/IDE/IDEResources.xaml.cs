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
    /// Filesystem tree for resources in the project path
    /// Can open resources, copy their package path to the clipboard, etc
    /// </summary>
    public partial class IDEResources : UserControl {
        public IDEResources() {
            InitializeComponent();
        }
    }
}

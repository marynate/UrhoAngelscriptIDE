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

namespace Debugger.Screens {
    /// <summary>
    /// Interaction logic for DebugScreen.xaml
    /// </summary>
    public partial class DebugScreen : UserControl {

        static DebugScreen inst_;

        public static DebugScreen inst() { return inst_; }
        public DebugScreen() {
            inst_ = this;
            InitializeComponent();
            logGrid.DataContext = Debugger.Debug.SessionData.inst();
            watchGrid.DataContext = Debugger.Debug.SessionData.inst();
            fileList.DataContext = Debugger.Debug.SessionData.inst();
            breakGrid.DataContext = Debugger.Debug.SessionData.inst();
            stackGrid.DataContext = Debugger.Debug.SessionData.inst();
            btnStack.DataContext = Debugger.Debug.SessionData.inst();
            stackReconnect.DataContext = Debugger.Debug.SessionData.inst();
            txtConnection.DataContext = Debugger.Debug.SessionData.inst();

            thisTree.View.DataContext = Debugger.Debug.SessionData.inst().ThisData;
            thisTree.PrefixWatches = "this.";
            globalsTree.DataContext = Debugger.Debug.SessionData.inst().GlobalData;
            globalsTree.PrefixWatches = "global.";
            localsTree.View.DataContext = Debugger.Debug.SessionData.inst().LocalData;

            //InputBindings.Add(new KeyBinding(ContCmd, new KeyGesture(Key.F5)));
            //InputBindings.Add(new KeyBinding(StepIn, new KeyGesture(Key.F5)));
            //InputBindings.Add(new KeyBinding(StepOut, new KeyGesture(Key.F5)));
            //InputBindings.Add(new KeyBinding(StepOver, new KeyGesture(Key.F5)));
        }

        public static readonly ICommand ContCmd = new RelayCommand(o => { Cont(); }, o => { return canDebug(); });
        public static readonly ICommand StepOverCmd = new RelayCommand(o => { StepOver(); }, o => { return canDebug(); });
        public static readonly ICommand StepOutCmd = new RelayCommand(o => { StepOut(); }, o => { return canDebug(); });
        public static readonly ICommand StepInCmd = new RelayCommand(o => { StepIn(); }, o => { return canDebug(); });

        public TreeView GlobalTree { get { return this.globalsTree.View; } }
        public TreeView ThisTree { get { return this.thisTree.View; } }
        public TreeView LocalsTree { get { return this.localsTree.View; } }
        public Editor.EditorTabs EditorTabs { get { return this.editTabs; } }

        private void fileList_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            editTabs.OpenFile(((Debugger.Debug.FileData)fileList.SelectedValue));
        }

        void onContinue(object o, EventArgs e) {Cont();}
        void onStepOver(object o, EventArgs e) {StepOver();}
        void onStepIn(object o, EventArgs e) {StepIn();}
        void onStepOut(object o, EventArgs e) {StepOut();}

        void onReconnect(object o, EventArgs e) {
            Net.DebugClient.inst().Connect(Debugger.Debug.SessionData.inst().Connection);
        }

        void stackDoubleClick(object sender, MouseEventArgs args) {
            DataGridRow row = sender as DataGridRow;
            Debugger.Debug.Callstack stack = row.DataContext as Debugger.Debug.Callstack;
            editTabs.OpenFile(stack.SectionID, stack.Line);
        }

        void bpDoubleClick(object sender, MouseEventArgs args) {
            DataGridRow row = sender as DataGridRow;
            Debugger.Debug.Breakpoint bp = row.DataContext as Debugger.Debug.Breakpoint;
            editTabs.OpenFile(bp.SectionID, bp.LineNumber);
        }

        static bool canDebug() {
            return Debugger.Debug.SessionData.inst().IsDebugging;
        }

        static void Cont() {
            Net.DebugClient.inst().Continue();
        }
        static void StepOver() {
            Net.DebugClient.inst().StepOver();
        }
        static void StepIn() {
            Net.DebugClient.inst().StepIn();
        }
        static void StepOut() {
            Net.DebugClient.inst().StepOut();
        }

        private void breakGrid_PreviewKeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Delete) {
                if (breakGrid.SelectedItems.Count > 0) {
                    foreach (Debugger.Debug.Breakpoint bp in breakGrid.SelectedItems)
                    {
                        bp.Active = false;
                        Debugger.Debug.FileData fd = Debugger.Debug.SessionData.inst().Files.FirstOrDefault(fl => fl.SectionID == bp.SectionID);
                        if (fd != null)
                            fd.BreakPoints.Remove(bp);
                    }
                }
            }
        }
    }
}

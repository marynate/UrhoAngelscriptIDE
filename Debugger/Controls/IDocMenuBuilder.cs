using FirstFloor.ModernUI.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Debugger.Controls {
    public interface IDocMenuBuilder {
        ContextMenu BuildContextMenuFor(IDE.API.APINode aNode);
    }

    public class EventSubscriptionDocMenu : IDocMenuBuilder {
        public ContextMenu BuildContextMenuFor(IDE.API.APINode aNode) {
            ContextMenu cmenu = new ContextMenu();
            cmenu.Items.Add(new MenuItem {
                Header = "Copy Subscription to clipboard",
                Command = new RelayCommand(p => {
                    System.Windows.Clipboard.SetText(string.Format("::SubscribeToEvent(\"{0}\", \"Handle{0}\");", aNode.Name));
                })
            });
            cmenu.Items.Add(new MenuItem {
                Header = "Copy Unsubscription to clipboard",
                Command = new RelayCommand(p =>
                {
                    System.Windows.Clipboard.SetText(string.Format("::UnsubscribeFromEvent(\"{0}\");", aNode.Name));
                })
            });
            cmenu.Items.Add(new MenuItem {
                Header = "Copy Handler to clipboard",
                Command = new RelayCommand(p =>
                {
                    System.Windows.Clipboard.SetText(string.Format("void Handle{0}(StringHash eventType, VariantMap& eventData)\n{{\n\n}}\n", aNode.Name));
                })
            });
            return cmenu;
        }
    }

    public class EventDataDocMenu : IDocMenuBuilder {
        public ContextMenu BuildContextMenuFor(IDE.API.APINode aNode) {
            ContextMenu cmenu = new ContextMenu();
            cmenu.Items.Add(new MenuItem {
                Header = "Copy to clipboard",
                Command = new RelayCommand(p => {
                    string str = aNode.Name.Substring(0, aNode.Name.IndexOf(':')).Trim();
                    System.Windows.Clipboard.SetText(string.Format("eventData[\"{0}\"]", str));
                })
            });
            return cmenu;
        }
    }

    public class AttrDocMenu : IDocMenuBuilder {
        public ContextMenu BuildContextMenuFor(IDE.API.APINode aNode) {
            ContextMenu cmenu = new ContextMenu();
            cmenu.Items.Add(new MenuItem {
                Header = "Copy Getter to clipboard",
                Command = new RelayCommand(p =>
                {
                    string str = aNode.Name.Substring(0, aNode.Name.IndexOf(':')).Trim();
                    System.Windows.Clipboard.SetText(string.Format("GetAttribute(\"{0}\");", str));
                })
            });
            cmenu.Items.Add(new MenuItem {
                Header = "Copy Setter to clipboard",
                Command = new RelayCommand(p =>
                {
                    string str = aNode.Name.Substring(0, aNode.Name.IndexOf(':')).Trim();
                    System.Windows.Clipboard.SetText(string.Format("SetAttribute(\"{0}\", VALUE);", str));
                })
            });
            return cmenu;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Debugger.IDE.API {

    public class APILeaf : APINode {
    }

    public class APINode : NamedBaseClass {
        public APINode() {
            children_ = new ObservableCollection<APINode>();
            content_ = new List<string>();
            Context = new List<KeyValuePair<string, string>>();
        }

        public APINode Parent { get; set; }
        public int ParentCount {
            get {
                if (Parent == null)
                    return 0;
                return 1 + Parent.ParentCount;
            }
        }

        public List<KeyValuePair<string, string>> Context { get; set; }
        ObservableCollection<APINode> children_;
        List<string> content_;

        public bool WantContextMenu;

        public APINode Search(string text) {
            if (Name.ToLower().Trim().Contains(text))
                return this;
            foreach (APINode nd in Children) {
                APINode n = nd.Search(text);
                if (n != null)
                    return n;
            } return null;
        }

        public void Prune() {
            foreach (APINode nd in Children)
                nd.Prune();
            foreach (APINode nd in Children) {
                if (nd.GetItemCount() == 0)
                    Children.Remove(nd);
            }
        }

        int GetItemCount() {
            int i = 0;
            foreach (APINode nd in Children) {
                if (nd is APILeaf)
                    ++i;
                else
                    i += nd.GetItemCount();
            }
            return i;
        }

        public ObservableCollection<APINode> Children { get { return children_; } }
    }

    public class APIDocumentation : BaseClass {
        public APIDocumentation() {
            if (root_ == null) {
                string dir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
                dir = System.IO.Path.Combine(System.IO.Path.Combine(dir, "bin"), "ScriptAPI.dox");

                if (!System.IO.File.Exists(dir))
                {
                    //\todo, do something about the case where it doesn't exist? invoke the script API
                }

                string[] lines = File.ReadAllLines(dir);
                APINode current = new APINode { Name = "Root" };
                APINode lastPage = null;
                APINode lastSection = null;
                APINode lastSubsection = null;
                foreach (string line in lines) {
                    if (line.StartsWith("\\page")) {
                        string[] words = line.Split(' ');
                        APINode nd = new APINode { Name = string.Join(" ", words, 2, words.Length - 2) };
                        current.Children.Add(nd);
                        nd.Parent = current;
                        lastPage = nd;
                        lastSection = null;
                        lastSubsection = null;
                    } else if (line.StartsWith("\\section")) {
                        string[] words = line.Split(' ');
                        APINode nd = new APINode { Name = string.Join(" ", words, 2, words.Length - 2) };
                        if (lastSection != null) {
                            lastSection.Children.Add(nd);
                            nd.Parent = lastSection;
                        } else {
                            lastPage.Children.Add(nd);
                            nd.Parent = lastPage;
                        }
                        lastSubsection = nd;
                    } else if (line.StartsWith("## ")) {
                        APINode nd = new APINode { Name = line.Replace("## ", "").Replace("%", "") };
                        lastSection = nd;
                        lastPage.Children.Add(nd);
                        nd.Parent = lastPage;
                    } else if (line.StartsWith("### ")) {
                        APINode nd = new APINode { Name = line.Replace("### ", "").Replace("%", "") };
                        if (lastSection == null) {
                            lastPage.Children.Add(nd);
                            nd.Parent = lastPage;
                        } else {
                            lastSection.Children.Add(nd);
                            nd.Parent = lastSection;
                        }
                        lastSubsection = nd;
                    } else if (line.StartsWith("- ")) {
                        APILeaf leaf = new APILeaf { Name = line.Replace("- ", "").Replace("%", "") };
                        lastSubsection.Children.Add(leaf);
                        leaf.Parent = lastSubsection;
                    }
                }
                //current.Prune();
                DocumentNode = current;

                foreach (APINode nd in DocumentNode.Children[0].Children) {
                    foreach (APINode n in nd.Children) {
                        n.Context.Add(new KeyValuePair<string, string>("Copy Subscriber", "::SubscribeToEvent(\"{0}\",\"Handle{0}\");"));
                        n.Context.Add(new KeyValuePair<string, string>("Copy Unsubscriber", "::UnsubscribeFromEvent(\"Handle{0}\");"));
                        n.Context.Add(new KeyValuePair<string, string>("Copy Handler", 
@"void Handle{0}(StringHash eventType, VariantMap& eventData)
{{
}}"));
                        foreach (APINode l in n.Children) {
                            l.Context.Add(new KeyValuePair<string, string>("Copy param getter", "eventData[\"{0}\"]"));
                        }
                    }
                }

                foreach (APINode nd in DocumentNode.Children[1].Children) {
                    foreach (APINode n in nd.Children) {
                        n.Context.Add(new KeyValuePair<string, string>("Copy Getter", "GetAttribute(\"{0}\");"));
                        n.Context.Add(new KeyValuePair<string, string>("Copy Setter", "SetAttribute(\"{0}\", VALUE);"));
                    }
                }
            }
        }

        static APINode root_;
        public APINode DocumentNode { get { return root_; } set { root_ = value; OnPropertyChanged("DocumentNode"); } }
    }
}

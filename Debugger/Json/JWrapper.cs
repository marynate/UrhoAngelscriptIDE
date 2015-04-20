using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Debugger.Json {

    /// <summary>
    /// Turns JSON received from the asPEEK daemon into a more easily used tree structure
    /// </summary>
    public class JWrapper : BaseClass {

        public static string[] SubArray(string[] data, int index, int length) {
            string[] result = new string[length];
            System.Array.Copy(data, index, result, 0, length);
            return result;
        }


        public new JWrapper Parent = null;
        protected JWrapper(string name) {
            Name = name;
        }

        public JWrapper(string name, JArray array) {
            rootArray = array;
            Name = name;
        }
        public JWrapper(string name, JObject obj) {
            rootObject = obj;
            Name = name;
        }

        JArray rootArray;
        public JArray Array { set { rootArray = value; OnPropertyChanged(); } }
        JObject rootObject;
        public JObject Object { set { rootObject = value; OnPropertyChanged(); } }

        public string Name {get; set;}

        public virtual string GetDotPath() {
            string ret = "";

            int voidResult = 0;
            // Cludge int parse to check for "stack level," is a parseable integer name is ever valid?
            if (Parent != null && !int.TryParse(Parent.Name, out voidResult))
            {
                string parentName = Parent.GetDotPath();
                if (parentName.Length > 0)
                    return Parent.GetDotPath() + "." + Name;
            }
            return Name;
        }

        public string GetTildePath() {
            string ret = "";
            JWrapper cur = this;
            while (cur.Parent != null) {
                ret = cur.Name + (ret.Length > 0 ? "~" : "") + ret;
                cur = cur.Parent;
            }
            return ret;
        }

        public JWrapper ResolveDotPath(params string[] words) {
            foreach (JWrapper child in Children) {
                // Cludge for script engine registration vs asPEEK registration
                //\todo Determine most appropriate means of registering objects in Urho3D asPEEK implementation
                //Script types are registered differently than types are named in C++
                if (child.Name.Equals(words[0]) || (child.Name.EndsWith("_") && child.Name.Replace("_","").Equals(words[0]))) {
                    if (words.Length > 1) {
                        JWrapper r = child.ResolveDotPath(SubArray(words, 1, words.Length-1));
                        if (r != null)
                            return r;
                    } else
                        return child;
                } else if (Parent == null) { //we're a root holder, applies to "Stack Level nodes"
                    JWrapper r = child.ResolveDotPath(words);
                    if (r != null)
                        return r;
                }
            }
            // Cludge for script engine registration vs asPEEK registration
            if (Name.Equals(words[0]) || (Name.EndsWith("_") && Name.Replace("_","").Equals(words[0])))
                return this;
            return null;
        }

        List<JWrapper> backing_;
        public List<JWrapper> Children {
            get {
                if (backing_ != null)
                    return backing_;
                List<JWrapper> ret = new List<JWrapper>();
                if (rootArray != null) {
                    int idx = 0;
                    foreach (JObject obj in rootArray.Children<JObject>()) {
                        if (obj.Properties().Count() == 1) {
                            if (obj.Properties().First().Value.Type == JTokenType.Object)
                                ret.Add(new JWrapper(obj.Properties().First().Name, obj.Properties().First().Value.ToObject<JObject>()) { Parent = this });
                            else
                                ret.Add(new JLeaf(obj.Properties().First().Name, obj.Properties().First().Value.ToString()) { Parent = this });
                        } else
                            ret.Add(new JWrapper(Name + idx, obj) { Parent = this });
                        ++idx;
                    }
                } else if (rootObject != null) {
                    foreach (JProperty p in rootObject.Properties()) {
                        if (p.Value.Type == JTokenType.Object) {
                            ret.Add(new JWrapper(p.Name, p.Value.ToObject<JObject>()) { Parent = this });
                        } else {
                            if (p.Name.StartsWith("_"))
                                ToolTip = p.Value.ToString();
                            else
                                ret.Add(new JLeaf(p.Name, p.Value.ToString()) { Parent = this });
                        }
                    }
                }
                backing_ = ret;
                return backing_;
            }
        }

        public virtual JWrapper ContainsKey(string key) {
            foreach (JWrapper obj in Children) {
                if (obj.Name.Equals(key))
                    return obj;
                JWrapper r = obj.ContainsKey(key);
                if (r != null)
                    return r;
            }
            return null;
        }

        string tooltip_;
        public string ToolTip { get { return tooltip_; } set { tooltip_ = value; OnPropertyChanged("ToolTip"); } }
    }

    public class JLeaf : JWrapper {
        public JLeaf(string name, string value) : base(name) {
            Value = value;
        }
        string value_;
        public string Value { get { return value_; } set { value_ = value; } }

        public override JWrapper ContainsKey(string key) {
            if (value_.Equals(key))
                return this;
            return null;
        }
    }
}

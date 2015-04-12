using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Debugger.IDE.Snippets {

    public class CodeSnippetInput : NamedBaseClass {
        string key_;
        public string Key { get { return key_; } set { key_ = value; OnPropertyChanged("Key"); } }
    }

    public class CodeSnippetOption : CodeSnippetInput
    {
    }

    public class CodeBlock
    {
        public string Code { get; set; }
        public string[] Requires { get; set; }
    }

    public class CodeSnippet : NamedBaseClass {
        ObservableCollection<CodeBlock> code_ = new ObservableCollection<CodeBlock>();

        public CodeSnippet() {
            Inputs = new ObservableCollection<CodeSnippetInput>();
        }

        public string CreateCode(Dictionary<string, string> aValues) {
            string ret = "";
            foreach (CodeBlock cb in Code)
            {
                bool passes = true;
                if (cb.Requires != null)
                {
                    // Scan required settings, required settings are displayed as checkboxes
                    foreach (string req in cb.Requires)
                    {
                        if (aValues.ContainsKey(req) && aValues[req].Equals("true"))
                            continue;
                        else
                        {
                            passes = false;
                            break;
                        }
                    }
                }
                if (passes)
                {
                    if (ret.Length > 0)
                        ret += "\r\n";
                    ret += cb.Code;
                }
            }
            foreach (CodeSnippetInput input in Inputs) {
                if (aValues.ContainsKey(input.Name)) {
                    ret = ret.Replace("{{" + input.Key + "}}", aValues[input.Name]);
                }
            }
            return ret;
        }

        public ObservableCollection<CodeSnippetInput> Inputs { get; set; }
        public ObservableCollection<CodeBlock> Code { get { return code_; } }
        public string Extension { get; set; }

        public static CodeSnippet FromFile(string aFile) {
            XmlDocument doc = new XmlDocument();
            doc.Load(aFile);
            CodeSnippet ret = new CodeSnippet();
            ret.Name = doc.DocumentElement.GetAttribute("name");
            ret.Extension = doc.DocumentElement.GetAttribute("extension");

            foreach (XmlElement elem in doc.DocumentElement.GetElementsByTagName("option"))
            {
                CodeSnippetOption opt = new CodeSnippetOption { Name = elem.GetAttribute("name"), Key = elem.GetAttribute("key") };
                ret.Inputs.Add(opt);
            }

            foreach (XmlElement elem in doc.DocumentElement.GetElementsByTagName("input")) {
                CodeSnippetInput input = new CodeSnippetInput();
                input.Name = elem.GetAttribute("name");
                input.Key = elem.GetAttribute("key");
                ret.Inputs.Add(input);
            }
            foreach (XmlElement elem in doc.DocumentElement.GetElementsByTagName("code")) {
                string requires = elem.GetAttribute("requires");
                if (requires != null && requires.Length > 0)
                {
                    ret.Code.Add(new CodeBlock { Code = elem.InnerText, Requires = requires.Split(' ') });
                }
                else
                    ret.Code.Add(new CodeBlock { Code = elem.InnerText, Requires = null });
            }
            return ret;
        }
    }

    public class SnippetData {
        public static SnippetData inst_;
        public static SnippetData inst() {
            if (inst_ == null) {
                inst_ = new SnippetData();
                inst_.Snippets = new ObservableCollection<CodeSnippet>();

                string path = System.IO.Path.GetDirectoryName(Assembly.GetAssembly(typeof(SnippetData)).Location);
                path = System.IO.Path.Combine(path, "snippets");
                if (System.IO.Directory.Exists(path)) {
                    foreach (string file in System.IO.Directory.EnumerateFiles(path))
                        inst_.Snippets.Add(CodeSnippet.FromFile(file));
                }
            }
            return inst_;
        }

        public SnippetData() { }
        public ObservableCollection<CodeSnippet> Snippets { get; set; }
    }
}

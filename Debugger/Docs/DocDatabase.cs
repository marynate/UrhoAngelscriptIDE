using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Debugger.Docs
{
    public class DocDatabase
    {
        Dictionary<string, string> documentation_;

        public DocDatabase()
        {
            
            string dir = System.Reflection.Assembly.GetEntryAssembly().Location;
            dir = System.IO.Path.GetDirectoryName(dir);
            dir = System.IO.Path.Combine("bin");
            dir = System.IO.Path.Combine("Docs.xml");
            if (System.IO.File.Exists(dir))
            {
                try
                {
                    documentation_ = ReadDocDB(dir);
                }
                catch (Exception ex)
                {
                    documentation_ = new Dictionary<string, string>();
                    ErrorHandler.inst().Error(ex);
                }
            }
            else
            {
                documentation_ = new Dictionary<string, string>();
            }
        }

        public void SaveDocs()
        {
            string dir = System.Reflection.Assembly.GetEntryAssembly().Location;
            dir = System.IO.Path.GetDirectoryName(dir);
            dir = System.IO.Path.Combine("bin");
            dir = System.IO.Path.Combine("Docs.xml");
            WriteDocDB(documentation_, dir);
        }

        public string GetDocumentationFor(string aName)
        {
            if (documentation_.ContainsKey(aName))
                return documentation_[aName];
            return null;
        }

        public void Document(string aName)
        {
            string curDocs = GetDocumentationFor(aName);
            Debugger.Docs.DocumentDlg dlg = new Debugger.Docs.DocumentDlg(aName, curDocs != null ? curDocs : "");
            bool? result = dlg.ShowDialog();
            if (result.HasValue && result.Value)
            {
                documentation_[aName] = dlg.DocText;
                SaveDocs();
            }
        }

        static Dictionary<string,string> ReadDocDB(string file)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(file);
            return ReadDocDB(doc);
        }

        static Dictionary<string,string> ReadDocDB(XmlDocument doc)
        {
            Dictionary<string, string> ret = new Dictionary<string, string>();
            foreach (XmlElement elem in doc.DocumentElement.GetElementsByTagName("doc"))
                ret[elem.GetAttribute("name")] = elem.InnerText;
            return ret;
        }

        static void WriteDocDB(Dictionary<string,string> docs, string filePath)
        {
            XmlDocument doc = new XmlDocument();
            WriteDocDB(docs, doc);
            doc.Save(filePath);
        }

        static void WriteDocDB(Dictionary<string,string> docs, XmlDocument doc)
        {
            XmlElement root = doc.CreateElement("documentation");
            doc.AppendChild(root);
            foreach (string item in docs.Keys)
            {
                XmlElement elem = doc.CreateElement("doc");
                elem.SetAttribute("name", item);
                elem.InnerText = docs[item];
                root.AppendChild(elem);
            }
        }
    }
}

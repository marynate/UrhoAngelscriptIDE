using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Debugger.Json {

    public class JsonUtils {
        public static XmlDocument Parse(string aJSON) {
            return (XmlDocument)JsonConvert.DeserializeXmlNode(aJSON, "Root");
        }

        public static JArray ParseArray(string aJSON) {
            return JArray.Parse(aJSON);
        }
    }
}

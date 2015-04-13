using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Debugger.IDE {

    public interface PossiblyIncomplete {
        void ResolveIncompletion(Globals globs);
    }

    public class PropInfo : PossiblyIncomplete {
        public string Name { get; set; }
        public bool ReadOnly { get; set; }
        public bool IsReference { get; set; }
        public TypeInfo Container { get; set; } //only relevant to the master class list
        public TypeInfo Type { get; set; }
        public TypeInfo WrappedType { get; set; } //only relevant when we're a template

        public string ImgSource {
            get {
                if (ReadOnly)
                    return "/Images/all/roproperty.png";
                return "/Images/all/property.png";
            }
        }

        public void ResolveIncompletion(Globals globs) {
            if (!Type.IsComplete && globs.Classes.ContainsKey(Type.Name))
                Type = globs.Classes[Type.Name];
            if (WrappedType != null && !WrappedType.IsComplete && globs.Classes.ContainsKey(WrappedType.Name))
                WrappedType = globs.Classes[WrappedType.Name];
        }
    }

    public abstract class BaseTypeInfo {
        public abstract BaseTypeInfo ResolvePropertyPath(Globals globals, params string[] path);
    }

    public class TypeInfo : BaseTypeInfo, PossiblyIncomplete {
        public TypeInfo() {
            Properties = new Dictionary<string, TypeInfo>();
            BaseTypeStr = new List<string>();
            BaseTypes = new List<TypeInfo>();
            Functions = new List<FunctionInfo>();
            ReadonlyProperties = new List<string>();
            IsComplete = true; //default we'll assume complete
            IsPrimitive = false;
        }

        public override BaseTypeInfo ResolvePropertyPath(Globals globals, params string[] path) {
            string str = path[0];
            if (str.Contains('(')) {
                string content = str.Substring(0, str.IndexOf('('));
                FunctionInfo fi = Functions.FirstOrDefault(f => f.Name.Equals(content));
                if (fi != null) {
                    if (str.Contains('[') && fi.ReturnType is TemplateInst)
                        return ((TemplateInst)fi.ReturnType).WrappedType;
                    else if (fi.ReturnType is TemplateInst) {
                        return globals.Classes[fi.ReturnType.Name.Substring(0, fi.ReturnType.Name.IndexOf('<'))];
                    }
                    return fi.ReturnType;
                }
            } else if (str.Contains('[')) {
                string content = str.Extract('[', ']');
                str = str.Replace(string.Format("[{0}]", content), "");
                TemplateInst ti = Properties[str] as TemplateInst;
                if (ti != null && path.Length > 1) {
                    TypeInfo t = ti.WrappedType;
                    return t.ResolvePropertyPath(globals, path.SubArray(1, path.Length - 1));
                }
                if (ti == null)
                    return null;
                else if (ti.WrappedType == null)
                    return ti;
                return globals.Classes[ti.WrappedType.Name];
            } else if (Properties.ContainsKey(path[0])) {
                BaseTypeInfo ti = Properties[path[0]];
                if (ti is TemplateInst)
                    ti = globals.Classes[((TemplateInst)ti).Name];
                if (path.Length > 1)
                    ti = ti.ResolvePropertyPath(globals, path.SubArray(1, path.Length-1));
                return ti;
            } return null;
        }

        public string Description {
            get {
                if (IsTemplate)
                    return "template ";
                if (IsPrimitive)
                    return "prim ";
                if (this is EnumInfo)
                    return "enum ";
                return "class ";
            }
        }

        public bool IsTemplate { get; set; }
        public bool IsPrimitive { get; set; }
        public bool IsComplete { get; set; }
        public List<string> BaseTypeStr { get; private set; }
        public List<TypeInfo> BaseTypes { get; private set; }
        public string Name { get; set; }
        public Dictionary<string, TypeInfo> Properties { get; private set; }
        public List<string> ReadonlyProperties { get; private set; }
        public List<FunctionInfo> Functions { get; private set; }

        public List<object> PropertyUI {
            get {
                List<object> ret = new List<object>();
                foreach (string key in Properties.Keys)
                    ret.Add(new PropInfo { Name = key, Type = Properties[key], ReadOnly = ReadonlyProperties.Contains(key) });
                foreach (FunctionInfo f in Functions)
                    ret.Add(f);
                return ret;
            }
        }

        public void ResolveIncompletion(Globals globs) {
            foreach (FunctionInfo f in Functions)
                f.ResolveIncompletion(globs);
            List<string> keys = new List<string>(Properties.Keys);
            foreach (string key in keys) {
                if (!Properties[key].IsComplete) {
                    Properties[key] = globs.Classes[Properties[key].Name];
                }
            }
        }
    }

    public class FunctionInfo : BaseTypeInfo, PossiblyIncomplete {
        public FunctionInfo() {
            Params = new List<TypeInfo>();
        }
        public string Name { get; set; }
        public TypeInfo ReturnType { get; set; }
        public string Inner { get; set; }
        public List<TypeInfo> Params { get; set; }
        public bool IsConst { get; set; }

        public override BaseTypeInfo ResolvePropertyPath(Globals globals, params string[] path) {
            return ReturnType;
        }

        public void ResolveIncompletion(Globals globs) {
            if (globs.Classes.ContainsKey(ReturnType.Name))
                ReturnType = globs.Classes[ReturnType.Name];
            if (ReturnType is TemplateInst) { //not found in globals
                ((TemplateInst)ReturnType).WrappedType = globs.Classes[((TemplateInst)ReturnType).WrappedType.Name];
            }
            for (int i = 0; i < Params.Count; ++i) {
                if (!Params[i].IsComplete && globs.Classes.ContainsKey(Params[i].Name))
                    Params[i] = globs.Classes[Params[i].Name];
            }
        }
    }

    public class EnumInfo : TypeInfo {
        public EnumInfo() {
            Values = new List<string>();
        }
        public List<string> Values { get; private set; }
    }

    public class TemplateInfo : TypeInfo {
    }

    public class TemplateInst : TypeInfo {
        public TypeInfo WrappedType { get; set; }
    }

    public class Globals {
        public Globals() {
            Properties = new Dictionary<string, TypeInfo>();
            Functions = new List<FunctionInfo>();
            Classes = new Dictionary<string, TypeInfo>();

            Classes["void"] = new TypeInfo() { Name = "void", IsPrimitive = true };
            Classes["int"] = new TypeInfo() { Name = "int", IsPrimitive = true };
            Classes["uint"] = new TypeInfo() { Name = "uint", IsPrimitive = true };
            Classes["float"] = new TypeInfo() { Name = "float", IsPrimitive = true };
            Classes["double"] = new TypeInfo() { Name = "double", IsPrimitive = true };
            Classes["bool"] = new TypeInfo() { Name = "bool", IsPrimitive = true };

            //extended types
            Classes["int8"] = new TypeInfo() { Name = "int8", IsPrimitive = true };
            Classes["int16"] = new TypeInfo() { Name = "int16", IsPrimitive = true };
            Classes["int64"] = new TypeInfo() { Name = "int64", IsPrimitive = true };
            Classes["uint8"] = new TypeInfo() { Name = "uint8", IsPrimitive = true };
            Classes["uint16"] = new TypeInfo() { Name = "uint16", IsPrimitive = true };
            Classes["uint64"] = new TypeInfo() { Name = "uint64", IsPrimitive = true };
        }

        public Dictionary<string, TypeInfo> Classes { get; private set; }
        public Dictionary<string, TypeInfo> Properties { get; private set; }
        public List<TypeInfo> TypeInfo { get { return new List<TypeInfo>(Classes.Values); } }
        public List<FunctionInfo> Functions { get; private set; }
    }

    public class ASParser {

        public void ParseDumpFile(StringReader rdr, Globals globals) {
            string line = "";
            while ((line = rdr.ReadLine()) != null) {
                if (line.Length == 0)
                    continue;
                if (line.Contains("class")) {
                    ParseDumpClass(line, rdr, globals);
                } else if (line.Contains("enum")) {
                    ParseDumpEnum(line, rdr, globals);
                } else if (line.StartsWith("// Global functions")) {
                    ParseDumpGlobFuncs(line, rdr, globals);
                } else if (line.StartsWith("// Global properties")) {
                    ParseDumpGlobProps(line, rdr, globals);
                } else if (line.StartsWith("// Global constants")) {
                    ParseDumpGlobProps(line, rdr, globals);
                }
            }
            _resolveNames(globals);
        }

        void _resolveNames(Globals globals) {
            foreach (string key in globals.Properties.Keys) {
                if (!globals.Properties[key].IsComplete)
                    globals.Properties[key] = globals.Classes[globals.Properties[key].Name];
                if (globals.Properties[key] is TemplateInst) {
                    TemplateInst ti = globals.Properties[key] as TemplateInst;
                    if (!ti.WrappedType.IsComplete)
                        ti.WrappedType = globals.Classes[ti.WrappedType.Name];
                }
            }

            foreach (FunctionInfo f in globals.Functions) {
                f.ResolveIncompletion(globals);
            }

            foreach (TypeInfo type in globals.Classes.Values) {
                type.ResolveIncompletion(globals);
            }
        }

        void ParseDumpGlobFuncs(string line, StringReader rdr, Globals globals) {
            while ((line = rdr.ReadLine()) != null) {
                if (line.Length == 0)
                    return;
                if (line.StartsWith("/*") || line.StartsWith("//"))
                    continue;
                else {
                    globals.Functions.Add(_parseFunction(line, globals));
                }
            }
        }

        void ParseDumpGlobProps(string line, StringReader rdr, Globals globals) {
            while ((line = rdr.ReadLine()) != null) {
                if (line.Length == 0)
                    return;
                if (line.StartsWith("/*") || line.StartsWith("//"))
                    continue;
                else {
                    string[] parts = line.Replace(";", "").Split(' '); //[TypeName] [PropertyName]
                    string pname = parts[0].EndsWith("@") ? parts[0].Substring(0, parts[0].Length - 1) : parts[0]; //handle
                    TypeInfo pType = null;
                    string myname = parts[1];
                    if (globals.Classes.ContainsKey(pname))
                        pType = globals.Classes[pname];
                    if (pType == null) { //create temp type to resolve
                        pType = new TypeInfo() { Name = pname, IsComplete = false };
                    }
                    globals.Properties[myname] = pType;
                }
            }
        }

        void ParseDumpEnum(string line, StringReader rdr, Globals globals) {
            string[] nameparts = line.Split(' ');
            string enumName = nameparts[1];
            List<string> enumValues = new List<string>();
            while ((line = rdr.ReadLine()) != null) {
                if (line.Equals("};")) {
                    EnumInfo ret = new EnumInfo { Name = enumName };
                    ret.Values.AddRange(enumValues);
                    globals.Classes[enumName] = ret;
                    return;
                } else if (line.Contains(',')) {
                    int sub = line.IndexOf(',');
                    enumValues.Add(line.Substring(0, sub));
                }
            }
        }

        void ParseDumpClass(string line, StringReader rdr, Globals globals) {
            bool isTemplate = false;
            if (line.Contains("template <class T> "))
                isTemplate = true;
            string[] nameparts = line.Replace(",","").Replace("template <class T> ","").Split(' '); //dump the commas
            string classname = nameparts[1]; //class is first
            string classtype = nameparts[0]; //it might be an interface

            TypeInfo classInfo = new TypeInfo() { IsTemplate = isTemplate };
            classInfo.Name = classname;
            globals.Classes[classInfo.Name] = classInfo;

            for (int i = 3; i < nameparts.Length; ++i) { //list bases 2 would be :, 3 will be first basetype
                classInfo.BaseTypeStr.Add(nameparts[i]); //add a base class
            }

            bool inprops = false;
            bool nextReadOnly = false;
            while ((line = rdr.ReadLine()) != null) {
                if (line.Length == 0) //empty line
                    continue;
                if (line.StartsWith("{"))
                    continue;
                if (line.Equals("};")) {
                    //TODO: push our class
                    return;
                } else if (line.StartsWith("/* readonly */")) {
                    nextReadOnly = true;
                    continue;
                } else if (line.StartsWith("/*")) {
                    continue;
                } else if (line.Contains("// Properties:")) {
                    inprops = true;
                } else if (line.StartsWith("//")) { // // Methods:
                    continue;
                } else if (inprops) { //property
                    string[] parts = line.Replace(";", "").Split(' '); //[TypeName] [PropertyName]
                    if (parts[0].Contains('<')) {
                        string templateType = parts[0].Substring(0, parts[0].IndexOf('<'));
                        string containedType = parts[0].Extract('<', '>');
                        TypeInfo wrapped = globals.Classes.FirstOrDefault(t => t.Key.Equals(containedType)).Value;
                        TemplateInst ti = new TemplateInst() { Name = templateType, IsTemplate = true, WrappedType = wrapped != null ? wrapped : new TypeInfo { Name = containedType, IsComplete = false } };
                        classInfo.Properties[parts[1]] = ti;
                        if (nextReadOnly)
                            classInfo.ReadonlyProperties.Add(parts[1]);
                    } else {
                        string pname = parts[0].EndsWith("@") ? parts[0].Substring(0, parts[0].Length - 1) : parts[0]; //handle
                        TypeInfo pType = null;
                        if (globals.Classes.ContainsKey(pname))
                            pType = globals.Classes[pname];
                        if (pType == null) { //create temp type to resolve
                            pType = new TypeInfo() { Name = pname, IsComplete = false };
                        }
                        classInfo.Properties[parts[1]] = pType;
                        if (nextReadOnly)
                            classInfo.ReadonlyProperties.Add(parts[1]);
                    }
                    nextReadOnly = false;

                } else { //function
                    nextReadOnly = false;
                    classInfo.Functions.Add(_parseFunction(line, globals));
                }
            }
        }

        public static FunctionInfo _parseFunction(string line, Globals globals) {
            int firstParen = line.IndexOf('(');
            int lastParen = line.LastIndexOf(')');
            string baseDecl = line.Substring(0, firstParen);
            string paramDecl = line.Substring(firstParen, lastParen - firstParen+1); //-1 for the ;
            string[] nameParts = baseDecl.Split(' ');
            TypeInfo retType = null;

            //TODO: split the name parts
            if (globals.Classes.ContainsKey(nameParts[0])) {
                retType = globals.Classes[nameParts[0]];
            } else if (nameParts[0].Contains('<')) {
                string wrappedType = nameParts[0].Extract('<', '>');
                string templateType = nameParts[0].Replace(string.Format("<{0}>", wrappedType), "");
                TypeInfo wrapped = globals.Classes.FirstOrDefault(t => t.Key.Equals(wrappedType)).Value;
                TemplateInst ti = new TemplateInst() { Name = nameParts[0], IsTemplate = true, WrappedType = wrapped != null ? wrapped : new TypeInfo { Name = wrappedType, IsComplete = false } };
                retType = ti;
            } else {
                retType = new TypeInfo() { Name = nameParts[0], IsPrimitive = false };
            }
            return new FunctionInfo {Name = nameParts[1], ReturnType = retType, Inner = paramDecl};
        }

    }
}

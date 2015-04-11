using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Debugger.IDE {
    [Serializable]
    public class IDESettings : BaseClass {
        string projectPath_ = "";
        string compilerPath_ = "";
        string compileArgs_ = "";
        string runExe_ = "";
        string debugExe_ = "";
        string debugParams_ = "";
        string runParams_ = "";

        public IDESettings() {
        }

        public static IDESettings GetOrCreate(string aPath) {
            UserData aUserData = UserData.inst();
            IDESettings existing = aUserData.IDESettings.FirstOrDefault(s => s.ProjectPath.Equals(aPath));
            if (existing != null)
                return existing;
            IDESettings ret = new IDESettings { ProjectPath = aPath };
            ret.Parent = aUserData;
            aUserData.IDESettings.Add(ret);
            return ret;
        }

        public string ProjectPath { get { return projectPath_; } set { projectPath_ = value; } }
        public string CompilerPath { get { return compilerPath_; } set { compilerPath_ = value; OnPropertyChanged("CompilerPath"); } }
        public string DebugExe { get { return debugExe_; } set { debugExe_ = value; OnPropertyChanged("DebugExe"); } }
        public string RunExe { get { return runExe_; } set { runExe_ = value; OnPropertyChanged("RunExe"); } }
        public string DebugParams { get { return debugParams_; } set { debugParams_ = value; OnPropertyChanged("DebugParams"); } }
        public string RunParams { get { return runParams_; } set { runParams_ = value; OnPropertyChanged("RunParams"); } }
    }
}

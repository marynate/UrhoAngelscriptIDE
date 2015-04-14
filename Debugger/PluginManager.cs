using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Debugger
{
    public class PluginInfo
    {
        public PluginInfo(string aName, string[] nameParts)
        {
            Name = aName;
            Parts = nameParts;
            Components = new List<string>();
        }

        public string Name { get; private set; }
        public string[] Parts { get; private set; }

        public List<string> Components { get; private set; }
    }

    public class PluginManager
    {
        static PluginManager inst_;
        List<PluginLib.ISearchService> searchServices_ = new List<PluginLib.ISearchService>();
        List<PluginLib.IFileEditor> fileServices_ = new List<PluginLib.IFileEditor>();
        List<PluginLib.ICompilerService> compilers_ = new List<PluginLib.ICompilerService>();
        List<PluginLib.IInfoTab> infoTabs_ = new List<PluginLib.IInfoTab>();

        // List of loaded plugins
        List<PluginInfo> assemblies = new List<PluginInfo>();

        /// <summary>
        /// Constructs and scans a directory for compatible plugins
        /// </summary>
        /// <param name="dir">Directory to scan for DLLs</param>
        public PluginManager(string dir)
        {
            inst_ = this;

            string path = Path.Combine(Directory.GetCurrentDirectory(), dir);

            if (!Directory.Exists(path))
                return;

            foreach (string file in Directory.GetFiles(path))
            {
                if (Path.GetExtension(file).Equals(".dll") && File.Exists(file))
                {
                    try
                    {
                        Assembly asm = Assembly.LoadFile(file);
                        Type[] types = asm.GetExportedTypes();
                        FileVersionInfo myFileVersionInfo = FileVersionInfo.GetVersionInfo(asm.Location);
                        PluginInfo plugin = new PluginInfo(asm.ManifestModule.Name, new string[] { myFileVersionInfo.ProductName, myFileVersionInfo.ProductVersion, myFileVersionInfo.CompanyName, myFileVersionInfo.LegalCopyright, myFileVersionInfo.Comments });

                        foreach (Type t in types)
                        {
                            if (t.GetInterface("PluginLib.IFileEditor") != null)
                            {
                                fileServices_.Add((PluginLib.IFileEditor)Activator.CreateInstance(t));
                                plugin.Components.Add(t.Name);
                            }
                            else if (t.GetInterface("PluginLib.ISearchService") != null)
                            {
                                searchServices_.Add((PluginLib.ISearchService)Activator.CreateInstance(t));
                                plugin.Components.Add(t.Name);
                            } 
                            else if (t.GetInterface("PluginLib.ICompilerService") != null)
                            {
                                compilers_.Add((PluginLib.ICompilerService)Activator.CreateInstance(t));
                                plugin.Components.Add(t.Name);
                            }
                            else if (t.GetInterface("PluginLib.IInfoTab") != null)
                            {
                                infoTabs_.Add((PluginLib.IInfoTab)Activator.CreateInstance(t));
                                plugin.Components.Add(t.Name);
                            }
                        }

                        if (plugin.Components.Count > 0)
                            assemblies.Add(plugin);
                    } 
                    catch (Exception ex)
                    {
                        ErrorHandler.inst().Error(ex);
                    }
                }
            }
        }

        public static PluginManager inst()
        {
            return inst_;
        }

        public List<PluginLib.ISearchService> SearchServices {
            get {
                return searchServices_;
            }
        }

        public List<PluginLib.IFileEditor> FileEditors {
            get {
                return fileServices_;
            }
        }

        public List<PluginLib.ICompilerService> Compilers {
            get {
                return compilers_;
            }
        }
    }
}

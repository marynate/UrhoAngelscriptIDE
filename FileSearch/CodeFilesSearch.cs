using PluginLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileSearch
{
    public class CodeFilesSearch : PluginLib.ISearchService
    {
        public string Name { get { return "Code"; } }

        public void Search(string projectPath, string[] searchTerms, PluginLib.ISearchPublisher publishTo)
        {
            Thread thread = new Thread(delegate()
            {
                ScanFolder(projectPath, searchTerms, true, publishTo);
            });
            thread.Start();
        }

        void ScanFolder(string aPath, string[] aTerms, bool aScanSubdirs, PluginLib.ISearchPublisher publishTo)
        {
            DirectoryInfo info = new DirectoryInfo(aPath);
            if (info.Exists)
            {
                foreach (FileInfo file in info.GetFiles())
                {
                    if (file.Length > 128 * 1024) // limit searching to files less than 128kb
                        continue;
                    string lCaseExt = file.Extension.ToLowerInvariant();
                    if (!lCaseExt.Contains("as"))
                        continue;
                    ScanFile(file.FullName, aTerms, publishTo);
                }
                if (aScanSubdirs)
                {
                    foreach (DirectoryInfo dir in info.GetDirectories())
                    {
                        ScanFolder(dir.FullName, aTerms, aScanSubdirs, publishTo);
                    }
                }
            }
        }

        void ScanFile(string aFile, string[] aTerms, PluginLib.ISearchPublisher publishTo)
        {
            System.IO.StreamReader file = new System.IO.StreamReader(aFile);
            string line;
            int lineNumber = 1;
            while ((line = file.ReadLine()) != null)
            {
                foreach (string term in aTerms)
                {
                    string lCaseLine = line.ToLowerInvariant();
                    if (lCaseLine.Contains(term.ToLowerInvariant()))
                    {
                        SearchResult result = new SearchResult
                        {
                            Column = line.IndexOf(term),
                            Line = lineNumber,
                            File = aFile,
                            Text = line.Trim()
                        };
                        publishTo.PublishSearchResult(result);
                    }
                }
                ++lineNumber;
            }
        }
    }
}

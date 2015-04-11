using Debugger.IDE;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Debugger.Search
{
    public class FileSearch
    {
        ObservableCollection<SearchResult> results_;
        Dispatcher dispatch_;

        public FileSearch(Dispatcher dispatch, ObservableCollection<SearchResult> resultsTarget)
        {
            dispatch_ = dispatch;
            results_ = resultsTarget;
        }

        void PushResult(SearchResult aResult)
        {
            dispatch_.Invoke(delegate() {
                results_.Add(aResult);
            });
        }

        public void ScanFolder(string aPath, string[] aTerms, bool aScanSubdirs)
        {
            DirectoryInfo info = new DirectoryInfo(aPath);
            if (info.Exists)
            {
                foreach (FileInfo file in info.GetFiles())
                {
                    if (file.Length > 128 * 1024) // limit searching to files less than 128kb
                        continue;
                    string lCaseExt = file.Extension.ToLowerInvariant();
                    if (!(lCaseExt.Contains("lua") || lCaseExt.Contains("as") || lCaseExt.Contains("xml") || lCaseExt.Contains("txt")))
                        continue;
                    ScanFile(file.FullName, aTerms);
                }
                if (aScanSubdirs)
                {
                    foreach (DirectoryInfo dir in info.GetDirectories())
                    {
                        ScanFolder(dir.FullName, aTerms, aScanSubdirs);
                    }
                }
            }
        }

        void ScanFile(string aFile, string[] aTerms)
        {
            System.IO.StreamReader file = new System.IO.StreamReader(aFile);
            string line;
            int lineNumber = 1;
            while ((line = file.ReadLine()) != null)
            {
                foreach (string term in aTerms)
                {
                    string lCaseLine = line.ToLowerInvariant();
                    if (lCaseLine.Contains(term))
                    {
                        SearchResult result = new SearchResult { 
                            Column = line.IndexOf(term), 
                            Line = lineNumber, 
                            File = aFile,
                            Text = line };
                        PushResult(result);
                    }
                }
                ++lineNumber;
            }
        }
    }
}

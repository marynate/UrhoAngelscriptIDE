using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginLib
{
    /// <summary>
    /// Records to be published to ISearchPublisher
    /// </summary>
    public class SearchResult : BasePropertyBound
    {
        string text_;
        string file_;
        int line_;
        int column_;

        public String ShortFile { get { return System.IO.Path.GetFileName(File); } }

        public int Line { get { return line_; } set { line_ = value; OnPropertyChanged("Line"); } }
        public int Column { get { return column_; } set { column_ = value; OnPropertyChanged("Column"); } }
        public String Text { get { return text_; } set { text_ = value; OnPropertyChanged("Text"); } }
        public String File { get { return file_; } set { file_ = value; OnPropertyChanged("File"); } }

    }
}

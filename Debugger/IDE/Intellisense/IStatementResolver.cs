using ICSharpCode.AvalonEdit.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Debugger.IDE.Intellisense {
    public interface IStatementResolver {
        bool IsApplicable(TextDocument aDoc, int offset, int line);
        void GetSuggestions(TextDocument aDoc, int offset, int line, ref List<BaseCompletionData> aSuggestions);
    }
}

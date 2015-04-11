using ICSharpCode.AvalonEdit.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Debugger.IDE.Intellisense.StatementResolution {
    public class FunctionResolver : IStatementResolver {
        
        public bool IsApplicable(TextDocument aDoc, int offset, int line) {
            return false;
        }

        public void GetSuggestions(TextDocument aDoc, int offset, int line, ref List<BaseCompletionData> aSuggestions) {
            throw new NotImplementedException();
        }
    }
}

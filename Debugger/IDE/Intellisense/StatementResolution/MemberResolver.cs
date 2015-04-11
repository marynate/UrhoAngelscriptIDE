using ICSharpCode.AvalonEdit.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Debugger.IDE.Intellisense.StatementResolution {
    public class MemberResolver : IStatementResolver {

        public bool IsApplicable(TextDocument aDoc, int offset, int line) {
            if (IntellisenseHelper.ResemblesDotPath(aDoc, offset - 1, line - 1))
                return true;
            return false;
        }


        public void GetSuggestions(TextDocument aDoc, int offset, int line, ref List<BaseCompletionData> aSuggestions) {
            throw new NotImplementedException();
        }
    }
}

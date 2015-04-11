using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Debugger.Editor {
    public class BraceHighlighter : IBackgroundRenderer {
        TextEditor editor;

        public BraceHighlighter(TextEditor editor) {
            this.editor = editor;
        }

        public void Draw(TextView textView, System.Windows.Media.DrawingContext drawingContext) {
            
        }

        public KnownLayer Layer {
            get { return KnownLayer.Background; }
        }
    }
}

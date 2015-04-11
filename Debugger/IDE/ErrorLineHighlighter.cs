using ICSharpCode.AvalonEdit.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Debugger.IDE {
    class ErrorLineHighlighter : IBackgroundRenderer {
        Pen pen;
        FileBaseItem file;

        public ErrorLineHighlighter(FileBaseItem aFile) {
            file = aFile;
        }

        public void Draw(TextView textView, System.Windows.Media.DrawingContext drawingContext) {
            if (pen == null) {
                pen = new Pen(textView.FindResource("WavyBrush") as Brush, 4);
            }
            Size renderSize = textView.RenderSize;
            if (textView != null && textView.VisualLinesValid) {
                foreach (VisualLine line in textView.VisualLines) {
                    CompileError err = IDEProject.inst().CompileErrors.FirstOrDefault(l => l.Line == line.FirstDocumentLine.LineNumber && l.File.Equals(file.Path));
                    if (err != null) {
                        drawingContext.DrawLine(pen, 
                            new Point(0, line.VisualTop + line.Height - textView.ScrollOffset.Y),
                            new Point(renderSize.Width, line.VisualTop - textView.ScrollOffset.Y + line.Height));
                    }
                }
            }
        }

        public KnownLayer Layer {
            get { return KnownLayer.Background; }
        }
    }
}

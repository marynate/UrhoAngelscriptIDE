using ICSharpCode.AvalonEdit.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Debugger.Editor {
    class LineHighlighter : IBackgroundRenderer {
        Brush brush;

        public void Draw(TextView textView, System.Windows.Media.DrawingContext drawingContext) {
            if (brush == null)
                brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF0A0A0A"));
            Size renderSize = textView.RenderSize;
            if (textView != null && textView.VisualLinesValid) {
                foreach (VisualLine line in textView.VisualLines) {
                    if (line.FirstDocumentLine.LineNumber == textView.HighlightedLine) {
                        drawingContext.DrawRectangle(brush, null, new Rect(
                            new Point(0, line.VisualTop - textView.ScrollOffset.Y),
                            new Point(renderSize.Width, line.VisualTop - textView.ScrollOffset.Y + line.Height)));
                    }
                }
            }
        }

        public KnownLayer Layer {
            get { return KnownLayer.Background; }
        }
    }
}

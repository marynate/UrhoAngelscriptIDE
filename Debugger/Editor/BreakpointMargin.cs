using Debugger.Debug;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Debugger.Editor {
    class BreakpointMargin : AbstractMargin {

        //TextView textView;
        //public BreakpointMargin(TextView tv) {
        //    //textView = tv;
        //}
        FileData fileData;

        public BreakpointMargin(FileData aFileData) {
            fileData = aFileData;
        }

        bool subbed = false;
        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters) {
            // accept clicks even when clicking on the background
            if (TextView != null && subbed == false) {
                subbed = true;
                TextView.ScrollOffsetChanged += TextView_ScrollOffsetChanged;
            }
            return new PointHitTestResult(this, hitTestParameters.HitPoint);
        }

        void TextView_ScrollOffsetChanged(object sender, EventArgs e) {
            InvalidateVisual();
        }

        /// <inheritdoc/>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
            onClick(null, e);
        }
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
        }
        protected override void OnMouseWheel(MouseWheelEventArgs e) {
            InvalidateVisual();
        }
        protected override void OnMouseMove(MouseEventArgs e) { }
        protected override void OnMouseEnter(MouseEventArgs e) {
        }
        protected override void OnMouseLeave(MouseEventArgs e) {
        }

        void onClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            base.OnMouseUp(e);
            if (TextView != null && TextView.VisualLinesValid) {
                foreach (VisualLine line in TextView.VisualLines) {
                    Rect hitRect = new Rect(
                        new Point(0, line.VisualTop-TextView.ScrollOffset.Y),
                        new Point(RenderSize.Width,line.VisualTop-TextView.ScrollOffset.Y+line.Height)
                    );
                    
                    if (hitRect.Contains(e.GetPosition(this))) {
                        int ln = line.FirstDocumentLine.LineNumber;
                        Debugger.Debug.Breakpoint bp = fileData.BreakPoints.FirstOrDefault(b => b.LineNumber == ln);
                        if (bp == null) {
                            fileData.BreakPoints.Add(new Debugger.Debug.Breakpoint
                            {
                                LineNumber = ln,
                                File = fileData.SectionName,
                                SectionID = fileData.SectionID,
                                Active = true
                            });
                        } else
                            bp.Active = !bp.Active;
                    }
                }
            }
            this.InvalidateVisual();
        }

        protected override Size MeasureOverride(Size availableSize) {
            return new Size(18, 0);
        }

        Brush rb = new SolidColorBrush(Colors.Red);
        Brush wb = new SolidColorBrush(Colors.White);
        Brush db = new SolidColorBrush(Colors.DarkBlue);
        Brush lb = new SolidColorBrush(Colors.LightBlue);
        protected override void OnRender(DrawingContext drawingContext) {
            TextView textView = this.TextView;
            Size renderSize = this.RenderSize;
            if (textView != null && textView.VisualLinesValid) {
                foreach (VisualLine line in textView.VisualLines) {
                    Debugger.Debug.Breakpoint bp = fileData.BreakPoints.FirstOrDefault(b => b.LineNumber == line.FirstDocumentLine.LineNumber);
                    if (bp != null && bp.Active)
                        drawingContext.DrawEllipse(rb, null, new Point(renderSize.Width / 2, line.VisualTop - textView.ScrollOffset.Y + 9), 9, 9);
                    else if (bp != null) {
                        drawingContext.DrawEllipse(rb, null, new Point(renderSize.Width / 2, line.VisualTop - textView.ScrollOffset.Y + 9), 9, 9);
                        drawingContext.DrawEllipse(wb, null, new Point(renderSize.Width / 2, line.VisualTop - textView.ScrollOffset.Y + 9), 7, 7);
                    }

                    if (Debugger.Debug.SessionData.inst().CurrentLine == line.FirstDocumentLine.LineNumber && Debugger.Debug.SessionData.inst().CurrentSection == fileData.SectionID)
                    {
                        drawingContext.DrawRectangle(db, null, new Rect { 
                            X = 2, 
                            Width = renderSize.Width - 2, 
                            Y = line.VisualTop - textView.ScrollOffset.Y + 4, 
                            Height = renderSize.Width - 8 });
                        drawingContext.DrawRectangle(lb, null, new Rect { 
                            X = 4, 
                            Width = renderSize.Width - 4, 
                            Y = line.VisualTop - textView.ScrollOffset.Y + 6, 
                            Height = renderSize.Width - 10 });
                    }
                }
            }
        }
    }
}

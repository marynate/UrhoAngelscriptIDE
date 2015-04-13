using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Debugger {
    /// <summary>
    /// Unused functions meant for drawing curves between points
    /// </summary>
    static class DrawingUtility {
        // linear equation solver utility for ai + bj = c and di + ej = f
        static void solvexy(double a, double b, double c, double d, double e, double f, out double i, out double j) {
            j = (c - a / d * f) / (b - a * e / d);
            i = (c - (b * j)) / a;
        }

        // basis functions
        static double b0(double t) { return Math.Pow(1 - t, 3); }
        static double b1(double t) { return t * (1 - t) * (1 - t) * 3; }
        static double b2(double t) { return (1 - t) * t * t * 3; }
        static double b3(double t) { return Math.Pow(t, 3); }

        static void bez4pts1(double x0, double y0, double x4, double y4, double x5, double y5, double x3, double y3, out double x1, out double y1, out double x2, out double y2) {
            // find chord lengths
            double c1 = Math.Sqrt((x4 - x0) * (x4 - x0) + (y4 - y0) * (y4 - y0));
            double c2 = Math.Sqrt((x5 - x4) * (x5 - x4) + (y5 - y4) * (y5 - y4));
            double c3 = Math.Sqrt((x3 - x5) * (x3 - x5) + (y3 - y5) * (y3 - y5));
            // guess "best" t
            double t1 = c1 / (c1 + c2 + c3);
            double t2 = (c1 + c2) / (c1 + c2 + c3);
            // transform x1 and x2
            solvexy(b1(t1), b2(t1), x4 - (x0 * b0(t1)) - (x3 * b3(t1)), b1(t2), b2(t2), x5 - (x0 * b0(t2)) - (x3 * b3(t2)), out x1, out x2);
            // transform y1 and y2
            solvexy(b1(t1), b2(t1), y4 - (y0 * b0(t1)) - (y3 * b3(t1)), b1(t2), b2(t2), y5 - (y0 * b0(t2)) - (y3 * b3(t2)), out y1, out y2);
        }

        static public PathFigure BezierFromIntersection(Point startPt, Point int1, Point int2, Point endPt) {
            double x1, y1, x2, y2;
            bez4pts1(startPt.X, startPt.Y, int1.X, int1.Y, int2.X, int2.Y, endPt.X, endPt.Y, out x1, out y1, out x2, out y2);
            PathFigure p = new PathFigure { StartPoint = startPt };
            p.Segments.Add(new BezierSegment { Point1 = new Point(x1, y1), Point2 = new Point(x2, y2), Point3 = endPt });
            return p;
        }
    }
}

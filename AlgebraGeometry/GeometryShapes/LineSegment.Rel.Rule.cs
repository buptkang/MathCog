using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AlgebraGeometry
{
    public static class LineSegmentGenerationRule
    {
        public static LineSegmentSymbol GenerateLineSegment(Point pt1, Point pt2)
        {
            if (pt1.Equals(pt2)) return null;

            Debug.Assert(pt1.Concrete);
            Debug.Assert(pt2.Concrete);

            //TODO rules

            var ls  = new LineSegment(pt1, pt2);
            var lss = new LineSegmentSymbol(ls);
            var steps = lss.FromPointsToLineSegment(pt1, pt2, ls);

            if (steps == null) return lss;

            if (lss.Shape.Traces != null)
            {
                lss.Shape.Traces.AddRange(steps);
            }
            else
            {
                lss.Shape.Traces = steps;
            }
            return lss;
        }

        public static string IdentityPoints = "Cannot build the line as two identify points!";
    }
}

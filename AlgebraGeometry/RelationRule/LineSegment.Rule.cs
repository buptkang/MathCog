using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AlgebraGeometry
{
    public static class LineSegmentGenerationRule
    {
        public static LineSegment GenerateLineSegment(Point pt1, Point pt2)
        {
            if (pt1.Equals(pt2)) return null;

            Debug.Assert(pt1.Concrete);
            Debug.Assert(pt2.Concrete);

            //TODO rules

            return new LineSegment(pt1, pt2);
        }

        public static string IdentityPoints = "Cannot build the line as two identify points!";
    }
}

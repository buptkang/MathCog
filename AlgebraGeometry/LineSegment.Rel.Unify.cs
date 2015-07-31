using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using CSharpLogic;

namespace AlgebraGeometry
{
    public static class LineSegRelation
    {
        public static LineSegmentSymbol Unify(PointSymbol pt1, PointSymbol pt2)
        {
            //point identify check
            if (pt1.Equals(pt2)) return null;
            var point1 = pt1.Shape as Point;
            var point2 = pt2.Shape as Point;
            Debug.Assert(point1 != null);
            Debug.Assert(point2 != null);

            //Line build process
            if (pt1.Shape.Concrete && pt2.Shape.Concrete)
            {
                return LineSegmentGenerationRule.GenerateLineSegment(point1, point2);
            }
            else
            {
                //lazy evaluation    
                //Constraint solving on Graph
                var lineSeg = new LineSegment(null); //Ghost Line Segment
                lineSeg.Pt1 = point1;
                lineSeg.Pt2 = point2;
                return new LineSegmentSymbol(lineSeg);
            }
        }
    }
}

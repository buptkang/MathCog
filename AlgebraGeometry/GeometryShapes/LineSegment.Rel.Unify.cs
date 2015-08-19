using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using CSharpLogic;

namespace AlgebraGeometry
{
    public static class LineSegBinaryRelation
    {
        public static LineSegmentSymbol Unify(PointSymbol pt1, PointSymbol pt2)
        {
            //point identify check
            if (pt1.Equals(pt2)) return null;
            var point1 = pt1.Shape as Point;
            var point2 = pt2.Shape as Point;
            Debug.Assert(point1 != null);
            Debug.Assert(point2 != null);

            //lazy evaluation    
            //Constraint solving on Graph
            var lineSeg = new LineSegment(null); //Ghost Line Segment
            lineSeg.Pt1 = point1;
            lineSeg.Pt2 = point2;
            var lss = new LineSegmentSymbol(lineSeg);
            //Line build process
            if (pt1.Shape.Concrete)
            {
                if (pt2.Shape.Concrete)
                {
                    return LineSegmentGenerationRule.GenerateLineSegment(point1, point2);
                }

                if (pt2.CachedSymbols.Count != 0)
                {
                    foreach (ShapeSymbol ss in pt2.CachedSymbols)
                    {
                        var ps = ss as PointSymbol;
                        Debug.Assert(ps != null);
                        Debug.Assert(ps.Shape.Concrete);
                        var cachePoint = ps.Shape as Point;
                        Debug.Assert(cachePoint != null);
                        var gLss = LineSegmentGenerationRule.GenerateLineSegment(point1, cachePoint);
                        lss.CachedSymbols.Add(gLss);
                    }
                }
                return lss;
            }
            Debug.Assert(!pt1.Shape.Concrete);
            if (pt2.Shape.Concrete)
            {
                if (pt1.CachedSymbols.Count != 0)
                {
                    foreach (ShapeSymbol ss in pt1.CachedSymbols)
                    {
                        var ps = ss as PointSymbol;
                        Debug.Assert(ps != null);
                        Debug.Assert(ps.Shape.Concrete);
                        var cachePoint = ps.Shape as Point;
                        Debug.Assert(cachePoint != null);
                        var gLss = LineSegmentGenerationRule.GenerateLineSegment(cachePoint, point2);
                        lss.CachedSymbols.Add(gLss);
                    }
                }
                return lss;
            }
            Debug.Assert(!pt2.Shape.Concrete);

            foreach (ShapeSymbol ss1 in pt1.CachedSymbols)
            {
                foreach (ShapeSymbol ss2 in pt2.CachedSymbols)
                {
                    var ps1 = ss1 as PointSymbol;
                    Debug.Assert(ps1 != null);
                    Debug.Assert(ps1.Shape.Concrete);
                    var cachePoint1 = ps1.Shape as Point;
                    Debug.Assert(cachePoint1 != null);
                    var ps2 = ss2 as PointSymbol;
                    Debug.Assert(ps2 != null);
                    Debug.Assert(ps2.Shape.Concrete);
                    var cachePoint2 = ps2.Shape as Point;
                    Debug.Assert(cachePoint2 != null);
                    var gLss = LineSegmentGenerationRule.GenerateLineSegment(cachePoint1, cachePoint2);
                    lss.CachedSymbols.Add(gLss);
                }
            }
            return lss;
        }
    }

    public static class LineSegUnaryRelation
    {
        public static object Unify(this LineSegmentSymbol lss, object constraint)
        {
            var refObj = constraint as string;
            Debug.Assert(refObj != null);

            switch (refObj)
            {
                case LineSegmentAcronym.Distance1:
                case LineSegmentAcronym.Distance2:
                    return lss.InferDistance(refObj);
            }

            return null;
        }

        private static object InferDistance(this LineSegmentSymbol inputLineSymbol, string label)
        {
            var lineSeg = inputLineSymbol.Shape as LineSegment;
            Debug.Assert(lineSeg != null);

            if (lineSeg.Distance != null)
            {
                var lst = new List<object>();

                var goal = new EqGoal(new Var(label), lineSeg.Distance);
                var steps = inputLineSymbol.FromLineSegmentToDistance(lineSeg, goal);
                if (goal.Traces != null)
                {
                    goal.Traces.AddRange(steps);
                }
                else
                {
                    goal.Traces = steps;
                }
                return goal;
            }

            if (inputLineSymbol.CachedSymbols.Count != 0)
            {
                var goalList = new List<EqGoal>();
                foreach (var lss in inputLineSymbol.CachedSymbols)
                {
                    var cachedLss = lss as LineSegmentSymbol;
                    Debug.Assert(cachedLss != null);
                    var cachedLs = cachedLss.Shape as LineSegment;
                    Debug.Assert(cachedLs != null);
                    var goal = new EqGoal(new Var(label), cachedLs.Distance);
                    var steps = inputLineSymbol.FromLineSegmentToDistance(cachedLs, goal);
                    if (goal.Traces != null)
                    {
                        goal.Traces.AddRange(steps);
                    }
                    else
                    {
                        goal.Traces = steps;
                    }
                    goalList.Add(goal);
                }
                return goalList;
            }

            return null;
        }
    }
}

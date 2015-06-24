using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace AlgebraGeometry
{
    /// <summary>
    /// Type checking of relation objects and their corresponding types
    /// </summary>
    public static partial class RelationLogic
    {
        public static bool Reify(Shape currShape, object depend1, object depend2)
        {
            var shape1 = depend1 as Shape;
            var shape2 = depend2 as Shape;
            if (shape1 != null && shape2 != null)
            {
                return Reify(currShape, shape1, shape2);
            }

            return false;
        }

        public static bool Reify(Shape currShape, Shape shape1, Shape shape2)
        {
            var line = currShape as Line;
            if (line != null)
            {
                var pt1 = shape1 as Point;
                var pt2 = shape2 as Point;
                Debug.Assert(pt1 != null);
                Debug.Assert(pt2 != null);
                return line.Reify(pt1, pt2);
            }

            var lineSeg = currShape as LineSegment;
            if (lineSeg != null)
            {
                var pt1 = shape1 as Point;
                var pt2 = shape2 as Point;
                Debug.Assert(pt1 != null);
                Debug.Assert(pt2 != null);
                return lineSeg.Reify(pt1, pt2);
            }

            throw new Exception("TODO");
        }

        private static bool Reify(this LineSegment lineSeg, Point pt1, Point pt2)
        {
            if (!lineSeg.RelationStatus) return false;
            lineSeg.CachedSymbols.Clear(); //re-compute purpose

            var shape1Lst = new List<Point>();
            var shape2Lst = new List<Point>();

            #region Caching Point 1
            if (pt1.Concrete)
            {
                shape1Lst.Add(pt1);
            }
            else
            {
                foreach (var shape in pt1.CachedSymbols.ToList())
                {
                    var ptTemp = shape as Point;
                    Debug.Assert(ptTemp != null);
                    if (ptTemp.Concrete)
                    {
                        shape1Lst.Add(ptTemp);
                    }
                }
            }
            #endregion

            #region Caching Point 2
            if (pt2.Concrete)
            {
                shape2Lst.Add(pt2);
            }
            else
            {
                foreach (var shape in pt2.CachedSymbols.ToList())
                {
                    var ptTemp = shape as Point;
                    Debug.Assert(ptTemp != null);
                    if (ptTemp.Concrete)
                    {
                        shape2Lst.Add(ptTemp);
                    }
                }
            }
            #endregion

            #region Generate caching linesegment

            if (shape1Lst.Count == 0 || shape2Lst.Count == 0) return false;
            foreach (var gPt1 in shape1Lst)
            {
                foreach (var gPt2 in shape2Lst)
                {
                    Debug.Assert(gPt1 != null);
                    Debug.Assert(gPt2 != null);
                    Debug.Assert(gPt1.Concrete);
                    Debug.Assert(gPt2.Concrete);

                    var lineTemp = LineSegmentGenerationRule.GenerateLineSegment(gPt1, gPt2);
                    if (lineTemp != null)
                    {
                        lineSeg.CachedSymbols.Add(lineTemp);
                    }
                }
            }

            #endregion

            return true;
        }

        private static bool Reify(this Line line, Point pt1, Point pt2)
        {
            if (!line.RelationStatus) return false;
            line.CachedSymbols.Clear(); //re-compute purpose

            var shape1Lst = new List<Point>();
            var shape2Lst = new List<Point>();

            #region Caching Point 1
            if (pt1.Concrete)
            {
                shape1Lst.Add(pt1);
            }
            else
            {
                foreach (var shape in pt1.CachedSymbols.ToList())
                {
                    var ptTemp = shape as Point;
                    Debug.Assert(ptTemp != null);
                    if (ptTemp.Concrete)
                    {
                        shape1Lst.Add(ptTemp);
                    }
                }
            }
            #endregion

            #region Caching Point 2
            if (pt2.Concrete)
            {
                shape2Lst.Add(pt2);
            }
            else
            {
                foreach (var shape in pt2.CachedSymbols.ToList())
                {
                    var ptTemp = shape as Point;
                    Debug.Assert(ptTemp != null);
                    if (ptTemp.Concrete)
                    {
                        shape2Lst.Add(ptTemp);
                    }
                }
            }
            #endregion

            #region Generate caching line

            if (shape1Lst.Count == 0 || shape2Lst.Count == 0) return false;
            foreach (var gPt1 in shape1Lst)
            {
                foreach (var gPt2 in shape2Lst)
                {
                    Debug.Assert(gPt1 != null);
                    Debug.Assert(gPt2 != null);
                    Debug.Assert(gPt1.Concrete);
                    Debug.Assert(gPt2.Concrete);

                    var lineTemp = LineGenerationRule.GenerateLine(gPt1, gPt2);
                    if (lineTemp != null)
                    {
                        line.CachedSymbols.Add(lineTemp);
                    }
                }
            }

            #endregion
            
            return true;
        }
    }
}

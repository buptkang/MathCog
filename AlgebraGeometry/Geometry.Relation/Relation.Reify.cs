using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using CSharpLogic;

namespace AlgebraGeometry
{
    /// <summary>
    /// Type checking of relation objects and their corresponding types
    /// </summary>
    public static partial class RelationLogic
    {
        public static bool Reify(ShapeSymbol currShape, object depend1, object depend2)
        {
            var ss1 = depend1 as ShapeSymbol;
            var ss2 = depend2 as ShapeSymbol;
            if (ss1 != null && ss2 != null) return Reify(currShape, ss1, ss2);
            //TODO
            return false;
        }

        private static bool Reify(ShapeSymbol currShape, ShapeSymbol shape1, ShapeSymbol shape2)
        {
            var line = currShape as LineSymbol;
            if (line != null)
            {
                var pt1 = shape1 as PointSymbol;
                var pt2 = shape2 as PointSymbol;
                Debug.Assert(pt1 != null);
                Debug.Assert(pt2 != null);
                return line.Reify(pt1, pt2);
            }

            var lineSeg = currShape as LineSegmentSymbol;
            if (lineSeg != null)
            {
                var pt1 = shape1 as PointSymbol;
                var pt2 = shape2 as PointSymbol;
                Debug.Assert(pt1 != null);
                Debug.Assert(pt2 != null);
                return lineSeg.Reify(pt1, pt2);
            }

            throw new Exception("TODO");
        }

        private static bool Reify(this LineSegmentSymbol lineSegSymbol, 
                                 PointSymbol ps1, PointSymbol ps2)
        {
            //if (!lineSeg.RelationStatus) return false;
            lineSegSymbol.CachedSymbols.Clear(); //re-compute purpose

            var shape1Lst = new List<PointSymbol>();
            var shape2Lst = new List<PointSymbol>();

            #region Caching Point 1
            if (ps1.Shape.Concrete)
            {
                shape1Lst.Add(ps1);
            }
            else
            {
                foreach (var shapeSymbol in ps1.CachedSymbols.ToList())
                {
                    var ptTemp = shapeSymbol as PointSymbol;
                    Debug.Assert(ptTemp != null);
                    if (ptTemp.Shape.Concrete)
                    {
                        shape1Lst.Add(ptTemp);
                    }
                }
            }
            #endregion

            #region Caching Point 2
            if (ps2.Shape.Concrete)
            {
                shape2Lst.Add(ps2);
            }
            else
            {
                foreach (var shapeSymbol in ps2.CachedSymbols.ToList())
                {
                    var ptTemp = shapeSymbol as PointSymbol;
                    Debug.Assert(ptTemp != null);
                    if (ptTemp.Shape.Concrete)
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

                    var gPoint1 = gPt1.Shape as Point;
                    var gPoint2 = gPt2.Shape as Point;
                    Debug.Assert(gPoint1 !=null);
                    Debug.Assert(gPoint2 !=null);
                    Debug.Assert(gPoint1.Concrete);
                    Debug.Assert(gPoint2.Concrete);

                    var lineTemp = LineSegmentGenerationRule.GenerateLineSegment(gPoint1, gPoint2);
                    if (lineTemp != null)
                    {
                        lineSegSymbol.CachedSymbols.Add(lineTemp);
                    }
                }
            }

            #endregion

            return true;
        }

        private static bool Reify(this LineSymbol line, PointSymbol pt1, PointSymbol pt2)
        {
            //if (!line.RelationStatus) return false;
            line.CachedSymbols.Clear(); //re-compute purpose

            var shape1Lst = new List<PointSymbol>();
            var shape2Lst = new List<PointSymbol>();

            #region Caching Point 1
            if (pt1.Shape.Concrete)
            {
                shape1Lst.Add(pt1);
            }
            else
            {
                foreach (var shapeSymbol in pt1.CachedSymbols.ToList())
                {
                    var ptTemp = shapeSymbol as PointSymbol;
                    Debug.Assert(ptTemp != null);
                    if (ptTemp.Shape.Concrete)
                    {
                        shape1Lst.Add(ptTemp);
                    }
                }
            }
            #endregion

            #region Caching Point 2
            if (pt2.Shape.Concrete)
            {
                shape2Lst.Add(pt2);
            }
            else
            {
                foreach (var shapeSymbol in pt2.CachedSymbols.ToList())
                {
                    var ptTemp = shapeSymbol as PointSymbol;
                    Debug.Assert(ptTemp != null);
                    if (ptTemp.Shape.Concrete)
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
                    var gPoint1 = gPt1.Shape as Point;
                    var gPoint2 = gPt2.Shape as Point;
                    Debug.Assert(gPoint1 != null);
                    Debug.Assert(gPoint2 != null);
                    Debug.Assert(gPoint1.Concrete);
                    Debug.Assert(gPoint2.Concrete);
                    var lineTemp = LineGenerationRule.GenerateLine(gPoint1, gPoint2);
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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AlgebraGeometry
{
    public class RelationLogic
    { 
        #region Relation CRUD

        public static Shape BuildRelation(Shape shape1, Shape shape2, ShapeType shapeType)
        {
            var pt1 = shape1 as Point;
            var pt2 = shape2 as Point;

            if (pt1 != null && pt2 != null)
            {
                if (shapeType == ShapeType.None)
                {
                    //BuildLineRelation(pt1, pt2);
                    //BuildLineSegmentRelation(pt1, pt2);
                    throw new Exception("TODO Constraint solving");
                }
                else if(shapeType == ShapeType.Line)
                {
                    return LineRelationExtension.Unify(pt1, pt2);
                }
                else if (shapeType == ShapeType.Segment)
                {
                    
                }
                else
                {
                    throw new Exception("TODO");
                }
            }

            return null;
        }

        public static bool UpdateRelation(Shape currShape, Shape atomShape1, Shape atomShape2)
        {
            //lazy evaluation
            var pt1 = atomShape1 as Point;
            var pt2 = atomShape2 as Point;

            if (pt1 != null && pt2 != null)
            {
                if (currShape.ShapeType == ShapeType.None)
                {
                    //BuildLineRelation(pt1, pt2);
                    //BuildLineSegmentRelation(pt1, pt2);
                    throw new Exception("TODO Constraint solving");
                }
                else if (currShape.ShapeType == ShapeType.Line)
                {
                    var line = currShape as Line;
                    return line.Reify(pt1, pt2);
                }
                else if (currShape.ShapeType == ShapeType.Segment)
                {

                }
                else
                {
                    throw new Exception("TODO");
                }
            }

            return false;
        }

        #endregion
    }

    public static class LineRelationExtension
    {
        public static bool Reify(this Line line, Point pt1, Point pt2)
        {
            if (!line.RelationStatus) return false;
            line.CachedSymbols.Clear(); //re-compute purpose

            var shape1Lst = new List<Point>();
            var shape2Lst = new List<Point>();
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

            if (shape1Lst.Count == 0 || shape2Lst.Count == 0) return false;
            foreach (var gPt1 in shape1Lst)
            {
                foreach (var gPt2 in shape2Lst)
                {
                    Debug.Assert(gPt1 != null);
                    Debug.Assert(gPt2 != null);
                    Debug.Assert(gPt1.Concrete);
                    Debug.Assert(gPt2.Concrete);

                    var lineTemp = RelationFactory.GenerateLine(gPt1, gPt2);
                    if (lineTemp != null)
                    {
                        line.CachedSymbols.Add(lineTemp);
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// construct a line through two points
        /// </summary>
        /// <param name="pt1"></param>
        /// <param name="pt2"></param>
        /// <returns></returns>
        public static Line Unify(Point pt1, Point pt2)
        {
            //point identify check
            if (pt1.Equals(pt2)) return null;

            //Line build process
            if (pt1.Concrete && pt2.Concrete)
            {
                var line = RelationFactory.GenerateLine(pt1, pt2);
                return line;
            }
            else
            {
                //lazy evaluation    
                //Constraint solving on Graph
                var line = new Line(null); //ghost line
                return line;
            }
            return null;
        }
    }

    public static class LineSegRelationExtension
    {
        private static object BuildLineSegmentRelation(Point pt1, Point pt2)
        {
            return null;
        }        
    }


    //        public static Relation CreateRelation(Shape shape1, Shape shape2)
    //        {
    //            if (shape1 is Point && shape2 is Point)
    //            {
    //                return CreateTwoPoints(shape1 as Point, shape2 as Point);
    //            }
    //            else if(shape1 is Line && shape2 is Line)
    //            {
    //                return CreateTwoLines(shape1 as Line, shape2 as Line);
    //            }
    //            else if (shape1 is Point && shape2 is Line)
    //            {
    //                return CreatePointLine(shape1 as Point, shape2 as Line);
    //            }
    //            else if (shape1 is Line && shape2 is Point)
    //            {
    //                return CreatePointLine(shape2 as Point, shape1 as Line);
    //            }
    //            else if (shape1 is Line && shape2 is Circle)
    //            {
    //                return CreateLineCircle(shape1 as Line, shape2 as Circle);
    //            }
    //            else if (shape1 is Circle && shape2 is Line)
    //            {
    //                return CreateLineCircle(shape2 as Line, shape1 as Circle);
    //            }

    //            return null;
    //        }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using CSharpLogic;


namespace AlgebraGeometry
{
    public partial class Point : Shape
    {
        public static string X = "x";
        public static string Y = "y";

        public void GenerateXCacheSymbol(object obj, EqGoal goal)
        {
            CachedGoals.Add(new KeyValuePair<object, EqGoal>(X,goal));

            if (CachedSymbols.Count == 0)
            {
                var gPoint = new Point(Label, obj, YCoordinate);
                gPoint.CachedGoals.Add(new KeyValuePair<object, EqGoal>(X, goal));
                CachedSymbols.Add(gPoint);
            }
            else
            {
                foreach (Shape shape in CachedSymbols.ToList())
                {
                    var pt = shape as Point;
                    if (pt != null)
                    {
                        var xResult = LogicSharp.Reify(pt.XCoordinate, goal.ToDict());
                        if (!pt.XCoordinate.Equals(xResult))
                        {
                            //substitute
                            pt.XCoordinate = xResult;
                            pt.CachedGoals.Add(new KeyValuePair<object, EqGoal>(X,goal));
                        }
                        else
                        {
                            //generate
                            var gPoint = new Point(pt.Label, obj, pt.YCoordinate);
                            gPoint.CachedGoals.Add(new KeyValuePair<object, EqGoal>(X, goal));
                            foreach (KeyValuePair<object, EqGoal> pair in pt.CachedGoals)
                            {
                                if (pair.Key.Equals(Y))
                                {
                                    gPoint.CachedGoals.Add(new KeyValuePair<object, EqGoal>(Y, pair.Value));
                                }
                            } 
                            CachedSymbols.Add(gPoint);
                        }
                    }
                }                
            }
        }

        public void GenerateYCacheSymbol(object obj, EqGoal goal)
        {
            CachedGoals.Add(new KeyValuePair<object, EqGoal>(Y, goal));
            if (CachedSymbols.Count == 0)
            {
                var gPoint = new Point(Label, XCoordinate, obj);
                gPoint.CachedGoals.Add(new KeyValuePair<object, EqGoal>(Y, goal));
                CachedSymbols.Add(gPoint);
            }
            else
            {
                foreach (Shape shape in CachedSymbols.ToList())
                {
                    var pt = shape as Point;
                    if (pt != null)
                    {
                        var yResult = LogicSharp.Reify(pt.YCoordinate, goal.ToDict());
                        if (!pt.YCoordinate.Equals(yResult))
                        {
                            //substitute
                            pt.YCoordinate = yResult;
                            pt.CachedGoals.Add(new KeyValuePair<object, EqGoal>(Y,goal));
                        }
                        else
                        {
                            //generate
                            var gPoint = new Point(pt.Label, pt.XCoordinate, obj);
                            gPoint.CachedGoals.Add(new KeyValuePair<object, EqGoal>(Y, goal));
                            foreach (KeyValuePair<object, EqGoal> pair in pt.CachedGoals)
                            {
                                if (pair.Key.Equals(X))
                                {
                                    gPoint.CachedGoals.Add(new KeyValuePair<object, EqGoal>(X, pair.Value));
                                }
                            } 
                            CachedSymbols.Add(gPoint);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// for generated shapes
        /// </summary>
        /// <param name="goal"></param>
        /// <param name="parent"></param>
        public override void UndoGoal(EqGoal goal, object p)
        {
            var parent = p as Point;
            if (parent == null) return;

            string field = null;
            foreach (KeyValuePair<object, EqGoal> eg in CachedGoals)
            {
                if (eg.Value.Equals(goal))
                {
                    field = eg.Key.ToString();
                }
            }

            if (X.Equals(field))
            {
                this.XCoordinate = parent.XCoordinate;
            }
            else if (Y.Equals(field))
            {
                this.YCoordinate = parent.YCoordinate;
            }

            this.RemoveGoal(goal);
        }
    }

    public static class PointExtension
    {
        /// <summary>
        /// This method quarantees that the substitution term is unique toward each variable.
        /// 
        /// dict: {{x:3},{x:4}} is not allowed 
        /// dict: {{x:3}, {y:4}} is allowed
        /// </summary>
        /// <param name="point"></param>
        /// <param name="dict"></param>
        /// <returns></returns>

        public static bool Reify(this Point point, EqGoal goal)
        {
            if (!goal.IsAssignment()) return false;
            if (point.Concrete) return false;

            object xResult = point.EvalGoal(point.XCoordinate, goal);
            object yResult = point.EvalGoal(point.YCoordinate, goal);

            //Atomic operation
            if (!point.XCoordinate.Equals(xResult))
            {
                point.GenerateXCacheSymbol(xResult,goal);
                return true;                
            }
            else if (!point.YCoordinate.Equals(yResult))
            {
                point.GenerateYCacheSymbol(yResult,goal);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool UnReify(this Point point, EqGoal goal)
        {
            if (!point.ContainGoal(goal)) return false;
            var updateLst = new HashSet<Shape>();
            var unchangedLst = new HashSet<Shape>();
            foreach (var shape in point.CachedSymbols.ToList())
            {
                var pt = shape as Point;
                if (pt == null) continue;
                if (pt.ContainGoal(goal))
                {
                    pt.UndoGoal(goal, point);
                    if (pt.CachedGoals.Count != 0)
                    {
                        updateLst.Add(pt);
                    }
                }
                else
                {
                    unchangedLst.Add(shape);
                }
            }

            if (unchangedLst.Count != 0)
            {
                point.CachedSymbols = unchangedLst;
            }
            else
            {
                point.CachedSymbols = updateLst;
            }
            
            point.RemoveGoal(goal); 
            return true;
        }

        public static bool Reify(this Point point, IEnumerable<EqGoal> goals)
        {
            foreach (EqGoal goal in goals)
            {
                if (!point.Reify(goal))
                {
                    //TODO transaction need to trace back
                    return false;
                }
            }
            return true;
        }
    }
}

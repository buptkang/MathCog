using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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

                //Transform goal trace
                for (int i = goal.Traces.Count - 1; i >= 0; i--)
                {
                    gPoint.Traces.Insert(0, goal.Traces[i]);
                }              

                //Substitution trace
                var ts = new TraceStep(new PointSymbol(this), 
                    new PointSymbol(gPoint), 
                    SubstitutionRule.ApplySubstitute(this, goal));
                gPoint.Traces.Insert(0,ts);
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
                            var gPt = new Point(pt.Label, pt.XCoordinate, pt.YCoordinate);

                            //substitute
                            pt.XCoordinate = xResult;
                            pt.CachedGoals.Add(new KeyValuePair<object, EqGoal>(X,goal));

                            //transform goal trace
                            for (int i = goal.Traces.Count - 1; i >= 0; i--)
                            {
                                pt.Traces.Insert(0, goal.Traces[i]);
                            }    

                            var ts = new TraceStep(new PointSymbol(pt), 
                                new PointSymbol(gPt), 
                                SubstitutionRule.ApplySubstitute(pt, goal));
                            pt.Traces.Insert(0,ts);
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

                            //substitute
                            //Add traces from pt to gPoint
                            for (int i = pt.Traces.Count - 1; i >= 0; i--)
                            {
                                gPoint.Traces.Insert(0, pt.Traces[i]);
                            }

                            //transform goal trace
                            for (int i = goal.Traces.Count - 1; i >= 0; i--)
                            {
                                gPoint.Traces.Insert(0, goal.Traces[i]);
                            } 

                            var ts = new TraceStep(new PointSymbol(pt), 
                                new PointSymbol(gPoint), 
                                SubstitutionRule.ApplySubstitute(pt, goal));
                            gPoint.Traces.Insert(0,ts);
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

                //transform goal trace
                for (int i = goal.Traces.Count - 1; i >= 0; i--)
                {
                    gPoint.Traces.Insert(0, goal.Traces[i]);
                } 

                //Substitution trace
                var ts = new TraceStep(new PointSymbol(this), 
                    new PointSymbol(gPoint), SubstitutionRule.ApplySubstitute(this, goal));
                gPoint.Traces.Insert(0, ts);
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
                            var gPt = new Point(pt.Label, pt.XCoordinate, pt.YCoordinate);

                            //substitute
                            pt.YCoordinate = yResult;
                            pt.CachedGoals.Add(new KeyValuePair<object, EqGoal>(Y,goal));

                            //transform goal trace
                            for (int i = goal.Traces.Count - 1; i >= 0; i--)
                            {
                                pt.Traces.Insert(0, goal.Traces[i]);
                            } 

                            var ts = new TraceStep(new PointSymbol(pt), 
                                new PointSymbol(gPt), 
                                SubstitutionRule.ApplySubstitute(pt, goal));
                            pt.Traces.Insert(0, ts);
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

                            //substitute
                            //Add traces from pt to gPoint
                            for (int i = pt.Traces.Count - 1; i >= 0; i--)
                            {
                                gPoint.Traces.Insert(0, pt.Traces[i]);
                            }

                            //transform goal trace
                            for (int i = goal.Traces.Count - 1; i >= 0; i--)
                            {
                                gPoint.Traces.Insert(0, goal.Traces[i]);
                            } 

                            var ts = new TraceStep(new PointSymbol(pt), 
                                new PointSymbol(gPoint), 
                                SubstitutionRule.ApplySubstitute(pt, goal));
                            gPoint.Traces.Insert(0, ts);
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
            //pre-processing of goal
            EqGoal tempGoal;
            if (goal.TraceCount != 0)
            {
                var trace = goal.Traces[0];
                Debug.Assert(trace.Target != null);
                var traceGoal = trace.Target as EqGoal;
                Debug.Assert(traceGoal != null);
                tempGoal = traceGoal;
            }
            else
            {
                tempGoal = goal;
            }

            bool cond1 = Var.IsVar(tempGoal.Lhs) && LogicSharp.IsNumeric(tempGoal.Rhs);
            bool cond2 = Var.IsVar(tempGoal.Rhs) && LogicSharp.IsNumeric(tempGoal.Lhs);
            Debug.Assert(cond1 || cond2);

            if (point.Concrete) return false;

            object xResult = point.EvalGoal(point.XCoordinate, tempGoal);
            object yResult = point.EvalGoal(point.YCoordinate, tempGoal);

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
    }
}

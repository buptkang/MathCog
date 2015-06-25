using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using CSharpLogic;
using System.Diagnostics;

namespace AlgebraGeometry
{
    public partial class Line : Shape
    {
        public void CacheA(object obj, EqGoal goal)
        {
            CachedGoals.Add(new KeyValuePair<object, EqGoal>(LineAcronym.A, goal));

            if (CachedSymbols.Count == 0)
            {
                #region generate new object

                var gLine = new Line(Label, obj, B, C);
                gLine.CachedGoals.Add(new KeyValuePair<object, EqGoal>(LineAcronym.A, goal));
                CachedSymbols.Add(gLine);

                //Transform goal trace
                for (int i = goal.Traces.Count - 1; i >= 0; i--)
                {
                    gLine.Traces.Insert(0, goal.Traces[i]);
                }

                //Substitution trace
                var ts = new TraceStep(new LineSymbol(this),
                                       new LineSymbol(gLine),
                                       SubstitutionRule.ApplySubstitute(this, goal));
                gLine.Traces.Insert(0, ts);

                #endregion
            }
            else
            {
                #region Iterate existing point object

                foreach (Shape shape in CachedSymbols.ToList())
                {
                    var line = shape as Line;
                    if (line != null)
                    {
                        var aResult = LogicSharp.Reify(line.A, goal.ToDict());
                        if (!line.A.Equals(aResult))
                        {
                            var gline = new Line(line.Label, line.A, line.B, line.C);

                            //substitute
                            line.A = aResult;
                            line.CachedGoals.Add(new KeyValuePair<object, EqGoal>(LineAcronym.A, goal));

                            //transform goal trace
                            for (int i = goal.Traces.Count - 1; i >= 0; i--)
                            {
                                line.Traces.Insert(0, goal.Traces[i]);
                            }

                            var ts = new TraceStep(new LineSymbol(gline),
                                                   new LineSymbol(line),
                                                   SubstitutionRule.ApplySubstitute(line, goal));
                            line.Traces.Insert(0, ts);
                        }
                        else
                        {
                            //generate
                            var gLine = new Line(line.Label, obj, line.B, line.C);
                            gLine.CachedGoals.Add(new KeyValuePair<object, EqGoal>(LineAcronym.A, goal));
                            foreach (KeyValuePair<object, EqGoal> pair in line.CachedGoals)
                            {
                                if (pair.Key.Equals(LineAcronym.B))
                                {
                                    gLine.CachedGoals.Add(new KeyValuePair<object, EqGoal>(LineAcronym.B, pair.Value));
                                }
                                else if (pair.Key.Equals(LineAcronym.C))
                                {
                                    gLine.CachedGoals.Add(new KeyValuePair<object, EqGoal>(LineAcronym.C, pair.Value));                                    
                                }
                            }
                            CachedSymbols.Add(gLine);

                            //substitute
                            //Add traces from line to gLine
                            for (int i = line.Traces.Count - 1; i >= 0; i--)
                            {
                                gLine.Traces.Insert(0, line.Traces[i]);
                            }

                            //transform goal trace
                            for (int i = goal.Traces.Count - 1; i >= 0; i--)
                            {
                                gLine.Traces.Insert(0, goal.Traces[i]);
                            }

                            var ts = new TraceStep(new LineSymbol(line),
                                                   new LineSymbol(gLine),
                                                   SubstitutionRule.ApplySubstitute(line, goal));
                            gLine.Traces.Insert(0, ts);
                        }
                    }
                }
                #endregion
            }
        }

        public void CacheB(object obj, EqGoal goal)
        {
            CachedGoals.Add(new KeyValuePair<object, EqGoal>(LineAcronym.B, goal));

            if (CachedSymbols.Count == 0)
            {
                #region generate new object

                var gLine = new Line(Label, A, obj , C);
                gLine.CachedGoals.Add(new KeyValuePair<object, EqGoal>(LineAcronym.B, goal));
                CachedSymbols.Add(gLine);

                //Transform goal trace
                for (int i = goal.Traces.Count - 1; i >= 0; i--)
                {
                    gLine.Traces.Insert(0, goal.Traces[i]);
                }

                //Substitution trace
                var ts = new TraceStep(new LineSymbol(this),
                                       new LineSymbol(gLine),
                                       SubstitutionRule.ApplySubstitute(this, goal));
                gLine.Traces.Insert(0, ts);

                #endregion
            }
            else
            {
                #region Iterate existing point object

                foreach (Shape shape in CachedSymbols.ToList())
                {
                    var line = shape as Line;
                    if (line != null)
                    {
                        var bResult = LogicSharp.Reify(line.B, goal.ToDict());
                        if (!line.B.Equals(bResult))
                        {
                            var gline = new Line(line.Label, line.A, line.B, line.C);

                            //substitute
                            line.B = bResult;
                            line.CachedGoals.Add(new KeyValuePair<object, EqGoal>(LineAcronym.B, goal));

                            //transform goal trace
                            for (int i = goal.Traces.Count - 1; i >= 0; i--)
                            {
                                line.Traces.Insert(0, goal.Traces[i]);
                            }

                            var ts = new TraceStep(new LineSymbol(gline),
                                                   new LineSymbol(line),
                                                   SubstitutionRule.ApplySubstitute(line, goal));
                            line.Traces.Insert(0, ts);
                        }
                        else
                        {
                            //generate
                            var gLine = new Line(line.Label, line.A, obj, line.C);
                            gLine.CachedGoals.Add(new KeyValuePair<object, EqGoal>(LineAcronym.B, goal));
                            foreach (KeyValuePair<object, EqGoal> pair in line.CachedGoals)
                            {
                                if (pair.Key.Equals(LineAcronym.A))
                                {
                                    gLine.CachedGoals.Add(new KeyValuePair<object, EqGoal>(LineAcronym.A, pair.Value));
                                }
                                else if (pair.Key.Equals(LineAcronym.C))
                                {
                                    gLine.CachedGoals.Add(new KeyValuePair<object, EqGoal>(LineAcronym.C, pair.Value));
                                }
                            }
                            CachedSymbols.Add(gLine);

                            //substitute
                            //Add traces from line to gLine
                            for (int i = line.Traces.Count - 1; i >= 0; i--)
                            {
                                gLine.Traces.Insert(0, line.Traces[i]);
                            }

                            //transform goal trace
                            for (int i = goal.Traces.Count - 1; i >= 0; i--)
                            {
                                gLine.Traces.Insert(0, goal.Traces[i]);
                            }

                            var ts = new TraceStep(new LineSymbol(line),
                                                   new LineSymbol(gLine),
                                                   SubstitutionRule.ApplySubstitute(line, goal));
                            gLine.Traces.Insert(0, ts);
                        }
                    }
                }
                #endregion
            }
        }

        public void CacheC(object obj, EqGoal goal)
        {
            CachedGoals.Add(new KeyValuePair<object, EqGoal>(LineAcronym.C, goal));

            if (CachedSymbols.Count == 0)
            {
                #region generate new object

                var gLine = new Line(Label, A, B, obj);
                gLine.CachedGoals.Add(new KeyValuePair<object, EqGoal>(LineAcronym.C, goal));
                CachedSymbols.Add(gLine);

                //Transform goal trace
                for (int i = goal.Traces.Count - 1; i >= 0; i--)
                {
                    gLine.Traces.Insert(0, goal.Traces[i]);
                }

                //Substitution trace
                var ts = new TraceStep(new LineSymbol(this),
                                       new LineSymbol(gLine),
                                       SubstitutionRule.ApplySubstitute(this, goal));
                gLine.Traces.Insert(0, ts);

                #endregion
            }
            else
            {
                #region Iterate existing point object

                foreach (Shape shape in CachedSymbols.ToList())
                {
                    var line = shape as Line;
                    if (line != null)
                    {
                        var cResult = LogicSharp.Reify(line.C, goal.ToDict());
                        if (!line.C.Equals(cResult))
                        {
                            var gline = new Line(line.Label, line.A, line.B, line.C);

                            //substitute
                            line.C = cResult;
                            line.CachedGoals.Add(new KeyValuePair<object, EqGoal>(LineAcronym.C, goal));

                            //transform goal trace
                            for (int i = goal.Traces.Count - 1; i >= 0; i--)
                            {
                                line.Traces.Insert(0, goal.Traces[i]);
                            }

                            var ts = new TraceStep(new LineSymbol(gline),
                                                   new LineSymbol(line),
                                                   SubstitutionRule.ApplySubstitute(line, goal));
                            line.Traces.Insert(0, ts);
                        }
                        else
                        {
                            //generate
                            var gLine = new Line(line.Label, line.A, line.B, obj);
                            gLine.CachedGoals.Add(new KeyValuePair<object, EqGoal>(LineAcronym.C, goal));
                            foreach (KeyValuePair<object, EqGoal> pair in line.CachedGoals)
                            {
                                if (pair.Key.Equals(LineAcronym.A))
                                {
                                    gLine.CachedGoals.Add(new KeyValuePair<object, EqGoal>(LineAcronym.A, pair.Value));
                                }
                                else if (pair.Key.Equals(LineAcronym.B))
                                {
                                    gLine.CachedGoals.Add(new KeyValuePair<object, EqGoal>(LineAcronym.B, pair.Value));
                                }
                            }
                            CachedSymbols.Add(gLine);

                            //substitute
                            //Add traces from line to gLine
                            for (int i = line.Traces.Count - 1; i >= 0; i--)
                            {
                                gLine.Traces.Insert(0, line.Traces[i]);
                            }

                            //transform goal trace
                            for (int i = goal.Traces.Count - 1; i >= 0; i--)
                            {
                                gLine.Traces.Insert(0, goal.Traces[i]);
                            }

                            var ts = new TraceStep(new LineSymbol(line),
                                                   new LineSymbol(gLine),
                                                   SubstitutionRule.ApplySubstitute(line, goal));
                            gLine.Traces.Insert(0, ts);
                        }
                    }
                }
                #endregion
            }
        }

        public override void UndoGoal(EqGoal goal, object p)
        {
            var parent = p as Line;
            if (parent == null) return;

            string field = null;
            foreach (KeyValuePair<object, EqGoal> eg in CachedGoals)
            {
                if (eg.Value.Equals(goal))
                {
                    field = eg.Key.ToString();
                }
            }

            if (LineAcronym.A.Equals(field))
            {
                this.A = parent.A;
            }
            else if (LineAcronym.B.Equals(field))
            {
                this.B = parent.B;
            }
            else if (LineAcronym.C.Equals(field))
            {
                this.C = parent.C;
            }

            this.RemoveGoal(goal);
        }
    }

    public static class LineExtension
    {
        public static bool Reify(this Line line, EqGoal goal)
        {
            EqGoal tempGoal = goal.GetLatestDerivedGoal();
            bool cond1 = Var.IsVar(tempGoal.Lhs) && LogicSharp.IsNumeric(tempGoal.Rhs);
            bool cond2 = Var.IsVar(tempGoal.Rhs) && LogicSharp.IsNumeric(tempGoal.Lhs);
            Debug.Assert(cond1 || cond2);

            if (line.Concrete) return false;

            object aResult = line.EvalGoal(line.A, tempGoal);
            object bResult = line.EvalGoal(line.B, tempGoal);
            object cResult = line.EvalGoal(line.C, tempGoal);

            //Atomic operation
            if(aResult != null && !line.A.Equals(aResult))
            {
                line.CacheA(aResult, goal);
                return true;
            }

            if (bResult != null && !line.B.Equals(bResult))
            {
                line.CacheB(bResult, goal);
                return true;                
            }

            if (cResult != null && !line.C.Equals(cResult))
            {
                line.CacheC(cResult, goal);
                return true;
            }

            return false;
        }

        public static bool UnReify(this Line line, EqGoal goal)
        {
            if (!line.ContainGoal(goal)) return false;
            var updateLst = new HashSet<Shape>();
            var unchangedLst = new HashSet<Shape>();
            foreach (var shape in line.CachedSymbols.ToList())
            {
                var iline = shape as Line;
                if (iline == null) continue;
                if (iline.ContainGoal(goal))
                {
                    iline.UndoGoal(goal, line);
                    if (iline.CachedGoals.Count != 0)
                    {
                        updateLst.Add(iline);
                    }
                }
                else
                {
                    unchangedLst.Add(shape);
                }
            }

            if (unchangedLst.Count != 0)
            {
                line.CachedSymbols = unchangedLst;
            }
            else
            {
                line.CachedSymbols = updateLst;
            }

            line.RemoveGoal(goal);
            return true;
        }
    }
}

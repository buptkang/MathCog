using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using CSharpLogic;
using System.Diagnostics;
using NUnit.Framework;

namespace AlgebraGeometry
{
    public partial class LineSymbol : ShapeSymbol
    {
        public bool Reify(EqGoal goal)
        {
            var line = this.Shape as Line;
            Debug.Assert(line != null);
            EqGoal tempGoal = goal.GetLatestDerivedGoal();
            bool cond1 = Var.IsVar(tempGoal.Lhs) && LogicSharp.IsNumeric(tempGoal.Rhs);
            bool cond2 = Var.IsVar(tempGoal.Rhs) && LogicSharp.IsNumeric(tempGoal.Lhs);
            Debug.Assert(cond1 || cond2);

            if (line.Concrete) return false;

            object aResult = EvalGoal(line.A, tempGoal);
            object bResult = EvalGoal(line.B, tempGoal);
            object cResult = EvalGoal(line.C, tempGoal);

            //Atomic operation
            if (aResult != null && !line.A.Equals(aResult))
            {
                CacheA(aResult, goal);
                return true;
            }

            if (bResult != null && !line.B.Equals(bResult))
            {
                CacheB(bResult, goal);
                return true;
            }

            if (cResult != null && !line.C.Equals(cResult))
            {
                CacheC(cResult, goal);
                return true;
            }

            return false;
        }

        public bool UnReify(EqGoal goal)
        {
            var line = this.Shape as Line;
            Debug.Assert(line != null);
            if (!ContainGoal(goal)) return false;
            var updateLst = new HashSet<ShapeSymbol>();
            var unchangedLst = new HashSet<ShapeSymbol>();
            foreach (var shape in CachedSymbols.ToList())
            {
                var iline = shape as LineSymbol;
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
                CachedSymbols = unchangedLst;
            }
            else
            {
                CachedSymbols = updateLst;
            }
            RemoveGoal(goal);
            return true;
        }

        #region Utilities

        public void CacheA(object obj, EqGoal goal)
        {
            CachedGoals.Add(new KeyValuePair<object, EqGoal>(LineAcronym.A, goal));

            if (CachedSymbols.Count == 0)
            {
                #region generate new object

                var line = Shape as Line;
                Debug.Assert(line != null);

                var gLine = new Line(line.Label, obj, line.B, line.C);
                CachedGoals.Add(new KeyValuePair<object, EqGoal>(LineAcronym.A, goal));
                CachedSymbols.Add(new LineSymbol(gLine));

                //Transform goal trace
                for (int i = goal.Traces.Count - 1; i >= 0; i--)
                {
                    gLine.Traces.Insert(0, goal.Traces[i]);
                }

                //Substitution trace
                string rule  = SubstitutionRule.ApplySubstitute();
                string appliedRule = SubstitutionRule.ApplySubstitute(this, goal);
                var ts = new TraceStep(this,new LineSymbol(gLine),rule, appliedRule);
                gLine.Traces.Insert(0, ts);
                #endregion
            }
            else
            {
                #region Iterate existing point object

                foreach (ShapeSymbol shape in CachedSymbols.ToList())
                {
                    var ls = shape as LineSymbol;
                    if (ls != null)
                    {
                        var line = ls.Shape as Line;
                        Debug.Assert(line != null);
                        var aResult = LogicSharp.Reify(line.A, goal.ToDict());
                        if (!line.A.Equals(aResult))
                        {
                            var gline = new Line(line.Label, line.A, line.B, line.C);
                            //substitute
                            line.A = aResult;
                            CachedGoals.Add(new KeyValuePair<object, EqGoal>(LineAcronym.A, goal));
                            //transform goal trace
                            for (int i = goal.Traces.Count - 1; i >= 0; i--)
                            {
                                line.Traces.Insert(0, goal.Traces[i]);
                            }
                            string rule = SubstitutionRule.ApplySubstitute();
                            string appliedRule = SubstitutionRule.ApplySubstitute(line, goal);
                            var ts = new TraceStep(new LineSymbol(gline),
                                                   new LineSymbol(line),
                                                   rule, appliedRule);
                            line.Traces.Insert(0, ts);
                        }
                        else
                        {
                            //generate
                            var gLine = new Line(line.Label, obj, line.B, line.C);
                            var gLineSymbol = new LineSymbol(gLine);
                            CachedGoals.Add(new KeyValuePair<object, EqGoal>(LineAcronym.A, goal));
                            foreach (KeyValuePair<object, EqGoal> pair in CachedGoals)
                            {
                                if (pair.Key.Equals(LineAcronym.B))
                                {
                                    CachedGoals.Add(new KeyValuePair<object, EqGoal>(LineAcronym.B, pair.Value));
                                }
                                else if (pair.Key.Equals(LineAcronym.C))
                                {
                                    gLineSymbol.CachedGoals.Add(new KeyValuePair<object, EqGoal>(LineAcronym.C, pair.Value));                                    
                                }
                            }
                            CachedSymbols.Add(gLineSymbol);

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
                            string rule = SubstitutionRule.ApplySubstitute();
                            string appliedRule = SubstitutionRule.ApplySubstitute(line, goal);

                            var ts = new TraceStep(ls,gLineSymbol,rule, appliedRule);
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
                var line = Shape as Line;
                Debug.Assert(line != null);

                #region generate new object

                var gLine = new Line(line.Label, line.A, obj , line.C);
                var gLineSymbol = new LineSymbol(gLine);
                gLineSymbol.CachedGoals.Add(new KeyValuePair<object, EqGoal>(LineAcronym.B, goal));
                CachedSymbols.Add(gLineSymbol);

                //Transform goal trace
                for (int i = goal.Traces.Count - 1; i >= 0; i--)
                {
                    gLine.Traces.Insert(0, goal.Traces[i]);
                }
                //Substitution trace
                string rule = SubstitutionRule.ApplySubstitute();
                string appliedRule = SubstitutionRule.ApplySubstitute(gLineSymbol, goal);
                var ts = new TraceStep(this, gLineSymbol, rule, appliedRule);
                gLine.Traces.Insert(0, ts);

                #endregion
            }
            else
            {
                #region Iterate existing point object

                foreach (ShapeSymbol ss in CachedSymbols.ToList())
                {
                    var ls = ss as LineSymbol;
                    if (ls != null)
                    {
                        var line = ls.Shape as Line;
                        Debug.Assert(line != null);

                        var bResult = LogicSharp.Reify(line.B, goal.ToDict());
                        if (!line.B.Equals(bResult))
                        {
                            var gline = new Line(line.Label, line.A, line.B, line.C);
                            var gLineSymbol = new LineSymbol(gline);
                            //substitute
                            line.B = bResult;
                            ls.CachedGoals.Add(new KeyValuePair<object, EqGoal>(LineAcronym.B, goal));

                            //transform goal trace
                            for (int i = goal.Traces.Count - 1; i >= 0; i--)
                            {
                                line.Traces.Insert(0, goal.Traces[i]);
                            }
                            string rule = SubstitutionRule.ApplySubstitute();
                            string appliedRule = SubstitutionRule.ApplySubstitute(ls, goal);

                            var ts = new TraceStep(ls,gLineSymbol, rule, appliedRule);
                            line.Traces.Insert(0, ts);
                        }
                        else
                        {
                            //generate
                            var gLine = new Line(line.Label, line.A, obj, line.C);
                            var gLineSymbol = new LineSymbol(gLine);
                            gLineSymbol.CachedGoals.Add(new KeyValuePair<object, EqGoal>(LineAcronym.B, goal));
                            foreach (KeyValuePair<object, EqGoal> pair in ls.CachedGoals)
                            {
                                if (pair.Key.Equals(LineAcronym.A))
                                {
                                    gLineSymbol.CachedGoals.Add(new KeyValuePair<object, EqGoal>(LineAcronym.A, pair.Value));
                                }
                                else if (pair.Key.Equals(LineAcronym.C))
                                {
                                    gLineSymbol.CachedGoals.Add(new KeyValuePair<object, EqGoal>(LineAcronym.C, pair.Value));
                                }
                            }
                            CachedSymbols.Add(gLineSymbol);

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
                            string rule = SubstitutionRule.ApplySubstitute();
                            string appliedRule = SubstitutionRule.ApplySubstitute(line, goal);
                            var ts = new TraceStep(ls,gLineSymbol,rule, appliedRule);
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
                var line = Shape as Line;
                Debug.Assert(line != null);

                #region generate new object

                var gLine = new Line(line.Label, line.A, line.B, obj);
                var gLineSymbol = new LineSymbol(gLine);
                gLineSymbol.CachedGoals.Add(new KeyValuePair<object, EqGoal>(LineAcronym.C, goal));
                CachedSymbols.Add(gLineSymbol);

                //Transform goal trace
                for (int i = goal.Traces.Count - 1; i >= 0; i--)
                {
                    gLine.Traces.Insert(0, goal.Traces[i]);
                }

                //Substitution trace
                string rule = SubstitutionRule.ApplySubstitute();
                string appliedRule = SubstitutionRule.ApplySubstitute(this, goal);
                var ts = new TraceStep(this, gLineSymbol, rule, appliedRule);
                gLine.Traces.Insert(0, ts);

                #endregion
            }
            else
            {
                #region Iterate existing point object

                foreach (ShapeSymbol ss in CachedSymbols.ToList())
                {
                    var line = ss.Shape as Line;
                    if (line != null)
                    {
                        var cResult = LogicSharp.Reify(line.C, goal.ToDict());
                        if (!line.C.Equals(cResult))
                        {
                            var gline = new Line(line.Label, line.A, line.B, line.C);
                            var gLineSymbol = new LineSymbol(gline);
                            //substitute
                            line.C = cResult;
                            ss.CachedGoals.Add(new KeyValuePair<object, EqGoal>(LineAcronym.C, goal));

                            //transform goal trace
                            for (int i = goal.Traces.Count - 1; i >= 0; i--)
                            {
                                line.Traces.Insert(0, goal.Traces[i]);
                            }

                            string rule = SubstitutionRule.ApplySubstitute();
                            string appliedRule = SubstitutionRule.ApplySubstitute(line, goal);
                            var ts = new TraceStep(gLineSymbol,ss, rule, appliedRule);
                            line.Traces.Insert(0, ts);
                        }
                        else
                        {
                            //generate
                            var gLine = new Line(line.Label, line.A, line.B, obj);
                            var gLineSymbol = new LineSymbol(gLine);
                            gLineSymbol.CachedGoals.Add(new KeyValuePair<object, EqGoal>(LineAcronym.C, goal));
                            foreach (KeyValuePair<object, EqGoal> pair in ss.CachedGoals)
                            {
                                if (pair.Key.Equals(LineAcronym.A))
                                {
                                    gLineSymbol.CachedGoals.Add(new KeyValuePair<object, EqGoal>(LineAcronym.A, pair.Value));
                                }
                                else if (pair.Key.Equals(LineAcronym.B))
                                {
                                    gLineSymbol.CachedGoals.Add(new KeyValuePair<object, EqGoal>(LineAcronym.B, pair.Value));
                                }
                            }
                            CachedSymbols.Add(gLineSymbol);

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
                            string rule = SubstitutionRule.ApplySubstitute();
                            string appliedRule = SubstitutionRule.ApplySubstitute(line, goal);
                            var ts = new TraceStep(ss, gLineSymbol,rule, appliedRule);
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

            var line = Shape as Line;
            Debug.Assert(line != null);
            if (LineAcronym.A.Equals(field))
            {
                line.A = parent.A;
            }
            else if (LineAcronym.B.Equals(field))
            {
                line.B = parent.B;
            }
            else if (LineAcronym.C.Equals(field))
            {
                line.C = parent.C;
            }
            this.RemoveGoal(goal);
        }

        #endregion
    }
}

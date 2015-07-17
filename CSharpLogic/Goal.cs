using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace CSharpLogic
{
    public abstract class Goal : DyLogicObject
    {
        public abstract bool EarlySafe();
        public abstract bool Reify(Dictionary<object, object> substitutions);
        public abstract bool Unify(Dictionary<object, object> substitutions);
    }

    public partial class EqGoal : Goal
    {
        #region Properties and Constructors

        internal Func<Dictionary<object, object>, bool> Functor;

        public string Label { get; set; }
        public object Lhs { get; set; }
        public object Rhs { get; set; }
        public bool IsExpression { get { return Rhs == null; } }

        public EqGoal(object lhs, object rhs, bool generated = false)
        {
            Lhs = lhs;
            Rhs = rhs;
            if (generated) return;

            var lTerm = Lhs as Term;
            var rTerm = Rhs as Term;

            if (lTerm != null)
            {
                #region Term Trace Transformation

                object lhsUpdatedTerm = null;
                //TODO advanced term evaluation
                lhsUpdatedTerm = lTerm.Eval();
                if (!lhsUpdatedTerm.Equals(Lhs))
                {
                    //Tranform all traces to the goal
                    object recurHead = lhsUpdatedTerm;
                    for (int i = lTerm.Traces.Count - 1; i >= 0; i--)
                    {
                        var ts = lTerm.Traces[i];
                        if (ts.Target.Equals(recurHead))
                        {
                            var oldGoal = new EqGoal(ts.Source, Rhs, true);
                            var newGoal = new EqGoal(ts.Target, Rhs, true);
                            var newStep = new TraceStep(oldGoal, newGoal, ts.Rule);
                            Traces.Insert(0, newStep);
                            recurHead = ts.Source;
                        }
                    }
                    Lhs = lhsUpdatedTerm;
                }

                #endregion
            }

            if (rTerm != null)
            {
                #region Term Trace Transformation

                object rhsUpdatedTerm = null;
                //TODO advanced term evaluation
                rhsUpdatedTerm = rTerm.Eval();
                if (!rhsUpdatedTerm.Equals(Rhs))
                {
                    //Tranform all traces to the goal
                    object recurHead = rhsUpdatedTerm;
                    for (int i = rTerm.Traces.Count - 1; i >= 0; i--)
                    {
                        var ts = rTerm.Traces[i];
                        if (ts.Target.Equals(recurHead))
                        {
                            var oldGoal = new EqGoal(Lhs, ts.Source, true);
                            var newGoal = new EqGoal(Lhs, ts.Target, true);
                            var newStep = new TraceStep(oldGoal, newGoal, ts.Rule);
                            Traces.Insert(0, newStep);
                            recurHead = ts.Source;
                        }
                    }
                    Rhs = rhsUpdatedTerm;
                }

                #endregion
            }

            //Assert both side is the simplied version of term(expression)
            //Assert term is ordered with variable following by the constant.
            object result = Eval();
            if (!result.Equals(this))
            {
                var gGoal = result as EqGoal;
                Debug.Assert(gGoal != null);
                Functor = LogicSharp.Equal()(gGoal.Lhs, gGoal.Rhs);
            }
            else
            {
                Functor = LogicSharp.Equal()(Lhs, Rhs); 
            }
        }

        public EqGoal(object lhs, bool generated = false)
        {
            Lhs = lhs;
            if (generated) return;
            
            var lTerm = Lhs as Term;
            if (lTerm != null)
            {
                #region Term Trace Transformation

                object lhsUpdatedTerm = null;
                lhsUpdatedTerm = lTerm.Eval();
                if (!lhsUpdatedTerm.Equals(Lhs))
                {
                    //Tranform all traces to the goal
                    object recurHead = lhsUpdatedTerm;
                    for (int i = lTerm.Traces.Count - 1; i >= 0; i--)
                    {
                        var ts = lTerm.Traces[i];
                        if (ts.Target.Equals(recurHead))
                        {
                            var oldGoal = new EqGoal(ts.Source, true);
                            var newGoal = new EqGoal(ts.Target, true);
                            var newStep = new TraceStep(oldGoal, newGoal, ts.Rule);
                            Traces.Insert(0, newStep);
                            recurHead = ts.Source;
                        }
                    }
                    Lhs = lhsUpdatedTerm;
                }

                #endregion
            }
        }

        #endregion

        public virtual object Eval()
        {
            var lTerm = Lhs as Term;
            var rTerm = Rhs as Term;

            if(Var.ContainsVar(Rhs)) throw new Exception("TODO not supported now");
            Debug.Assert(LogicSharp.IsNumeric(Rhs));
            if (lTerm == null) return this;

            if (lTerm.Op.Method.Name.Equals("Add"))
            {
                var tuple = lTerm.Args as Tuple<object, object>;
                Debug.Assert(tuple!=null);
                if (LogicSharp.IsNumeric(tuple.Item2))
                {
                    //Rule 1: substract same term in both sides
                    var rhsTerm = new Term(Expression.Subtract, new Tuple<object, object>(Rhs, tuple.Item2));                
                    var newGoal = new EqGoal(tuple.Item1, rhsTerm, true);
                    string rule = EquationsRule.ApplyTransitiveProperty(Expression.Subtract, tuple.Item2);
                    var step = new TraceStep(this, newGoal, rule);
                  
                    //Rule 2: do the calculation again
                    object obj = rhsTerm.Eval();
                    Debug.Assert(rhsTerm.Traces.Count ==1);
                    var newGoal2 = new EqGoal(tuple.Item1, obj, true);
                    var exprTrace = rhsTerm.Traces[0] as TraceStep;
                    var step2 = new TraceStep(newGoal, newGoal2, exprTrace.Rule);

                    Traces.Insert(0,step);
                    Traces.Insert(0,step2);
                    return newGoal2;
                }
                else
                {
                    return this;
                }
            }
            else if (lTerm.Op.Method.Name.Equals("Substract"))
            {
                return this;
            }
            else if (lTerm.Op.Method.Name.Equals("Multiply"))
            {
                var tuple = lTerm.Args as Tuple<object, object>;
                Debug.Assert(tuple != null);
                if (LogicSharp.IsNumeric(tuple.Item1))
                {
                    //TODO
                }
                else
                {
                    return this;
                }
            }
            return this;
        }

        #region Goal Override Functions

        public override bool Reify(Dictionary<object, object> substitutions)
        {
            Lhs = LogicSharp.Reify(Lhs, substitutions);
            Rhs = LogicSharp.Reify(Rhs, substitutions);

            if (Var.ContainsVar(Lhs) || Var.ContainsVar(Rhs))
            {
                return true;
            }
            else
            {
                return Lhs.Equals(Rhs);
            }
        }

        public override bool Unify(Dictionary<object, object> substitutions)
        {
            return Functor(substitutions);
        }

        public override bool EarlySafe()
        {
            return !(Var.ContainsVar(Lhs) && Var.ContainsVar(Rhs));
        }

        public bool ContainsVar(Var variable)
        {
            var lhsVar = Lhs as Var;
            bool result;
            if (lhsVar != null)
            {
                result = lhsVar.Equals(variable);
                if (result) return true;
            }

            var rhsVar = Rhs as Var;
            if (rhsVar != null)
            {
                result = Rhs.Equals(variable);
                if (result) return true;
            }
            return false;
        }

        #endregion

        #region Object Override functions

        public override bool Equals(object obj)
        {
            var eqGoal = obj as EqGoal;
            if (eqGoal != null)
            {
                return Lhs.Equals(eqGoal.Lhs) && Rhs.Equals(eqGoal.Rhs);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return this.Lhs.GetHashCode() ^ this.Rhs.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0}={1}", Lhs, Rhs);
        }

        #endregion
    }

    public static class EqGoalExtension
    {
        public static Dictionary<object, object> ToDict(this EqGoal goal)
        {
            object variable;
            object value;
            if (Var.IsVar(goal.Lhs))
            {
                variable = goal.Lhs;
                value = goal.Rhs;
            }
            else
            {
                variable = goal.Rhs;
                value = goal.Lhs;
            }

            var pair = new KeyValuePair<object, object>(variable, value);
            var substitute = new Dictionary<object, object>();
            substitute.Add(pair.Key, pair.Value);
            return substitute;
        }

        /// <summary>
        /// Trace Derivation purpose
        /// </summary>
        /// <param name="goal"></param>
        /// <returns></returns>
        public static EqGoal GetLatestDerivedGoal(this EqGoal goal)
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
            return tempGoal;
        }
    }
}

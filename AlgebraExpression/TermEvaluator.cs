using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpLogic;

namespace AlgebraExpression
{
    public static class TermEvaluator
    {
        /// <summary>
        /// This func is the preprocessor of the Term.Eval()
        /// e.g (x+1)+2 -> x+(1+2)
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        public static object Rewrite(this Term term)
        {
            var Args = term.Args;
            var Op = term.Op;

            var tuple = Args as Tuple<object, object>;
            if (tuple != null)
            {
                #region Recursive Eval

                var item1Tuple = tuple.Item1 as Term;
                object arg1UpdatedTerm = null;
                bool item1Update = false;
                if (item1Tuple != null)
                {
                    arg1UpdatedTerm = item1Tuple.Eval();
                    if (!arg1UpdatedTerm.Equals(item1Tuple)) item1Update = true;
                }

                object arg2UpdatedTerm = null;
                var item2Tuple = tuple.Item2 as Term;
                bool item2Update = false;
                if (item2Tuple != null)
                {
                    arg2UpdatedTerm = item2Tuple.Eval();
                    if (!arg2UpdatedTerm.Equals(item2Tuple)) item2Update = true;
                }

                object item1 = null;
                if (item1Update)
                {
                    object recurHead = item1Tuple;
                    foreach (var ts in item1Tuple.Traces)
                    {
                        if (ts.Source.Equals(recurHead))
                        {
                            var oldTerm = new Term(Op,
                                new Tuple<object, object>(ts.Source, tuple.Item2));

                            var newTerm = new Term(Op,
                                new Tuple<object, object>(ts.Target, tuple.Item2));

                            var newStep = new TraceStep(oldTerm, newTerm, ts.Rule);
                            Traces.Add(newStep);
                            recurHead = ts.Target;
                        }
                    }
                    item1 = arg1UpdatedTerm;
                }
                else
                {
                    item1 = tuple.Item1;
                }

                object item2 = null;
                if (item2Update)
                {
                    object recurHead = item2Tuple;
                    foreach (var ts in item2Tuple.Traces)
                    {
                        if (ts.Source.Equals(recurHead))
                        {
                            var oldTerm = new Term(Op,
                                new Tuple<object, object>(tuple.Item1, ts.Source));

                            var newTerm = new Term(Op,
                                new Tuple<object, object>(tuple.Item1, ts.Target));

                            var newStep = new TraceStep(oldTerm, newTerm, ts.Rule);
                            Traces.Add(newStep);
                            recurHead = ts.Target;
                        }
                    }
                    item2 = arg2UpdatedTerm;
                }
                else
                {
                    item2 = tuple.Item2;
                }

                Args = new Tuple<object, object>(item1, item2);
                tuple = Args as Tuple<object, object>;

                #endregion

                #region Numeric Eval

                //if (!Var.ContainsVar(tuple.Item1) && !Var.ContainsVar(tuple.Item2))
                if (LogicSharp.IsNumeric(tuple.Item1) && LogicSharp.IsNumeric(tuple.Item2))
                {
                    var obj = LogicSharp.Calculate(Op, tuple.Item1, tuple.Item2);
                    string rule = ArithRule.CalcRule(Op.Method.Name, tuple.Item1, tuple.Item2, obj);
                    var newStep = new TraceStep(this, obj, rule);
                    Traces.Add(newStep);
                    return obj;
                }

                #endregion

                #region Variable Eval

                if (Var.IsVar(tuple.Item1) && Var.IsVar(tuple.Item2))
                {
                    var variable1 = tuple.Item1 as Var;
                    var variable2 = tuple.Item2 as Var;

                    if (variable1 == null || variable2 == null) return this;

                    if (variable1.ToString().Equals(variable2.ToString()))
                    {
                        //Identity Rule for the same variable
                        string appliedRule;
                        var newTerm = Rewrite.RewriteSameVariable(Op, variable1, out appliedRule);
                        var newStep = new TraceStep(this, newTerm, appliedRule);
                        Traces.Add(newStep);

                        var nTerm = newTerm as Term;
                        if (nTerm != null)
                        {
                            foreach (var step in Traces)
                            {
                                nTerm.Traces.Add(step);
                            }
                        }
                        return newTerm;
                    }
                    else
                    {
                        return this;
                    }
                }
                else
                {
/*
                    if (Var.IsVar(tuple.Item1))
                    {
                        var variable1 = tuple.Item1 as Var;
                        //Make commutative rule
                        string rule = RewriteRule.ApplyCommutativeOnTerm(tuple.Item1, tuple.Item2);
                        Args = new Tuple<object, object>(tuple.Item2, tuple.Item1);
                        return this;
                    }
 */
                }

                #endregion

                #region Term Eval

/*
                var term1 = tuple.Item1 as Term;
                var term2 = tuple.Item2 as Term;

                if (term1 != null && term2 != null)
                {
                    var args1 = term1.Args as Tuple<object, object>;
                    var args2 = term2.Args as Tuple<object, object>;
                    if(args1 == null || args2 == null) return this;
                    var variable1 = args1.Item2 as Var;
                    var variable2 = args2.Item2 as Var;
                    if (variable1 == null || variable2 == null) return this;

                    if (variable1.ToString().Equals(variable2.ToString())
                        && term1.Op.Equals(term2.Op))
                    {
                        #region Distributive Law

                        var param1 = args1.Item1;
                        var param2 = args2.Item1;
                        var newParamTerm = new Term(Op, new Tuple<object, object>(param1, param2));
                        Stack<TraceStep> internalSteps = null;
                        object internalTerm = null;
                        internalTerm = newParamTerm.UpdateTerm(out internalSteps);
                       
                        string appliedRule = RewriteRule.ApplyDistributiveLaw(term1, term2);
                        TraceStep newTraceStep;
                        if (internalTerm != null)
                        {
                            while (internalSteps.Count != 0)
                            {
                                Traces.Push(internalSteps.Pop());
                            }
                            newTraceStep = new TraceStep(this, internalTerm, appliedRule);
                            Traces.Push(newTraceStep);

                            Op = term1.Op;
                            Args = new Tuple<object, object>(internalTerm, variable1);
                        }
                        else
                        {
                            newTraceStep = new TraceStep(this, newParamTerm, appliedRule);
                            Traces.Push(newTraceStep);
                            Op = term1.Op;
                            Args = new Tuple<object, object>(newParamTerm, variable1);

                        }

                        #endregion
                    }
                }
*/

                #endregion
            }
            return this;
        }

/*      
        public static object Eval(this Term obj)
        {
            //Declarative:
        }

        public static bool Flattern(this Term term, out Term gTerm)
        {
            var args = new List<object>();
            var tuple = term.Args as Tuple<object, object>;
            if (tuple == null) throw new Exception("Cannot be null");

            var arg1Term = tuple.Item1 as Term;
            var arg2Term = tuple.Item2 as Term;

            Term arg1FlatTerm;
            if (arg1Term != null && arg1Term.Flattern(out arg1FlatTerm))
            {
                if (term.Op.Equals(arg1Term.Op))
                {
                    var arg1Lst = arg1FlatTerm.Args as List<object>;
                    if (arg1Lst != null) args.AddRange(arg1Lst);
                }
                else
                {
                    gTerm = null;
                    return false;
                }
            }
            else
            {
                args.Add(tuple.Item1);
            }

            Term arg2FlatTerm;
            if (arg2Term != null && arg2Term.Flattern(out arg2FlatTerm))
            {
                if (term.Op.Equals(arg2Term.Op))
                {
                    var arg2Lst = arg2FlatTerm.Args as List<object>;
                    if (arg2Lst != null) args.AddRange(arg2Lst);
                }
                else
                {
                    gTerm = null;
                    return false;
                }
            }
            else
            {
                args.Add(tuple.Item2);
            }

            gTerm = new Term(term.Op, args);
            return true;
        }    
    */
    }
}
﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using AlgebraGeometry;
using CSharpLogic;

namespace AlgebraExpression
{
    public static class LineTermEvaluator
    {
        private readonly static List<string> SpecialTerms = new List<string>()
        {
            "X", "x", "Y","y"
        };

        private static readonly Var xKey = new Var("a");
        private static readonly Var yKey = new Var('b');
        private static readonly Var cKey = new Var('c');

        private readonly static Term XTerm = new Term(Expression.Multiply,
            new Tuple<object, object>(xKey, new Var('X')));
        private readonly static Term xTerm = new Term(Expression.Multiply,
            new Tuple<object, object>(xKey, new Var('x')));
        private readonly static Term YTerm = new Term(Expression.Multiply,
           new Tuple<object, object>(yKey, new Var('Y')));
        private readonly static Term yTerm = new Term(Expression.Multiply,
           new Tuple<object, object>(yKey, new Var('y')));
        
        /// <summary>
        /// recognize x-term, such as ax, transform it.
        /// </summary>
        /// <param name="obj"></param>
        public static object RectifyLineTerm(object obj)
        {
            return TermEvaluator.RecognizeSpecialTerm(obj, SpecialTerms);
        }

        public static Line UnifyLineTerm(this Term term)
        {
            Debug.Assert(term.Op.Method.Name.Equals("Add"));
            var argLst = term.Args as List<object>;
            if (argLst == null) return null;

            if (argLst.Count > 3)
            {
                return null;
            }
            var dict = new Dictionary<Var, object>();
            Line line;
            return UnifyLineTerm(argLst, 0, dict);
        }

        private static Line UnifyLineTerm(List<object> args, int index,
                                         Dictionary<Var, object> dict)
        {

            if (index == args.Count)
            {
                object xCord = dict.ContainsKey(xKey) ? dict[xKey] : null;
                object yCord = dict.ContainsKey(yKey) ? dict[yKey] : null;
                object cCord = dict.ContainsKey(cKey) ? dict[cKey] : null;
                return new Line(xCord, yCord, cCord);
            }

            object currArg = args[index];
            bool finalResult = false;

            var dictTemp = new Dictionary<object, object>();
            bool result = LogicSharp.Unify(currArg, XTerm, dictTemp);
            if (result)
            {
                Debug.Assert(dictTemp.ContainsKey(xKey));
                if (dict.ContainsKey(xKey)) 
                    throw new Exception("cannot contain two terms with same var");
                dict.Add(xKey,dictTemp[xKey]);
                finalResult = true;
            }

            result = LogicSharp.Unify(currArg, xTerm, dictTemp);
            if (result)
            {
                Debug.Assert(dictTemp.ContainsKey(xKey));
                if (dict.ContainsKey(xKey))
                    throw new Exception("cannot contain two terms with same var");
                dict.Add(xKey, dictTemp[xKey]);
                finalResult = true;
            }

            result = LogicSharp.Unify(currArg, YTerm, dictTemp);
            if (result)
            {
                Debug.Assert(dictTemp.ContainsKey(yKey));
                if (dict.ContainsKey(yKey))
                    throw new Exception("cannot contain two terms with same var");
                dict.Add(yKey, dictTemp[yKey]);
                finalResult = true;
            }

            result = LogicSharp.Unify(currArg, yTerm, dictTemp);
            if (result)
            {
                Debug.Assert(dictTemp.ContainsKey(yKey));
                if (dict.ContainsKey(yKey))
                    throw new Exception("cannot contain two terms with same var");
                dict.Add(yKey, dictTemp[yKey]);
                finalResult = true;
            }

            double d;
            result = LogicSharp.IsDouble(currArg, out d);
            if (result)
            {        
                if (dict.ContainsKey(cKey))
                    throw new Exception("cannot contain two terms with same var");
                dict.Add(cKey, d);
                finalResult = true;
            }

            if (currArg is string)
            {
                if (dict.ContainsKey(cKey))
                    throw new Exception("cannot contain two terms with same var");
                dict.Add(cKey, new Var(currArg));
                finalResult = true;                
            }

            if (finalResult)
            {
                return UnifyLineTerm(args, index + 1, dict);
            }
            else
            {
                return null;
            }
        }        
    }

    public static class TermEvaluator
    {
        public static object RecognizeSpecialTerm(object obj, List<string> names)
        {
            if (LogicSharp.IsNumeric(obj)) return obj;

            var variable = obj as Var;
            if (variable != null)
            {
                var token = variable.ToString();
                Debug.Assert(token != null);
                if (token.Length == 1)
                {
                    foreach (string name in names)
                    {
                        if (token.Equals(name))
                        {
                            return new Term(Expression.Multiply,
                                new Tuple<object, object>(1, new Var(token)));
                        }
                    }
                    return token;
                }
                var coeffVar = token.Substring(0, token.Length - 1);
                var specicalTerm = token.Substring(token.Length - 1, 1);
                foreach (string name in names)
                {
                    if (specicalTerm.Equals(name))
                    {
                        Term tt;
                        if (LogicSharp.IsNumeric(coeffVar))
                        {
                            double d;
                            LogicSharp.IsDouble(coeffVar, out d);

                            tt = new Term(Expression.Multiply,
                                            new Tuple<object, object>(d, new Var(specicalTerm)));
                        }
                        else
                        {
                            tt = new Term(Expression.Multiply,
                                            new Tuple<object, object>(coeffVar, new Var(specicalTerm)));
                        }
                        return tt;
                    }
                }

                return obj;
            }
            var term = obj as Term;
            if (term != null)
            {
                var tuple = term.Args as Tuple<object, object>;
                Debug.Assert(tuple != null);
                object obj1, obj2;
                if (term.Op.Method.Name.Equals("Add"))
                {
                    obj1 = RecognizeSpecialTerm(tuple.Item1, names);
                    obj2 = RecognizeSpecialTerm(tuple.Item2, names);                    
                }
                else
                {
                    obj1 = tuple.Item1;
                    obj2 = tuple.Item2;
                }

                if (!obj1.Equals(tuple.Item1) || !obj2.Equals(tuple.Item2))
                {
                    return new Term(term.Op, new Tuple<object, object>(obj1, obj2));
                }
                else
                {
                    return obj;
                }
            }
            return obj;
        }

        public static Term Flattern(this Term term,
                        Func<Expression, Expression, BinaryExpression> parentOp = null)
        {
            var args = new List<object>();
            var tuple = term.Args as Tuple<object, object>;
            if (tuple == null) throw new Exception("Cannot be null");

            if (parentOp == null) parentOp = term.Op;

            var arg1Term = tuple.Item1 as Term;
            var arg2Term = tuple.Item2 as Term;

            #region Arg1 flattern
            if (arg1Term != null)
            {
                if (parentOp.Method.Name.Equals(arg1Term.Op.Method.Name))
                {
                    Term gTempTerm = arg1Term.Flattern(parentOp);
                    var arg1Lst = gTempTerm.Args as List<object>;
                    if (arg1Lst != null) args.AddRange(arg1Lst);
                }
                else
                {
                    args.Add(arg1Term);
                }
            }
            else
            {
                args.Add(tuple.Item1);
            }
            #endregion

            #region Arg2 flattern

            if (arg2Term != null)
            {
                if (parentOp.Method.Name.Equals(arg2Term.Op.Method.Name))
                {
                    Term gTempTerm = arg2Term.Flattern(parentOp);
                    var arg2Lst = gTempTerm.Args as List<object>;
                    if (arg2Lst != null) args.AddRange(arg2Lst);
                }
                else
                {
                    args.Add(arg2Term);
                }
            }
            else
            {
                args.Add(tuple.Item2);
            }

            #endregion

            if (arg1Term != null || arg2Term != null)
            {
                return new Term(term.Op, args);
            }
            else
            {
                return term;
            }
        }

        public static Term Treeify(this Term term)
        {
            var args = term.Args as List<object>;
            if (args == null || args.Count == 1) throw new Exception("It should be list instead of Tuple");

            if (args.Count == 2)
            {
                return new Term(term.Op, new Tuple<object, object>(args[0], args[1]));
            }
            else
            {
                var innerTerm = new Term(term.Op, new Tuple<object, object>(args[0], args[1]));

                for (int i = 2; i < args.Count; i++)
                {
                    innerTerm = new Term(term.Op, new Tuple<object, object>(innerTerm, args[i]));
                }
                return innerTerm;
            }
        }

        #region TODO

        /// <summary>
        /// This func is the preprocessor of the Term.Eval()
        /// e.g (x+1)+2 -> x+(1+2)
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>

        //        public static object Rewrite(this Term term)
        //        {
        //            var Args = term.Args;
        //            var Op = term.Op;

        //            var tuple = Args as Tuple<object, object>;
        //            if (tuple != null)
        //            {
        //                #region Recursive Eval

        //                var item1Tuple = tuple.Item1 as Term;
        //                object arg1UpdatedTerm = null;
        //                bool item1Update = false;
        //                if (item1Tuple != null)
        //                {
        //                    arg1UpdatedTerm = item1Tuple.Eval();
        //                    if (!arg1UpdatedTerm.Equals(item1Tuple)) item1Update = true;
        //                }

        //                object arg2UpdatedTerm = null;
        //                var item2Tuple = tuple.Item2 as Term;
        //                bool item2Update = false;
        //                if (item2Tuple != null)
        //                {
        //                    arg2UpdatedTerm = item2Tuple.Eval();
        //                    if (!arg2UpdatedTerm.Equals(item2Tuple)) item2Update = true;
        //                }

        //                object item1 = null;
        //                if (item1Update)
        //                {
        //                    object recurHead = item1Tuple;
        //                    foreach (var ts in item1Tuple.Traces)
        //                    {
        //                        if (ts.Source.Equals(recurHead))
        //                        {
        //                            var oldTerm = new Term(Op,
        //                                new Tuple<object, object>(ts.Source, tuple.Item2));

        //                            var newTerm = new Term(Op,
        //                                new Tuple<object, object>(ts.Target, tuple.Item2));

        //                            var newStep = new TraceStep(oldTerm, newTerm, ts.Rule);
        //                            Traces.Add(newStep);
        //                            recurHead = ts.Target;
        //                        }
        //                    }
        //                    item1 = arg1UpdatedTerm;
        //                }
        //                else
        //                {
        //                    item1 = tuple.Item1;
        //                }

        //                object item2 = null;
        //                if (item2Update)
        //                {
        //                    object recurHead = item2Tuple;
        //                    foreach (var ts in item2Tuple.Traces)
        //                    {
        //                        if (ts.Source.Equals(recurHead))
        //                        {
        //                            var oldTerm = new Term(Op,
        //                                new Tuple<object, object>(tuple.Item1, ts.Source));

        //                            var newTerm = new Term(Op,
        //                                new Tuple<object, object>(tuple.Item1, ts.Target));

        //                            var newStep = new TraceStep(oldTerm, newTerm, ts.Rule);
        //                            Traces.Add(newStep);
        //                            recurHead = ts.Target;
        //                        }
        //                    }
        //                    item2 = arg2UpdatedTerm;
        //                }
        //                else
        //                {
        //                    item2 = tuple.Item2;
        //                }

        //                Args = new Tuple<object, object>(item1, item2);
        //                tuple = Args as Tuple<object, object>;

        //                #endregion

        //                #region Numeric Eval

        //                //if (!Var.ContainsVar(tuple.Item1) && !Var.ContainsVar(tuple.Item2))
        //                if (LogicSharp.IsNumeric(tuple.Item1) && LogicSharp.IsNumeric(tuple.Item2))
        //                {
        //                    var obj = LogicSharp.Calculate(Op, tuple.Item1, tuple.Item2);
        //                    string rule = ArithRule.CalcRule(Op.Method.Name, tuple.Item1, tuple.Item2, obj);
        //                    var newStep = new TraceStep(this, obj, rule);
        //                    Traces.Add(newStep);
        //                    return obj;
        //                }

        //                #endregion

        //                #region Variable Eval

        //                if (Var.IsVar(tuple.Item1) && Var.IsVar(tuple.Item2))
        //                {
        //                    var variable1 = tuple.Item1 as Var;
        //                    var variable2 = tuple.Item2 as Var;

        //                    if (variable1 == null || variable2 == null) return this;

        //                    if (variable1.ToString().Equals(variable2.ToString()))
        //                    {
        //                        //Identity Rule for the same variable
        //                        string appliedRule;
        //                        var newTerm = Rewrite.RewriteSameVariable(Op, variable1, out appliedRule);
        //                        var newStep = new TraceStep(this, newTerm, appliedRule);
        //                        Traces.Add(newStep);

        //                        var nTerm = newTerm as Term;
        //                        if (nTerm != null)
        //                        {
        //                            foreach (var step in Traces)
        //                            {
        //                                nTerm.Traces.Add(step);
        //                            }
        //                        }
        //                        return newTerm;
        //                    }
        //                    else
        //                    {
        //                        return this;
        //                    }
        //                }
        //                else
        //                {
        ///*
        //                    if (Var.IsVar(tuple.Item1))
        //                    {
        //                        var variable1 = tuple.Item1 as Var;
        //                        //Make commutative rule
        //                        string rule = RewriteRule.ApplyCommutativeOnTerm(tuple.Item1, tuple.Item2);
        //                        Args = new Tuple<object, object>(tuple.Item2, tuple.Item1);
        //                        return this;
        //                    }
        // */
        //                }

        //                #endregion

        //                #region Term Eval

        ///*
        //                var term1 = tuple.Item1 as Term;
        //                var term2 = tuple.Item2 as Term;

        //                if (term1 != null && term2 != null)
        //                {
        //                    var args1 = term1.Args as Tuple<object, object>;
        //                    var args2 = term2.Args as Tuple<object, object>;
        //                    if(args1 == null || args2 == null) return this;
        //                    var variable1 = args1.Item2 as Var;
        //                    var variable2 = args2.Item2 as Var;
        //                    if (variable1 == null || variable2 == null) return this;

        //                    if (variable1.ToString().Equals(variable2.ToString())
        //                        && term1.Op.Equals(term2.Op))
        //                    {
        //                        #region Distributive Law

        //                        var param1 = args1.Item1;
        //                        var param2 = args2.Item1;
        //                        var newParamTerm = new Term(Op, new Tuple<object, object>(param1, param2));
        //                        Stack<TraceStep> internalSteps = null;
        //                        object internalTerm = null;
        //                        internalTerm = newParamTerm.UpdateTerm(out internalSteps);

        //                        string appliedRule = RewriteRule.ApplyDistributiveLaw(term1, term2);
        //                        TraceStep newTraceStep;
        //                        if (internalTerm != null)
        //                        {
        //                            while (internalSteps.Count != 0)
        //                            {
        //                                Traces.Push(internalSteps.Pop());
        //                            }
        //                            newTraceStep = new TraceStep(this, internalTerm, appliedRule);
        //                            Traces.Push(newTraceStep);

        //                            Op = term1.Op;
        //                            Args = new Tuple<object, object>(internalTerm, variable1);
        //                        }
        //                        else
        //                        {
        //                            newTraceStep = new TraceStep(this, newParamTerm, appliedRule);
        //                            Traces.Push(newTraceStep);
        //                            Op = term1.Op;
        //                            Args = new Tuple<object, object>(newParamTerm, variable1);

        //                        }

        //                        #endregion
        //                    }
        //                }
        //*/

        //                #endregion
        //            }
        //            return this;
        //        }

        /*      
                public static object Eval(this Term obj)
                {
                    //Declarative:
                }

                
            */

        #endregion
    }
}
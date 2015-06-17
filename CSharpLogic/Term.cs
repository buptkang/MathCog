using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CSharpLogic
{
    /// <summary>
    /// Substitution or term
    /// </summary>
    public class Term : DyLogicObject
    {
        public Func<Expression, Expression, BinaryExpression> Op { get; set; }
        public object Args { get; set; }

        public Term(Func<Expression, Expression, BinaryExpression> _op, object _args)
        {
            Op = _op;
            Args = _args;
        }

        public override bool Equals(object obj)
        {
            if (obj is Term)
            {
                var term = obj as Term;
                return Op.Equals(term.Op) && Args.Equals(term.Args);
            }
            return false;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            var tuple = Args as Tuple<object, object>;
            if(tuple == null) throw new Exception("cannot be null");

            builder.Append('(');

            var lTerm = tuple.Item1;
            var rTerm = tuple.Item2;
          
            builder.Append(lTerm.ToString());
            
            if (Op.Method.Name.Equals("Add"))
            {
                builder.Append('+');
            }
            else if (Op.Method.Name.Equals("Substract"))
            {
                builder.Append('-');
            }
            else if (Op.Method.Name.Equals("Multiply"))
            {
                builder.Append('*');
            }
            else if (Op.Method.Name.Equals("Divide"))
            {
                builder.Append('/');
            }
                
            builder.Append(rTerm.ToString());
            builder.Append(')');
            return builder.ToString();
        }

        /// <summary>
        /// Evaluation with calculation functionality, it does not 
        /// take care of term rewriting.
        /// 
        /// Terms should follow the below rules:
        /// tp: "x" , "2", "2+2",  "2+3-1", "2+2*2", "x+(1+2)", "x + y + 3"
        /// tp: "x^2+x+2+1" 
        /// fn: "(x+1)+2" or "1+x+2", "x+x"
        /// </summary>
        /// <returns></returns>
        public object Eval()
        {
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
                    object recurHead = arg1UpdatedTerm;
                    for (int i = item1Tuple.Traces.Count - 1; i >= 0; i--)
                    {
                        var ts = item1Tuple.Traces[i];
                        if (ts.Target.Equals(recurHead))
                        {
                            var oldTerm = new Term(Op,
                               new Tuple<object, object>(ts.Source, tuple.Item2));
                            var newTerm = new Term(Op,
                               new Tuple<object, object>(ts.Target, tuple.Item2));
                            var newStep = new TraceStep(oldTerm, newTerm, ts.Rule);
                            Traces.Insert(0, newStep);
                            recurHead = ts.Source;
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
                    object recurHead = arg2UpdatedTerm;
                    for (int i = item2Tuple.Traces.Count - 1; i >= 0; i--)
                    {
                        var ts = item2Tuple.Traces[i];
                        if (ts.Target.Equals(recurHead))
                        {
                            var oldTerm = new Term(Op,
                               new Tuple<object, object>(tuple.Item1,ts.Source));
                            var newTerm = new Term(Op,
                               new Tuple<object, object>(tuple.Item1,ts.Target));
                            var newStep = new TraceStep(oldTerm, newTerm, ts.Rule);
                            Traces.Insert(0, newStep);
                            recurHead = ts.Source;
                        }
                    }
                    item2 = arg2UpdatedTerm;
                }
                else
                {
                    item2 = tuple.Item2;
                }

                //Args = new Tuple<object, object>(item1, item2);
                //tuple = Args as Tuple<object, object>;
                tuple = new Tuple<object, object>(item1, item2); 

                #endregion

                if (LogicSharp.IsNumeric(tuple.Item1) && LogicSharp.IsNumeric(tuple.Item2))
                {
                    var obj = LogicSharp.Calculate(Op, tuple.Item1, tuple.Item2);
                    string rule = ArithRule.CalcRule(Op.Method.Name, tuple.Item1, tuple.Item2, obj);
                    var newStep = new TraceStep(this, obj, rule);
                    Traces.Add(newStep);
                    return obj;
                }
                else
                {                   
                    return new Term(Op, tuple);
                }
            }
            return this;
        }

        public bool ContainsVar(Var variable)
        {
            var tuple1 = Args as Tuple<object>;
            if (tuple1 != null)
            {
                var term1 = tuple1.Item1 as Term;
                if (term1 != null) return term1.ContainsVar(variable);
                var variable1 = tuple1.Item1 as Var;
                if (variable1 != null) return variable1.Equals(variable);
                return false; //constant
            }

            var tuple2 = Args as Tuple<object, object>;
            if (tuple2 != null)
            {
                bool result;
                var term1 = tuple2.Item1 as Term;
                if (term1 != null)
                {
                    result = term1.ContainsVar(variable);
                    if (result) return true;
                }
                var variable1 = tuple2.Item1 as Var;
                if (variable1 != null)
                {
                    result = variable1.Equals(variable);
                    if (result) return true;
                }

                var term2 = tuple2.Item2 as Term;
                if (term2 != null)
                {
                    result = term2.ContainsVar(variable);
                    if (result) return true;
                }
                var variable2 = tuple2.Item2 as Var;
                if (variable2 != null)
                {
                    result = variable.Equals(variable2);
                    if (result) return true;
                }
                return false;
            }

            throw new Exception("Add tuple3, tuple4...");            
        }
    }

    public static class TermExtention
    {
        public static Term Reify(this Term term, Dictionary<object, object> s)
        {
            var gArgs = LogicSharp.Reify(term.Args, s);
            return new Term(term.Op, gArgs);
        }   
    }
}

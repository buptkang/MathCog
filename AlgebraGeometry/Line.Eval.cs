using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using CSharpLogic;

namespace AlgebraGeometry
{
    public static class LineEquationExtension
    {
        public static bool IsLineEquation(this Equation eq, out LineSymbol ls)
        {
            Debug.Assert(eq != null);
            Debug.Assert(eq.Rhs != null);
            ls = null;

            Equation outputEq;
            bool? result = eq.Eval(out outputEq, false);
            if (result != null) return false;

            //Equation Semantic Unification
            //general     form of line equation ax+by+c=0
            //point-slope form of line equation y = mx + b

            Line line;
            bool matched = SatisfyLineGeneralForm(outputEq, out line);
            if (matched)
            {
                line.Traces = eq.CloneTrace();
                ls = new LineSymbol(line);
                return true;
            }
            matched = SatisfyLineSlopeInterceptForm(outputEq, out line);
            if (matched)
            {
                line.Traces = eq.CloneTrace();
                ls = new LineSymbol(line);
                return true;
            }

            result = eq.Eval(out outputEq);
            if (result != null) return false;
            matched = SatisfyLineGeneralForm(outputEq, out line);
            if (matched)
            {
                //line.Traces = eq.Traces;
                ls = new LineSymbol(line);
                return true;
            }
            else
            {
                //TODO
                eq.Traces.Clear();
                return false;
            }
        }

        private static bool SatisfyLineGeneralForm(Equation equation, out Line line)
        {
            line = null;
            var lhsTerm = equation.Lhs as Term;
            if (lhsTerm == null || !equation.Rhs.Equals(0)) return false;

            line = lhsTerm.UnifyLineTerm();
            return line != null;
        }

        private static bool SatisfyLineSlopeInterceptForm(Equation equation, out Line line)
        {
            line = null;
            var lhsVar = equation.Lhs as Var;
            if (lhsVar != null && (lhsVar.Equals(yTerm) || lhsVar.Equals(YTerm)))
            {
                var rhsTerm = equation.Rhs as Term;
                if (rhsTerm == null) return false;

                line = rhsTerm.UnifyLineSlopeInterceptTerm();
                return line != null;                
            }
            return false;
        }

        #region Slope-Intercept Form

        private static Line UnifyLineSlopeInterceptTerm(this Term term)
        {
            if (term.Op.Method.Name.Equals("Add"))
            {
                var argLst = term.Args as List<object>;
                if (argLst == null) return null;
                if (argLst.Count > 2)
                {
                    return null;
                }
                var dict = new Dictionary<string, object>();
                return UnifyLineSlopeInterceptTerm(argLst, 0, dict);
            }
            else
            {
                var lst = new List<object>() { term };
                var dict = new Dictionary<string, object>();
                return UnifyLineSlopeInterceptTerm(lst, 0, dict);
            }
        }

        private static Line UnifyLineSlopeInterceptTerm(List<object> args, int index,
                                        Dictionary<string, object> dict)
        {

            if (index == args.Count)
            {
                object slope     = dict.ContainsKey(A) ? dict[A] : null;
                object intercept = dict.ContainsKey(C) ? dict[C] : null;
                return new Line(slope, intercept);
            }

            object currArg = args[index];
            bool finalResult = false;

            object coeff;
            bool result = IsXTerm(currArg, out coeff);
            if (result)
            {
                if (dict.ContainsKey(A))
                    throw new Exception("cannot contain two terms with same var");
                dict.Add(A, coeff);
                finalResult = true;
            }
            double d;
            result = LogicSharp.IsDouble(currArg, out d);
            if (result)
            {
                if (dict.ContainsKey(C))
                    throw new Exception("cannot contain two terms with same var");
                dict.Add(C, d);
                finalResult = true;
            }

            if (currArg is string)
            {
                if (dict.ContainsKey(C))
                    throw new Exception("cannot contain two terms with same var");
                dict.Add(C, new Var(currArg));
                finalResult = true;
            }

            if (finalResult)
            {
                return UnifyLineSlopeInterceptTerm(args, index + 1, dict);
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region General Form

        private readonly static List<string> SpecialLabels = new List<string>()
        {
            "X", "x", "Y","y"
        };

        private static readonly List<Var> SpecialVars = new List<Var>()
        {
            new Var('x'),
            new Var('X'),
            new Var('y'),
            new Var('Y')
        };

        public static Regex xRegex = new Regex(@"x$");
        public static Regex XRegex = new Regex(@"X$");
        public static Regex yRegex = new Regex(@"y$");
        public static Regex YRegex = new Regex(@"Y$");

        private static readonly string A = "a";
        private static readonly string B = "b";
        private static readonly string C = "c";

        private readonly static Var XTerm = new Var('X');
        private readonly static Var xTerm = new Var('x');
        private readonly static Var YTerm = new Var('Y');
        private static readonly Var yTerm = new Var('y');

        private static Line UnifyLineTerm(this Term term)
        {
            if (term.Op.Method.Name.Equals("Add"))
            {
                var argLst = term.Args as List<object>;
                if (argLst == null) return null;
                if (argLst.Count > 3)
                {
                    return null;
                }
                var dict = new Dictionary<string, object>();
                return UnifyLineTerm(argLst, 0, dict);                
            }
            else
            {
                var lst = new List<object>() {term};
                var dict = new Dictionary<string, object>();
                return UnifyLineTerm(lst, 0, dict);   
            }
        }

        private static Line UnifyLineTerm(List<object> args, int index,
                                        Dictionary<string, object> dict)
        {

            if (index == args.Count)
            {
                object xCord = dict.ContainsKey(A) ? dict[A] : null;
                object yCord = dict.ContainsKey(B) ? dict[B] : null;
                object cCord = dict.ContainsKey(C) ? dict[C] : null;
                return new Line(xCord, yCord, cCord);
            }

            object currArg = args[index];
            bool finalResult = false;

            object coeff;
            bool result = IsXTerm(currArg, out coeff);
            if (result)
            {
                if (dict.ContainsKey(A))
                    throw new Exception("cannot contain two terms with same var");
                dict.Add(A, coeff);
                finalResult = true; 
            }
            result = IsYTerm(currArg, out coeff);
            if (result)
            {
                if (dict.ContainsKey(B))
                    throw new Exception("cannot contain two terms with same var");
                dict.Add(B, coeff);
                finalResult = true; 
            }
            double d;
            result = LogicSharp.IsDouble(currArg, out d);
            if (result)
            {
                if (dict.ContainsKey(C))
                    throw new Exception("cannot contain two terms with same var");
                dict.Add(C, d);
                finalResult = true;
            }

            if (currArg is string)
            {
                if (dict.ContainsKey(C))
                    throw new Exception("cannot contain two terms with same var");
                dict.Add(C, new Var(currArg));
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

        private static bool IsXTerm(object obj, out object coeff)
        {
            coeff = null;
            var variable = obj as Var;
            if (variable != null)
            {
                if (variable.Equals(XTerm) || variable.Equals(xTerm))
                {
                    coeff = 1;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            var term = obj as Term;
            if (term != null && term.Op.Method.Name.Equals("Multiply"))
            {
                var lst = term.Args as List<object>;
                Debug.Assert(lst != null);
                var lastObj = lst[lst.Count - 1];
                if (lastObj.Equals(xTerm) || lastObj.Equals(XTerm))
                {
                    if (lst.Count == 2)
                    {
                        coeff = lst[0];
                        return true;
                    }
                    else
                    {
                        coeff = new Term(Expression.Multiply, lst.GetRange(0, lst.Count - 1));
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        private static bool IsYTerm(object obj, out object coeff)
        {
            coeff = null;
            var variable = obj as Var;
            if (variable != null)
            {
                if (variable.Equals(YTerm) || variable.Equals(yTerm))
                {
                    coeff = 1;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            var term = obj as Term;
            if (term != null && term.Op.Method.Name.Equals("Multiply"))
            {
                var lst = term.Args as List<object>;
                Debug.Assert(lst != null);
                var lastObj = lst[lst.Count - 1];
                if (lastObj.Equals(yTerm) || lastObj.Equals(YTerm))
                {
                    if (lst.Count == 2)
                    {
                        coeff = lst[0];
                        return true;
                    }
                    else
                    {
                        coeff = new Term(Expression.Multiply, lst.GetRange(0, lst.Count - 1));
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
               
        #endregion
    }
}

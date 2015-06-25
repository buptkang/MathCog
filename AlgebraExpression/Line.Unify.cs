using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AlgebraGeometry;
using CSharpLogic;

namespace AlgebraExpression
{
    public static class LineEvaluator
    {
        public static bool Unify(object lhs, object rhs, out Line line)
        {
            line = null;
            //Rectify such as ax,2x
            lhs = LineTermEvaluator.RectifyLineTerm(lhs);
            rhs = LineTermEvaluator.RectifyLineTerm(rhs);

            if (lhs is string) lhs = new Var(lhs);
            if (rhs is string) rhs = new Var(rhs);

            var lTerm     = lhs as Term;
            var lVar      = lhs as Var;

            double lNumber;
            bool lNumeric = LogicSharp.IsDouble(lhs, out lNumber);

            var rTerm     = rhs as Term;
            var rVar      = rhs as Var;
            double rNumber;
            bool rNumeric = LogicSharp.IsDouble(rhs, out rNumber);

            if (lTerm != null && rTerm != null)
            {
                return Unify(lTerm, rTerm, out line);
            }

            if (lTerm != null && rVar != null)
            {
                return Unify(lTerm, rVar, out line);
            }

            if (lTerm != null && rNumeric)
            {
                return Unify(lTerm, rNumber, out line);
            }

            if (lVar != null && rTerm != null)
            {
                return Unify(lVar, rTerm, out line);
            }

            if (lVar != null && rVar != null)
            {
                return Unify(lVar, rVar, out line);
            }

            if (lVar != null && rNumeric)
            {
                return Unify(lVar, rNumber, out line);
            }

            if (lNumeric && rTerm != null)
            {
                return Unify(lNumber, rTerm, out line);
            }

            if (lNumeric && rVar != null)
            {
                return Unify(lNumber, rVar, out line);
            }

            if (lNumeric && rNumeric)
            {
                return Unify(lNumber, rNumber, out line);
            }

            throw new Exception("Cannot reach here");
        }

        /// <summary>
        /// lhs must be constant variable after rectify
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        private static bool Unify(Var lhs, double rhs, out Line line)
        {
            line = null;
            return false;
        }

        /// <summary>
        /// a = 2x
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        private static bool Unify(Var lhs, Term rhs, out Line line)
        {
            //TODO
            line = null;
            return false;
        }

        private static bool Unify(Var lhs, Var rhs, out Line line)
        {
            line = null;
            return false;            
        }

        private static bool Unify(double lhs, double rhs, out Line line)
        {
            line = null;
            return false;
        }

        private static bool Unify(double lhs, Var rhs, out Line line)
        {
            //TODO
            line = null;
            return false;
        }

        private static bool Unify(double lhs, Term rhs, out Line line)
        {
            //TODO
            line = null;
            return false;
        }

        /// <summary>
        /// x=1, 2x=1,2x+3y=1
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        private static bool Unify(Term lhs, double rhs, out Line line)
        {
            line = null;
            if (Math.Abs(rhs) > 0.00001)
            {
                //move the numeric from right to left
                lhs = new Term(Expression.Add, new Tuple<object,object>(lhs, -1*rhs));
            }
            //lhs.Eval();
            Term lhsFlatternTerm = lhs.Flattern(Expression.Add);
            line = lhsFlatternTerm.UnifyLineTerm();
            return line != null;
        }

        /// <summary>
        /// x=a
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        private static bool Unify(Term lhs, Var rhs, out Line line)
        {
            //TODO
            line = null;
            return false;
        }

        /// <summary>
        /// 2x = y
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        private static bool Unify(Term lhs, Term rhs, out Line line)
        {
            //TODO
            line = null;
            return false;
        }
    }

    public static class LineTermEvaluator
    {
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
            return TermEvaluator.RecognizeSpecialTerm(obj, SpecialLabels);
        }

        public static Line UnifyLineTerm(this Term term)
        {
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
                dict.Add(xKey, dictTemp[xKey]);
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
}

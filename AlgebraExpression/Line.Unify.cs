using System;
using System.Collections.Generic;
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
            line = null;
            throw new Exception("TODO");            
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
            line = null;
            throw new Exception("TODO");
        }

        private static bool Unify(double lhs, Term rhs, out Line line)
        {
            line = null;
            throw new Exception("TODO");            
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
            line = null;
            throw new Exception("TODO");       
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
            line = null;
            throw new Exception("TODO"); 
        }
    }
}

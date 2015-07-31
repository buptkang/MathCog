using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace CSharpLogic
{
    public interface IArithmeticLogic : IEval
    {
        /// <summary>
        //with calculation functionality, it does not 
        /// take care of term rewriting.
        /// 
        /// Terms should follow the below rules:
        /// tp: "2+2",  "2+3-1", "2+2*2"
        /// </summary>
        /// <returns> can be term or value</returns>
        object EvalArithmetic();
    }

    public static class ArithmeticEvalExtension
    {
        private static List<object> FindArithValues(this Term term)
        {
            var lst = term.Args as List<object>;
            Debug.Assert(lst != null);

            if (term.Op.Method.Name.Equals("Add"))
            {
                #region Add Strategy
                int arithStartIndex = -1;
                for (var i = 0; i < lst.Count; i++)
                {
                    if (LogicSharp.IsNumeric(lst[i]))
                    {
                        arithStartIndex = i;
                        break;
                    }

                    var termObj = lst[i] as Term;
                    if (termObj != null && !termObj.ContainsVar())
                    {
                        arithStartIndex = i;
                        break;
                    }
                }
                //no arithmetic value
                if (arithStartIndex == -1) return null;
                return lst.GetRange(arithStartIndex, lst.Count - arithStartIndex);
                #endregion
            }
            else if (term.Op.Method.Name.Equals("Multiply"))
            {
                //e.g 3*3*x
                #region Multiply Strategy
                int arithStartIndex = -1;
                for (var i = 0; i < lst.Count; i++)
                {
                    if (!LogicSharp.IsNumeric(lst[i]))
                    {
                        arithStartIndex = i;
                        break;
                    }

                    var termObj = lst[i] as Term;
                    if (termObj != null && termObj.ContainsVar())
                    {
                        arithStartIndex = i;
                        break;
                    }
                }
                //no arithmetic value
                if (arithStartIndex == -1) return lst;
                return lst.GetRange(0, arithStartIndex);
                #endregion
            }
            else if (term.Op.Method.Name.Equals("Divide"))
            {
                return lst;
            }

           throw new Exception("Arithmetic.cs: Cannot reach here");
        }

        public static object Arithmetic(this object obj, Term rootTerm)
        {
            var term = obj as Term;
            if (term == null) return obj;

            Term localTerm = term.DepthFirstSearch(rootTerm);

            #region Arithmetic

            List<object> objs = localTerm.FindArithValues();
            if (objs == null || objs.Count == 0 || objs.Count == 1) return localTerm;

            if (LogicSharp.IsNumeric(objs[0]) && LogicSharp.IsNumeric(objs[1]))
            {
                var objCalc = Calculate(term.Op, objs[0], objs[1]);
                string rule = ArithRule.CalcRule(term.Op.Method.Name, objs[0], objs[1], objCalc);
                rootTerm.GenerateTrace(objs[0], objs[1], objCalc, rule);
                return localTerm.Substitute(objs[0], objs[1], objCalc);
            }
            #endregion
            return localTerm;
        }

        public static object Calculate(Func<Expression, Expression, BinaryExpression> func,
            object x, object y)
        {
            double xDoubleVal;
            double yDoubleVal;
            bool isXDouble = LogicSharp.IsDouble(x, out xDoubleVal);
            bool isYDouble = LogicSharp.IsDouble(y, out yDoubleVal);

            if (isXDouble || isYDouble)
            {
                var xExpr = Expression.Constant(xDoubleVal);
                var yExpr = Expression.Constant(yDoubleVal);
                var rExpr = func(xExpr, yExpr);
                var result = Expression.Lambda<Func<double>>(rExpr).Compile().Invoke();

                int iResult;
                if (LogicSharp.IsInt(result, out iResult))
                {
                    return iResult;
                }
                else
                {
                    return result;
                }
            }
            else
            {
                return null;
            }
        }
    }
}

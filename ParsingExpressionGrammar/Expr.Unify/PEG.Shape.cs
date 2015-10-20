using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using CSharpLogic;
using NUnit.Framework;
using starPadSDK.MathExpr;

namespace ExprPatternMatch
{
    public static partial class ExpressionPatternExtensions
    {
        /// <summary>
        /// The purpose of parse string is to re-format the str
        /// x, 2x, -2x, ax, -ax,2ax, 3y,34y, y25
        /// </summary>
        /// <param name="str"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static object TransformString(string str)
        {
            char[] charArr = str.ToCharArray();
            if (charArr.Length == 1) return new Var(str);

            bool isNeg = false;
            string parseStr;
            if (charArr[0].Equals('-'))
            {
                isNeg = true;
                parseStr = str.Substring(1, str.Length-1);
            }
            else
            {
                parseStr = str;
            }

            //TODO tackle decimal number 
            string[] strs = Regex.Split(parseStr, "(?<=\\D)(?=\\d)|(?<=\\d)(?=\\D)");
            //string[] strs = Regex.Split(parseStr, "(?<=\\D)(?=\\d)|(?<=(\\d+\\.\\d+))(?=\\D)");
            var lst = new List<object>();
            for(var i = 0; i < strs.Length; i++)
            {
                if(strs[i].Equals(".")) throw new Exception("Cannot be decimal input");

                if (i == 0)
                {
                    if (LogicSharp.IsNumeric(strs[i]))
                    {
                        int iNum;
                        bool result000 = LogicSharp.IsInt(strs[i], out iNum);
                        if (result000)
                        {
                            iNum = isNeg ? iNum * -1 : iNum;
                            lst.Add(iNum);
                        }
                        else
                        {
                            double dNum;
                            LogicSharp.IsDouble(strs[i], out dNum);
                            dNum = isNeg ? dNum * -1 : dNum;
                            lst.Add(dNum);                            
                        }
                    }
                    else
                    {
                        char[] tempArr = strs[i].ToCharArray();
                        if (isNeg)
                        {
                            lst.Add(-1);                            
                        }
                        lst.AddRange(tempArr.Select(c => new Var(c)).Cast<object>());
                    }
                }
                else
                {
                    if (LogicSharp.IsNumeric(strs[i]))
                    {
                        int iNum;
                        bool result000 = LogicSharp.IsInt(strs[i], out iNum);
                        if (result000)
                        {
                            lst.Add(iNum);
                        }
                        else
                        {
                            double dNum;
                            LogicSharp.IsDouble(strs[i], out dNum);
                            lst.Add(dNum);
                        }
                    }
                    else
                    {
                        char[] tempArr = strs[i].ToCharArray();
                        lst.AddRange(tempArr.Select(c => new Var(c)).Cast<object>());
                    }
                }
            }
            if (lst.Count == 1) return lst[0];
            return new Term(Expression.Multiply, lst);
        }

        public static object TransformString2(string str)
        {
            char[] charArr = str.ToCharArray();
            if (charArr.Length == 1) return new Var(str);
            bool isNeg = charArr[0].Equals('-');

            var lst = new List<object>();
            int j = - 1;
            for (int i = charArr.Count() - 1; i >= 0; i--)
            {
                if (Char.IsLetter(charArr[i]))
                {
                    lst.Insert(0, new Var(charArr[i]));
                }
                else
                {
                    j = i;
                    break;
                }
            }
            if (isNeg)
            {
                if (j == 0)
                {
                    var term1 = new Term(Expression.Multiply, new List<object>() {-1, lst[0]});
                    lst[0] = term1;

                    if (lst.Count == 1)
                    {
                        return lst[0];
                    }
                    return new Term(Expression.Multiply, lst);
                }
                var subStr = str.Substring(1, j);
                bool result = LogicSharp.IsNumeric(subStr);
                if (!result) return null;

                int iNum;
                bool result000 = LogicSharp.IsInt(subStr, out iNum);
                if (result000)
                {
                    lst.Insert(0,-1*iNum);
                }
                else
                {
                    double dNum;
                    LogicSharp.IsDouble(subStr, out dNum);
                    lst.Insert(0,-1*dNum);
                }
            }
            else
            {
                if (j == -1)
                {
                    if (lst.Count == 1)
                    {
                        return lst[0];
                    }
                    return new Term(Expression.Multiply, lst);
                }
                var subStr = str.Substring(0, j+1);

                bool result = LogicSharp.IsNumeric(subStr);
                if (!result) return null;

                int iNum;
                bool result000 = LogicSharp.IsInt(subStr, out iNum);
                if (result000)
                {
                    lst.Insert(0,iNum);
                }
                else
                {
                    double dNum;
                    LogicSharp.IsDouble(subStr, out dNum);
                    lst.Insert(0,dNum);
                }
            }
            return new Term(Expression.Multiply, lst);
        }
    }
}

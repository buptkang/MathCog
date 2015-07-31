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
            var lst = new List<object>();
            for(var i = 0; i < strs.Length; i++)
            {
                if(strs[i].Equals(".")) throw new Exception("Cannot be decimal input");

                if (i == 0)
                {
                    if (LogicSharp.IsNumeric(strs[0]))
                    {
                        double dNum;
                        LogicSharp.IsDouble(strs[0], out dNum);
                        dNum = isNeg ? dNum *-1 : dNum;
                        lst.Add(dNum);
                    }
                    else
                    {
                        char[] tempArr = strs[0].ToCharArray();
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
                        double dNum;
                        LogicSharp.IsDouble(strs[i], out dNum);
                        lst.Add(dNum);
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
    }
}

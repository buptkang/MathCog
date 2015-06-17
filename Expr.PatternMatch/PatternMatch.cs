using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlgebraGeometry;
using FunctionalCSharp;
using FunctionalCSharp.DiscriminatedUnions;
using starPadSDK.MathExpr;

namespace ExprSemantic
{
    public class ExprVisitor
    {
        #region Singleton

        private static ExprVisitor _instance;

        private ExprVisitor()
        {            
        }

        public static ExprVisitor Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ExprVisitor();                                        
                }

                return _instance;
            }
        }

        #endregion

        public object Match(starPadSDK.MathExpr.Expr exp)
        {
            var dict = new Dictionary<PatternEnum, object>();
            object obj;

            bool result = exp.IsNumeric(out obj);
            if (result) dict.Add(PatternEnum.Numeric, obj);

            result = exp.IsLabel(out obj);
            if (result) dict.Add(PatternEnum.Label, obj);

            result = exp.IsExpression(out obj); //Term
            if (result) dict.Add(PatternEnum.Expression, obj);

            result = exp.IsPoint(out obj);
            if (result) dict.Add(PatternEnum.Point, obj);

            LineSymbol ls;
            result = exp.IsLine(out ls);
            if (result) dict.Add(PatternEnum.Line, ls);
       
            result = exp.IsGoal(out obj);
            if (result) dict.Add(PatternEnum.Goal, obj);

            return dict;
        }

        public object MatchQuery(starPadSDK.MathExpr.Expr exp)
        {
            object obj;
            bool result = exp.IsQuery(out obj); 
            //Query -> KeyValuePair<string,object>
            if (result) return obj;

            return null;
        }
    }

    public enum PatternEnum
    {
        Numeric,
        Label,
        Expression,
        Point,
        Line,
        Goal
    }
}

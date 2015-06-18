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

            bool result = exp.IsLabel(out obj);
            if (result) dict.Add(PatternEnum.Label, obj); //string

            result = exp.IsPoint(out obj); //Algebraic point form
            if (result) dict.Add(PatternEnum.Point, obj); //PointSymbol

            LineSymbol ls; 
            result = exp.IsLine(out ls); //Algebraic line form
            if (result) dict.Add(PatternEnum.Line, ls); //LineSymbol
       
            result = exp.IsGoal(out obj); //Property form
            if (result) dict.Add(PatternEnum.Goal, obj); //EqGoal

            return dict.Count == 1 ? obj : dict;
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

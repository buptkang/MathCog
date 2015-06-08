using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            object obj;
            bool result = exp.IsNumeric(out obj);
            if (result) return obj;

            result = exp.IsLabel(out obj);
            if (result) return obj;

            result = exp.IsExpression(out obj); //Term
            if (result) return obj;

            result = exp.IsGoal(out obj);
            //result = exp.IsTerm(out obj); //Goal
            if (result) return obj;

            result = exp.IsPoint(out obj);
            if (result) return obj;

            return null;
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
}

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
            bool result = exp.IsPoint(out obj);
            if (result)
            {
                return obj;
            }

            result = exp.IsTerm(out obj);
            if (result)
            {
                return obj;
            }

            return null;
        }
    }
}

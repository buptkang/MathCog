using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSharpLogic;
using starPadSDK.MathExpr;

namespace AlgebraGeometry.Expr
{
    /// <summary>
    /// such as S= 5.0, m = 4;
    /// </summary>
    public class AGPropertyExpr : EqGoal, IKnowledgeExpr 
    {
        private starPadSDK.MathExpr.Expr _inputExpr;

        public AGPropertyExpr(starPadSDK.MathExpr.Expr expr, object lhs, object rhs)
            :base(lhs, rhs)
        {
            _inputExpr = expr;
        }

        public object GetInputObject()
        {
            return _inputExpr;
        }
    }
}

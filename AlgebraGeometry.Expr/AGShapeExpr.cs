using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSharpLogic;
using starPadSDK.MathExpr;
using AlgebraGeometry;

namespace AlgebraGeometry.Expr
{
    public class AGShapeExpr : ShapeSymbol, IKnowledgeExpr
    {
        private starPadSDK.MathExpr.Expr _inputExpr;

        public AGShapeExpr(starPadSDK.MathExpr.Expr expr, Shape shape)
            :base(shape)
        {
            _inputExpr = expr;
        }

        public object GetInputObject()
        {
            return _inputExpr;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using starPadSDK.MathExpr;

namespace AlgebraGeometry.Expr
{
    /// <summary>
    /// Knowledge Interface with parsing support(starPadSDK.MathExpr)
    /// </summary>
    public interface IKnowledgeExpr
    {
        object GetInputObject();
    }
}

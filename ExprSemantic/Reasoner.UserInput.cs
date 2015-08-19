using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using AlgebraGeometry;
using AlgebraGeometry.Expr;
using CSharpLogic;
using ExprPatternMatch;
using starPadSDK.MathExpr;

namespace MathReason
{
    /// <summary>
    /// User modelling in problem-solving
    /// </summary>
    public partial class Reasoner
    {
        private bool EvalUserModel(Expr expr,
            object obj,
            ShapeType? st,
            out object output)
        {
            output = null;
            return false;
        }

        private void UnEvalInUserModel(object obj)
        {
            
        }

    }
}

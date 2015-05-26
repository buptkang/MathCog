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
    public abstract class IKnowledge
    {
        public abstract IEnumerable<IKnowledge> RetrieveGeneratedShapes();

        private starPadSDK.MathExpr.Expr _inputExpr;
        public starPadSDK.MathExpr.Expr Expr
        {
            get { return _inputExpr; }
            set { _inputExpr = value; }
        }

        protected IKnowledge(starPadSDK.MathExpr.Expr exp)
        {
            _inputExpr = exp;
        }
    }
}

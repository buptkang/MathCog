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
    public class AGPropertyExpr : IKnowledge
    {
        private EqGoal _goal;
        public EqGoal Goal
        {
            get { return _goal; }
            set { _goal = value; }
        }

        public AGPropertyExpr(starPadSDK.MathExpr.Expr expr, EqGoal goal)
            :base(expr)
        {
            _goal = goal;
        }

        public override IEnumerable<IKnowledge> RetrieveGeneratedShapes()
        {
            throw new NotImplementedException();
        }

    }
}

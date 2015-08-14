using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSharpLogic;
using ExprGenerator;
using starPadSDK.MathExpr;
using ExprSemantic;

namespace AlgebraGeometry.Expr
{
    /// <summary>
    /// such as S= 5.0, m = 4, x=2+1
    /// </summary>
    public class AGPropertyExpr : AGEquationExpr
    {
        #region Properties and Constructors

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

        #endregion

        #region Override functions

        public override void GenerateSolvingTrace()
        {
            if (IsSelected)
            {
                var traces = _goal.Traces;
                if (traces.Count == 0) return;
                var lst = new List<TraceStepExpr>();
                TraceStepExpr tse;
                for (int i = 0; i < traces.Count; i++)
                {
                    var ts = traces[i];
                    tse = new TraceStepExpr(ts);
                    lst.Add(tse);
                }
                AutoTrace = lst;
                return;
            }

            if (RenderKnowledge == null) return;

            foreach (var temp in RenderKnowledge)
            {
                if (temp.IsSelected)
                {
                    temp.GenerateSolvingTrace();
                }
            }
        }

        public override void RetrieveRenderKnowledge()
        {
            base.RetrieveRenderKnowledge();
        }

        #endregion
    }
}
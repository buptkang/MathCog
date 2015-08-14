using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using CSharpLogic;
using starPadSDK.MathExpr;

namespace AlgebraGeometry.Expr
{
    /// <summary>
    /// Knowledge Interface with parsing support(starPadSDK.MathExpr)
    /// </summary>
    public class IKnowledge
    {
        #region Properties and Constructors

        private starPadSDK.MathExpr.Expr _inputExpr;
        public starPadSDK.MathExpr.Expr Expr
        {
            get { return _inputExpr; }
            set { _inputExpr = value; }
        }

        public bool IsSelected { get; set; } // query handler

        public List<IKnowledge> RenderKnowledge { get; set; }

        public List<TraceStepExpr> AutoTrace { get; set; }

        public IKnowledge(starPadSDK.MathExpr.Expr exp)
        {
            _inputExpr = exp;
        }

        #endregion

        #region Virtual Functions and Utils

        public virtual void GenerateSolvingTrace()
        {
        }

        public virtual void RetrieveRenderKnowledge()
        {
        }

        public IKnowledge FindSelectedKnowledge()
        {
            if (IsSelected) return this;
            return RenderKnowledge.FirstOrDefault(tempKnowledge => tempKnowledge.IsSelected);
        }

        #endregion
    }
}

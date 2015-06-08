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
    public abstract class IKnowledge
    {
        private starPadSDK.MathExpr.Expr _inputExpr;
        public starPadSDK.MathExpr.Expr Expr
        {
            get { return _inputExpr; }
            set { _inputExpr = value; }
        }

        protected IKnowledge(starPadSDK.MathExpr.Expr exp)
        {
            _inputExpr = exp;
            KnowledgeTrace = new List<TraceStepExpr>();
        }

        protected void GenerateTrace(DyLogicObject obj)
        {
            TraceStepExpr tse;
            for (int i = obj.Traces.Count - 1; i >= 0; i--)
            {
                var ts = obj.Traces[i] as TraceStep;
                tse = new TraceStepExpr(ts);
                KnowledgeTrace.Add(tse);
            }
        }

        //declarative trace to record computation path 
        public List<TraceStepExpr> KnowledgeTrace { get; set; } 
    }
}

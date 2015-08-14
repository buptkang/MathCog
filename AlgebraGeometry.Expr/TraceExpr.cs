using System.Collections.Generic;
using System.Diagnostics;
using CSharpLogic;
using ExprGenerator;

namespace AlgebraGeometry.Expr
{
    public class TraceStepExpr
    {
        #region Properties and Constructors

        public string MetaRule { get; set; } //Tutor Mode
        public string AppliedRule { get; set; } //Demonstration Mode

        public starPadSDK.MathExpr.Expr Source { get; set; }
        public starPadSDK.MathExpr.Expr Target { get; set; }
        public starPadSDK.MathExpr.Expr StepExpr { get; set; }

        public TraceStepExpr(TraceStep ts)
        {
            MetaRule = ts.Rule as string;
            AppliedRule = ts.AppliedRule as string;
            Source = ExprG.Generate(ts.Source);
            Target = ExprG.Generate(ts.Target);
            StepExpr = ExprG.Derive(Source, Target);
        }

        #endregion

        public override bool Equals(object obj)
        {
            var traceStep = obj as TraceStepExpr;
            if (traceStep == null) return false;

            return traceStep.AppliedRule.Equals(AppliedRule)
                   && traceStep.Source.Equals(Source)
                   && traceStep.Target.Equals(Target);
        }

        public override int GetHashCode()
        {
            return Source.GetHashCode() ^ 
                Target.GetHashCode() ^ AppliedRule.GetHashCode();
        }
    }
}

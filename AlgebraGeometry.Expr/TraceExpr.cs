using System.Collections.Generic;
using System.Diagnostics;
using CSharpLogic;
using ExprGenerator;

namespace AlgebraGeometry.Expr
{
    public class TraceStepExpr
    {
        public string AppliedRule { get; set; }

        public starPadSDK.MathExpr.Expr Source { get; set; }

        public starPadSDK.MathExpr.Expr Target { get; set; }

        public TraceStepExpr(TraceStep ts)
        {
            AppliedRule = ts.Rule as string;
            Source = ExprG.Generate(ts.Source);
            Target = ExprG.Generate(ts.Target);
        }

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

using NUnit.Framework;
using starPadSDK.MathExpr;
using System.Collections.Generic;

namespace ExprSemantic
{
    public class TraceStep
    {        
        public string AppliedRule { get; set; }

        public Expr TraceExpr { get; set; }

        public TraceStep(Expr target, string appliedRule)
        {
            AppliedRule = appliedRule;
            TraceExpr = target;
        }
    }

    public class Trace
    {
        public List<TraceStep> Steps { get; set; }

        public Trace(List<TraceStep> steps)
        {
            Steps = steps;
        }

        //TODO
        public string StrategyHint { get; set; } 
    }
}
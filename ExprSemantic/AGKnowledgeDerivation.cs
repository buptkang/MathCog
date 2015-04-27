using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AGSemantic.KnowledgeBase;
using ExprSemantic.KnowledgeRelation;
using starPadSDK.MathExpr;

namespace ExprSemantic
{
    public class AGKnowledgeTracer
    {
        public string WhyHints { get; set; }
        public string StrategyHints { get; set; }
        public string AppliedRule { get; set; }

        public Expr Source { get; set; }
        public Expr Target { get; set; }

        public AGKnowledgeTracer(Expr source, Expr target, string whyHint, string strategyHint, string appliedRule)
        {
            StrategyHints = strategyHint;
            WhyHints = whyHint;
            AppliedRule = appliedRule;
            Source = source;
            Target = target;
        }
    }
}
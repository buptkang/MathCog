using starPadSDK.MathExpr;

namespace ExprSemantic
{
    public class Tracer
    {
        public string WhyHints { get; set; }
        public string StrategyHints { get; set; }
        public string AppliedRule { get; set; }

        public Expr Source { get; set; }
        public Expr Target { get; set; }

        public Tracer(Expr source, Expr target, string whyHint, string strategyHint, string appliedRule)
        {
            StrategyHints = strategyHint;
            WhyHints = whyHint;
            AppliedRule = appliedRule;
            Source = source;
            Target = target;
        }
    }
}
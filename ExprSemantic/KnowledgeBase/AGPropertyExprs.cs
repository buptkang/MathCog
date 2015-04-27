using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AGSemantic.KnowledgeBase;
using starPadSDK.MathExpr;

namespace ExprSemantic.KnowledgeBase
{
    public class AGPropertyExpr : IKnowledgeExpr
    {
        //The knowledge the property belongs to
        public KeyValuePair<Expr,IKnowledgeExpr> Knowledge { get; set; }

        public Expr PropertyExpr { get; set; }

        public Expr PropertyAnswerExpr { get; set; }

        public AGPropertyExpr(Expr expr)
        {
            PropertyExpr = expr;
        }

        public void AddAnswer(KeyValuePair<Expr, IKnowledgeExpr> knowledge,
            Expr propertyAnswerExpr, List<AGKnowledgeTracer> propertyTracer)
        {
            Knowledge = knowledge;
            PropertyAnswerExpr = propertyAnswerExpr;
            Tracers = propertyTracer;
        }
    }
}

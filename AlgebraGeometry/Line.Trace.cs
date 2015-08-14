using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using CSharpLogic;
using NUnit.Framework;

namespace AlgebraGeometry
{
    public partial class LineSymbol : ShapeSymbol
    {
        public List<TraceStep> FromLineGeneralFormToSlopeTrace(object target)
        {
            var line = Shape as Line;
            Debug.Assert(line != null);
            Debug.Assert(line.InputType == LineType.GeneralForm);

            var lst = new List<TraceStep>();

            var lhs = new Var('m');
            var rhs = new Term(Expression.Divide, new List<object>() {SymA, SymB});
            var eq1 = new Equation(lhs, rhs);
            string step1metaRule = "Slope Concept TODO";
            string step1AppliedRule = String.Format("m = {0} / {1}", SymA, SymB);
            var ts = new TraceStep(this, eq1, step1metaRule, step1AppliedRule);

            lst.Add(ts);
            string step2metaRule = ArithRule.CalcRule("Divide");
            string step2AppliedRule = ArithRule.CalcRule("Divide", SymA, SymB, SymSlope);
            var ts2 = new TraceStep(eq1, target, step2metaRule, step2AppliedRule);
            lst.Add(ts2);
            return lst;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace CSharpLogic
{
    public static class EquationsRule
    {
        public enum EquationRuleType
        {
            Transitive,
            Symmetric,
            Inverse
        }

        public static string Rule(EquationRuleType ruleType,
            object obj1, object obj2)
        {
            switch (ruleType)
            {
                case EquationRuleType.Inverse:
                    return "TODO";
                case EquationRuleType.Symmetric:
                    return string.Format("Apply Symmetric law on equation {0}", obj1);
                case EquationRuleType.Transitive:
                    return "TODO";
            }
            return null;
        }

        public static string ApplyTransitiveProperty(Func<Expression, Expression, BinaryExpression> op, object obj)
        {
            Debug.Assert(op.Method != null);
            return string.Format("{0} {1} from both side of the equation.",
                op.Method.Name, obj.ToString());
        }
    }
}

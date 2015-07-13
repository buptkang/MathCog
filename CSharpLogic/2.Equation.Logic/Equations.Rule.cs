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
        public static string ApplyTransitiveProperty(Func<Expression, Expression, BinaryExpression> op, object obj)
        {
            Debug.Assert(op.Method != null);
            return string.Format("{0} {1} from both side of the equation.",
                op.Method.Name, obj.ToString());
        }
    }
}

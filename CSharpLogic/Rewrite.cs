using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace CSharpLogic
{
    public class Rewrite
    {
        public static object RewriteSameVariable(
            Func<Expression, Expression, BinaryExpression> op, 
            Var variable, out string rule)
        {
            rule = null;
            rule = RewriteRule.MergeSameVariable(op.Method.Name, variable);
            if (op == Expression.Add)
            {
                return new Term(Expression.Multiply, new Tuple<object, object>(2, variable));
            }
            else if (op == Expression.Subtract)
            {
                return 0;
            }
            else if (op == Expression.Multiply)
            {
                return new Term(Expression.Power, new Tuple<object, object>(2, variable));
            }
            else if (op == Expression.Divide)
            {
                return 1;
            }

            return null;
        }
  
    }
}

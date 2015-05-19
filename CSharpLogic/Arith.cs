using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace CSharpLogic
{
    public partial class LogicSharp
    {
        //x + y = z
        public static Goal Add(object x, object y, object z)
        {
            var addOp = new BinOp(Expression.Add, Expression.Subtract);
            return addOp.GenerateGoal(x,y,z);
        }

        // x - y = z
        public static Goal Sub(object x, object y, object z)
        {    
            return Add(z, y, x);
        }

        // z = x * y
        public static Goal Mul(object x, object y, object z)
        {
            var mul = new BinOp(Expression.Multiply, Expression.Divide);
            return mul.GenerateGoal(x,y,z);
        }

        // z = x/y
        public static Goal Div(object x, object y, object z)
        {
            return Mul(y,z,x);
        }

    }

    public class BinOp
    {
        public Func<Expression, Expression, BinaryExpression> Op { get; set; }
        public Func<Expression, Expression, BinaryExpression> RevOp { get; set; }

        public BinOp(Func<Expression, Expression, BinaryExpression> _op,
            Func<Expression, Expression, BinaryExpression> _revOp)
        {
            Op = _op;
            RevOp = _revOp;
        }

        public Goal GenerateGoal(object x, object y, object z)
        {
            if (Var.IsNotVar(x) && Var.IsNotVar(y))
            {
                var obj = LogicSharp.Calculate(Op,x,y);
                return new EqGoal(obj, z);
            }
            else if(Var.IsNotVar(y) && Var.IsNotVar(z) && RevOp != null)
            {
                var obj = LogicSharp.Calculate(RevOp, z, y);
                return new EqGoal(x, obj);
            }
            else if(Var.IsNotVar(x) && Var.IsNotVar(z) && RevOp != null)
            {
                var obj = LogicSharp.Calculate(RevOp, z, x);
                return new EqGoal(y, obj);
            }
            else if(Var.IsVar(x) && Var.IsVar(y))
            {
                var term1 = new Term(RevOp, new Tuple<object, object>(z, y));
                return new EqGoal(x, term1);
            }
            else if(Var.IsVar(y) && Var.IsVar(z))
            {
                var term1 = new Term(Op, new Tuple<object, object>(x, y));
                return new EqGoal(term1, z);
            }
            else if(Var.IsVar(x) && Var.IsVar(z))
            {
                //var oper = new Operator(RevOp.Method.Name);
                var term1 = new Term(Op, new Tuple<object, object>(x, y));
                return new EqGoal(term1, z);
            }
            else
            {
                return null;
            }
        }

    }
}

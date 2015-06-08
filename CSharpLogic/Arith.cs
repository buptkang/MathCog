using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace CSharpLogic
{
    public partial class LogicSharp
    {
        //x + y = z
        public static Goal Add(object x, object y, object z)
        {
            var addOp = new BinOp(Expression.Add, Expression.Subtract);
            return addOp.GenerateGoal(new Tuple<object, object>(x,y), z);
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
            return mul.GenerateGoal(new Tuple<object, object>(x,y),z);
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

        public Goal GenerateGoal(Tuple<object,object> tuple, object z)
        {
            object x = tuple.Item1;
            object y = tuple.Item2;

            if (LogicSharp.IsNumeric(x) && LogicSharp.IsNumeric(y))
            {
                var obj = LogicSharp.Calculate(Op,x,y);

                string rule = ArithRule.CalcRule(Op.Method.Name, x, y, obj);
                var step = new TraceStep(new Tuple<object,object>(x,y), obj, rule);

                var goal = new EqGoal(obj, z);
                goal.Traces.Add(step);
                return goal;
            }
            else if (LogicSharp.IsNumeric(y) && LogicSharp.IsNumeric(z) && RevOp != null)
            {
                string rule = null;//RewriteRule.MoveTerm(y, tuple, z);
                var step1 = new TraceStep(tuple, new Tuple<object, object>(z, y), rule);
                
                
                var obj = LogicSharp.Calculate(RevOp, z, y);
                rule = ArithRule.CalcRule(RevOp.Method.Name, z, y, obj);
                var step2 = new TraceStep(new Tuple<object, object>(z, y), obj, rule); 

                var goal = new EqGoal(x, obj);
                goal.Traces.Add(step1);
                goal.Traces.Add(step2);
                return goal;
            }
            else if (LogicSharp.IsNumeric(x) && LogicSharp.IsNumeric(z) && RevOp != null)
            {
                string rule = null;//RewriteRule.MoveTerm(x, tuple, z);
                var step1 = new TraceStep(tuple, new Tuple<object, object>(z, x), rule);

                var obj = LogicSharp.Calculate(RevOp, z, x);
                rule = ArithRule.CalcRule(RevOp.Method.Name, z, x, obj);
                var step2 = new TraceStep(new Tuple<object, object>(z, x), obj, rule);

                var goal = new EqGoal(x, obj);
                goal.Traces.Add(step1);
                goal.Traces.Add(step2);
                return goal;
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

        public TraceStep DoNumericCalcStep(Func<Expression, Expression, BinaryExpression> Op,
            object x, object y)
        {
            if (!LogicSharp.IsNumeric(x) || !LogicSharp.IsNumeric(y)) return null;

            var obj = LogicSharp.Calculate(Op, x, y);
            string rule = ArithRule.CalcRule(Op.Method.Name, x, y, obj);
            return new TraceStep(new Tuple<object, object>(x, y), obj, rule);
        }

        public TraceStep GenerateBasic(object x, object y, object z)
        {
            if (LogicSharp.IsNumeric(x) && LogicSharp.IsNumeric(y))
            {
                return DoNumericCalcStep(Op, x, y);
            }
            else if (LogicSharp.IsNumeric(y) && LogicSharp.IsNumeric(z) && RevOp != null)
            {
                return DoNumericCalcStep(RevOp, z, y);
            }
            else if (LogicSharp.IsNumeric(x) && LogicSharp.IsNumeric(z) && RevOp != null)
            {
                return DoNumericCalcStep(RevOp, z, x);
            }
            return null;
        }

        public TraceStep GenerateTerm(Term t)
        {
            return null;
        }

        public Goal GenerateGoal2(object x, object y, object z)
        {
            var xTerm = x as Term;
            if (xTerm != null)
            {
                xTerm.Eval();
            }
            /*
            TraceStep step = GenerateBasic(x, y, z);
            if (step != null)
            {
                //simply calculation
                var goal = new EqGoal(, step.Target);
                goal.Traces.Push(step);
                return goal;
            }
            */
            return null;
        }
    }
}

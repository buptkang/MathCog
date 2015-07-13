using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace CSharpLogic
{
    public interface IArithmeticLogic : IEval
    {
        /// <summary>
        //with calculation functionality, it does not 
        /// take care of term rewriting.
        /// 
        /// Terms should follow the below rules:
        /// tp: "2+2",  "2+3-1", "2+2*2"
        /// </summary>
        /// <returns> can be term or value</returns>
        object EvalArithmetic();
    }

    public static class ArithmeticEvalExtension
    {
        private static List<object> FindArithValues(this Term term)
        {
            var lst = term.Args as List<object>;
            Debug.Assert(lst != null);
            int arithStartIndex = -1;
            for (var i = 0; i < lst.Count; i++)
            {
                if (LogicSharp.IsNumeric(lst[i]))
                {
                    arithStartIndex = i;
                    break;
                }

                var termObj = lst[i] as Term;
                if (termObj != null && !termObj.ContainsVar())
                {
                    arithStartIndex = i;
                    break;
                }
            }
            //no arithmetic value
            if (arithStartIndex == -1) return null;
            return lst.GetRange(arithStartIndex, lst.Count - arithStartIndex);
        }

        public static object Calc(this Term term)
        {
            List<object> objs = term.FindArithValues();
            if (objs == null) return term;

            if (objs.Count == 1)
            {
                var term1 = objs[0] as Term;
                if (term1 != null)
                {
                    var gObj1 = term1.Calc();
                    //transform gTerm trace onto term
                    TraceUtils.MoveTraces(term1, term);
                    return term.Generate(term1, gObj1);
                }
                return term;
            }

            object gObj = null; 
            if (objs.Count == 2)
            {
                var obj11 = objs[0];
                var obj22 = objs[1];
                gObj = term.Numeric(obj11, obj22);
                return term.Generate(obj11, obj22, gObj);
            }

            var obj1 = objs[0];
            var obj2 = objs[1];
            gObj = term.Numeric(obj1, obj2);
            var gTerm = term.Generate(obj1, obj2, gObj) as Term;
            if (gTerm == null) throw new Exception("Must be term");
            object rObj = gTerm.Calc();
            //transform gTerm trace onto term
            TraceUtils.MoveTraces(gTerm, term);
            return rObj;
        }

        public static object Numeric(this Term term, object obj1, object obj2)
        {
            //obj1, obj2 can be the numeric value or term
            // 4 different combinations
            var term1 = obj1 as Term;
            if (term1 != null)
            {
                obj1 = term1.Calc();
                if (!obj1.Equals(term1)) TraceUtils.MoveTraces(term1, term);
            }

            var term2 = obj2 as Term;
            if (term2 != null)
            {
                obj2 = term2.Calc();
                if (!obj2.Equals(term2)) TraceUtils.MoveTraces(term2, term);
            }

            if (LogicSharp.IsNumeric(obj1) && LogicSharp.IsNumeric(obj2))
            {
                var obj = Calculate(term.Op, obj1, obj2);
                string rule = ArithRule.CalcRule(term.Op.Method.Name, obj1, obj2, obj);
                var ts = new TraceStep(new Tuple<object, object>(obj1, obj2), obj, rule);
                term.Traces.Add(ts);
                return obj;
            }
            return term;
        }

        public static object Calculate(Func<Expression, Expression, BinaryExpression> func,
            object x, object y)
        {
            double xDoubleVal;
            double yDoubleVal;
            bool isXDouble = LogicSharp.IsDouble(x, out xDoubleVal);
            bool isYDouble = LogicSharp.IsDouble(y, out yDoubleVal);

            if (isXDouble || isYDouble)
            {
                var xExpr = Expression.Constant(xDoubleVal);
                var yExpr = Expression.Constant(yDoubleVal);
                var rExpr = func(xExpr, yExpr);
                var result = Expression.Lambda<Func<double>>(rExpr).Compile().Invoke();

                int iResult;
                if (LogicSharp.IsInt(result, out iResult))
                {
                    return iResult;
                }
                else
                {
                    return result;
                }
            }
            else
            {
                return null;
            }
        }
    }

    public partial class LogicSharp
    {
        //x + y = z
        public static Goal Add(object x, object y, object z)
        {
            var addOp = new BinOp(Expression.Add, Expression.Subtract);
            return addOp.GenerateGoal(new Tuple<object, object>(x, y), z);
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
            return mul.GenerateGoal(new Tuple<object, object>(x, y), z);
        }

        // z = x/y
        public static Goal Div(object x, object y, object z)
        {
            return Mul(y, z, x);
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

        public Goal GenerateGoal(Tuple<object, object> tuple, object z)
        {
            object x = tuple.Item1;
            object y = tuple.Item2;

            if (LogicSharp.IsNumeric(x) && LogicSharp.IsNumeric(y))
            {
                var obj = LogicSharp.Calculate(Op, x, y);

                string rule = ArithRule.CalcRule(Op.Method.Name, x, y, obj);
                var step = new TraceStep(new Tuple<object, object>(x, y), obj, rule);

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
            else if (Var.IsVar(x) && Var.IsVar(y))
            {
                var term1 = new Term(RevOp, new Tuple<object, object>(z, y));
                return new EqGoal(x, term1);
            }
            else if (Var.IsVar(y) && Var.IsVar(z))
            {
                var term1 = new Term(Op, new Tuple<object, object>(x, y));
                return new EqGoal(term1, z);
            }
            else if (Var.IsVar(x) && Var.IsVar(z))
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

        public static Func<Expression, Expression, BinaryExpression> ReverseOp(Func<Expression, Expression, BinaryExpression> op)
        {
            if (op.Method.Name.Equals("Add"))
            {
                return Expression.Subtract;
            }
            else if (op.Method.Name.Equals("Subtract"))
            {
                return Expression.Add;
            }
            else if (op.Method.Name.Equals("Multiply"))
            {
                return Expression.Divide;
            }
            else if (op.Method.Name.Equals("Divide"))
            {
                return Expression.Multiply;
            }
            else
            {
                return null;
            }
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

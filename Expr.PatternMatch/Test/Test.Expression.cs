using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using ExprSemantic;
using NUnit.Framework;
using Text = starPadSDK.MathExpr.Text;
using starPadSDK.MathExpr;
using CSharpLogic;

namespace ExprPatternMatchTest
{
    [TestFixture]
    public class TestExpression
    {
        [Test]
        public void Test1()
        {
            //x+1+2
            string txt = "x+1+2";
            starPadSDK.MathExpr.Expr expr = Text.Convert(txt);
            object result = ExprVisitor.Instance.Match(expr);
            Assert.IsInstanceOf(typeof(Term), result);

            var term = result as Term;
            Assert.NotNull(term);
            var tuple = term.Args as Tuple<object, object>;
            Assert.NotNull(tuple);
            Assert.True(tuple.Item2.Equals(2));
            var term1 = tuple.Item1 as Term;
            Assert.NotNull(term1);
            var tuple1 = term1.Args as Tuple<object, object>;
            Assert.NotNull(tuple1);
            Assert.True(tuple1.Item2.Equals(1));
            var variable = tuple1.Item1 as Var;
            Assert.NotNull(variable);
            Assert.True(variable.ToString().Equals("x"));

            //term evaluation
            Assert.True(term.TraceCount == 0);

        }

        [Test]
        public void Test2()
        {
            //2-1+3
            string txt = "2-1+3";
            starPadSDK.MathExpr.Expr expr = Text.Convert(txt);
            object result = ExprVisitor.Instance.Match(expr);
            Assert.IsInstanceOf(typeof(Term), result);

            var term = result as Term;
            Assert.NotNull(term);
            var tuple = term.Args as Tuple<object, object>;
            Assert.NotNull(tuple);
            Assert.True(tuple.Item2.Equals(3));
            var term1 = tuple.Item1 as Term;
            Assert.NotNull(term1);
            var tuple1 = term1.Args as Tuple<object, object>;
            Assert.NotNull(tuple1);
            Assert.True(tuple1.Item2.Equals(-1));
            Assert.True(tuple1.Item1.Equals(2));

            object evalResult = term.Eval();
            Assert.True(evalResult.Equals(4));
            Assert.True(term.TraceCount == 2);
        }

        [Test]
        public void Test3()
        {
            //2-3+4*1
            string txt = "2-3+4*1";
            starPadSDK.MathExpr.Expr expr = Text.Convert(txt);
            object result = ExprVisitor.Instance.Match(expr);
            Assert.IsInstanceOf(typeof(Term), result);

            var term = result as Term;
            Assert.NotNull(term);
            object evalResult = term.Eval();
            Assert.True(evalResult.Equals(3));
            Assert.True(term.TraceCount == 3);
        }

        [Test]
        public void Test_1()
        {
            string txt = "x+1";
            Expr expr = Text.Convert(txt);
            object obj;
            bool result = expr.IsExpression(out obj);
            Assert.True(result);
            Assert.True(obj is Term);
            var term = obj as Term;
            Assert.NotNull(term);
            Assert.True(term.Op.Method.Name.Equals("Add"));
            var tuple = term.Args as Tuple<object, object>;
            Assert.NotNull(tuple);
            var arg1 = tuple.Item1 as Var;
            Assert.NotNull(arg1);
            Assert.True(arg1.ToString().Equals("x"));
            var arg2 = tuple.Item2;
            Assert.True(LogicSharp.IsNumeric(arg2));
        }

        [Test]
        public void Test_2()
        {
            string txt = "1-1";
            Expr expr = Text.Convert(txt);
            object obj;
            bool result = expr.IsExpression(out obj);
            Assert.True(result);
            Assert.True(obj is Term);
            var term = obj as Term;
            Assert.NotNull(term);
            Assert.True(term.Op.Method.Name.Equals("Add"));
            var tuple = term.Args as Tuple<object, object>;
            Assert.NotNull(tuple);
            var arg1 = tuple.Item1;
            Assert.True(LogicSharp.IsNumeric(arg1));
            var arg2 = tuple.Item2;
            Assert.True(LogicSharp.IsNumeric(arg2));
        }

        [Test]
        public void Test_3()
        {
            string txt = "1+2.1+3";
            Expr expr = Text.Convert(txt);
            object obj;
            bool result = expr.IsExpression(out obj);
            Assert.True(result);
            Assert.True(obj is Term);
            var term = obj as Term;
            Assert.NotNull(term);
            Assert.True(term.Op.Method.Name.Equals("Add"));
            var tuple = term.Args as Tuple<object, object>;
            Assert.NotNull(tuple);
            var arg1 = tuple.Item1 as Term;
            var arg2 = tuple.Item2;
            Assert.NotNull(arg1);
            int number;
            Assert.True(LogicSharp.IsInt(arg2, out number));
            Assert.True(3.Equals(number));

            Assert.True(arg1.Op.Method.Name.Equals("Add"));
            var arg1Args = arg1.Args as Tuple<object, object>;
            Assert.NotNull(arg1Args);
            Assert.True(LogicSharp.IsInt(arg1Args.Item1, out number));
            Assert.True(1.Equals(number));
            double number2;
            Assert.True(LogicSharp.IsDouble(arg1Args.Item2, out number2));
            Assert.True(2.1.Equals(number2));
        }

        [Test]
        public void Test_4()
        {
            string txt = "1-1 + 2";
            Expr expr = Text.Convert(txt);
            object obj;
            bool result = expr.IsExpression(out obj);
            Assert.True(result);
            Assert.True(obj is Term);
            var term = obj as Term;
            Assert.NotNull(term);
            Assert.True(term.Op.Method.Name.Equals("Add"));
            var tuple = term.Args as Tuple<object, object>;
            Assert.NotNull(tuple);
            var arg1 = tuple.Item1 as Term;
            var arg2 = tuple.Item2;
            Assert.NotNull(arg1);
            int number;
            Assert.True(LogicSharp.IsInt(arg2, out number));
            Assert.True(2.Equals(number));

            Assert.True(arg1.Op.Method.Name.Equals("Add"));
            var arg1Args = arg1.Args as Tuple<object, object>;
            Assert.NotNull(arg1Args);
            Assert.True(LogicSharp.IsInt(arg1Args.Item1, out number));
            Assert.True(1.Equals(number));
            Assert.True(LogicSharp.IsInt(arg1Args.Item2, out number));
            Assert.True(number.Equals(-1));
        }
    }
}

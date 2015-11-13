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

namespace ExprPatternMatch
{
    [TestFixture]
    public class TestExpression
    {
        [Test]
        public void Test_Numeric()
        {
            string txt = "3";
            starPadSDK.MathExpr.Expr expr = Text.Convert(txt);
            object obj;
            bool result = expr.IsNumeric(out obj);
            Assert.True(result);
            Assert.True(obj is int);
            Assert.True(3.Equals(obj));

            txt = "3.3";
            expr = Text.Convert(txt);
            result = expr.IsNumeric(out obj);
            Assert.True(result);
            Assert.True(obj is double);
            Assert.True(obj.Equals(3.3));

            txt = "-4";
            expr = Text.Convert(txt);
            result = expr.IsNumeric(out obj);
            Assert.True(result);
            Assert.True(obj is int);
            Assert.True(obj.Equals(-4));

            txt = "-4.4";
            expr = Text.Convert(txt);
            result = expr.IsNumeric(out obj);
            Assert.True(result);
            Assert.True(obj is double);
            Assert.True(obj.Equals(-4.4));

        }

        [Test]
        public void Test_Arith_1()
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

            var lst = term.Args as List<object>;
            Assert.NotNull(lst);
            Assert.True(lst.Count == 2);
            Assert.True(LogicSharp.IsNumeric(lst[0]));
            Assert.True(LogicSharp.IsNumeric(lst[1]));

            //Term Eval
            obj = term.Eval();
            Assert.True(0.Equals(obj));
            Assert.True(term.Traces.Count == 1);
        }

        public void Test_numerics_2()
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
            Assert.True(term.Traces.Count == 2);
        }

        public void Test_numerics_3()
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
            Assert.True(term.Traces.Count == 3);
        }

        public void Test_numerics_4()
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

        public void Test_numerics_5()
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

        public void Test_numerics_6()
        {
            //2+4*1
            string txt = "2+4*1";
            starPadSDK.MathExpr.Expr expr = Text.Convert(txt);
            object obj;
            bool result = expr.IsExpression(out obj);
            Assert.True(result);
            Assert.IsInstanceOf(typeof(Term), obj);
            var term = obj as Term;
            Assert.NotNull(term);
            var tuple = term.Args as Tuple<object, object>;
            Assert.NotNull(tuple);
            Assert.True(tuple.Item1.Equals(2));
            var term1 = tuple.Item2 as Term;
            Assert.NotNull(term1);
            var tuple1 = term1.Args as Tuple<object, object>;
            Assert.NotNull(tuple1);
            Assert.True(tuple1.Item1.Equals(4));
            Assert.True(tuple1.Item2.Equals(1));
        }

        [Test]
        public void Test_numerics_7()
        {
            var expr1 = new CompositeExpr(WellKnownSym.plus,
                new Expr[] {new IntegerNumber("16"), new IntegerNumber("9")});
            var expr2 = new CompositeExpr(WellKnownSym.root, 
                new Expr[] {new IntegerNumber("2"), expr1});
            var expr3 = new CompositeExpr(WellKnownSym.equals,
                new Expr[] {new LetterSym('d'), expr2});

            object obj;
            bool result = expr2.IsExpression(out obj);
            Assert.True(result);
            var gTerm = obj as Term;
            Assert.NotNull(gTerm);

            result = expr3.IsEquation(out obj);
            Assert.True(result);
            var gEquation = obj as Equation;
            Assert.NotNull(gEquation);
        }

        [Test]
        public void Test_numerics_8()
        {
            var expr1 = new CompositeExpr(WellKnownSym.divide,
                new Expr[] {new DoubleNumber(0.5)});

            var expr2 = new CompositeExpr(WellKnownSym.times,
                new Expr[] {-1, expr1});

            object obj;
            bool result = expr2.IsExpression(out obj);
            Assert.True(result);
        }    
    }
}
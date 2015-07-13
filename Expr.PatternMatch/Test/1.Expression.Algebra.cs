using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSharpLogic;
using ExprSemantic;
using NUnit.Framework;
using starPadSDK.MathExpr;
using Text = starPadSDK.MathExpr.Text;

namespace ExprPatternMatch
{
    [TestFixture]
    public class TestExpressionAlgebra
    {
        #region Label Test

        [Test]
        public void Test_Label_1()
        {
            ///"A", "c", "XT","c12","c_1"         

            string txt = "A";
            Expr expr = Text.Convert(txt);
            object obj;
            bool result = expr.IsLabel(out obj);
            Assert.True(result);
            result = expr.IsWordTerm(out obj);
            Assert.False(result);

            txt = "c";
            expr = Text.Convert(txt);
            result = expr.IsLabel(out obj);
            Assert.True(result);
            result = expr.IsWordTerm(out obj);
            Assert.False(result);

            txt = "XT";
            expr = Text.Convert(txt);
            result = expr.IsLabel(out obj);
            Assert.True(result);
            result = expr.IsWordTerm(out obj);
            Assert.False(result);

            txt = "c12";
            expr = Text.Convert(txt);
            result = expr.IsLabel(out obj);
            Assert.True(result);
            result = expr.IsWordTerm(out obj);
            Assert.False(result);

            txt = "2m1";
            expr = Text.Convert(txt);
            result = expr.IsLabel(out obj);
            Assert.True(result);
            result = expr.IsWordTerm(out obj);
            Assert.False(result);

            txt = "aX";
            expr = Text.Convert(txt);
            result = expr.IsLabel(out obj);
            Assert.True(result);
            result = expr.IsWordTerm(out obj);
            Assert.False(result);
        }

        [Test]
        public void Test_Label_2()
        {
            /// False: 2A, 12mm, 2XY, -2Y
            string txt = "2A";
            Expr expr = Text.Convert(txt);
            object obj;
            bool result = expr.IsLabel(out obj);
            Assert.True(result);
            result = expr.IsWordTerm(out obj);
            Assert.True(result);
            Assert.IsInstanceOf(typeof(Term), obj);
            var term = obj as Term;
            Assert.NotNull(term);
            Assert.True(term.Op.Method.Name.Equals("Multiply"));
            var tuple = term.Args as Tuple<object, object>;
            Assert.NotNull(tuple);
            Assert.True(tuple.Item1.Equals(2));
            var variable = tuple.Item2 as Var;
            Assert.NotNull(variable);
            Assert.True(variable.ToString().Equals("A"));

            txt = "12mm";
            expr = Text.Convert(txt);
            result = expr.IsLabel(out obj);
            Assert.True(result);
            result = expr.IsWordTerm(out obj);
            Assert.True(result);
            Assert.IsInstanceOf(typeof(Term), obj);
            term = obj as Term;
            Assert.NotNull(term);
            Assert.True(term.Op.Method.Name.Equals("Multiply"));
            tuple = term.Args as Tuple<object, object>;
            Assert.NotNull(tuple);
            Assert.True(tuple.Item1.Equals(12));
            variable = tuple.Item2 as Var;
            Assert.NotNull(variable);
            Assert.True(variable.ToString().Equals("mm"));

            txt = "2XY";
            expr = Text.Convert(txt);
            result = expr.IsLabel(out obj);
            Assert.True(result);
            result = expr.IsWordTerm(out obj);
            Assert.True(result);
            Assert.IsInstanceOf(typeof(Term), obj);
            term = obj as Term;
            Assert.NotNull(term);
            Assert.True(term.Op.Method.Name.Equals("Multiply"));
            tuple = term.Args as Tuple<object, object>;
            Assert.NotNull(tuple);
            Assert.True(tuple.Item1.Equals(2));
            variable = tuple.Item2 as Var;
            Assert.NotNull(variable);
            Assert.True(variable.ToString().Equals("XY"));

            txt = "-2Y";
            expr = Text.Convert(txt);
            result = expr.IsLabel(out obj);
            Assert.False(result);
            result = expr.IsWordTerm(out obj);
            Assert.True(result);
            Assert.IsInstanceOf(typeof(Term), obj);
            term = obj as Term;
            Assert.NotNull(term);
            Assert.True(term.Op.Method.Name.Equals("Multiply"));
            tuple = term.Args as Tuple<object, object>;
            Assert.NotNull(tuple);
            Assert.True(tuple.Item1.Equals(-2));
            variable = tuple.Item2 as Var;
            Assert.NotNull(variable);
            Assert.True(variable.ToString().Equals("Y"));
        }

        #endregion

        #region Algebra pattern match and eval

        [Test]
        public void Test1()
        {
            //a+1+2
            string txt = "a+1+2";
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
        public void Test_1()
        {
            string txt = "a+1";
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

        #endregion       
    }
}

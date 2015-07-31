using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
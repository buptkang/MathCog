using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpLogic;
using ExprSemantic;
using FunctionalCSharp.Parser;
using NUnit.Framework;
using Text = starPadSDK.MathExpr.Text;

namespace ExprPatternMatchTest
{
    [TestFixture]
    public class PatternTest
    {
        [Test]
        public void TestNumber()
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
        public void TestCoordinate()
        {
            string txt = "-3.6";
            starPadSDK.MathExpr.Expr expr = Text.Convert(txt);
            object obj;
            bool result = expr.IsCoordinateTerm(out obj);
            Assert.True(result);
            Assert.True(obj is double);
            Assert.True(obj.Equals(-3.6));

            txt = "3 = x";
            expr = Text.Convert(txt);
            result = expr.IsCoordinateTerm(out obj);
            Assert.True(result);
            Assert.IsInstanceOf(typeof(EqGoal), obj);
            //var dict = (KeyValuePair<object, object>)obj;
            var dict = (EqGoal) obj;
            Assert.NotNull(dict);
            Assert.True(dict.Lhs.Equals("x"));
            Assert.True(dict.Rhs.Equals(3));

            txt = "Y = 4.0";
            expr = Text.Convert(txt);
            result = expr.IsCoordinateTerm(out obj);
            Assert.True(result);
            Assert.NotNull(obj);
            Assert.IsInstanceOf(typeof(EqGoal), obj);
            dict = (EqGoal)obj;
            Assert.NotNull(dict);
            Assert.True(dict.Lhs.Equals("Y"));
            Assert.True(dict.Rhs.Equals(4.0));

        }

        [Test]
        public void TestExpr()
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
    }
}

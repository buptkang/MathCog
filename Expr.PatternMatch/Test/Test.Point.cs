using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExprSemantic;
using NUnit.Framework;
using starPadSDK.MathExpr;
using Text = starPadSDK.MathExpr.Text;
using AlgebraGeometry;
using CSharpLogic;

namespace ExprPatternMatchTest
{
    [TestFixture]
    public class TestPoint
    {
        [Test]
        public void Test1()
        {
            string txt = "(1,1)";
            starPadSDK.MathExpr.Expr expr = Text.Convert(txt);
            object result = ExprVisitor.Instance.Match(expr);

            Assert.IsInstanceOf<PointSymbol>(result);
            var ps = result as PointSymbol;
            Assert.NotNull(ps);
            Assert.True(ps.SymXCoordinate.Equals("1"));
            Assert.True(ps.SymYCoordinate.Equals("1"));
            Assert.Null(ps.Shape.Label);
        }

        [Test]
        public void Test2()
        {
            string txt = "A(x,2)";
            starPadSDK.MathExpr.Expr expr = Text.Convert(txt);
            object result = ExprVisitor.Instance.Match(expr);
            var ps = result as PointSymbol;
            Assert.NotNull(ps);
            Assert.True(ps.SymXCoordinate.Equals("x"));
            Assert.True(ps.SymYCoordinate.Equals("2"));
            Assert.True(ps.Shape.Label.Equals("A"));
            
            const string txt1 = "x = 2.0";
            expr = Text.Convert(txt1);
            result = ExprVisitor.Instance.Match(expr);
            Assert.NotNull(result);
            var dict = result as Dictionary<PatternEnum, object>;
            Assert.NotNull(dict);

            // Assert.IsInstanceOf(typeof(KeyValuePair<object,object>), result);
        }

        [Test]
        public void Test3()
        {
            string txt = "(-3.0,y)";
            starPadSDK.MathExpr.Expr expr = Text.Convert(txt);
            object result = ExprVisitor.Instance.Match(expr);
            var ps = result as PointSymbol;
            Assert.NotNull(ps);
            Assert.True(ps.SymXCoordinate.Equals("-3"));
            Assert.True(ps.SymYCoordinate.Equals("y"));
            Assert.Null(ps.Shape.Label);
        }

        [Test]
        public void Test4()
        {
            //TODO
/*            txt = "(-3.0,y + 1 = 2)";
            expr = Text.Convert(txt);
            result = ExprVisitor.Instance.Match(expr);
            ps = result as PointSymbol;
            Assert.NotNull(ps);*/
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
            var dict = (EqGoal)obj;
            Assert.NotNull(dict);
            
            Assert.True(dict.Lhs.ToString().Equals("x"));
            Assert.True(dict.Rhs.Equals(3));

            txt = "Y = 4.0";
            expr = Text.Convert(txt);
            result = expr.IsCoordinateTerm(out obj);
            Assert.True(result);
            Assert.NotNull(obj);
            Assert.IsInstanceOf(typeof(EqGoal), obj);
            dict = (EqGoal)obj;
            Assert.NotNull(dict);
            Assert.True(dict.Lhs.ToString().Equals("Y"));
            Assert.True(dict.Rhs.Equals(4.0));

        }
    }
}

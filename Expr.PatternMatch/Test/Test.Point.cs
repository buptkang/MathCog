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

namespace ExprPatternMatchTest
{
    [TestFixture]
    public class TestPoint
    {
        [Test]
        public void Test1()
        {
            string txt = "(2,3)";
            starPadSDK.MathExpr.Expr expr = Text.Convert(txt);
            object result = ExprVisitor.Instance.Match(expr);

            Assert.IsInstanceOf<PointSymbol>(result);
            var ps = result as PointSymbol;
            Assert.NotNull(ps);
            Assert.True(ps.SymXCoordinate.Equals("2"));
            Assert.True(ps.SymYCoordinate.Equals("3"));
            Assert.Null(ps.Shape.Label);
        }

        [Test]
        public void Test2()
        {
            string txt = "A(x,2)";
            Expr expr = Text.Convert(txt);
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
            Assert.IsInstanceOf(typeof(KeyValuePair<object,object>), result);
        }

        [Test]
        public void Test3()
        {
            string txt = "(-3.0,y)";
            Expr expr = Text.Convert(txt);
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
    }
}

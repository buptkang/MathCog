using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExprPatternMatch;
using ExprSemantic;
using NUnit.Framework;
using starPadSDK.MathExpr;
using Text = starPadSDK.MathExpr.Text;
using AlgebraGeometry;
using CSharpLogic;

namespace ExprPatternMatch
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

        [Test]
        public void Test_Point()
        {
            object x = 3;
            object y = -3.9;

            PointSymbol ps = PointPatternExtensions.CreatePointSymbol(x, y);
            Assert.NotNull(ps);
            Assert.True(ps.SymXCoordinate.Equals("3"));
            Assert.True(ps.SymYCoordinate.Equals("-3.9"));
            Assert.True(ps.ToString().Equals("(3,-3.9)"));
            var pt = ps.Shape as Point;
            Assert.NotNull(pt);
            Assert.True(pt.Concrete);

            string label = "A";
            ps = PointPatternExtensions.CreatePointSymbol(label, x, y);
            Assert.NotNull(ps);
            Assert.True(ps.SymXCoordinate.Equals("3"));
            Assert.True(ps.SymYCoordinate.Equals("-3.9"));
            Assert.True(ps.ToString().Equals("A(3,-3.9)"));
            pt = ps.Shape as Point;
            Assert.NotNull(pt);
            Assert.True(pt.Concrete);

            x = "X";
            y = "2";
            ps = PointPatternExtensions.CreatePointSymbol(x, y);
            Assert.NotNull(ps);
            Assert.True(ps.SymXCoordinate.Equals("X"));
            Assert.True(ps.SymYCoordinate.Equals("2"));
            Assert.True(ps.ToString().Equals("(X,2)"));
            pt = ps.Shape as Point;
            Assert.NotNull(pt);
            Assert.False(pt.Concrete);

            /*            
                       var dict  = new KeyValuePair<object, object>("m", 4);
                       var dict2 = new KeyValuePair<object, object>("n", 5);

                       x = dict;
                       y = dict2;
                       ps = ExprKnowledgeFactory.CreatePointSymbol(x, y);
                       Assert.NotNull(ps);
                       Assert.True(ps.SymXCoordinate.Equals("4"));
                       Assert.True(ps.SymYCoordinate.Equals("5"));
                       pt = ps.Shape as Point;
                       Assert.NotNull(pt);
                       Assert.True(pt.XCoordinate.Equals("m"));
                       Assert.True(pt.YCoordinate.Equals("n"));
                       Assert.True(ps.ToString().Equals("(4,5)"));
                       Assert.True(pt.Concrete); 
            */
        }
    }
}

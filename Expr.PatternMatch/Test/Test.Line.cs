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
    public class TestLine
    {
        [Test]
        public void Test_Line1()
        {
            //x=2 
            const string txt = "x=2";
            starPadSDK.MathExpr.Expr expr = Text.Convert(txt);
            LineSymbol ls;
            bool result = expr.IsLine(out ls);
            Assert.True(result);
            Assert.NotNull(ls);
            Assert.True(ls.SymA.Equals("1"));
            Assert.True(ls.ToString().Equals("x-2=0"));
        }

        [Test]
        public void Test_Line2()
        {
            //y=1
            const string txt = "y=1";
            starPadSDK.MathExpr.Expr expr = Text.Convert(txt);
            LineSymbol ls;
            bool result = expr.IsLine(out ls);
            Assert.True(result);
            Assert.NotNull(ls);
            Assert.Null(ls.SymA);
            Assert.True(ls.SymB.Equals("1"));
            Assert.True(ls.ToString().Equals("y-1=0"));
        }

        [Test]
        public void Test_Line3()
        {
            const string txt = "ax+by+1=0";
            starPadSDK.MathExpr.Expr expr = Text.Convert(txt);
            LineSymbol ls;
            bool result = expr.IsLine(out ls);
            Assert.True(result);
            Assert.NotNull(ls);
            Assert.True(ls.SymA.Equals("a"));
            Assert.True(ls.SymB.Equals("b"));
            Assert.True(ls.ToString().Equals("ax+by+1=0"));
        }

        [Test]
        public void Test_Line4()
        {
            const string txt = "2x+y+1=0";
            starPadSDK.MathExpr.Expr expr = Text.Convert(txt);
            LineSymbol ls;
            bool result = expr.IsLine(out ls);
            Assert.True(result);
            Assert.NotNull(ls);
            Assert.True(ls.SymA.Equals("2"));
            Assert.True(ls.SymB.Equals("1"));
            Assert.True(ls.ToString().Equals("2x+y+1=0"));
        }

        [Test]
        public void Test_Line5()
        {
            //2+1=0
            const string txt = "2+1=0";
            starPadSDK.MathExpr.Expr expr = Text.Convert(txt);
            LineSymbol ls;
            bool result = expr.IsLine(out ls);
            Assert.False(result);
        }

        [Test]
        public void Test_Line6()
        {
            //2z+1=0
            const string txt = "2z+1=0";
            starPadSDK.MathExpr.Expr expr = Text.Convert(txt);
            LineSymbol ls;
            bool result = expr.IsLine(out ls);
            Assert.False(result);
        }

        public void Test_line3()
        {
            //TODO
            const string txt = "ax-by+1=0";
        }
    }
}

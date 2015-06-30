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
        /*
         * True Positive Test:
         * 
         *  1: x=2 
         *  2: y=1
         *  3: x+by+1=0
         *  4: 2x+y+1=0
         *  5: ax=0
         *  6: by=0
         *  7: cy=0
         *  8: -2.1x-3y-1=0
         *  9: -2x+3.2y+1=0
         *  
         * 
         *  TODO: ax-by+1=0
         *  TODO: -ax-by-9=0
         * 
         * False Negative Test:
         *  
         *  1: 2+1=0
         *  2: 2z+1=0
         */

        [Test]
        public void Test_Line_TruePositive_1()
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
        public void Test_Line_TruePositive_2()
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
        public void Test_Line_TruePositive_3()
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
        public void Test_Line_TruePositive_4()
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
        public void Test_Line_TruePositive_5()
        {
            //ax=0
            var a = new Var('a');
            const string txt = "ax=0";
            Expr expr = Text.Convert(txt);
            LineSymbol ls;
            bool result = expr.IsLine(out ls);
            Assert.True(result);
            Assert.NotNull(ls);
            Assert.True(ls.SymA.Equals(a.ToString()));
            Assert.Null(ls.SymB);
            Assert.True(ls.SymC.Equals("0"));
        }

        [Test]
        public void Test_Line_TruePositive_6()
        {
            //by=0
            var b = new Var('b');
            const string txt = "by=0";
            Expr expr = Text.Convert(txt);
            LineSymbol ls;
            bool result = expr.IsLine(out ls);
            Assert.True(result);
            Assert.NotNull(ls);
            Assert.Null(ls.SymA);
            Assert.True(ls.SymB.Equals(b.ToString()));
            Assert.True(ls.SymC.Equals("0"));
        }

        [Test]
        public void Test_Line_TruePositive_7()
        {
            //cy=0
            var c = new Var('c');
            const string txt = "cy=0";
            Expr expr = Text.Convert(txt);
            LineSymbol ls;
            bool result = expr.IsLine(out ls);
            Assert.True(result);
            Assert.NotNull(ls);
            Assert.Null(ls.SymA);
            Assert.True(ls.SymB.Equals(c.ToString()));
            Assert.True(ls.SymC.Equals("0"));
        }

        [Test]
        public void Test_Line_TruePositive_8()
        {
            const string txt = "-2.1x-3y-1=0";
            Expr expr = Text.Convert(txt);
            LineSymbol ls;
            bool result = expr.IsLine(out ls);
            Assert.True(result);
            Assert.NotNull(ls);
            Assert.True(ls.SymA.Equals("-2.1"));
            Assert.True(ls.SymB.Equals("-3"));
            Assert.True(ls.SymC.Equals("-1"));
        }

        [Test]
        public void Test_Line_TruePositive_9()
        {
            const string txt = "-2x+3.2y+1=0";
            Expr expr = Text.Convert(txt);
            LineSymbol ls;
            bool result = expr.IsLine(out ls);
            Assert.True(result);
            Assert.NotNull(ls);
            Assert.True(ls.SymA.Equals("-2"));
            Assert.True(ls.SymB.Equals("3.2"));
            Assert.True(ls.SymC.Equals("1"));
        }

        [Test]
        public void Test_Line_FalseNegative_1()
        {
            //2+1=0
            const string txt = "2+1=0";
            starPadSDK.MathExpr.Expr expr = Text.Convert(txt);
            LineSymbol ls;
            bool result = expr.IsLine(out ls);
            Assert.False(result);
        }

        [Test]
        public void Test_Line_FalseNegative_2()
        {
            //2z+1=0
            const string txt = "2z+1=0";
            starPadSDK.MathExpr.Expr expr = Text.Convert(txt);
            LineSymbol ls;
            bool result = expr.IsLine(out ls);
            Assert.False(result);
        }
    }
}

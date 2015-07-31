using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ExprSemantic;
using NUnit.Framework;
using starPadSDK.MathExpr;
using Text = starPadSDK.MathExpr.Text;
using AlgebraGeometry;
using CSharpLogic;

namespace ExprPatternMatch
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
         *  5: ax=2
         *  6: by=0
         *  7: cy=0
         *  \\TODO 8: -2.1x-3y-1=0
         *  \\TODO 9: -2x+3.2y+1=0
         *  10: ax-by+1=0
         *  11: -ax-by-9=0 
         *  12: 2x=1
         *  13: 2x+3y=1
         *  14: 3y-2x+1=0
         *  15: 2y+y-x+1=0
         *  16: y = 2x+1
         *  17: y = -x+3
         *  18: y = -ax+3
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
            Expr expr = Text.Convert(txt);
            object obj;
            bool result = expr.IsEquation(out obj);
            Assert.True(result);
            var eq = obj as Equation;
            Assert.NotNull(eq);
            LineSymbol ls;
            result = eq.IsLineEquation(out ls);
            Assert.True(result);
            Assert.NotNull(ls);
            Assert.True(ls.SymA.Equals("1"));
            Assert.True(ls.ToString().Equals("x-2=0"));
        }

        [Test]
        public void Test_Line_TruePositive_2()
        {
            const string txt = "y=1";
            Expr expr = Text.Convert(txt);
            object obj;
            LineSymbol ls;
            bool result = expr.IsEquation(out obj);
            Assert.True(result);
            var eq = obj as Equation;
            Assert.NotNull(eq);
            result = eq.IsLineEquation(out ls);
            Assert.True(result);
            Assert.Null(ls.SymA);
            Assert.True(ls.SymB.Equals("1"));
            Assert.True(ls.ToString().Equals("y-1=0"));
        }

        [Test]
        public void Test_Line_TruePositive_3()
        {
            const string txt = "ax+by+1=0";
            Expr expr = Text.Convert(txt);
            object obj;
            LineSymbol ls;
            bool result = expr.IsEquation(out obj);
            Assert.True(result);
            var eq = obj as Equation;
            Assert.NotNull(eq);
            result = eq.IsLineEquation(out ls);
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
            Expr expr = Text.Convert(txt);
            object obj;
            LineSymbol ls;
            bool result = expr.IsEquation(out obj);
            Assert.True(result);
            var eq = obj as Equation;
            Assert.NotNull(eq);
            result = eq.IsLineEquation(out ls);
            Assert.True(result);
            Assert.NotNull(ls);
            Assert.True(ls.SymA.Equals("2"));
            Assert.True(ls.SymB.Equals("1"));
            Assert.True(ls.ToString().Equals("2x+y+1=0"));
        }

        [Test]
        public void Test_Line_TruePositive_5()
        {
            //ax=2
            var a = new Var('a');
            const string txt = "ax=2";
            Expr expr = Text.Convert(txt);
            object obj;
            LineSymbol ls;
            bool result = expr.IsEquation(out obj);
            Assert.True(result);
            var eq = obj as Equation;
            Assert.NotNull(eq);
            result = eq.IsLineEquation(out ls);
            Assert.True(result);
            Assert.NotNull(ls);
            Assert.True(ls.SymA.Equals(a.ToString()));
            Assert.Null(ls.SymB);
            Assert.True(ls.SymC.Equals("-2"));
        }

        [Test]
        public void Test_Line_TruePositive_6()
        {
            //by=0
            var b = new Var('b');
            const string txt = "by=0";
            Expr expr = Text.Convert(txt);
            object obj;
            LineSymbol ls;
            bool result = expr.IsEquation(out obj);
            Assert.True(result);
            var eq = obj as Equation;
            Assert.NotNull(eq);
            result = eq.IsLineEquation(out ls);
            Assert.True(result);
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
            object obj;
            LineSymbol ls;
            bool result = expr.IsEquation(out obj);
            Assert.True(result);
            var eq = obj as Equation;
            Assert.NotNull(eq);
            result = eq.IsLineEquation(out ls);
            Assert.True(result);
            Assert.Null(ls.SymA);
            Assert.True(ls.SymB.Equals(c.ToString()));
            Assert.True(ls.SymC.Equals("0"));
        }

        [Test]
        public void Test_Line_TruePositive_8()
        {
            //TODO has issue here
            const string txt = "-2x-3y-1=0";
            Expr expr = Text.Convert(txt);
            object obj;
            LineSymbol ls;
            bool result = expr.IsEquation(out obj);
            Assert.True(result);
            var eq = obj as Equation;
            Assert.NotNull(eq);
            result = eq.IsLineEquation(out ls);
            Assert.True(result);
            Assert.True(ls.SymA.Equals("-2"));
            Assert.True(ls.SymB.Equals("-3"));
            Assert.True(ls.SymC.Equals("-1"));
        }

        [Test]
        public void Test_Line_TruePositive_9()
        {
            const string txt = "-2x+3y+1=0";
            Expr expr = Text.Convert(txt);
            object obj;
            LineSymbol ls;
            bool result = expr.IsEquation(out obj);
            Assert.True(result);
            var eq = obj as Equation;
            Assert.NotNull(eq);
            result = eq.IsLineEquation(out ls);
            Assert.True(result);
            Assert.True(ls.SymA.Equals("-2"));
            Assert.True(ls.SymB.Equals("3"));
            Assert.True(ls.SymC.Equals("1"));
        }

        [Test]
        public void Test_Line_TruePositive_10()
        {
            const string txt = "ax-by+1=0";
            Expr expr = Text.Convert(txt);
            object obj;
            LineSymbol ls;
            bool result = expr.IsEquation(out obj);
            Assert.True(result);
            var eq = obj as Equation;
            Assert.NotNull(eq);
            result = eq.IsLineEquation(out ls);
            Assert.True(result);
            Assert.True(ls.SymA.Equals("a"));
            Assert.True(ls.SymB.Equals("(-1*b)"));
            Assert.True(ls.SymC.Equals("1"));
        }

        [Test]
        public void Test_Line_TruePositive_11()
        {
            //11: -ax-by-9=0 
            const string txt = "-ax-by-9=0";
            Expr expr = Text.Convert(txt);
            object obj;
            LineSymbol ls;
            bool result = expr.IsEquation(out obj);
            Assert.True(result);
            var eq = obj as Equation;
            Assert.NotNull(eq);
            result = eq.IsLineEquation(out ls);
            Assert.True(result);
            Assert.True(ls.SymA.Equals("(-1*a)"));
            Assert.True(ls.SymB.Equals("(-1*b)"));
            Assert.True(ls.SymC.Equals("-9"));
        }

        [Test]
        public void Test_Line_TruePositive_12()
        {
            //12: 2x=1
            const string txt = "2x=1";
            Expr expr = Text.Convert(txt);
            object obj;
            LineSymbol ls;
            bool result = expr.IsEquation(out obj);
            Assert.True(result);
            var eq = obj as Equation;
            Assert.NotNull(eq);
            result = eq.IsLineEquation(out ls);
            Assert.True(result);
            Assert.True(ls.SymA.Equals("2"));
            Assert.Null(ls.SymB);
            Assert.True(ls.SymC.Equals("-1"));
        }

        [Test]
        public void Test_Line_TruePositive_13()
        {
            //13: 2x+3y=1
            const string txt = "2x+3y=1";
            Expr expr = Text.Convert(txt);
            object obj;
            LineSymbol ls;
            bool result = expr.IsEquation(out obj);
            Assert.True(result);
            var eq = obj as Equation;
            Assert.NotNull(eq);
            result = eq.IsLineEquation(out ls);
            Assert.True(result);
            Assert.True(ls.SymA.Equals("2"));
            Assert.True(ls.SymB.Equals("3"));
            Assert.True(ls.SymC.Equals("-1"));
        }

        [Test]
        public void Test_Line_TruePositive_14()
        {
            //14: 3y-2x+1=0
            const string txt = "3y-2x+1=0";
            Expr expr = Text.Convert(txt);
            object obj;
            LineSymbol ls;
            bool result = expr.IsEquation(out obj);
            Assert.True(result);
            var eq = obj as Equation;
            Assert.NotNull(eq);
            result = eq.IsLineEquation(out ls);
            Assert.True(result);
            Assert.True(ls.SymA.Equals("-2"));
            Assert.True(ls.SymB.Equals("3"));
            Assert.True(ls.SymC.Equals("1"));
            Assert.True(ls.ToString().Equals("-2x+3y+1=0"));
        }

        [Test]
        public void Test_Line_TruePositive_15()
        {
            // 15: 2y+y-x+1=0
            const string txt = "2y+y-x+1=0";
            Expr expr = Text.Convert(txt);
            object obj;
            LineSymbol ls;
            bool result = expr.IsEquation(out obj);
            Assert.True(result);
            var eq = obj as Equation;
            Assert.NotNull(eq);
            result = eq.IsLineEquation(out ls);
            Assert.True(result);
            Assert.True(ls.SymA.Equals("-1"));
            Assert.True(ls.SymB.Equals("3"));
            Assert.True(ls.SymC.Equals("1"));
        }

        [Test]
        public void Test_Line_TruePositive_16()
        {
            //16: y = 2x+1
            const string txt = "y=2x+1";
            Expr expr = Text.Convert(txt);
            object obj;
            LineSymbol ls;
            bool result = expr.IsEquation(out obj);
            Assert.True(result);
            var eq = obj as Equation;
            Assert.NotNull(eq);
            result = eq.IsLineEquation(out ls);
            Assert.True(result);
            Assert.True(ls.SymSlope.Equals("2"));
            Assert.True(ls.SymIntercept.Equals("1"));
            Assert.True(ls.ToString().Equals("y=2x+1"));
/*
            Assert.True(ls.SymA.Equals("-1"));
            Assert.True(ls.SymB.Equals("3"));
            Assert.True(ls.SymC.Equals("1"));
*/
        }

        [Test]
        public void Test_Line_TruePositive_17()
        {
            //17: y = -x+3
            const string txt = "y=-x+3";
            Expr expr = Text.Convert(txt);
            object obj;
            LineSymbol ls;
            bool result = expr.IsEquation(out obj);
            Assert.True(result);
            var eq = obj as Equation;
            Assert.NotNull(eq);
            result = eq.IsLineEquation(out ls);
            Assert.True(result);
            Assert.True(ls.SymSlope.Equals("-1"));
            Assert.True(ls.SymIntercept.Equals("3"));
            Assert.True(ls.ToString().Equals("y=-x+3"));
        }

        [Test]
        public void Test_Line_TruePositive_18()
        {
            //18: y = -ax-3
            const string txt = "y=-ax-3";
            Expr expr = Text.Convert(txt);
            object obj;
            LineSymbol ls;
            bool result = expr.IsEquation(out obj);
            Assert.True(result);
            var eq = obj as Equation;
            Assert.NotNull(eq);
            result = eq.IsLineEquation(out ls);
            Assert.True(result);
            Assert.True(ls.SymSlope.Equals("(-1*a)"));
            Assert.True(ls.SymIntercept.Equals("-3"));
            Assert.True(ls.ToString().Equals("y=(-1*a)x-3"));
        }

        [Test]
        public void Test_Line_FalseNegative_1()
        {
            //2+1=0
            const string txt = "2+1=0";
            starPadSDK.MathExpr.Expr expr = Text.Convert(txt);
            object obj;
            LineSymbol ls;
            bool result = expr.IsEquation(out obj);
            Assert.True(result);
            var eq = obj as Equation;
            Assert.NotNull(eq);
            result = eq.IsLineEquation(out ls);
            Assert.False(result);
        }

        [Test]
        public void Test_Line_FalseNegative_2()
        {
            //2z+1=0
            const string txt = "2z+1=0";
            starPadSDK.MathExpr.Expr expr = Text.Convert(txt);
            object obj;
            LineSymbol ls;
            bool result = expr.IsEquation(out obj);
            Assert.True(result);
            var eq = obj as Equation;
            Assert.NotNull(eq);
            result = eq.IsLineEquation(out ls);
            Assert.False(result);
        }
    }
}

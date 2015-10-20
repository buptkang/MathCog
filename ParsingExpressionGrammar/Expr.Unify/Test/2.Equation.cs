using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using starPadSDK.MathExpr;
using Text = starPadSDK.MathExpr.Text;

namespace ExprPatternMatch
{
    [TestFixture]
    public class TestEquation
    {
        [Test]
        public void Test_1()
        {
            string txt = "2=1-1";
            Expr expr = Text.Convert(txt);
            object obj;
            bool result = expr.IsEquation(out obj);
            Assert.True(result);
        }

        [Test]
        public void Test_2()
        {
            string txt = "1+1+1";
            Expr expr = Text.Convert(txt);
            object obj;
            bool result = expr.IsEquation(out obj);
            Assert.False(result);
        }

        [Test]
        public void Test_3()
        {
            string txt = "d＾2 = 16+9";
            Expr expr = Text.Convert(txt);
            object obj;
            bool result = expr.IsEquation(out obj);
            Assert.True(result);
        }

        [Test]
        public void Test_4()
        {
            string txt = "m1*m2=-1";
            Expr expr = Text.Convert(txt);

            object obj;
            bool result = expr.IsEquationLabel(out obj);
            Assert.True(result);
        }

        [Test]
        public void Test_5()
        {
            var expr0 = new CompositeExpr(WellKnownSym.times,
                new Expr[] { new LetterSym('m'), new IntegerNumber(1) });

            var expr11 = new CompositeExpr(WellKnownSym.divide,
                new Expr[] { 0.5 });

            var expr1 = new CompositeExpr(WellKnownSym.times,
                new Expr[] { -1, expr11 });
            var expr2 = new CompositeExpr(WellKnownSym.equals,
                new Expr[] { expr0, expr1 });

            object obj;
            bool result = expr0.IsExpression(out obj);
            Assert.True(result);
            
            result = expr2.IsEquationLabel(out obj);
            Assert.True(result);
        }
    }
}

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
    }
}

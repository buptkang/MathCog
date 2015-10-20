using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;
using starPadSDK.MathExpr;
using Text = starPadSDK.MathExpr.Text;

namespace ExprPatternMatch
{
    [TestFixture]
    public class TestLabel
    {
        [Test]
        public void Test1()
        {
            var expr0 = new CompositeExpr(WellKnownSym.times,
              new Expr[] { new LetterSym('m'), new IntegerNumber(1) });

            object output;
            bool result = expr0.IsLabel(out output);
            Assert.True(result);
        }
    }
}

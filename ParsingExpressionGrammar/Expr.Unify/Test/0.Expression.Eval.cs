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
    public class TestExpressionEval
    {
        [Test]
        public void Test_Label_1()
        {
            ///"A", "c", "XT","c12","c_1" 

            string txt = "A";
            Expr expr = Text.Convert(txt);
            object obj;
            bool result = expr.IsLabel(out obj);
            Assert.True(result);
            result = expr.IsExpression(out obj);
            Assert.True(result);

            txt = "c";
            expr = Text.Convert(txt);
            result = expr.IsLabel(out obj);
            Assert.True(result);
            result = expr.IsExpression(out obj);
            Assert.True(result);

            txt = "XT";
            expr = Text.Convert(txt);
            result = expr.IsLabel(out obj);
            Assert.True(result);
            result = expr.IsExpression(out obj);
            Assert.True(result);

            txt = "c12";
            expr = Text.Convert(txt);
            result = expr.IsLabel(out obj);
            Assert.True(result);
            result = expr.IsExpression(out obj);
            Assert.True(result);

            txt = "2m1";
            expr = Text.Convert(txt);
            result = expr.IsLabel(out obj);
            Assert.True(result);
            result = expr.IsExpression(out obj);
            Assert.True(result);
        }

        [Test]
        public void Test_Label_2()
        {
            //-12x, -2x, 2x, x, 2y,2xy, -3y,2A, 12mm, 2XY, -2Y
            string txt = "-12x";
            Expr expr = Text.Convert(txt);
            object obj;
            bool result = expr.IsLabel(out obj);
            Assert.False(result);
            result = expr.IsExpression(out obj);
            Assert.True(result);

            txt = "-2x";
            expr = Text.Convert(txt);
            result = expr.IsLabel(out obj);
            Assert.False(result);
            result = expr.IsExpression(out obj);
            Assert.True(result);

            txt = "2x";
            expr = Text.Convert(txt);
            result = expr.IsLabel(out obj);
            Assert.True(result);
            result = expr.IsExpression(out obj);
            Assert.True(result);

            txt = "x";
            expr = Text.Convert(txt);
            result = expr.IsLabel(out obj);
            Assert.True(result);
            result = expr.IsExpression(out obj);
            Assert.True(result);

            txt = "2y";
            expr = Text.Convert(txt);
            result = expr.IsLabel(out obj);
            Assert.True(result);
            result = expr.IsExpression(out obj);
            Assert.True(result);

            txt = "2xy";
            expr = Text.Convert(txt);
            result = expr.IsLabel(out obj);
            Assert.True(result);
            result = expr.IsExpression(out obj);
            Assert.True(result);

            txt = "-3y";
            expr = Text.Convert(txt);
            result = expr.IsLabel(out obj);
            Assert.False(result);
            result = expr.IsExpression(out obj);
            Assert.True(result);

            txt = "2A";
            expr = Text.Convert(txt);
            result = expr.IsLabel(out obj);
            Assert.True(result);
            result = expr.IsExpression(out obj);
            Assert.True(result);

            txt = "12mm";
            expr = Text.Convert(txt);
            result = expr.IsLabel(out obj);
            Assert.True(result);
            result = expr.IsExpression(out obj);
            Assert.True(result);

            txt = "2XY";
            expr = Text.Convert(txt);
            result = expr.IsLabel(out obj);
            Assert.True(result);
            result = expr.IsExpression(out obj);
            Assert.True(result);

            txt = "-2Y";
            expr = Text.Convert(txt);
            result = expr.IsLabel(out obj);
            Assert.False(result);
            result = expr.IsExpression(out obj);
            Assert.True(result);
        }

        [Test]
        public void Test_Label_3()
        {
            //ax, 2ax, -ax, -2ax, axy
            string txt = "ax";        
            Expr expr = Text.Convert(txt);
            object obj;
            bool result = expr.IsLabel(out obj);
            Assert.True(result);
            result = expr.IsExpression(out obj);
            Assert.True(result);

            txt = "2ax";
            expr = Text.Convert(txt);
            result = expr.IsLabel(out obj);
            Assert.True(result);
            result = expr.IsExpression(out obj);
            Assert.True(result);

            txt = "-ax";
            expr = Text.Convert(txt);
            result = expr.IsLabel(out obj);
            Assert.False(result);
            result = expr.IsExpression(out obj);
            Assert.True(result);

            txt = "-2ax";
            expr = Text.Convert(txt);
            result = expr.IsLabel(out obj);
            Assert.False(result);
            result = expr.IsExpression(out obj);
            Assert.True(result);

            txt = "axy";
            expr = Text.Convert(txt);
            result = expr.IsLabel(out obj);
            Assert.True(result);
            result = expr.IsExpression(out obj);
            Assert.True(result);
        }

        [Test]
        public void Test_Label_4()
        {
            //TODO decimal Pattern Match
            //2.1x
            string txt = "2.1x";

            Match sss = Regex.Match(txt, @"([a-z]+)|([0-9]+)");
            foreach (Group gp in sss.Groups)
            {
                Console.Write(gp.Value);
            }

            Expr expr = Text.Convert(txt);
            object obj;
            bool result = expr.IsLabel(out obj);
            Assert.True(result);
            result = expr.IsExpression(out obj);
            Assert.True(result);
        }
    
    }
}

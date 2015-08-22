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

namespace ExprPatternMatch
{
    [TestFixture]
    public class TestLineRelation
    {
        [Test]
        public void Test0()
        {
            const string txt3 = "AB";
            Expr expr3 = Text.Convert(txt3);
            object result = ExprVisitor.Instance.Match(expr3);

            var str = result as String;
            Assert.NotNull(str);
/*            var dict = result as Dictionary<PatternEnum, object>;
            Assert.NotNull(dict);
            Assert.True(dict.Count==2);
            Assert.True(dict.ContainsKey(PatternEnum.Label));
            Assert.True(dict.ContainsKey(PatternEnum.Expression));
 */
        }
    }
}

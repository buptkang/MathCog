using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using CSharpLogic;
using NUnit.Framework;
using Text = starPadSDK.MathExpr.Text;
using ExprSemantic;

namespace ExprPatternMatch
{
    [TestFixture]
    public class TestQueryProperty
    {
        [Test]
        public void Test1()
        {
            //x=
            //x+1=
            const string txt1 = "x=";
            starPadSDK.MathExpr.Expr expr = Text.Convert(txt1);
            
            object label;
            bool result = expr.IsQuery(out label);
            Assert.True(result);
            var variable = new Var('x');
            var kv = (KeyValuePair<string, object>)label;
            Assert.True(kv.Key.Equals("Label"));
            Assert.True(kv.Value.ToString().Equals(variable.ToString()));

            const string txt2 = "x+1=";
            expr = Text.Convert(txt2);
            result = expr.IsQuery(out label);
            Assert.True(result);
            kv = (KeyValuePair<string, object>)label;
            Assert.True(kv.Key.Equals("Term"));

            Assert.IsInstanceOf(typeof(Term), kv.Value);
            var term = kv.Value as Term;
            Assert.NotNull(term);
            var tuple = term.Args as Tuple<object, object>;
            Assert.NotNull(tuple);
            Assert.True(tuple.Item2.Equals(1));
        }
    }
}

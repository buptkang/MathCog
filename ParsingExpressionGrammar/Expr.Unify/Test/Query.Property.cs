using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using CSharpLogic;
using NUnit.Framework;
using Text = starPadSDK.MathExpr.Text;
using System.Linq.Expressions;

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
            
            object obj;
            bool result = expr.IsQuery(out obj);
            Assert.True(result);
            var query = obj as Query;
            Assert.NotNull(query);
            Assert.True(query.Constraint1.ToString().Equals("x"));

            const string txt2 = "x+1=";
            expr = Text.Convert(txt2);
            result = expr.IsQuery(out obj);
            Assert.True(result);

            var variable = new Var('x');
            var term = new Term(Expression.Add, new List<object>() { variable, 1 });

            query = obj as Query;
            Assert.NotNull(query);
            Assert.True(query.Constraint1.Equals(term));
        }
    }
}
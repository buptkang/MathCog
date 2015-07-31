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
            var equation = obj as Equation;
            Assert.NotNull(equation);
            Assert.Null(equation.Rhs);
            var term = equation.Lhs as Term;
            Assert.NotNull(term);
            var list = term.Args as List<object>;
            Assert.NotNull(list);
            Assert.True(list[1].Equals(1));
        }
    }
}

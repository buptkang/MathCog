using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using NUnit.Framework;

namespace CSharpLogic
{
    [TestFixture]
    public partial class QueryTest
    {
        [Test]
        public void Goal_Gen_1()
        {
            //2=2
            var eq = new Equation(2, 2);
            EqGoal eqGoal;
            bool result = eq.IsEqGoal(out eqGoal);
            Assert.False(result);

            //3=4
            eq = new Equation(3,4);
            result = eq.IsEqGoal(out eqGoal);
            Assert.False(result);

            //3=5-2
            var term = new Term(Expression.Add, new List<object>() {5, -2});
            eq = new Equation(3, term);
            result = eq.IsEqGoal(out eqGoal);
            Assert.False(result);

            //x = x
            var variable = new Var('x');
            eq = new Equation(variable, variable);
            result = eq.IsEqGoal(out eqGoal);
            Assert.False(result);

            //x = 2x-x
            term      = new Term(Expression.Multiply, new List<object>(){2, variable});
            var term0 = new Term(Expression.Multiply, new List<object>() { -1, variable });
            var term1 = new Term(Expression.Add, new List<object>() {term, term0});
            eq = new Equation(variable, term1);
            result = eq.IsEqGoal(out eqGoal);
            Assert.False(result);
        }
    }
}

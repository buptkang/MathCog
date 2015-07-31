using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using NUnit.Framework;

namespace CSharpLogic
{
    [TestFixture]
    public class GoalGeneration
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

        [Test]
        public void Goal_Gen_2()
        {
            //x = 3
            var x = new Var('a');
            var eq = new Equation(x, 3);
            EqGoal eqGoal;
            bool result = eq.IsEqGoal(out eqGoal);
            Assert.True(result);

            //x = y
            var y = new Var('y');
            eq = new Equation(x, y);
            result = eq.IsEqGoal(out eqGoal);
            Assert.True(result);
        }

        [Test]
        public void Goal_Gen_3()
        {
            //x = 2+3
            var x = new Var('x');
            var term = new Term(Expression.Add, new List<object>() {2, 3});
            var eq = new Equation(x, term);
            EqGoal eqGoal;
            bool result = eq.IsEqGoal(out eqGoal);
            Assert.True(eqGoal.Traces.Count == 1);
            Assert.True(eqGoal.Lhs.Equals(x));
            Assert.True(eqGoal.Rhs.Equals(5));
            Assert.True(result);
        }

        [Test]
        public void Goal_Gen_4()
        {
            // 3 = x
            // 1+2=x
            // 1+x=3
            // 3*2=x
            // 2*x=6
        }

        [Test]
        public void Goal_Gen_5()
        {
            //x + 1 = 2
            //x + 1 - 3 = 2
            //x + 1 - 5　＝　2+1
            //x = 2+x+1            
        }

        public void Goal_Gen_6()
        {
            // a - b = 2
            
        }
    }
}

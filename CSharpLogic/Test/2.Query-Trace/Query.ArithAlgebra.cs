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
            var term = new Term(Expression.Add, new List<object>() { 2, 3 });
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

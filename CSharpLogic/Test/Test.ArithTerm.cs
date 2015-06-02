using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using NUnit.Framework;
using System.Linq.Expressions;

namespace CSharpLogic.Test
{
    [TestFixture]
    public class TestArithTerms
    {
        [Test]
        public void TestAdd0()
        {
            var rhs = new Term(Expression.Add, new Tuple<object, object>(1, 2));
            var lhs = new Var('x');
            var goal = new EqGoal(lhs, rhs);


            
            


//            Assert.False(goal.IsValid);

        }



        [Test]
        public void TestAdd1()
        {
            var x = new Var("x");
            var term = new Term(Expression.Add, new Tuple<object, object>(1, x));
            var goal = new EqGoal(term, 4.0);
/*          
            object result = LogicSharp.Run(x, goal);
            Assert.NotNull(result);
            Assert.True(result.Equals(3.0));
*/

            //var goal = LogicSharp.Add(1, 2, 3) as EqGoal;

        }



    }
}
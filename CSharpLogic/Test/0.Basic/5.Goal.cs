using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace CSharpLogic.Test
{
    [TestFixture]
    public class TestGoal
    {
        [Test]
        public void Test_Goal_1()
        {
            //a = 2
            var x = new Var('a');
            var eqGoal = new EqGoal(x, 2);
            var substitutions = new Dictionary<object, object>();
            var result = eqGoal.Unify(substitutions);
            Assert.True(result);
            Assert.True(eqGoal.Traces.Count == 0);
            Assert.True(substitutions.Count == 1);
            Assert.True(substitutions.ContainsKey(x));
            Assert.True(substitutions[x].Equals(2));
            Assert.True(eqGoal.EarlySafe());
        }

        [Test]
        public void Test_Goal_Unify_1()
        {
            //S = a*b;
            var s = new Var('S');
            var a = new Var('a');
            var b = new Var('b');
            var term = new Term(Expression.Multiply, new List<object>() {a, b});
            var eqGoal = new EqGoal(s, term);
            var substitutions = new Dictionary<object, object>();
            bool result = eqGoal.Unify(substitutions);
            Assert.True(result);
            Assert.True(substitutions.Count == 1);
            Assert.True(substitutions.ContainsKey(s));

            Assert.True(Var.ContainsVar(substitutions[s]));
        }

        #region Reification

        [Test]
        public void test_reify_object2()
        {
            var foo = new DyLogicObject();
            foo.Properties.Add("y", 1);
            var goal = new EqGoal(new Var("x"), 2);
            foo.Reify(goal);

            Assert.True(foo.Properties.Count == 2);
        }

        #endregion 
    }
}

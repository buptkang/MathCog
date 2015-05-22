using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace CSharpLogic.Test
{
    [TestFixture]
    public class TestGoal
    {
        [Test]
        public void TestEqGoal_EarlySafe()
        {
//            x, y = var('x'), var('y')
//            assert earlysafe((eq, 2, 2))
//            assert earlysafe((eq, 2, 3))
//            assert earlysafe((membero, x, (1,2,3)))
//            assert not earlysafe((membero, x, y))

            var x = new Var('x');
            var y = new Var('y');
            var eqGoal = new EqGoal(2, 2);
            Assert.True(eqGoal.EarlySafe());
            eqGoal = new EqGoal(2,x);
            Assert.True(eqGoal.EarlySafe());
            eqGoal = new EqGoal(y, x);
            Assert.False(eqGoal.EarlySafe());
        }

        [Test]
        public void TestEqGoal()
        {
            /*
             * x = var('x')
              assert tuple(eq(x, 2)({})) == ({x: 2},)
              assert tuple(eq(x, 2)({x: 3})) == ()
            */
            var x = new Var('x');
            var goal = new EqGoal(x, 2);            
            var substitutions = new Dictionary<object, object>();

            bool result = goal.Unify(substitutions);
            Assert.True(result);
            Assert.True(substitutions.Count == 1);
            Assert.True(substitutions.ContainsKey(x));
            Assert.True(substitutions[x].Equals(2));

            substitutions = new Dictionary<object, object>();
            substitutions.Add(x, 3);
            result = goal.Unify(substitutions);
            Assert.False(result);
        }

        [Test]
        public void TestMemberof()
        {
            /*
            * x = var('x')
            assert set(run(5, x, membero(x, (1,2,3)),
                membero(x, (2,3,4)))) == set((2,3))
            assert run(5, x, membero(2, (1, x, 3))) == (2,)
            */
        }

        [Test]
        public void test_reify_object2()
        {
            var foo = new DyLogicObject();
            foo.Properties.Add("y", 1);
            var goal = new EqGoal(new Var("x"), 2);
            foo.Reify(goal);

            Assert.True(foo.Properties.Count == 2);
        }
    }
}

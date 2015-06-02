using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace CSharpLogic.Test
{
    [TestFixture]
    public class TestArith
    {
        [Test]
        public void Test_Add()
        {
            var goal = LogicSharp.Add(1, 2, 3) as EqGoal;
            Assert.NotNull(goal);
            var dict = new Dictionary<object, object>();
            bool result = goal.Unify(dict);
            Assert.True(result);
            Assert.True(dict.Count == 0);
            Assert.True(goal.Traces.Count == 1);

            goal = LogicSharp.Add(1, 2, 4) as EqGoal;
            Assert.NotNull(goal);
            dict = new Dictionary<object, object>();
            result = goal.Unify(dict);
            Assert.False(result);

            //assert results(add(1, 2, x)) == [{x: 3}]
            var variable = new Var('x');
            goal = LogicSharp.Add(1, 2, variable) as EqGoal;
            Assert.NotNull(goal);
            dict = new Dictionary<object, object>();
            result = goal.Unify(dict);
            Assert.True(result);
            Assert.True(dict.Count == 1);
            Assert.True(dict.ContainsKey(variable));
            Assert.True(dict[variable].Equals(3));

            //assert results(add(1, x, 3)) == [{x: 2}]
            goal = LogicSharp.Add(1, variable, 3) as EqGoal;
            Assert.NotNull(goal);
            dict = new Dictionary<object, object>();
            result = goal.Unify(dict);
            Assert.True(result);
            Assert.True(dict.Count == 1);
            Assert.True(dict.ContainsKey(variable));
            Assert.True(dict[variable].Equals(2));

            //assert results(add(x, 2, 3)) == [{x: 1}]
            goal = LogicSharp.Add(variable, 2, 3) as EqGoal;
            Assert.NotNull(goal);
            dict = new Dictionary<object, object>();
            result = goal.Unify(dict);
            Assert.True(result);
            Assert.True(dict.Count == 1);
            Assert.True(dict.ContainsKey(variable));
            Assert.True(dict[variable].Equals(1));
        }

        [Test]
        public void Test_Sub()
        {
            var goal = LogicSharp.Sub(3, 2, 1) as EqGoal;
            Assert.NotNull(goal);
            var dict = new Dictionary<object, object>();
            bool result = goal.Unify(dict);
            Assert.True(result);
            Assert.True(dict.Count == 0);
           
            //assert not results(sub(4, 2, 1))
            goal = LogicSharp.Sub(4, 2, 1) as EqGoal;
            Assert.NotNull(goal);
            dict = new Dictionary<object, object>();
            result = goal.Unify(dict);
            Assert.False(result);

            //assert results(sub(3, 2, x)) == [{x: 1}]
            var variable = new Var('x');
            goal = LogicSharp.Sub(3, 2, variable) as EqGoal;
            Assert.NotNull(goal);
            dict = new Dictionary<object, object>();
            result = goal.Unify(dict);
            Assert.True(result);
            Assert.True(dict.Count == 1);
            Assert.True(dict.ContainsKey(variable));
            Assert.True(dict[variable].Equals(1));

            //assert results(sub(3, x, 1)) == [{x: 2}]
            variable = new Var('x');
            goal = LogicSharp.Sub(3, variable, 1) as EqGoal;
            Assert.NotNull(goal);
            dict = new Dictionary<object, object>();
            result = goal.Unify(dict);
            Assert.True(result);
            Assert.True(dict.Count == 1);
            Assert.True(dict.ContainsKey(variable));
            Assert.True(dict[variable].Equals(2));

            //assert results(sub(x, 2, 1)) == [{x: 3}]
            variable = new Var('x');
            goal = LogicSharp.Sub(variable, 2, 1) as EqGoal;
            Assert.NotNull(goal);
            dict = new Dictionary<object, object>();
            result = goal.Unify(dict);
            Assert.True(result);
            Assert.True(dict.Count == 1);
            Assert.True(dict.ContainsKey(variable));
            Assert.True(dict[variable].Equals(3));
        }

        [Test]
        public void Test_Mul()
        {
            var goal = LogicSharp.Mul(3, 2, 6) as EqGoal;
            Assert.NotNull(goal);
            var dict = new Dictionary<object, object>();
            bool result = goal.Unify(dict);
            Assert.True(result);
            Assert.True(dict.Count == 0);

            goal = LogicSharp.Mul(3, 2, 7) as EqGoal;
            Assert.NotNull(goal);
            dict = new Dictionary<object, object>();
            result = goal.Unify(dict);
            Assert.False(result);

            //assert results(mul(2, 3, x)) == [{x: 6}]
            var variable = new Var('x');
            goal = LogicSharp.Mul(2, 3, variable) as EqGoal;
            Assert.NotNull(goal);
            dict = new Dictionary<object, object>();
            result = goal.Unify(dict);
            Assert.True(result);
            Assert.True(dict.Count == 1);
            Assert.True(dict.ContainsKey(variable));
            Assert.True(dict[variable].Equals(6));

            // assert results(mul(2, x, 6)) == [{x: 3}]
            variable = new Var('x');
            goal = LogicSharp.Mul(2, variable, 6) as EqGoal;
            Assert.NotNull(goal);
            dict = new Dictionary<object, object>();
            result = goal.Unify(dict);
            Assert.True(result);
            Assert.True(dict.Count == 1);
            Assert.True(dict.ContainsKey(variable));
            Assert.True(dict[variable].Equals(3));

            // assert results(mul(x, 3, 6)) == [{x: 2}]
            variable = new Var('x');
            goal = LogicSharp.Mul(variable,3, 6) as EqGoal;
            Assert.NotNull(goal);
            dict = new Dictionary<object, object>();
            result = goal.Unify(dict);
            Assert.True(result);
            Assert.True(dict.Count == 1);
            Assert.True(dict.ContainsKey(variable));
            Assert.True(dict[variable].Equals(2));
        }

        [Test]
        public void Test_Complex()
        {
            var variable1 = new Var('x');
            var variable2 = new Var('y');

            // y - x = 2
            var goal1 = LogicSharp.Sub(variable2, variable1, 2) as EqGoal;
            // 1 + x = 3
            var goal2 = LogicSharp.Add(1, variable1, 3) as EqGoal;
            var lst = new List<Goal>()
            {
                goal1, goal2
            };

            var tuple = Tuple.Create(variable1, variable2);
            var result = LogicSharp.Run(tuple, lst) as Dictionary<object, object>;
            Assert.NotNull(result);
            Assert.True(result.Count == 2);
            Assert.True(result.ContainsKey(variable1));
            Assert.True(result.ContainsKey(variable2));
            Assert.True(result[variable1].Equals(2));
            Assert.True(result[variable2].Equals(4));
        }    
    }
}

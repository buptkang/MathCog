using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace CSharpLogic.Test
{
    [TestFixture]
    public class TestCore
    {
        [Test]
        public void TestWalk()
        {
            /*
             * s = {1: 2, 2: 3}
            assert walk(2, s) == 3
            assert walk(1, s) == 3
            assert walk(4, s) == 4 
            Utils.transitive_get() 
             */

            var dict = new Dictionary<object, object>();
            dict.Add(1,2);
            dict.Add(2,3);

            object obj = LogicSharp.transitive_get(2, dict);
            Assert.True(3.Equals(obj));

            obj = LogicSharp.transitive_get(1, dict);
            Assert.True(3.Equals(obj));

            obj = LogicSharp.transitive_get(4, dict);
            Assert.True(4.Equals(obj));
        }

        [Test]
        public void TestDeepWalk()
        {
            var dict = new Dictionary<object, object>();
            var z = new Var('z');
            var y = new Var('y');
            var x = new Var('x');
            dict.Add(z, 6);
            dict.Add(y, 5);
            var tuple = new Tuple<object, object>(y, z);
            dict.Add(x, tuple);

            object obj = LogicSharp.transitive_get(x, dict);
            Assert.True(obj.Equals(tuple));

            obj = LogicSharp.deep_transitive_get(x, dict);
            Assert.IsInstanceOf(typeof(Tuple<object,object>), obj);
            var result = obj as Tuple<object, object>;
            Assert.IsNotNull(result);
            Assert.True(5.Equals(result.Item1));
            Assert.True(6.Equals(result.Item2));
            //    Transitive get that propagates within tuples
            //    >>> d = {1: (2, 3), 2: 12, 3: 13}
            //    >>> transitive_get(1, d)
            //    (2, 3)
            //    >>> deep_transitive_get(1, d)
            //    (12, 13)

            dict = new Dictionary<object, object>();
            tuple = new Tuple<object, object>(2,3);
            dict.Add(1, tuple);
            dict.Add(2,12);
            dict.Add(3,13);
            obj = LogicSharp.deep_transitive_get(1, dict);
            Assert.IsInstanceOf(typeof(Tuple<object,object>), obj);
            result = obj as Tuple<object, object>;
            Assert.IsNotNull(result);
            Assert.True(12.Equals(result.Item1));
            Assert.True(13.Equals(result.Item2));
        }

        [Test]
        public void TestEq()
        {
            /*
             * x = var('x')
            assert tuple(eq(x, 2)({})) == ({x: 2},)
            assert tuple(eq(x, 2)({x: 3})) == ()
             */
            var eq = LogicSharp.Goal_Equal();

            var variable = new Var('x');
            var dict = new Dictionary<object, object>();

            object obj = eq(variable, 2)(dict);
            Assert.IsInstanceOf(typeof(bool), obj);
            Assert.True((bool)obj);
            Assert.True(dict.Count == 1);
            Assert.True(dict.ContainsKey(variable));
            Assert.True(dict[variable].Equals(2));

            var dict2 = new Dictionary<object, object>();
            dict2.Add(variable, 3);
            obj = eq(variable, 2)(dict2);
            Assert.False((bool)obj); 
        }

        [Test]
        public void TestLogicAny()
        {
            var x = new Var('x');
            //Func<>
/*            
            assert len(tuple(lany(eq(x, 2), eq(x, 3))({}))) == 2
            assert len(tuple(lany((eq, x, 2), (eq, x, 3))({}))) == 2
 */
            var eq = LogicSharp.Goal_Equal();
            Func<Dictionary<object, object>, bool> goal1 = eq(x, 2);
            Func<Dictionary<object, object>, bool> goal2 = eq(x, 2);
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
    }
}

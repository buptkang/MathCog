using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace CSharpLogic.Test
{
    [TestFixture]
    public class TestUnification
    {
        [Test]
        public void TestReify()
        {
            var x = new Var();
            var y = new Var(); 
            var z = new Var();
            var dict = new Dictionary<Var, object>();
            dict.Add(x, 1);
            dict.Add(y, 2);
            dict.Add(z, new Tuple<object, object>(x, y));
           
            Assert.True(Unifier.Reify(x, dict).Equals(1));
            Assert.True(Unifier.Reify(10, dict).Equals(10));

            var tuple = new Tuple<object, object>(1, y);
            Assert.True(Unifier.Reify(tuple, dict)
                        .Equals(new Tuple<object, object>(1,2)));

            // assert reify((1, (x, (y, 2))), s) == (1, (1, (2, 2)))

            var tuple1 = new Tuple<object, object>(y, 2);
            var tuple2 = new Tuple<object, object>(x, tuple1);
            var tuple3 = new Tuple<object, object>(1, tuple2);

            var obj = Unifier.Reify(tuple3, dict);

            var tuple10 = new Tuple<object, object>(2, 2);
            var tuple20 = new Tuple<object, object>(1, tuple10);
            var tuple30 = new Tuple<object, object>(1, tuple20);
            Assert.True(obj.Equals(tuple30));

            // assert reify(z, s) == (1, 2)
            Assert.True(Unifier.Reify(z, dict).Equals(new Tuple<object, object>(1, 2)));
        }
    
        [Test]
        public void TestReify_Dict()
        {
            var x = new Var();
            var y = new Var();
/*           
 *          s = {x: 2, y: 4}
            e = {1: x, 3: {5: y}}
            assert reify(e, s) == {1: 2, 3: {5: 4}}
 */
            var dict = new Dictionary<Var, object>();
            dict.Add(x, 2);
            dict.Add(y, 4);

            var testDict = new Dictionary<object, object>();
            testDict.Add(1, x);
            var embedDict = new Dictionary<object, object>();
            embedDict.Add(5,y);
            testDict.Add(3, embedDict);

            var mockDict = new Dictionary<object, object>();
            mockDict.Add(1, 2);
            var mockEmbedDict = new Dictionary<object, object>();
            mockEmbedDict.Add(5,4);
            mockDict.Add(3, mockEmbedDict);

            var obj = Unifier.Reify(testDict, dict) as Dictionary<object, object>;
            var test1 = obj[1];

            Assert.True(test1.Equals(mockDict[1]));

            var test2 = obj[3] as Dictionary<object, object>;
            var mocktest2 = mockDict[3] as Dictionary<object, object>;
            Assert.True(test2[5].Equals(mocktest2[5]));

        }

        [Test]
        public void TestReify_List()
        {
/*          x, y = var(), var()
            s = {x: 2, y: 4}
            e = [1, [x, 3], y]
            assert reify(e, s) == [1, [2, 3], 4]   */

            var x = new Var();
            var y = new Var();
            var dict = new Dictionary<Var, object>();
            dict.Add(x, 2);
            dict.Add(y, 4);

            var lst = new List<object>();
            lst.Add(1);
            lst.Add(new List<object>(){x,3});
            lst.Add(y);

            var obj = Unifier.Reify(lst, dict) as List<object>;
            Assert.IsNotNull(obj);

            var mockList = new List<object>()
            {
                1,
                new List<object>() {2, 3},
                4
            };
            var test1 = obj[1] as List<object>;
            Assert.IsNotNull(test1);
            Assert.True(test1[0].Equals(2));
            var test2 = obj[2];
            Assert.IsNotNull(test2);
            Assert.True(test2.Equals(4));
        }

        [Test]
        public void TestReify_Complex()
        {
            /*            
             * x, y = var(), var()
            s = {x: 2, y: 4}
            e = {1: [x], 3: (y, 5)}

            assert reify(e, s) == {1: [2], 3: (4, 5)}
             */
            var x = new Var();
            var y = new Var();
            var dict = new Dictionary<Var, object>();
            dict.Add(x, 2);
            dict.Add(y, 4);

            var input = new Dictionary<object, object>();
            input.Add(1, new List<object>(){x});
            input.Add(3, new Tuple<object,object>(y, 5));

            var obj = Unifier.Reify(input, dict) as Dictionary<object,object>;
            Assert.IsNotNull(obj);
            var test1 = obj[1] as List<object>;
            Assert.IsNotNull(test1);
            Assert.True(2.Equals(test1[0]));

            var test2 = obj[3] as Tuple<object, object>;
            Assert.IsNotNull(test2);
            Assert.True(4.Equals(test2.Item1));
        }

        [Test]
        public void TestUnify()
        {
/*
            assert unify(1, 1, {}) == {}
            assert unify(1, 2, {}) == False
            assert unify(var(1), 2, {}) == {var(1): 2}
            assert unify(2, var(1), {}) == {var(1): 2}
*/
            var dict = new Dictionary<object, object>();
            bool result = Unifier.Unify(1, 1, dict);
            Assert.True(result);
            dict = new Dictionary<object, object>();
            result = Unifier.Unify(1, 2, dict);
            Assert.False(result);

            dict = new Dictionary<object, object>();
            var variable = new Var(1);
            result = Unifier.Unify(variable, 2, dict);
            Assert.True(result);
            Assert.True(dict.Count == 1);
            Assert.True(dict.ContainsKey(variable));
            Assert.True(dict[variable].Equals(2));

            dict = new Dictionary<object, object>();
            result = Unifier.Unify(2, variable, dict);
            Assert.True(result);
            Assert.True(dict.Count == 1);
            Assert.True(dict.ContainsKey(variable));
            Assert.True(dict[variable].Equals(2));
        }

        [Test]
        public void TestUnify2()
        {
//            assert unify((1, 2), (1, 2), {}) == {}
//            assert unify([1, 2], [1, 2], {}) == {}
//            assert unify((1, 2), (1, 2, 3), {}) == False
//            assert unify((1, var(1)), (1, 2), {}) == {var(1): 2}
//            assert unify((1, var(1)), (1, 2), {var(1): 3}) == False
            var tuple1 = new Tuple<object, object>(1, 2);
            var tuple2 = new Tuple<object, object>(1, 2);
            var dict = new Dictionary<object, object>();
            bool result = Unifier.Unify(tuple1, tuple2, dict);
            Assert.True(dict.Count == 0);
            Assert.True(result);

            var lst1 = new List<object>() {1, 2};
            var lst2 = new List<object>() {1, 2};
            dict = new Dictionary<object, object>();
            result = Unifier.Unify(lst1, lst2, dict);
            Assert.True(dict.Count == 0);
            Assert.True(result);

            var variable = new Var(1);
            var tuple3 = new Tuple<object, object>(1, variable);
            dict = new Dictionary<object, object>();
            result = Unifier.Unify(tuple1, tuple3, dict);
            Assert.True(dict.Count == 1);
            Assert.True(result);
            Assert.True(dict.ContainsKey(variable));
            Assert.True(dict[variable].Equals(2));

            dict = new Dictionary<object, object>();
            dict.Add(variable, 3);
            result = Unifier.Unify(tuple1, tuple3, dict);
            Assert.False(result);
        }

        [Test]
        public void TestUnify3()
        {
            /*
            assert unify({1: 2}, {1: 2}, {}) == {}
            assert unify({1: 2}, {1: 3}, {}) == False
            assert unify({2: 2}, {1: 2}, {}) == False
            assert unify({1: var(5)}, {1: 2}, {}) == {var(5): 2}
             */
            var dict1 = new Dictionary<object, object>();
            dict1.Add(1,2);
            var dict2 = new Dictionary<object, object>();
            dict2.Add(1,2);
            var dict = new Dictionary<object, object>();

            bool result = Unifier.Unify(dict1, dict2, dict);
            Assert.True(result);

            var dict3 = new Dictionary<object, object>();
            dict3.Add(1, 3);
            dict = new Dictionary<object, object>();
            result = Unifier.Unify(dict1, dict3, dict);
            Assert.False(result);

            var dict4 = new Dictionary<object, object>();
            dict4.Add(2, 2);
            dict = new Dictionary<object, object>();
            result = Unifier.Unify(dict1, dict4, dict);
            Assert.False(result);

            var variable = new Var(5);
            var dict5 = new Dictionary<object, object>();
            dict5.Add(1, variable);
            dict = new Dictionary<object, object>();
            result = Unifier.Unify(dict1, dict5, dict);
            Assert.True(result);
            Assert.True(dict.Count == 1);
            Assert.True(dict.ContainsKey(variable));
            Assert.True(dict[variable].Equals(2));
        }

        [Test]
        public void TestUnifyComplex()
        {
        //    assert unify((1, {2: 3}), (1, {2: 3}), {}) == {}
        //    assert unify((1, {2: 3}), (1, {2: 4}), {}) == False
        //    assert unify((1, {2: var(5)}), (1, {2: 4}), {}) == {var(5): 4}
        //
        //    assert unify({1: (2, 3)}, {1: (2, var(5))}, {}) == {var(5): 3}
        //    assert unify({1: [2, 3]}, {1: [2, var(5)]}, {}) == {var(5): 3}

            var dict = new Dictionary<object, object>();

            var dict1 = new Dictionary<object, object>();
            dict1.Add(2,3);

            var tuple1 = new Tuple<object, object>(1, dict1);
            var tuple2 = new Tuple<object, object>(1, dict1);

            bool result = Unifier.Unify(tuple1, tuple2, dict);
            Assert.True(result);
            Assert.True(dict.Count == 0);

            var dict2 = new Dictionary<object, object>();
            dict2.Add(2, 4);
            var tuple3 = new Tuple<object, object>(1, dict2);
            result = Unifier.Unify(tuple1, tuple3, dict);
            Assert.False(result);

            var variable = new Var(5);
            var dict3 = new Dictionary<object, object>();
            dict3.Add(2,variable);
            var tuple4 = new Tuple<object, object>(1, dict3);
            result = Unifier.Unify(tuple3, tuple4, dict);
            Assert.True(result);
            Assert.True(dict.Count == 1);
            Assert.True(dict.ContainsKey(variable));
            Assert.True(dict[variable].Equals(4));
        }
    }
}

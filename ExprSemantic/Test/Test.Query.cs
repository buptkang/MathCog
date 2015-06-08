using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlgebraGeometry;
using AlgebraGeometry.Expr;
using NUnit.Framework;
using CSharpLogic;
using ExprSemantic;
using System.Linq.Expressions;

namespace ExprSemanticTest
{
    [TestFixture]
    public class TestInfer
    {
        #region Query Property

        [Test]
        public void QueryProp1()
        {
            /*
            * Input Fact: x = 1
            * Query: x=?
            * =>
            Answer: x = 1
            Why: given fact
            * */
            const string input = "x = 1";
            Reasoner.Instance.Load(input);
            var variable = new Var('x');
            object obj;
            bool result = Reasoner.Instance.Answer(variable, out obj);
            Assert.True(result);
            Assert.IsInstanceOf(typeof(List<object>), obj);
            var lst = obj as List<object>;
            Assert.NotNull(lst);
            Assert.True(lst.Count == 1);

            var propQueryResult = lst[0] as PropertyQueryResult;
            Assert.NotNull(propQueryResult);
            var agExprs = Reasoner.Instance.TestGetProperties();
            Assert.True(agExprs.Count == 1);
            var inputKnowledge = agExprs[0];
            Assert.True(inputKnowledge.Expr.Equals(propQueryResult.Answer));
            Assert.Null(propQueryResult.Trace);
        }

        [Test]
        public void QueryProp2()
        {
            /*
            *   Input Fact: x = 1
            *   Query: y =?
            *   =>
            *   Answer : y = null
            *   Why: No knowledge input about y
            */
            const string input = "x = 1";
            Reasoner.Instance.Load(input);
            var variable = new Var('y');
            object obj;
            bool result = Reasoner.Instance.Answer(variable, out obj);
            Assert.False(result);
        }

        [Test]
        public void QueryProp3()
        {
            /*
            * Input Fact: x+1=2
            * Query: x = ?
            * =>
            * Answer: x=1
            * Why: Simplify the given quation, draw down arrow to check details.
            */
            const string input = "x+1=2";
            Reasoner.Instance.Load(input);
            var variable = new Var('x');
            object obj;
            bool result = Reasoner.Instance.Answer(variable, out obj);
            Assert.True(result);
            Assert.IsInstanceOf(typeof(List<object>), obj);
            var lst = obj as List<object>;
            Assert.NotNull(lst);
            Assert.True(lst.Count == 1);

            var propQueryResult = lst[0] as PropertyQueryResult;
            Assert.NotNull(propQueryResult);
            var agExprs = Reasoner.Instance.TestGetProperties();
            Assert.True(agExprs.Count == 1);            
            Assert.NotNull(propQueryResult.Trace);
            Assert.True(propQueryResult.Trace.Count == 2);
        }

        [Test]
        public void TestProp4()
        {
            /*
             * Input Fact: x=1, x=2
             * Query: x = ?
             * =>
             * Answser: ambiguity
             * Why: two facts are shown
             */

            const string input1 = "x=2";
            const string input2 = "x=1";
            Reasoner.Instance.Load(input1);
            Reasoner.Instance.Load(input2);
            var variable = new Var('x');
            object obj;
            bool result = Reasoner.Instance.Answer(variable, out obj);
            Assert.True(result);
            Assert.IsInstanceOf(typeof(List<object>), obj);
            var lst = obj as List<object>;
            Assert.NotNull(lst);
            Assert.True(lst.Count == 2);
        }

        [Test]
        public void TestProp5()
        {
            /*
             * Input Fact: x=1
             * Query: x+1 = ?
             * =>
             * Answser: x=2
             * Why: two traces
             */
            const string input2 = "x=1";
            Reasoner.Instance.Load(input2);
            var term = new Term(Expression.Add, new Tuple<object, object>(new Var('x'),1));
            object obj;
            bool result = Reasoner.Instance.Answer(term, out obj);
            Assert.True(result);
            Assert.IsInstanceOf(typeof(List<object>), obj);
            var lst = obj as List<object>;
            Assert.NotNull(lst);
            Assert.True(lst.Count == 2);
        }

        #endregion

        /*
        #region Query

        [Test]
        public void QueryTest1()
        {
            var point = new Point(2.0, 3.0);
            var x = new Var('x');
            var y = new Var('y');
            object result = AGLogic.Infer(x, point);
            Assert.IsTrue(2.0.Equals(result));

            var m = new Var('t');
            result = LogicSharp.Run(m, point);
            Assert.Null(result);

            result = AGLogic.Infer(y, point);
            Assert.IsTrue(3.0.Equals(result));
        }

        [Test]
        public void QueryTest2()
        {
            var x = new Var('x');
            var goal = new EqGoal(x, 4.0);

            object result = AGLogic.Infer(x, goal);
            Assert.NotNull(result);
            Assert.True(result.Equals(4.0));
        }

        #endregion
         */ 
    }
}

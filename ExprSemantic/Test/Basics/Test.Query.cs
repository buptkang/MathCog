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
    public class TestQuery
    {
        #region Explicit Query

        [Test]
        public void QueryProp0()
        {
            /*
            * Input Fact: a = 1
            * Query: a=?
            * =>
            Answer: a = 1
            Why: given fact
            * */
            const string input = "a=1";
            Reasoner.Instance.Load(input);

            List<AGPropertyExpr> lst = Reasoner.Instance.TestGetProperties();
            Assert.True(lst.Count == 1);

            const string query = "a=";
            object result = Reasoner.Instance.Load(query);
            var agQueryExpr = result as AGQueryExpr;
            Assert.NotNull(agQueryExpr);
            var queryTag = agQueryExpr.QueryTag;
            Assert.NotNull(queryTag);
            Assert.True(queryTag.Success);
            Assert.True(queryTag.CachedEntities.Count == 1);
            var cachedGoal = queryTag.CachedEntities.ToList()[0] as EqGoal;
            Assert.NotNull(cachedGoal);
            Assert.True(cachedGoal.Rhs.Equals(1));
        }

        [Test]
        public void QueryProp1()
        {
            /*
            * Input Fact: x = 1 (Line)
            * Query: x=?
            * =>
            No answer, cannot find property x!. 
            * */
            /*            
            var reasoner = new Reasoner();
            const string input = "x = 1";
            reasoner.Load(input);
            var variable = new Var('x');
            object obj;
            bool result = reasoner.Answer(variable, out obj);
            Assert.False(result);
            */
        }

        [Test]
        public void QueryProp2()
        {
            /*
            *   Input Fact: a = 1
            *   Query: y =?
            *   =>
            *   Answer : y = null
            *   Why: No knowledge input about y
            */
            const string input = "a = 1";
            Reasoner.Instance.Load(input);

            const string query = "y=";
            object result = Reasoner.Instance.Load(query);
            var agQueryExpr = result as AGQueryExpr;
            Assert.NotNull(agQueryExpr);
            Assert.False(agQueryExpr.QueryTag.Success);
        }

        [Test]
        public void QueryProp3()
        {
            /*
            * Input Fact: a+1=2
            * Query: a = ?
            * =>
            * Answer: a=1
            * Why: Simplify the given quation, draw down arrow to check details.
            */
       /*     const string input = "a+1=2";
            Reasoner.Instance.Load(input);

            const string query = "a = ?";
            object result = Reasoner.Instance.Load(query);
            var agQueryExpr = result as AGQueryExpr;
            Assert.NotNull(agQueryExpr);
            var eqGoal = agQueryExpr.QueryTag as EqGoal;*/
      /*      Assert.NotNull(eqGoal);
            Assert.True(eqGoal.CachedEntities.Count == 1);
            var cachedGoal = eqGoal.CachedEntities.ToList()[0] as EqGoal;
            Assert.NotNull(cachedGoal);
            Assert.True(cachedGoal.Rhs.Equals(1));*/
        }

        [Test]
        public void QueryProp4()
        {
            /*
             * Input Fact: a=1, a=2
             * Query: a = ?
             * =>
             * Answser: ambiguity
             * Why: two facts are shown
             */

            const string input1 = "a=2";
            const string input2 = "a=1";
            Reasoner.Instance.Load(input1);
            Reasoner.Instance.Load(input2);

            const string query = "a = ";
            object result = Reasoner.Instance.Load(query);
            var agQueryExpr = result as AGQueryExpr;
            Assert.NotNull(agQueryExpr);
            var queryTag = agQueryExpr.QueryTag;
            Assert.NotNull(queryTag);
            Assert.True(queryTag.CachedEntities.Count == 2);
        }

        [Test]
        public void TestProp5()
        {
            /*
             * Input Fact: a=1
             * Query: a+1 = ?
             * =>
             * Answser: a=2
             * Why: two traces
             */
            const string input = "a=1";
            Reasoner.Instance.Load(input);

            const string query = "a+1=?";
            object result = Reasoner.Instance.Load(query);
            var agQueryExpr = result as AGQueryExpr;
            Assert.NotNull(agQueryExpr);
            var equation = agQueryExpr.QueryTag as Equation;
            Assert.NotNull(equation);
            //TODO cache equation
        }

        #endregion

        #region Single Point Test (TODO)

        [Test]
        public void test1()
        {
            /*
             * Input Fact: A(2,3) 
             * Query: X=?
             * =>
             * Answer: X=2
             * Why: given fact
             */
            //TODO 
        }

        [Test]
        public void test2()
        {
            /*
             * Input Fact: (3.0,y)
             * Query: Y=?
             * =>
             * Answer: Unknown
             */
            //TODO
        }

        [Test]
        public void test3()
        {
            //TODO
            /*
             * Input Fact: (3.0,y), x = 2
             * Query: x=?
             * =>
             * Answer: ambiguity, choose the point or the line?
             */
        }

        [Test]
        public void test4()
        {
            /*
             * Input Fact: A(x,2), x = -3.1
             * Query: A=?
             * =>
             * Answer: A=(-3.1,2)
             * Why: trace
             */
        }

        [Test]
        public void test5()
        {
            /*
             * Input Fact: B(x,y), x+1=3-1
             * Query1: x =?
             * =>
             * Answer: x = 1
             * Why: 2 trace 
             * =>
             * Query2: B=?
             * =>
             * Answer B=(1,y)
             * Why: 1 trace
             */
        }

        #endregion
    }
}

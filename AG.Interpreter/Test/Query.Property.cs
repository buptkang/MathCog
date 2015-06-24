using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlgebraGeometry.Expr;
using NUnit.Framework;
using ExprSemantic;

namespace AG.Interpreter.Test
{
    [TestFixture]
    public class QueryProperty
    {
        [Test]
        public void test1()
        {
            /*
             * Input Fact: x = 1
             * Query: x=
             * =>
             Answer: x = 1
             Why: given fact
             * */
/*
            const string fact = "x=1";
            //Reasoner.Instance.Load(fact);
            Interpreter.Instance.Load(fact);
            List<AGPropertyExpr> facts = Reasoner.Instance.TestGetProperties();
            Assert.True(facts.Count == 1);

            const string query = "x=";
            Interpreter.Instance.Load(query);
            int number = Interpreter.Instance.GetNumberOfQueries();
            Assert.True(number == 1);

            object output = Interpreter.Instance.SearchCurrentQueryResult(query);
            Assert.NotNull(output);
            var queryResult = output as AGQueryExpr;
            Assert.NotNull(queryResult);
            Interpreter.Instance.UnLoadQuery(query);
            number = Interpreter.Instance.GetNumberOfQueries();
            Assert.True(number == 0);
 */ 
        }
/*
        [Test]
        public void test2()
        {
            const string fact = "x=1";
            Reasoner.Instance.Load(fact);

            /////////////////////////////////////////////////////

            const string query = "y";
            bool result = Interpreter.Instance.LoadQuery(query);
            Assert.True(result);

            object output = Interpreter.Instance.SearchCurrentQueryResult(query);
            Assert.Null(output);

            int number = Interpreter.Instance.GetNumberOfQueries();
            Assert.True(number == 1);

            Interpreter.Instance.UnLoadQuery(query);
            number = Interpreter.Instance.GetNumberOfQueries();
            Assert.True(number == 0);
        }

        [Test]
        public void test3()
        {
            const string fact = "x+1=2";
            Reasoner.Instance.Load(fact);

            const string variable = "x";
            bool result = Interpreter.Instance.LoadQuery(variable);
            Assert.True(result);

            object output =
              Interpreter.Instance.SearchCurrentQueryResult(variable);
            Assert.NotNull(output);

            //Not only answer, but also the procedural to derive to.
            Assert.False(output.Equals(1));

            //Assert.IsInstanceOf(typeof(Trace), output);
        }
*/
        /*
         * Input Fact: x=1, x=2
         * Query: x = ?
         * =>
         * Answser: ambiguity
         * Why: two facts are shown
         */
    }
}

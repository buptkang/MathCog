using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExprSemantic;
using NUnit.Framework;

namespace AG.Interpreter.Test
{
    [TestFixture]
    public class QueryPoint
    {
        #region Single Point Test

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

            const string fact = "A(2,3)";
            Reasoner.Instance.Load(fact);

            const string variable = "Y";
            object obj = Interpreter.Instance.LoadQuery(variable);
            Assert.Null(obj);

            object output =
                Interpreter.Instance.SearchCurrentQueryResult(variable);
            Assert.NotNull(output);
            Assert.True(output.Equals(3));

            int number = Interpreter.Instance.GetNumberOfQueries();
            Assert.True(number == 1);

            Interpreter.Instance.UnLoadQuery(variable);
            number = Interpreter.Instance.GetNumberOfQueries();
            Assert.True(number == 0);
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

            const string fact = "(2,y)";
            Reasoner.Instance.Load(fact);

            const string variable = "Y";
            object obj = Interpreter.Instance.LoadQuery(variable);
            Assert.NotNull(obj);

            object output =
               Interpreter.Instance.SearchCurrentQueryResult(variable);
            Assert.Null(output);

            const string fact1 = "y=1";
            Reasoner.Instance.Load(fact1);
            obj = Interpreter.Instance.LoadQuery(variable);
            Assert.NotNull(obj);
            output = Interpreter.Instance.SearchCurrentQueryResult(variable);
            Assert.NotNull(output);
            Assert.True(output.Equals(1));
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

        #region Multiple points 

        [Test]
        public void test_multiple_1()
        {
            const string fact  = "A(2,3)";
            const string fact2 = "B(4,5)";
            Reasoner.Instance.Load(fact);
            Reasoner.Instance.Load(fact2);

            const string variable = "Y";
            object result = Interpreter.Instance.LoadQuery(variable);
            Assert.NotNull(result);

            object output =
                Interpreter.Instance.SearchCurrentQueryResult(variable);
            Assert.NotNull(output);
            //output could be 3 or 5
            Assert.IsInstanceOf(typeof(Dictionary<object,object>), output);
            var dict = output as Dictionary<object, object>;
            Assert.NotNull(dict);
            Assert.True(dict.ContainsKey(fact));
            Assert.True(dict.ContainsKey(fact2));
            Assert.True(dict[fact].Equals(3));
            Assert.True(dict[fact2].Equals(5));

            int number = Interpreter.Instance.GetNumberOfQueries();
            Assert.True(number == 1);

            Interpreter.Instance.UnLoadQuery(variable);
            number = Interpreter.Instance.GetNumberOfQueries();
            Assert.True(number == 0);
        }

        #endregion 
    }
}

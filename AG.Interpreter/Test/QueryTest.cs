using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExprSemantic;
using NUnit.Framework;

namespace AG.Interpreter.Test
{
    [TestFixture]
    public class QueryTest
    {
        [Test]
        public void TestQuery1()
        {
            const string fact = "x=1";
            Reasoner.Instance.Load(fact);

            /////////////////////////////////////////////////////

            const string variable = "x";
            bool result = Interpreter.Instance.LoadQuery(variable);
            Assert.True(result);

            object output = 
                Interpreter.Instance.SearchCurrentQueryResult(variable);
            Assert.NotNull(output);
            Assert.True(output.Equals(1));

            int number = Interpreter.Instance.GetNumberOfQueries();
            Assert.True(number == 1);

            Interpreter.Instance.UnLoadQuery(variable);
            number = Interpreter.Instance.GetNumberOfQueries();
            Assert.True(number == 0);

            ////////////////////////////////////////////////////////

            const string variable1 = "y";
            result = Interpreter.Instance.LoadQuery(variable1);
            Assert.True(result);

            output = Interpreter.Instance.SearchCurrentQueryResult(variable);
            Assert.Null(output);

            number = Interpreter.Instance.GetNumberOfQueries();
            Assert.True(number == 1);

            Interpreter.Instance.UnLoadQuery(variable);
            number = Interpreter.Instance.GetNumberOfQueries();
            Assert.True(number == 0);
        }

        [Test]
        public void TestQuery2_0()
        {
            const string fact = "A(2,3)";
            Reasoner.Instance.Load(fact);

            const string variable = "Y";
            bool result = Interpreter.Instance.LoadQuery(variable);
            Assert.True(result);

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
        public void TestQuery2_1()
        {
            const string fact  = "A(2,3)";
            const string fact2 = "B(4,5)";
            Reasoner.Instance.Load(fact);
            Reasoner.Instance.Load(fact2);

            const string variable = "Y";
            bool result = Interpreter.Instance.LoadQuery(variable);
            Assert.True(result);

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

        [Test]
        public void TestQuery2_2()
        {
            const string fact = "(2,y)";
            Reasoner.Instance.Load(fact);

            const string variable = "Y";
            bool result = Interpreter.Instance.LoadQuery(variable);
            Assert.True(result);

            object output =
               Interpreter.Instance.SearchCurrentQueryResult(variable);
            Assert.Null(output);

            const string fact1 = "y=1";
            Reasoner.Instance.Load(fact1);
            result = Interpreter.Instance.LoadQuery(variable);
            Assert.True(result);
            output = Interpreter.Instance.SearchCurrentQueryResult(variable);
            Assert.NotNull(output);
            Assert.True(output.Equals(1));
        }

        [Test]
        public void TestQuery3()
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

            Assert.IsInstanceOf(typeof(Trace), output);




        }
    }
}

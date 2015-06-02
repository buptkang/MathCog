using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace CSharpLogic.Test
{
    [TestFixture]
    public class TestTerm
    {
        [Test]
        public void UnifyTest()
        {
            var term1 = new Term(Expression.Add, new Tuple<object, object>(1, 2));
            var x = new Var('x');
            var term2 = new Term(Expression.Add, new Tuple<object, object>(1, x));

            var dict = new Dictionary<object, object>();
            bool result = term1.Unify(term2, dict);

            Assert.True(result);
            Assert.True(dict.Count == 1); 
        }

        [Test]
        public void Test_Eval_0()
        {
            // 1 + x
            var x = new Var('x');
            var term = new Term(Expression.Add, new Tuple<object, object>(1, x));
            object result = term.Eval();
            Assert.True(result.Equals(term));
            Assert.True(term.Traces.Count == 0);

            //1+2
            term = new Term(Expression.Subtract, new Tuple<object, object>(1, 2));
            result = term.Eval();
            Assert.NotNull(result);
            Assert.True(result.Equals(-1));
            Assert.True(term.Traces.Count == 1);

            // 1+2-1
            var term1 = new Term(Expression.Add, new Tuple<object, object>(1, 2));
            term = new Term(Expression.Subtract, new Tuple<object,object>(term1, 1));
            result = term.Eval();
            Assert.NotNull(result);
            Assert.True(result.Equals(2));
            Assert.True(term.Traces.Count == 2);

            //x - x
            term = new Term(Expression.Subtract, new Tuple<object, object>(x, x));
            result = term.Eval();
            Assert.NotNull(result);
            Assert.True(result.Equals(0));
            Assert.True(term.Traces.Count == 1);

            //x + x
            term = new Term(Expression.Add, new Tuple<object, object>(x, x));
            result = term.Eval();
            Assert.NotNull(result);
            Assert.IsInstanceOf(typeof(Term), result);
            var gTerm = result as Term;
            Assert.NotNull(gTerm);
            Assert.True(gTerm.Op.Method.Name.Equals("Multiply"));
            var gTuple = gTerm.Args as Tuple<object, object>;
            Assert.NotNull(gTuple);
            Assert.True(gTuple.Item1.Equals(2));
            Assert.True(gTuple.Item2.Equals(x));
            Assert.True(term.Traces.Count == 1);
            Assert.True(gTerm.Traces.Count == 1);
        }

        [Test]
        public void Test_Eval_Recursive()
        {
            //x+x-y

            //x+y-x

            //x + (x-3)

            //2x + x
            //add(mul(2,x),x) 

            //distributive law
            //identity law
        }
    
    }
}

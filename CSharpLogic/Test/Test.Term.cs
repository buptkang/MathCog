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
        public void Test_Term_Eval()
        {
            var x = new Var('x');

            //1-2 -> -1
            var term = new Term(Expression.Subtract, new Tuple<object, object>(1, 2));
            object result = term.Eval();
            Assert.NotNull(result);
            Assert.True(result.Equals(-1));
            Assert.True(term.Traces.Count == 1);

            // 1+2-1 -> 2
            var term1 = new Term(Expression.Add, new Tuple<object, object>(1, 2));
            term = new Term(Expression.Subtract, new Tuple<object,object>(term1, 1));
            result = term.Eval();
            Assert.NotNull(result);
            Assert.True(result.Equals(2));
            Assert.True(term.Traces.Count == 2);

            //1-2*3
            term1 = new Term(Expression.Multiply, new Tuple<object,object>(2,3));
            term = new Term(Expression.Subtract, new Tuple<object,object>(1,term1));
            result = term.Eval();
            Assert.NotNull(result);
            Assert.True(result.Equals(-5));
            Assert.True(term.Traces.Count == 2);

            //x- 2*5
            term1 = new Term(Expression.Multiply, new Tuple<object, object>(2, 5));
            term = new Term(Expression.Subtract, new Tuple<object, object>(x, term1));
            result = term.Eval();
            Assert.IsInstanceOf(typeof(Term), result);
            var rTerm = result as Term;
            Assert.NotNull(rTerm);
            var tuple = rTerm.Args as Tuple<object, object>;
            Assert.NotNull(tuple);
            Assert.True(tuple.Item1.Equals(x));
            Assert.True(tuple.Item2.Equals(10));
            Assert.True(term.Traces.Count == 1);

            //x - (2*2 + 3)
            term1 = new Term(Expression.Multiply, new Tuple<object, object>(2, 2));
            var term2 = new Term(Expression.Add, new Tuple<object, object>(term1, 3));
            term = new Term(Expression.Subtract, new Tuple<object,object>(x, term2));
            result = term.Eval();
            Assert.IsInstanceOf(typeof(Term), result);
            rTerm = result as Term;
            Assert.NotNull(rTerm);
            tuple = rTerm.Args as Tuple<object, object>;
            Assert.NotNull(tuple);
            Assert.True(tuple.Item1.Equals(x));
            Assert.True(tuple.Item2.Equals(7));
            Assert.True(term.Traces.Count == 2);
        }    
    }
}

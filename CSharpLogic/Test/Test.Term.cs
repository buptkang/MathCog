using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
        public void Test_Eval()
        {
            var x = new Var('x');
            var term = new Term(Expression.Add, new Tuple<object, object>(1, x));
            object result = term.Eval();
            Assert.Null(result);

            term = new Term(Expression.Subtract, new Tuple<object, object>(1, 2));
            result = term.Eval();
            Assert.NotNull(result);
            Assert.True(result.Equals(-1));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using CSharpLogic;
using System.Linq.Expressions;

namespace AlgebraExpression.Test
{
    [TestFixture]
    public class TestTermEvaluator
    {
        [Test]
        public void Test_basic()
        {
            // 1+x -> x + 1
            var x = new Var('x');
            var term = new Term(Expression.Add, new Tuple<object, object>(1, x));
            object result = term.Eval();
            Assert.True(result.Equals(term));
            Assert.True(term.Traces.Count == 1);

            //x - x -> 0
            term = new Term(Expression.Subtract, new Tuple<object, object>(x, x));
            result = term.Eval();
            Assert.NotNull(result);
            Assert.True(result.Equals(0));
            Assert.True(term.Traces.Count == 1);

            //x + x -> 2*x
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
            //x+x -> 2*x, y*y -> y^2 

            //(x+1)-3

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
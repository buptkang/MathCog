using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using CSharpLogic.Rules;
using NUnit.Framework;

namespace CSharpLogic.Test
{
    [TestFixture]
    public class TestEvaluator
    {
        [Test]
        public void Test_AssociativeLaw()
        {
            //(x+1)+x -> x +(x+1)
            var x = new Var('x');
            var a = new Term(Expression.Add, new Tuple<object, object>(x, 1));
            var b = new Term(Expression.Add, new Tuple<object, object>(a, x));
     
            Assert.True(a.ToString().Equals("x+1"));
            Assert.True(b.ToString().Equals("(x+1)+x"));

            Term gTerm;
            bool result = b.AssociativeLaw(out gTerm);
            Assert.True(result);
            Assert.True(gTerm.ToString().Equals("x+(1+x)"));
        }

        [Test]
        public void Test_CommutativeLaw()
        {
            //x + 3 -> 3 + x
            var x = new Var('x');
            var a = new Term(Expression.Add, new Tuple<object, object>(x, 3));
            Assert.True(a.ToString().Equals("x+3"));

            Term gTerm;
            bool result = a.CommutativeLaw(out gTerm);
            Assert.True(result);
            Assert.True(gTerm.ToString().Equals("3+x"));
        }

        [Test]
        public void Test_DistributiveLaw()
        {
            //3*(x+1) -> 3x + 3
            var x = new Var('x');
            var a = new Term(Expression.Add, new Tuple<object, object>(x, 1));
            var b = new Term(Expression.Multiply, new Tuple<object, object>(3, a));
            Assert.True(b.ToString().Equals("3*(x+1)"));

            Term gTerm;
            bool result = b.DistributeLaw(out gTerm);
            Assert.True(result);
            Assert.True(gTerm.ToString().Equals("(3*x)+(3*1)"));
        }    
    }
}

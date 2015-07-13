using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using NUnit.Framework;

namespace CSharpLogic
{
    /*
    Example 1:
    y+y=> (1+1)*y(distributive) => 2*y(calc)

    Example 2:
    (2+y)+1 => (y+2)+1(commutative) => y+(2+1)(associative) => y+3(calc)

    Example 3:
    ((2+y)+1)+y => ((y+2)+1)+y(commutative) => (y+(2+1))+y(associative)  => (y+3)+y(calc) 
    => (y+y)+3(associative) => ((1+1)*y)+3(distributive) => 2*y+3(calc)

    Example 4:
    ((y+1)+x)-1 => (x+(y+1))-1(commutative) => ((x+y)+1)-1(associative)
    => (x+y)+(1-1)(associative) => (x+y)+0(calc) =>

    Example 5:
    (2*(2+y))+1 =>(4+2y)+1(distributive) => (2y+4)+1(commutative) 
    =>2y+(4+1)(associative) =>2y+5(calc)
    */

    [TestFixture]
    public class AlgebraTest
    {
        [Test]
        public void BubbleSort_1()
        {
            int a = 1;
            var b = new Var('b');
            int c = 1;
            var lst = new List<object>()
            {
                a,
                b,
                c
            };
            var term = new Term(Expression.Add, lst);
            Term gTerm = term.Commutative();
            var llst = term.Args as List<object>;
            Assert.NotNull(llst);
            Assert.True(term.Traces.Count == 1);
        }




        [Test]
        public void Test_AssociativeLaw()
        {
          /*  //(x+1)+x -> x +(x+1)
            var x = new Var('x');
            var a = new Term(Expression.Add, new Tuple<object, object>(x, 1));
            var b = new Term(Expression.Add, new Tuple<object, object>(a, x));

            Assert.True(a.ToString().Equals("x+1"));
            Assert.True(b.ToString().Equals("(x+1)+x"));

            Term gTerm;
            bool result = b.AssociativeLaw(out gTerm);
            Assert.True(result);
            Assert.True(gTerm.ToString().Equals("x+(1+x)"));*/
        }

        [Test]
        public void Test_CommutativeLaw()
        {
           /* //x + 3 -> 3 + x
            var x = new Var('x');
            var a = new Term(Expression.Add, new Tuple<object, object>(x, 3));
            Assert.True(a.ToString().Equals("x+3"));

            Term gTerm;
            bool result = a.CommutativeLaw(out gTerm);
            Assert.True(result);
            Assert.True(gTerm.ToString().Equals("3+x"));*/
        }

        [Test]
        public void Test_DistributiveLaw()
        {
          /*  //3*(x+1) -> 3x + 3
            var x = new Var('x');
            var a = new Term(Expression.Add, new Tuple<object, object>(x, 1));
            var b = new Term(Expression.Multiply, new Tuple<object, object>(3, a));
            Assert.True(b.ToString().Equals("3*(x+1)"));

            Term gTerm;
            bool result = b.Distribute(out gTerm);
            Assert.True(result);
            Assert.True(gTerm.ToString().Equals("(3*x)+(3*1)"));*/
        }   
    }
}

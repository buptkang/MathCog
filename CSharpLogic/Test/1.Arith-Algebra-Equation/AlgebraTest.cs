using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using NUnit.Framework;

namespace CSharpLogic
{
    /*
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
        #region Commutative

        [Test]
        public void Test_Commutative_1()
        {
            //3+x -> x+3             
            var x = new Var('x');
            var a = new Term(Expression.Add, new List<object>() { 3, x });
            object result = a.EvalAlgebra();
            var gTerm = result as Term;
            Assert.NotNull(gTerm);
            Assert.NotNull(result);
            Assert.True(result.ToString().Equals("((1*x)+3)"));
            Assert.True(a.Traces.Count == 2);
        }

        [Test]
        public void Test_Commutative_2()
        {
            //x*3 -> 3*x
            var x = new Var('x');
            var a = new Term(Expression.Multiply, new List<object>() { x, 3 });
            object result = a.EvalAlgebra();
            Assert.NotNull(result);
            Assert.True(result.ToString().Equals("(3*x)"));
            Assert.True(a.Traces.Count == 1);
        }

        [Test]
        public void Test_Commutative_3()
        {
            //3*x*3 -> 9*x
            var x = new Var('x');
            var a = new Term(Expression.Multiply, new List<object>() { 3, x, 3 });
            object result = a.EvalAlgebra();
            Assert.NotNull(result);
            Assert.True(result.ToString().Equals("(9*x)"));
            Assert.True(a.Traces.Count == 2);
        }

        [Test]
        public void Test_Commutative_4()
        {
            //1+(1+a) -> (a+1)+1
            var a = new Var('a');
            var term = new Term(Expression.Add, new List<object>() { 1, a });
            var term1 = new Term(Expression.Add, new List<object>() { 1, term });
            Assert.True(term1.ToString().Equals("(1+(1+a))"));

            object result = term1.EvalAlgebra();
            Assert.NotNull(result);
            Assert.True(result.ToString().Equals("(a+2)"));
            Assert.True(term1.Traces.Count == 4);
        }

        [Test]
        public void Test_Commutative_5()
        {
            //(1+a)*2 -> 2*(a+1)
            var a = new Var('a');
            var term = new Term(Expression.Add, new List<object>() { 1, a });
            var term1 = new Term(Expression.Multiply, new List<object>() { term, 2 });
            Assert.True(term1.ToString().Equals("((1+a)*2)"));

            object result = term1.EvalAlgebra();
            Assert.NotNull(result);
            Assert.True(result.ToString().Equals("((2*a)+2)"));
            Assert.True(term1.Traces.Count == 4);
        }

        #endregion

        #region Identity

        [Test]
        public void Test_Identity_1()
        {
            //x->1*x
            var x = new Var('x');
            //TODO            
        }

        [Test]
        public void Test_Identity_2()
        {
            //x+3->1*x+3
            var x = new Var('x');
            var term = new Term(Expression.Add, new List<object>() { x, 3 });
            object result = term.EvalAlgebra();
            Assert.NotNull(result);
            Assert.True(result.ToString().Equals("((1*x)+3)"));
            Assert.True(term.Traces.Count == 1);
        }

        [Test]
        public void Test_Identity_3()
        {
            //x+x->1*x+1*x
            var x = new Var('x');
            var term = new Term(Expression.Add, new List<object>() { x, x });
            object result = term.EvalAlgebra();
            Assert.NotNull(result);
            Assert.True(result.ToString().Equals("(2*x)"));
            Assert.True(term.Traces.Count == 4);
        }

        [Test]
        public void Test_Identity_4()
        {
            //y+2*y ->1*y+2*y
            var y = new Var('y');
            var term = new Term(Expression.Multiply, new List<object>() { 2, y });
            var term1 = new Term(Expression.Add, new List<object>() { y, term });
            object result = term1.EvalAlgebra();
            Assert.NotNull(result);
            Assert.True(result.ToString().Equals("(3*y)"));
            Assert.True(term1.Traces.Count == 3);
        }

        [Test]
        public void Test_Identity_5()
        {
            //x*y ->1*x*y
            var x = new Var('x');
            var y = new Var('y');
            var xy = new Var("xy");
            //TODO           
        }

        #endregion

        #region Zero

        [Test]
        public void Test_Zero_1()
        {
            //x+0=>x
            var x = new Var('x');
            var term = new Term(Expression.Add, new List<object>() { x, 0 });
            Assert.True(term.ToString().Equals("(x+0)"));
            var result = term.Eval();
            Assert.NotNull(result);
            Assert.True(term.Traces.Count == 2);
        }

        [Test]
        public void Test_Zero_2()
        {
            //0*x=>0
            var x = new Var('x');
            var term = new Term(Expression.Multiply, new List<object>() { 0, x });
            Assert.True(term.ToString().Equals("(0*x)"));
            var result = term.Eval();
            Assert.NotNull(result);
            Assert.True(result.Equals(0));
            Assert.True(term.Traces.Count == 1);
        }

        #endregion

        #region Distributive

        [Test]
        public void Test_Distributive_01()
        {
            //3*3*y
            var y = new Var('y');
            var term = new Term(Expression.Multiply, new List<object>() { 3, 3, y });
            object obj = term.Eval();
            Assert.NotNull(obj);
            Assert.True(obj.ToString().Equals("(9*y)"));
            Assert.True(term.Traces.Count == 1);
        }

        [Test]
        public void Test_Distributive_0()
        {
            //(1+1)*y
            var term  = new Term(Expression.Add, new List<object>() { 1, 1 });
            var y = new Var('y');
            var term1 = new Term(Expression.Multiply, new List<object>() { term, y });
            object obj = term1.Eval();
            Assert.NotNull(obj);
            Assert.True(obj.ToString().Equals("(2*y)"));
            Assert.True(term1.Traces.Count == 1);

            //(1+1+1)*y -> (2+1)*y
            term = new Term(Expression.Add, new List<object>() { 1, 1 ,1});
            term1 = new Term(Expression.Multiply, new List<object>() { term, y });
            obj = term1.Eval();
            Assert.NotNull(obj);
            Assert.True(obj.ToString().Equals("(3*y)"));
            Assert.True(term1.Traces.Count == 2);
        }

        [Test]
        public void Test_Distributive_1()
        {
            //y+y=> 1*y+1*y=> (1+1)*y => 2*y
            var y = new Var('y');
            var term = new Term(Expression.Add, new List<object>(){y, y});
            //Assert.True(term.ToString().Equals("(y+y)"));
            object obj = term.Eval();
            Assert.NotNull(obj);
            Assert.True(obj.ToString().Equals("(2*y)"));
            Assert.True(term.Traces.Count == 4);
        }

        [Test]
        public void Test_Distributive_2()
        {
            // y+2*y  -> (1+2)*y  
            var y = new Var('y');
            var term1 = new Term(Expression.Multiply, new List<object>() { 2, y });
            var term = new Term(Expression.Add, new List<object>() { y, term1 });
            object obj = term.Eval();
            Assert.NotNull(obj);
            Assert.True(obj.ToString().Equals("(3*y)"));
            Assert.True(term.Traces.Count == 3);
        }

        [Test]
        public void Test_Distributive_3()
        {
            // y+2*y-4*y  -> -1*y
            var y = new Var('y');
            var term1 = new Term(Expression.Multiply, new List<object>() { 2, y });
            var term2 = new Term(Expression.Multiply, new List<object>() { -4, y });
            var term = new Term(Expression.Add, new List<object>() {y, term1, term2});
            object obj = term.Eval();
            Assert.NotNull(obj);
            Assert.True(obj.ToString().Equals("(-1*y)"));
            Assert.True(term.Traces.Count == 5);
        }

        [Test]
        public void Test_Distributive_4()
        {
            //a*y+y+x           -> (a+1)*y+x
            var x = new Var('x');
            var y = new Var('y');
            var a = new Var('a');
            var term1 = new Term(Expression.Multiply, new List<object>() { a, y });
            var term = new Term(Expression.Add, new List<object>() {term1, y, x});
            object obj = term.Eval();
            Assert.NotNull(obj);
            Assert.True(obj.ToString().Equals("(((a+1)*y)+(1*x))"));
            Assert.True(term.Traces.Count == 3);
        }

        [Test]
        public void Test_Distributive_5()
        {
            //3*(x+1) -> 3x + 3
            var x = new Var('x');
            var term = new Term(Expression.Add, new List<object>() { x, 1 });
            var term1 = new Term(Expression.Multiply, new List<object> { 3, term });
            object obj = term1.Eval();
            Assert.NotNull(obj);
            Assert.True(obj.ToString().Equals("((3*x)+3)"));
            Assert.True(term1.Traces.Count == 5);
        }

        [Test]
        public void Test_Distributive_6()
        {
            //a*(x+1) -> ax+a
            var x = new Var('x');
            var term = new Term(Expression.Add, new List<object>() { x, 1 });
            var a = new Var('a');
            var term1 = new Term(Expression.Multiply, new List<object> { a, term });
            object obj = term1.Eval();
            Assert.NotNull(obj);
            Assert.True(obj.ToString().Equals("((a*x)+a)"));
            Assert.True(term1.Traces.Count == 7);
        }

        [Test]
        public void Test_Distributive_7()
        {
            //x*(a+1)         -> xa + x 
        }

        #endregion

        #region Associative

        [Test]
        public void Test_Associative_1()
        {
            //(3*(2*x)) => (3*2)*x
            var x = new Var('x');
            var term  = new Term(Expression.Multiply, new List<object>() {2, x});
            var term1 = new Term(Expression.Multiply, new List<object>() {3, term});
            object obj = term1.Eval();
            Assert.NotNull(obj);
            Assert.True(obj.ToString().Equals("(6*x)"));
            Assert.True(term1.Traces.Count == 2);
        }

        [Test]
        public void Test_Associative_2()
        {
            //(a+1)+1 -> a+(1+1)
            var a = new Var('a');
            var term = new Term(Expression.Add, new List<object>() { a, 1 });
            var term1 = new Term(Expression.Add, new List<object>() { term, 1 });
            object obj = term1.Eval();
            Assert.NotNull(obj);
            Assert.True(obj.ToString().Equals("(a+2)"));
            Assert.True(term1.Traces.Count == 2);
        }

        #endregion

        #region Integrated

        [Test]
        public void Test_LinePatterMatch()
        {
            //(2*y)+(2*x)+(-1*y)+(2*x)+4
            var x = new Var('x');
            var y = new Var('y');

            var term1 = new Term(Expression.Multiply, new List<object>() { 2, y });
            var term2 = new Term(Expression.Multiply, new List<object>() { 2, x });
            var term3 = new Term(Expression.Multiply, new List<object>() { -1, y });
            var term4 = new Term(Expression.Multiply, new List<object>() { 2, x });
            var term = new Term(Expression.Add, new List<object>() { term1, term2, term3, term4, 4});
            object obj = term.Eval();
            Assert.NotNull(obj);
            var gTerm = obj as Term;
            Assert.NotNull(gTerm);
            var lst = gTerm.Args as List<object>;
            Assert.NotNull(lst);
            Assert.True(lst.Count == 3);
        }

        [Test]
        public void Test_LinePatternMatch2()
        {
            //4*x+y+-1*4
            var x = new Var('x');
            var y = new Var('y');
            var term1 = new Term(Expression.Multiply, new List<object>() { 4, x });
            var term3 = new Term(Expression.Multiply, new List<object>() { -1, 4});
            var term = new Term(Expression.Add, new List<object>() { term1, y, term3});
            object obj = term.Eval();
            Assert.NotNull(obj);
            var gTerm = obj as Term;
        }

        [Test]
        public void Test_Simple_1()
        {
            var variable = new Var('x');
            var term1 = new Term(Expression.Multiply, new List<object>() { 2, variable });
            var term2 = new Term(Expression.Multiply, new List<object>() { -1, variable});
            //2x-x -> x
            var term = new Term(Expression.Add, new List<object>() { term1, term2 });
            var obj = term.Eval();
            Assert.True(obj.Equals(variable));
        }


        #endregion
    }
}
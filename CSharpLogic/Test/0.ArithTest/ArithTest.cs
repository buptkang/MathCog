using System;
using System.Collections.Generic;
using NUnit.Framework;
using System.Linq.Expressions;

namespace CSharpLogic
{
    [TestFixture]
    public class TestArith
    {
        #region Without Algebra

        [Test]
        public void Term_Arith_1()
        {
            //Addition
            var lst = new List<object>() {1, 2, 3};
            var term = new Term(Expression.Add, lst);
            object obj = term.Eval();
            Assert.NotNull(obj);
            Assert.True(obj.Equals(6));
            Assert.True(term.Traces.Count == 2);
        }

        [Test]
        public void Term_Arith_2()
        {
            //Substraction
            var lst = new List<object>() { 1, -2, -3 };
            var term = new Term(Expression.Add, lst);
            object obj = term.Eval();
            Assert.NotNull(obj);
            Assert.True(obj.Equals(-4));
            Assert.True(term.Traces.Count == 2);
        }

        [Test]
        public void Term_Arith_3()
        {
            //Substraction
            var lst = new List<object>() { 1, -2, -3 };
            var term = new Term(Expression.Multiply, lst);
            object obj = term.Eval();
            Assert.NotNull(obj);
            Assert.True(obj.Equals(6));
            Assert.True(term.Traces.Count == 2);
        }

        [Test]
        public void Term_Arith_4()
        {
            //1+2*3
            var lst = new List<object>() {2, 3};
            var term = new Term(Expression.Multiply, lst);
            var lst1 = new List<object>() {1, term};
            var term1 = new Term(Expression.Add, lst1);
            object obj = term1.Eval();
            Assert.NotNull(obj);
            Assert.True(obj.Equals(7));
            Assert.True(term1.Traces.Count == 2);
        }

        [Test]
        public void Term_Arith_5()
        {
            //1-2*3
            var lst = new List<object>() { 2, 3 };
            var term = new Term(Expression.Multiply, lst);
            var lst1 = new List<object>() { 1, term };
            var term1 = new Term(Expression.Subtract, lst1);
            object obj = term1.Eval();
            Assert.NotNull(obj);
            Assert.True(obj.Equals(-5));
            Assert.True(term1.Traces.Count == 2);
        }

        #endregion

        #region With Algebra

        [Test]
        public void Term_Algebra_Arith_1()
        {
            var x = new Var('x');
            //x- 2*5
            var lst = new List<object>() { 2, 5 };
            var term = new Term(Expression.Multiply, lst);
            var lst1 = new List<object>() { x, term };
            var term1 = new Term(Expression.Subtract, lst1);
            object obj = term1.Eval();
            Assert.NotNull(obj);
            var gTerm = obj as Term;
            Assert.NotNull(gTerm);
            var glst = gTerm.Args as List<object>;
            Assert.NotNull(glst);
            Assert.True(glst.Count == 2);
            Assert.True(term1.Traces.Count == 1);
        }

        [Test]
        public void Term_Algebra_Arith_2()
        {
            var x = new Var('x');
            // 2*5 + x
            var lst = new List<object>() { 2, 5 };
            var term = new Term(Expression.Multiply, lst);
            var lst1 = new List<object>() { term, x };
            var term1 = new Term(Expression.Add, lst1);
            object obj = term1.Eval();
            Assert.NotNull(obj);
            var gTerm = obj as Term;
            Assert.NotNull(gTerm);
            var glst = gTerm.Args as List<object>;
            Assert.NotNull(glst);
            Assert.True(glst.Count == 2);
            Assert.True(term1.Traces.Count == 2);
        }

        #endregion
    }
}

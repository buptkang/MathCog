using System;
using System.Collections.Generic;
using System.Globalization;
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
            var term1 = new Term(Expression.Add, new List<object>() { 1, 1 });
            var x = new Var('x');
            var term2 = new Term(Expression.Add, new List<object>() {1,x});

            var dict = new Dictionary<object, object>();
            bool result = LogicSharp.Unify(term1,term2, dict);

            Assert.True(result);
            Assert.True(dict.Count == 1); 
        }

        [Test]
        public void Test_print()
        {
            var variable = new Var('x');
            var term = new Term(Expression.Multiply, new Tuple<object, object>(1, variable));

            Assert.True(term.ToString().Equals("(1*x)"));
        }

        [Test]
        public void Test_containVar()
        {
            var variable = new Var('x');
            var term = new Term(Expression.Add, new Tuple<object,object>(variable, 2));
            Assert.True(term.ContainsVar(variable));

            var term2 = new Term(Expression.Add, new Tuple<object, object>(term, 1.0));
            Assert.True(term2.ContainsVar(variable));
        }

        [Test]
        public void Test_Clone()
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
            var term1 = term.Clone();

            var args = term.Args as List<object>;
            Assert.NotNull(args);
            args[0] = 2;

            var args1 = term1.Args as List<object>;
            Assert.NotNull(args1);
            Assert.True(args1[0].Equals(1));
        }

        [Test]
        public void Test_Equal()
        {
            var x = new Var('x');
            var term1 = new Term(Expression.Add, new List<object>() {x, 1});
            var term2 = new Term(Expression.Add, new List<object>() { x, 1 });
            Assert.True(term1.Equals(term2));
        }

        [Test]
        public void Test_Reconstruct()
        {
            var term = new Term(Expression.Add, new List<object>() {1});
            Assert.True(term.ReConstruct().Equals(1));
        }

    }
}
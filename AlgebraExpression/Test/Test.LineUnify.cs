using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlgebraGeometry;
using CSharpLogic;
using NUnit.Framework;
using System.Linq.Expressions;

namespace AlgebraExpression.Test
{
    [TestFixture]
    public class TestLineUnify
    {
        [Test]
        public void Test1()
        {
            //x=2
            object obj1 = new Var("x");
            object obj2 = 2;

            Line line;
            bool result = LineEvaluator.Unify(obj1, obj2, out line);
            Assert.True(result);
            Assert.True(line.Concrete);
            Assert.True(line.A.Equals(1.0));
            Assert.Null(line.B);
            Assert.True(line.C.Equals(-2.0));
        }

        [Test]
        public void Test1_1()
        {
            //ax=2
            var a = new Var('a');
            object obj1 = new Var("x");
            var term = new Term(Expression.Multiply, new Tuple<object, object>(a, obj1));
            object obj2 = 2;

            //need to rectify the term
            //change a from Var to string because line rule

            Line line;
            bool result = LineEvaluator.Unify(term, obj2, out line);
            Assert.True(result);
            Assert.False(line.Concrete);
            Assert.True(line.A.Equals(a));
            Assert.Null(line.B);
            Assert.True(line.C.Equals(-2.0));
        }

        [Test]
        public void Test1_2()
        {
            //bx=2
            object obj1 = new Var("x");
            object a = new Var('b');
            var term = new Term(Expression.Multiply, new Tuple<object, object>(a, obj1));
            object obj2 = 2;

            Line line;
            bool result = LineEvaluator.Unify(term, obj2, out line);
            Assert.True(result);
            Assert.False(line.Concrete);
            Assert.True(line.A.Equals(a));
            Assert.Null(line.B);
            Assert.True(line.C.Equals(-2.0));
        }


        [Test]
        public void Test2()
        {
            //y=1
            object obj1 = new Var("y");
            object obj2 = 1;

            Line line;
            bool result = LineEvaluator.Unify(obj1, obj2, out line);
            Assert.True(result);
            Assert.True(line.Concrete);
            Assert.Null(line.A);
            Assert.True(line.B.Equals(1.0));
            Assert.True(line.C.Equals(-1.0));
        }

        [Test]
        public void Test3()
        {
            //2x+1=0
            var x = new Var('x');
            var term = new Term(Expression.Multiply, new Tuple<object, object>(2, x));
            var obj1 = new Term(Expression.Add, new Tuple<object, object>(term, 1));
            const int obj2 = 0;

            Line line;
            bool result = LineEvaluator.Unify(obj1, obj2, out line);
            Assert.True(result);
            Assert.True(line.Concrete);
            Assert.True(line.A.Equals(2.0));
            Assert.Null(line.B);
            Assert.True(line.C.Equals(1.0));
        }

        [Test]
        public void Test4()
        {
            //ax+1=0
            const string a = "a";
            var x = new Var('x');
            var term = new Term(Expression.Multiply, new Tuple<object, object>(a, x));
            var obj1 = new Term(Expression.Add, new Tuple<object, object>(term, 1));
            const int obj2 = 0;

            Line line;
            bool result = LineEvaluator.Unify(obj1, obj2, out line);
            Assert.True(result);
            Assert.False(line.Concrete);
            Assert.True(line.A.ToString().Equals(a));
            Assert.Null(line.B);
            Assert.True(line.C.Equals(1.0));

            //bx+1=0
            const string b = "b";
            x = new Var('x');
            term = new Term(Expression.Multiply, new Tuple<object, object>(b, x));
            obj1 = new Term(Expression.Add, new Tuple<object, object>(term, 1));

            result = LineEvaluator.Unify(obj1, obj2, out line);
            Assert.True(result);
            Assert.False(line.Concrete);
            Assert.True(line.A.ToString().Equals(b));
            Assert.Null(line.B);
            Assert.True(line.C.Equals(1.0));
        }

        [Test]
        public void Test5()
        { 
            //ax+by+1=0
            const string a = "a";
            var x = new Var('x');
            var term = new Term(Expression.Multiply, new Tuple<object, object>(a, x));
            string b = "b";
            var y = new Var('y');
            var term2 = new Term(Expression.Multiply, new Tuple<object, object>(b, y));
            var term3 = new Term(Expression.Add, new Tuple<object, object>(term, term2));
            var term4 = new Term(Expression.Add, new Tuple<object, object>(term3, 1));

            Line line;
            bool result = LineEvaluator.Unify(term4, 0, out line);
            Assert.True(result);
            Assert.False(line.Concrete);
            Assert.True(line.A.ToString().Equals(a));
            Assert.True(line.B.ToString().Equals(b));
            Assert.True(line.C.Equals(1.0));

            // 2x+ay+1=0
            b = "a";
            term = new Term(Expression.Multiply, new Tuple<object, object>(2, x));
            term2 = new Term(Expression.Multiply, new Tuple<object, object>(b, y));
            term3 = new Term(Expression.Add, new Tuple<object, object>(term, term2));
            term4 = new Term(Expression.Add, new Tuple<object, object>(term3, 1));

            result = LineEvaluator.Unify(term4, 0, out line);
            Assert.True(result);
            Assert.False(line.Concrete);
            Assert.True(line.A.Equals(2.0));
            Assert.True(line.B.ToString().Equals(b));
            Assert.True(line.C.Equals(1.0));
        }

        [Test]
        public void Test6()
        {
            //a=2
            const string a = "a";
            Line line;
            bool result = LineEvaluator.Unify(a, 2, out line);
            Assert.False(result);
        }

        [Test]
        public void Test7()
        {
            //aX+bY+1=0
            const string a = "a";
            var x = new Var('X');
            var term = new Term(Expression.Multiply, new Tuple<object, object>(a, x));
            string b = "b";
            var y = new Var('Y');
            var term2 = new Term(Expression.Multiply, new Tuple<object, object>(b, y));
            var term3 = new Term(Expression.Add, new Tuple<object, object>(term, term2));
            var term4 = new Term(Expression.Add, new Tuple<object, object>(term3, 1));

            Line line;
            bool result = LineEvaluator.Unify(term4, 0, out line);
            Assert.True(result);
            Assert.False(line.Concrete);
            Assert.True(line.A.ToString().Equals(a));
            Assert.True(line.B.ToString().Equals(b));
            Assert.True(line.C.Equals(1.0));

            // 2X+aY+1=0
            b = "a";
            term = new Term(Expression.Multiply, new Tuple<object, object>(2, x));
            term2 = new Term(Expression.Multiply, new Tuple<object, object>(b, y));
            term3 = new Term(Expression.Add, new Tuple<object, object>(term, term2));
            term4 = new Term(Expression.Add, new Tuple<object, object>(term3, 1));

            result = LineEvaluator.Unify(term4, 0, out line);
            Assert.True(result);
            Assert.False(line.Concrete);
            Assert.True(line.A.Equals(2.0));
            Assert.True(line.B.ToString().Equals(b));
            Assert.True(line.C.Equals(1.0));
        }
    }
}
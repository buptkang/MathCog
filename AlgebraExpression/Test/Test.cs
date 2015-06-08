using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpLogic;
using System.Linq.Expressions;
using NUnit.Framework;

namespace AlgebraExpression.Test
{
    class Test
    {
        [Test]
        public void Test_Flattern()
        {
            /*
            var x = new Var('x');
            var a = new Term(Expression.Add, new Tuple<object, object>(x, 1));
            var b = new Term(Expression.Add, new Tuple<object, object>(a, x));
            var c = new Term(Expression.Add, new Tuple<object, object>(b, 2));

            Term obj;
            bool result = c.Flattern(out obj);
            Assert.True(result);
            Assert.NotNull(obj);
            Assert.IsInstanceOf(typeof(List<object>), obj.Args);
            var lst = obj.Args as List<object>;
            Assert.NotNull(lst);
            Assert.True(lst.Count == 4);
             */ 
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using CSharpLogic;

namespace CSharpLogic.Test
{
    [TestFixture]
    public class TestVariable
    {
        [Test]
        public void Test_IsVar()
        {
            Assert.IsTrue(Var.IsVar(new Var(3)));
            Assert.IsFalse(Var.IsVar(3));

            Assert.IsTrue((new Var(1)).Equals(new Var(1)));
            Assert.IsFalse((new Var()).Equals(new Var()));
        }

        [Test]
        public void Test_IsVar_2()
        {
            //var itself
            object ob2 = new Var('1');
            Assert.True(Var.ContainsVar(ob2));

            //Tuple
            object ob1 = new Tuple<object, object>(new Var('x'), 1);
            object ob0 = new Tuple<object>(1);
            Assert.True(Var.ContainsVar(ob1));
            Assert.False(Var.ContainsVar(ob0));

            //IEnumerable
            object ob3 = new List<object>() {1, 1, 1};
            object ob4 = new List<object>() { new Var('x'), 1 };
            Assert.False(Var.ContainsVar(ob3));
            Assert.True(Var.ContainsVar(ob4));

            //Dictionary
            var ob5 = new Dictionary<object, object>();
            ob5.Add(new Var('y'), 1);
            var ob6 = new Dictionary<object, object>();
            ob6.Add(2,3);
            Assert.True(Var.ContainsVar(ob5));
            Assert.False(Var.ContainsVar(ob6));
        }
    }
}

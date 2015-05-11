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
    }
}

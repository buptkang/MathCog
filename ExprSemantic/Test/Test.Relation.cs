using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlgebraGeometry;
using AlgebraGeometry.Expr;
using NUnit.Framework;

namespace ExprSemantic
{
    [TestFixture]
    public class TestRelation
    {
        [Test]
        public void Test1()
        {
            var reasoner = new Reasoner();
            const string fact1 = "A(1,1)";
            reasoner.Load(fact1);

            const string fact2 = "B(2,1)";
            reasoner.Load(fact2);

            //Uniform Search Input(Sketh or Touch)
            List<AGShapeExpr> selectFacts = reasoner.TestGetShapeFacts();
            Assert.True(selectFacts.Count == 2);
            const string query = "AB";
            object result = reasoner.Load(query);

            var types = result as List<ShapeType>;
            Assert.NotNull(types);
        }
    }
}

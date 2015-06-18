using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            const string fact1 = "(1,1)";
            Reasoner.Instance.Load(fact1);

            const string fact2 = "(2,1)";
            Reasoner.Instance.Load(fact2);

            //Uniform Search Input(Sketh or Touch)
            List<IKnowledge> selectFacts = Reasoner.Instance.TestGetKnowledgeFacts();

            Assert.True(selectFacts.Count == 2);
            var tupleKnowledge = new Tuple<IKnowledge, IKnowledge>(selectFacts[0], selectFacts[1]);

            const string query = "AB";

            //pattern-match or generate
            object result;
            bool result = Reasoner.Instance.InferRelation(null, tupleKnowledge, out result);

        }
    }
}

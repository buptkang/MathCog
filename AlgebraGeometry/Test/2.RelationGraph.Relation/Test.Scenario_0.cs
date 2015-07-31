using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using CSharpLogic;
using NUnit.Framework;

namespace AlgebraGeometry
{
    [TestFixture]
    public partial class TestProblemSolvingScenarios
    {
        [Test]
        public void TestScenario0_CSP_1()
        {
            /*
             * 1: a=1
             * 2: a=?
             */
            var variable = new Var('a');
            var eqGoal = new EqGoal(variable, 1);
            var query = new Query(variable);
            var graph = new RelationGraph();

            graph.AddNode(eqGoal);
            var queryNode = graph.AddNode(query) as QueryNode;
            Assert.NotNull(queryNode);
            Assert.True(queryNode.InternalNodes.Count == 1);
            Assert.True(query.CachedEntities.Count == 1);
        }
    }
}
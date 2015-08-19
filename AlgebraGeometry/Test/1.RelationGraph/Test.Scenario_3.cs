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
        public void TestScenario3_CSP_1()
        {
            /*
             * Input sequence:
             * 1. y = 2x + 1
             * 2: m = ?
             */
            var graph = new RelationGraph();
            var line = new Line(2, 1);
            var lineSymbol = new LineSymbol(line);
            Assert.True(line.InputType == LineType.SlopeIntercept);
            graph.AddNode(lineSymbol);
            var variable = new Var('m');
            var query = new Query(variable);
            GraphNode gn = graph.AddNode(query);
            var qn = gn as QueryNode;
            Assert.True(qn != null);

            Assert.True(qn.InternalNodes.Count == 1);
            var goalNode = qn.InternalNodes[0] as GoalNode;
            Assert.NotNull(goalNode);
            var eqGoal = goalNode.Goal as EqGoal;
            Assert.NotNull(eqGoal);
            Assert.True(eqGoal.Rhs.Equals(2.0));
            Assert.True(eqGoal.Lhs.Equals(variable));

            //Output Usage
            Assert.True(query.CachedEntities.Count == 1);
            var cachedGoal = query.CachedEntities.ToList()[0] as EqGoal;
            Assert.NotNull(cachedGoal);
            Assert.True(cachedGoal.Rhs.Equals(2.0));
        }

        [Test]
        public void TestScenario3_CSP_2()
        {
            /*
             * Input sequence:
             * 1. 2x + y + 1 = 0
             * 2: m = ?
             */
            var graph = new RelationGraph();
            var line = new Line(2, 1, 1);
            var lineSymbol = new LineSymbol(line);
            Assert.True(line.InputType == LineType.GeneralForm);
            graph.AddNode(lineSymbol);
            var variable = new Var('m');
            var query = new Query(variable);
            var qn = graph.AddNode(query) as QueryNode;
            Assert.True(qn != null);
            Assert.True(qn.InternalNodes.Count == 1);
            var goalNode = qn.InternalNodes[0] as GoalNode;
            Assert.NotNull(goalNode);
            var eqGoal = goalNode.Goal as EqGoal;
            Assert.NotNull(eqGoal);
            Assert.True(eqGoal.Rhs.Equals(-2));
            Assert.True(eqGoal.Lhs.Equals(variable));
            //Output Usage
            Assert.True(query.CachedEntities.Count == 1);
            var cachedGoal = query.CachedEntities.ToList()[0] as EqGoal;
            Assert.NotNull(cachedGoal);
            Assert.True(cachedGoal.Rhs.Equals(-2));
        }

        [Test]
        public void TestScenario3_CSP_3()
        {
            var graph = new RelationGraph();
            var m = new Var('m');
            var k = new Var('k');
            var eqGoal1 = new EqGoal(m, 3); //m=3
            var eqGoal2 = new EqGoal(k, 2); //k=2
            graph.AddNode(eqGoal1);
            graph.AddNode(eqGoal2);
            var query = new Query(ShapeType.Line);
            var qn = graph.AddNode(query) as QueryNode;
            Assert.NotNull(qn);
            Assert.NotNull(qn.Query);
            Assert.True(qn.Query.Equals(query));
            Assert.True(query.Success);
            Assert.Null(query.FeedBack);
            Assert.True(qn.InternalNodes.Count == 1);
            var sn = qn.InternalNodes[0] as ShapeNode;
            Assert.NotNull(sn);
            var ls = sn.ShapeSymbol as LineSymbol;
            Assert.NotNull(ls);
            Assert.True(ls.ToString().Equals("y=3x+2"));

            //Output Usage
            Assert.True(query.CachedEntities.Count == 1);
            ls = query.CachedEntities.ToList()[0] as LineSymbol;
            Assert.NotNull(ls);
            Assert.True(ls.ToString().Equals("y=3x+2"));
        }

        [Test]
        public void TestScenario3_CSP_4()
        {
            var graph = new RelationGraph();
            var m = new Var('m');
            var k = new Var('k');
            var eqGoal1 = new EqGoal(m, 3); //m=3
            var eqGoal3 = new EqGoal(m, 4); //m=4
            var eqGoal2 = new EqGoal(k, 2); //k=2
            graph.AddNode(eqGoal1);
            graph.AddNode(eqGoal2);
            graph.AddNode(eqGoal3);
            var query = new Query(ShapeType.Line);
            var qn = graph.AddNode(query) as QueryNode;
            Assert.NotNull(qn);
            Assert.NotNull(qn.Query);
            Assert.True(qn.Query.Equals(query));
            Assert.True(query.Success);
            Assert.Null(query.FeedBack);
            Assert.True(qn.InternalNodes.Count == 2);

            Assert.True(query.CachedEntities.Count == 2);
        }
    }
}

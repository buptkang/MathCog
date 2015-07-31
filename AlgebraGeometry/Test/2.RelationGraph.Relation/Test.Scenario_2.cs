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
        #region Constraint Solving Issue case 1-4

        [Test]
        public void TestScenario2_CSP_1()
        {
            /*
             * 1: A(2,3) [Point] => Point
             * 2: B(3,4) [Point] => Point
             * 3: AB [Label]     => [Line, LineSegment]
             */
            var graph = new RelationGraph();
            var ptA = new Point("A", 2, 3);
            var ptASymbol = new PointSymbol(ptA);
            var ptB = new Point("B", 3, 4);
            var ptBSymbol = new PointSymbol(ptB);
            graph.AddNode(ptASymbol);
            graph.AddNode(ptBSymbol);
            const string label = "AB";
            var query = new Query(label);
            var qn = graph.AddNode(query);
			Assert.Null(qn);
			Assert.False(query.Success);
			Assert.NotNull(query.FeedBack);
            var types = query.FeedBack as List<ShapeType>;
            Assert.NotNull(types);
            Assert.True(types.Count == 2);
            var shapes = graph.RetrieveShapes();
            Assert.True(shapes.Count == 2);
        }

		[Test]
        public void TestScenario2_CSP_2()
        {
            /*
               * 1: A(2,3) [Point] => Point
               * 2: B(3,4) [Point] => Point
               * 3: AB [Label]     => [Line, LineSegment]
             */
            var graph = new RelationGraph();
            var ptA = new Point("A", 2, 3);
		    var ptASymbol = new PointSymbol(ptA);
            var ptB = new Point("B", 3, 4);
		    var ptBSymbol = new PointSymbol(ptB);
            graph.AddNode(ptASymbol);
            graph.AddNode(ptBSymbol);
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
            Assert.True(ls.ToString().Equals("x-y+1=0"));
        }

        [Test]
        public void TestScenario2_CSP_3()
        {
            /*
           * 1: A(2,3) [Point] => Point
           * 2: B(3,4) [Point] => Point
           * 3: AB [Label]     => [Line, LineSegment]
          */
            var graph = new RelationGraph();
            var ptA = new Point("A", 2, 3);
            var ptASymbol = new PointSymbol(ptA);
            var ptB = new Point("B", 3, 4);
            var ptBSymbol = new PointSymbol(ptB);
            graph.AddNode(ptASymbol);
            graph.AddNode(ptBSymbol);
            const string label = "AB";
            var query = new Query(label, ShapeType.Line);
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
            Assert.True(ls.ToString().Equals("x-y+1=0"));
        }

		[Test]
        public void TestScenario2_CSP_4()
        {
            /*
			  * 1: A(2,3) [Point] => Point
			  * 2: B(3,4) [Point] => Point
			  * 3: AB [Label]     => [Line, LineSegment]
			 */
            var graph = new RelationGraph();
            var ptA = new Point("A", 2, 3);
            var ptASymbol = new PointSymbol(ptA);
            var ptB = new Point("B", 3, 4);
            var ptBSymbol = new PointSymbol(ptB);
            graph.AddNode(ptASymbol);
            graph.AddNode(ptBSymbol);
            const string label = "AB";
            var query = new Query(label);
		    var qn = graph.AddNode(query) as QueryNode;
            Assert.Null(qn);
            Assert.False(query.Success);
            Assert.NotNull(query.FeedBack);
            var types = query.FeedBack as List<ShapeType>;
            Assert.NotNull(types);
            Assert.True(types.Count == 2);
            var shapes = graph.RetrieveShapes();
            Assert.True(shapes.Count == 2);
            Assert.True(graph.Nodes.Count == 2);
           
		    query.Constraint2 = ShapeType.Line;
		    var queryNode = graph.RetrieveQueryNode(query);
            Assert.NotNull(queryNode);
            Assert.True(queryNode.Query.Success);
            Assert.Null(query.FeedBack);
            Assert.True(queryNode.InternalNodes.Count == 1);
            var sn = queryNode.InternalNodes[0] as ShapeNode;
            Assert.NotNull(sn);
            var ls = sn.ShapeSymbol as LineSymbol;
            Assert.NotNull(ls);
            Assert.True(ls.ToString().Equals("x-y+1=0"));
            Assert.True(graph.Cache.Count == 3);

		    query.Constraint2 = ShapeType.LineSegment;
            queryNode = graph.RetrieveQueryNode(query);
            Assert.NotNull(queryNode);
            Assert.True(queryNode.Query.Success);
            Assert.Null(query.FeedBack);
            Assert.True(queryNode.InternalNodes.Count == 1);
            sn = queryNode.InternalNodes[0] as ShapeNode;
            Assert.NotNull(sn);
            var lls = sn.ShapeSymbol as LineSegmentSymbol;
            Assert.NotNull(lls);
        }	

        #endregion

        #region Relation Building
        
        [Test]
        public void TestScenario2_Relation_1()
        {
            /*
			 * 1: A(2,3) [Point] => Point
			 * 2: B(3,4) [Point] => Point
			 * 3: AB [Label]     => [Line, LineSegment]
			 * 4: ask slope = ?
			 */
            var graph = new RelationGraph();
            var ptA = new Point("A", 2, 3);
            var ptASymbol = new PointSymbol(ptA);
            var ptB = new Point("B", 3, 4);
            var ptBSymbol = new PointSymbol(ptB);
            graph.AddNode(ptASymbol);
            graph.AddNode(ptBSymbol);
            const string label = "AB";
            var query = new Query(label, ShapeType.Line);
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
            Assert.True(ls.ToString().Equals("x-y+1=0"));
            var shapes = graph.RetrieveShapeNodes();
            Assert.True(shapes.Count == 3);

            var variable = new Var('m');
            var query1 = new Query(variable); //m=
            qn = graph.AddNode(query1) as QueryNode;
            Assert.NotNull(qn);
            Assert.NotNull(qn.Query);
            Assert.True(query.Success);
            Assert.Null(query.FeedBack);
            Assert.True(qn.InternalNodes.Count == 1);
            var gn = qn.InternalNodes[0] as GoalNode;
            Assert.NotNull(gn);
            var eqGoal = gn.Goal as EqGoal;
			Assert.NotNull(eqGoal);
			Assert.True(eqGoal.Lhs.Equals(variable));
			Assert.True(eqGoal.Rhs.Equals(1));

            shapes = graph.RetrieveShapeNodes();
            Assert.True(shapes.Count == 3);
        }

        #endregion

        #region Input Sequence Issue (Caching), CRUD function

        [Test]
        public void TestScenario2_SequenceInput_1()
        {
            /*
             * Input sequence:
             * 1: Hat(AB) [Line]  => Line
             * 2: A(2,3)          => Point
             * 3: B(3,4)          => Point
             * Update: AB [Label] => Line
             */
            var graph = new RelationGraph();
            const string label = "AB";
            var query = new Query(label, ShapeType.Line);
            var qn = graph.AddNode(query);
			Assert.Null(qn);
			Assert.False(query.Success);

            var ptA = new Point("A", 2, 3);
            var ptASymbol = new PointSymbol(ptA);
            var ptB = new Point("B", 3, 4);
            var ptBSymbol = new PointSymbol(ptB);
            graph.AddNode(ptASymbol);
            graph.AddNode(ptBSymbol);

            var shapes = graph.RetrieveShapeSymbols(ShapeType.Line);
			Assert.True(shapes.Count == 1);
            var line = shapes[0] as LineSymbol;
			Assert.NotNull(line);
            Assert.True(line.ToString().Equals("x-y+1=0"));
        }

        #endregion
    }
}

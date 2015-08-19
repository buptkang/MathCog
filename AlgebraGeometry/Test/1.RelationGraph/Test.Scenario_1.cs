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
        public void TestScenario_1()
        {
            /*
             * Problem 1: Find the distance betweeen A(2,0) and B(5,4)?
             */

            var pt1 = new Point("A", 2, 0);
            var pt1Symbol = new PointSymbol(pt1);
            var pt2 = new Point("B", 5, 4);
            var pt2Symbol = new PointSymbol(pt2);

            var graph = new RelationGraph();
            graph.AddNode(pt1Symbol);
            graph.AddNode(pt2Symbol);

            var query = new Query("AB");
            var queryNode = graph.AddNode(query) as QueryNode;
            Assert.Null(queryNode);
            var tuple = graph.Cache[query.QueryQuid] as Tuple<Query, object>;
            Assert.NotNull(tuple);
            var types = tuple.Item2 as List<ShapeType>;
            Assert.NotNull(types);

            query.Constraint2 = ShapeType.LineSegment;

        }

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
    }
}
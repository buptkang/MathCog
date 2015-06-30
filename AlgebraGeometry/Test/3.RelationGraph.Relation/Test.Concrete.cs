using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSharpLogic;
using NUnit.Framework;

namespace AlgebraGeometry
{
    [TestFixture]
    public class TestRelation_Concrete
    {
        #region CRUD Input

        [Test]
        public void Test_Line_CRUD_1()
        {
            /*
             * Input sequence:
             * 1: Hat(AB) [Line]  => Line
             * 2: A(2,3)          => Point
             * 3: B(3,4)          => Point
             * Update: AB [Label] => Line
             */
            var graph = new RelationGraph();
            var line = new Line("AB");
            Assert.True(line.RelationStatus);
            graph.AddNode(line); //api call
            Assert.True(graph.Nodes.Count == 1);

            var A = new Point("A", 2, 3);
            graph.AddNode(A); //api call
            Assert.True(graph.Nodes.Count == 2);

            List<ShapeNode> nodes = graph.RetrieveShapeNodes(ShapeType.Line);
            Assert.True(nodes.Count == 1);
            Assert.True(nodes[0].InEdges.Count == 1);
            Assert.True(nodes[0].OutEdges.Count == 0);

            nodes = graph.RetrieveShapeNodes(ShapeType.Point);
            Assert.True(nodes.Count == 1);
            Assert.True(nodes[0].InEdges.Count == 0);
            Assert.True(nodes[0].OutEdges.Count == 1);

            var B = new Point("B", 3, 4);
            graph.AddNode(B); //api call
            Assert.True(graph.Nodes.Count == 3);

            nodes = graph.RetrieveShapeNodes(ShapeType.Line);
            Assert.True(nodes.Count == 1);
            Assert.True(nodes[0].InEdges.Count == 2);
            Assert.True(nodes[0].OutEdges.Count == 0);

            var lineObj = nodes[0].Shape as Line;
            Assert.NotNull(lineObj);
            Assert.True(lineObj.Label.Equals("AB"));
            Assert.True(lineObj.RelationStatus);
            Assert.True(lineObj.CachedSymbols.Count == 1);
            var lineImpl = lineObj.CachedSymbols.ToList()[0] as Line;
            Assert.NotNull(lineImpl);
            Assert.True(lineImpl.A.Equals(1.0));
            Assert.True(lineImpl.B.Equals(-1.0));
            Assert.True(lineImpl.C.Equals(1.0));
        }

        [Test]
        public void Test_Line_CRUD_2()
        {
            /*
            * Input sequence:
            * 1: A(2,3) [Point] => Point
            * 2: B(2,3) [Point] => Point
            * 3: Hat(AB)[Line]  => Line
            */
            var graph = new RelationGraph();
            //point identity test
            var A = new Point("A", 2, 3);
            var B = new Point("B", 2, 3);
            graph.AddNode(A);
            graph.AddNode(B);
            var line = new Line("AB");
            graph.AddNode(line);

            List<ShapeNode> nodes = graph.RetrieveShapeNodes(ShapeType.Line);
            Assert.True(nodes.Count == 1);
            Assert.True(nodes[0].InEdges.Count == 2);
            Assert.True(nodes[0].OutEdges.Count == 0);

            var lineObj = nodes[0].Shape as Line;
            Assert.NotNull(lineObj);
            Assert.True(lineObj.Label.Equals("AB"));
            Assert.True(lineObj.RelationStatus);
            Assert.True(lineObj.CachedSymbols.Count == 0);

        }

        [Test]
        public void Test_LineSegment_CRUD_1()
        {
            /*
             * Input sequence:
             * 1: (AB) [LineSegment]  => LineSegment
             * 2: A(2,3)          => Point
             * 3: B(3,4)          => Point
             * Update: AB [Label] => LineSegment
             */
            var graph = new RelationGraph();
            var lineSeg = new LineSegment("AB");
            Assert.True(lineSeg.RelationStatus);
            graph.AddNode(lineSeg); //api call
            Assert.True(graph.Nodes.Count == 1);

            var A = new Point("A", 2, 3);
            graph.AddNode(A); //api call
            Assert.True(graph.Nodes.Count == 2);

            List<ShapeNode> nodes = graph.RetrieveShapeNodes(ShapeType.LineSegment);
            Assert.True(nodes.Count == 1);
            Assert.True(nodes[0].InEdges.Count == 1);
            Assert.True(nodes[0].OutEdges.Count == 0);

            nodes = graph.RetrieveShapeNodes(ShapeType.Point);
            Assert.True(nodes.Count == 1);
            Assert.True(nodes[0].InEdges.Count == 0);
            Assert.True(nodes[0].OutEdges.Count == 1);

            var B = new Point("B", 3, 4);
            graph.AddNode(B); //api call
            Assert.True(graph.Nodes.Count == 3);

            nodes = graph.RetrieveShapeNodes(ShapeType.LineSegment);
            Assert.True(nodes.Count == 1);
            Assert.True(nodes[0].InEdges.Count == 2);
            Assert.True(nodes[0].OutEdges.Count == 0);

            var lineObj = nodes[0].Shape as LineSegment;
            Assert.NotNull(lineObj);
            Assert.True(lineObj.Label.Equals("AB"));
            Assert.NotNull(lineObj.Pt1);
            Assert.True(lineObj.Pt1.Equals(A));
            Assert.NotNull(lineObj.Pt2);
            Assert.True(lineObj.Pt2.Equals(B));
            Assert.True(lineObj.RelationStatus);
            Assert.True(lineObj.CachedSymbols.Count == 1);
        }

        [Test]
        public void Test_LineSegment_CRUD_2()
        {
            /*
            * Input sequence:
            * 1: (AB) [LineSegment]  => LineSegment
            * 2: A(2,3)          => Point
            * 3: B(2,3)          => Point
            * Update: AB [Label] => LineSegment
            */
            var graph = new RelationGraph();
            var lineSeg = new LineSegment("AB");
            Assert.True(lineSeg.RelationStatus);
            graph.AddNode(lineSeg); //api call
            Assert.True(graph.Nodes.Count == 1);

            var A = new Point("A", 2, 3);
            graph.AddNode(A); //api call
            Assert.True(graph.Nodes.Count == 2);

            List<ShapeNode> nodes = graph.RetrieveShapeNodes(ShapeType.LineSegment);
            Assert.True(nodes.Count == 1);
            Assert.True(nodes[0].InEdges.Count == 1);
            Assert.True(nodes[0].OutEdges.Count == 0);

            nodes = graph.RetrieveShapeNodes(ShapeType.Point);
            Assert.True(nodes.Count == 1);
            Assert.True(nodes[0].InEdges.Count == 0);
            Assert.True(nodes[0].OutEdges.Count == 1);

            var B = new Point("B", 2, 3);
            graph.AddNode(B); //api call
            Assert.True(graph.Nodes.Count == 3);

            nodes = graph.RetrieveShapeNodes(ShapeType.LineSegment);
            Assert.True(nodes.Count == 1);
            Assert.True(nodes[0].InEdges.Count == 2);
            Assert.True(nodes[0].OutEdges.Count == 0);

            var lineObj = nodes[0].Shape as LineSegment;
            Assert.NotNull(lineObj);
            Assert.True(lineObj.Label.Equals("AB"));
            Assert.True(lineObj.RelationStatus);
            Assert.True(lineObj.CachedSymbols.Count == 0);
        }

        #endregion

        #region Selection Input

        [Test]
        public void Test_Line_Selection_Deterministic1()
        {
            var graph = new RelationGraph();
            //point identity test
            var A = new Point(2.0, 3.0);
            var B = new Point(2.0, 3.0);
            graph.AddNode(A);
            graph.AddNode(B);

            object output;
            bool result = graph.AddRelation
                (new Tuple<object, object>(A, B), ShapeType.Line, out output);
            Assert.False(result);
            var str = output as String;
            Assert.NotNull(str);
            Assert.True(str.Equals(LineGenerationRule.IdentityPoints));
        }

        [Test]
        public void Test_Line_Selection_Deterministic2()
        {
            var graph = new RelationGraph();
            var A = new Point(2.0, 3.0);
            var B = new Point(2.0, -1.0);
            graph.AddShapeNode(A);
            graph.AddShapeNode(B);
            object output;
            bool result = graph.AddRelation(new Tuple<object, object>(A, B), ShapeType.Line, out output);
            Assert.True(result);
            var shape = output as Line;
            Assert.NotNull(shape);

            var shapes = graph.RetrieveShapes();
            Assert.True(shapes.Count == 3);
        }

        [Test]
        public void Test_Line_Selection_Deterministic3()
        {
            var graph = new RelationGraph();
            var A = new Point(1.0, -1.0);
            var B = new Point(2.0, -1.0);
            graph.AddShapeNode(A);
            graph.AddShapeNode(B);
            object output;
            bool result = graph.AddRelation(new Tuple<object, object>(A, B), ShapeType.Line, out output);
            Assert.True(result);
            var shape = output as Line;
            Assert.NotNull(shape);
            var shapes = graph.RetrieveShapes();
            Assert.True(shapes.Count == 3);
        }

        [Test]
        public void Test_Line_Selection_Deterministic4()
        {
            var graph = new RelationGraph();
            var A = new Point(2.0, -2.0);
            var B = new Point(2.0, -1.0);
            graph.AddShapeNode(A);
            graph.AddShapeNode(B);
            object output;
            bool result = graph.AddRelation(new Tuple<object, object>(A, B), ShapeType.Line, out output);
            Assert.True(result);
            var shape = output as Line;
            Assert.NotNull(shape);
            var shapes = graph.RetrieveShapes();
            Assert.True(shapes.Count == 3);
        }

        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSharpLogic;
using NUnit.Framework;
using AlgebraGeometry;

namespace GeometryLogicInference
{
    [TestFixture]
    public class TestPatterMatchSenarios
    {
        [Test]
        public void TestScenario1_1()
        {
            /*            
             * Input sequence:
             * 1: A(x,2) [Point]     => Point
             * 2: x=2 [Goal, Line]   => Goal
             * 
             * Delete sequence:
             * 1. delete A(x,2) => x=2 [Goal, Line] => Line
             * 
             */
            var variable = new Var('x');
            var pt1 = new Point("A", variable, 2.0);
            var opt1 = new Line(1.0, null, -2.0);
            var opt2 = new EqGoal(variable, 2.0);
            var opts = new List<object> {opt1, opt2};

            object pt1Obj = GeometryInference.Instance.Add(pt1);
            var pt1Shape = pt1Obj as Point;
            Assert.NotNull(pt1Shape);
            Assert.True(pt1Shape.Equals(pt1));
            Assert.True(GeometryInference.Instance.CurrentGraph.Nodes.Count == 1);

            object optsObj = GeometryInference.Instance.Add(opts);
            var goal = optsObj as EqGoal;
            Assert.NotNull(goal);
            Assert.True(goal.Equals(opt2));
            Assert.True(GeometryInference.Instance.CurrentGraph.Nodes.Count == 2);

            List<Shape> lines = GeometryInference.Instance.CurrentGraph.RetrieveSpecicShapes(ShapeType.Line);
            Assert.True(lines.Count == 0);

            List<Shape> points = GeometryInference.Instance.CurrentGraph.RetrieveSpecicShapes(ShapeType.Point);
            Assert.True(points.Count == 1);
            var point = points[0] as Point;
            Assert.NotNull(point);
            Assert.True(point.CachedSymbols.Count == 1);

            GeometryInference.Instance.Delete(pt1);
            lines = GeometryInference.Instance.CurrentGraph.RetrieveSpecicShapes(ShapeType.Line);
            Assert.True(lines.Count == 1);

        }

        [Test]
        public void TestScenario1_2()
        {
            /*
             * Input sequence:
             * 1: x=2 [Goal, Line]   => Line
             */
            var variable = new Var('x');
            var opt1 = new Line(1.0, null, -2.0);
            var opt2 = new EqGoal(variable, 2.0);
            var opts = new List<object> { opt1, opt2 };

            object optsObj = GeometryInference.Instance.Add(opts);
            var line = optsObj as Line;
            Assert.NotNull(line);
            Assert.True(line.Equals(opt1));
            Assert.True(GeometryInference.Instance.CurrentGraph.Nodes.Count == 1);

            var lines = GeometryInference.Instance.CurrentGraph.RetrieveSpecicShapes(ShapeType.Line);
            Assert.True(lines.Count == 1);
        }

        [Test]
        public void TestScenario1_3()
        {
            /*
             * Input sequence:
             * 1: x=2 [Goal, Line]      => Line
             * 2: A(x,2) [Point]        => Point
             * Update: x=2 [Goal, Line] => Goal 
             */
            var variable = new Var('x');
            var opt1 = new Line(1.0, null, -2.0);
            var opt2 = new EqGoal(variable, 2.0);
            var opts = new List<object> { opt1, opt2 };
            object optsObj = GeometryInference.Instance.Add(opts);
            Assert.True(GeometryInference.Instance.Cache.Count == 1);

            var pt1 = new Point("A", variable, 2.0);
            object pt1Obj = GeometryInference.Instance.Add(pt1);
            List<Shape> lines = GeometryInference.Instance.CurrentGraph.RetrieveSpecicShapes(ShapeType.Line);
            Assert.True(lines.Count == 0);

            List<Goal> goals = GeometryInference.Instance.CurrentGraph.RetrieveGoals();
            Assert.True(goals.Count == 1);
            Assert.True(GeometryInference.Instance.CurrentGraph.Nodes.Count == 2);

            GeometryInference.Instance.Delete(pt1);
            lines = GeometryInference.Instance.CurrentGraph.RetrieveSpecicShapes(ShapeType.Line);
            Assert.True(lines.Count == 1);
            Assert.True(GeometryInference.Instance.CurrentGraph.Nodes.Count == 1);
        }

        [Test]
        public void TestScenario2_1()
        {
            /*
             * 1: A(2,3) [Point] => Point
             * 2: B(3,4) [Point] => Point
             * 3: AB [Label]     => [Line, LineSegment]
             * 4: User Input to solve uncertainty
             */
            var ptA = new Point("A", 2, 3);
            var ptB = new Point("B", 3, 4);

            GeometryInference.Instance.Add(ptA);
            GeometryInference.Instance.Add(ptB);
            string label = "AB";
            object objs = GeometryInference.Instance.Add(label);
            var types = objs as List<ShapeType>;
            Assert.NotNull(types);
            object obj = GeometryInference.Instance.SearchCacheValue(label);
            Assert.NotNull(obj);
            Assert.True(obj.Equals(objs));

            List<Shape> shapes = GeometryInference.Instance.CurrentGraph.RetrieveShapes();
            Assert.True(shapes.Count == 2);

            GeometryInference.Instance.Delete(ptA);
            Assert.True(GeometryInference.Instance.CurrentGraph.Nodes.Count == 1);

            obj = GeometryInference.Instance.SearchCacheValue(label);
            Assert.Null(obj);
        }

        [Test]
        public void TestScenario2_2()
        {
            /*
             * 1: AB [Label]      => [Label]
             * 2: A(2,3)          => Point
             * 3: B(3,4)          => Point
             * Update: AB [Label] => [Line, LineSegment]
             * 4: User Input to solve uncertainty
             */
            string label = "AB";
            object objs = GeometryInference.Instance.Add(label);
            Assert.True(label.Equals(objs));
            var ptA = new Point("A", 2, 3);
            var ptB = new Point("B", 3, 4);
            GeometryInference.Instance.Add(ptA);
            GeometryInference.Instance.Add(ptB);

            List<Shape> shapes = GeometryInference.Instance.CurrentGraph.RetrieveShapes();
            Assert.True(shapes.Count == 2);
        }

        [Test]
        public void TestScenario3_1()
        {
            /*
             * 1: AB [Line]      => [Line]
             * 2: A(2,3)          => Point
             * 3: B(3,4)          => Point
             * Update: AB [Line] => [Line]
             */
            var line = new Line("AB");
            object objs = GeometryInference.Instance.Add(line);
            Assert.True(line.Equals(objs));
            var ptA = new Point("A", 2, 3);
            var ptB = new Point("B", 3, 4);
            GeometryInference.Instance.Add(ptA);
            GeometryInference.Instance.Add(ptB);

            List<Shape> shapes = GeometryInference.Instance.CurrentGraph.RetrieveShapes();
            Assert.True(shapes.Count == 3); 
        }
    }
}

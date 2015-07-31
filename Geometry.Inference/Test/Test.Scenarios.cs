using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
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
             * 4: ask slope = ?
             */
            var ptA = new Point("A", 2, 3);
            var ptB = new Point("B", 3, 4);

            GeometryInference.Instance.Add(ptA);
            GeometryInference.Instance.Add(ptB);

            //API 1:
            object line = GeometryInference.Instance.Add(null, ShapeType.Line);
            Assert.NotNull(line);

            List<Shape> shapes = GeometryInference.Instance.CurrentGraph.RetrieveShapes();
            Assert.True(shapes.Count == 3);

            GeometryInference.Instance.Delete(line);
            shapes = GeometryInference.Instance.CurrentGraph.RetrieveShapes();
            Assert.True(shapes.Count == 2);

            const string label = "AB"; //label constraint
            //API 2:
            line = GeometryInference.Instance.Add(label);
            var types = line as List<ShapeType>;
            Assert.NotNull(types);
            Assert.True(types.Count == 2);
            shapes = GeometryInference.Instance.CurrentGraph.RetrieveShapes();
            Assert.True(shapes.Count == 2);
            //API 3:
            line = GeometryInference.Instance.Add(label, ShapeType.Line);
            var gLine = line as Line;
            Assert.NotNull(gLine);
            object obj = GeometryInference.Instance.SearchCacheValue(label);
            Assert.NotNull(obj);
            shapes = GeometryInference.Instance.CurrentGraph.RetrieveShapes();
            Assert.True(shapes.Count == 3);
/*            GeometryInference.Instance.Delete(ptA);
            Assert.True(GeometryInference.Instance.CurrentGraph.Nodes.Count == 1);*/
            //API 4:
            var variable = new Var('s');
            var query = new EqGoal(variable, null);
            var answer = GeometryInference.Instance.Add(query);
            var answerGoal = answer as EqGoal;
            Assert.NotNull(answerGoal);
            var cachedGoal = answerGoal.CachedEntities.ToList()[0] as EqGoal;
            Assert.True(cachedGoal!= null);
            Assert.True(cachedGoal.Lhs.Equals(variable));
            Assert.True(cachedGoal.Rhs.Equals(1));
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

        [Test]
        public void TestScenario4()
        {
            var m = new Var('m');
            var k = new Var('k');
            var eqGoal1 = new EqGoal(m, 3); //m=3
            var eqGoal2 = new EqGoal(k, 2); //k=2
            GeometryInference.Instance.Add(eqGoal1);
            GeometryInference.Instance.Add(eqGoal2);

            object obj = GeometryInference.Instance.Add(null, ShapeType.Line);
            var line = obj as Line;
            Assert.NotNull(line);
            Assert.True(line.InputType == LineType.Relation);
        }

        [Test]
        public void TestScenario5()
        {
            var m = new Var('m');
            var k = new Var('k');
            var eqGoal1 = new EqGoal(m, 3); //m=3
            var eqGoal3 = new EqGoal(m, 4); //m=4
            var eqGoal2 = new EqGoal(k, 2); //k=2
            GeometryInference.Instance.Add(eqGoal1);
            GeometryInference.Instance.Add(eqGoal2);
            GeometryInference.Instance.Add(eqGoal3);

            object obj;
            bool result = GeometryInference.Instance.CurrentGraph.Infer(null, ShapeType.Line, out obj);
            Assert.True(result);
            var dict = obj as Dictionary<Tuple<GraphNode, GraphNode>, object>;
            Assert.NotNull(dict);
            Assert.True(dict.Count == 2);
            foreach (var tempObj in dict.Values.ToList())
            {
                var line = tempObj as Line;
                Assert.NotNull(line);
            }
        }
    }
}
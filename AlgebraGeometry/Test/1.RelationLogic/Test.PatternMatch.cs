﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSharpLogic;
using NUnit.Framework;

namespace AlgebraGeometry
{
    [TestFixture]
    public class TestPatternMatch
    {
        [Test]
        public void Test_PointLine_1()
        {
            var pt1 = new Point("A", 1.0, 2.0);
            var pt2 = new Point("B", 2.0, -2.0);
            object obj;

            //Non-deterministic
            bool result = RelationLogic.CreateRelation(pt1, pt2, out obj);
            Assert.False(result); //ambiguity
            Assert.NotNull(obj);
            var relTypes = obj as List<ShapeType>;
            Assert.NotNull(relTypes);
            Assert.True(relTypes.Count == 2);

            //Deterministic
            result = RelationLogic.CreateRelation(pt1, pt2, ShapeType.Line, out obj);
            Assert.True(result);

            //Deterministic
            result = RelationLogic.CreateRelation(pt1, pt2, ShapeType.LineSegment, out obj);
            Assert.True(result);
        }

        [Test]
        public void Test2_TODO()
        {
            var s = new Var('s');
            var goal1 = new EqGoal(s, 2.0);
            var pt = new Point(1.0, 2.0);
            object output;

            bool result = RelationLogic.CreateRelation(pt, goal1, out output);
            //Assert.True(result);
            Assert.False(result);
        }

        [Test]
        public void Test3_TODO()
        {
            //TODO
            var m = new Var('m');
            var k = new Var('k');
            var goal1 = new EqGoal(m, 2.0);
            var goal2 = new EqGoal(k, 1.0);
            object output;

            bool result = RelationLogic.CreateRelation(goal1, goal2, out output);
            //Assert.True(result);
            Assert.False(result);
        }
    }
}
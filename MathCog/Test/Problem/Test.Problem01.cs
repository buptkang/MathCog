/*******************************************************************************
 * Copyright (c) 2015 Bo Kang
 *   
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *  
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *******************************************************************************/

namespace MathCog
{
    using AlgebraGeometry;
    using CSharpLogic;
    using NUnit.Framework;
    using System.Collections.Generic;

    [TestFixture]
    public partial class TestProblems
    {
        /*
         * Problem 1: Find the distance betweeen A(2,0) and B(5,4)?
         */
        [Test]
        public void Test_Problem_01()
        {
            // general solving ability
            const string input1 = "A(2,0)"; 
            const string input2 = "B(5,4)";
            const string query = "d=";

            Reasoner.Instance.Load(input1);
            Reasoner.Instance.Load(input2);

            var obj = Reasoner.Instance.Load(query);
            Assert.NotNull(obj);
            var agQueryExpr = obj as AGQueryExpr;
            Assert.NotNull(agQueryExpr);
            Assert.True(agQueryExpr.RenderKnowledge == null);
            agQueryExpr.RetrieveRenderKnowledge();
            Assert.True(agQueryExpr.RenderKnowledge != null);
            Assert.True(agQueryExpr.RenderKnowledge.Count == 1);

            var answerExpr = agQueryExpr.RenderKnowledge[0] as AGPropertyExpr;
            Assert.NotNull(answerExpr);
            Assert.True(answerExpr.Goal.Rhs.Equals(5.0));
            Assert.Null(answerExpr.AutoTrace);

            answerExpr.IsSelected = true;
            answerExpr.GenerateSolvingTrace();
            Assert.NotNull(answerExpr.AutoTrace);
            Assert.True(answerExpr.AutoTrace.Count == 3);
          /*  
            int count = answerExpr.RetrieveStepsNumbers();
            Assert.True(count == 8);*/

            Reasoner.Instance.Reset();
        }

        [Test]
        public void Test_Problem_01_Sequence()
        {
            //sequence input test
            const string query = "d=";
            const string input1 = "A(2,0)";
            const string input2 = "B(5,4)";
            var obj = Reasoner.Instance.Load(query);
            Assert.NotNull(obj);
            var agQueryExpr = obj as AGQueryExpr;
            Assert.NotNull(agQueryExpr);
            agQueryExpr.RetrieveRenderKnowledge();
            Assert.True(agQueryExpr.RenderKnowledge == null);

            Reasoner.Instance.Load(input1);
            Reasoner.Instance.Load(input2);

            agQueryExpr.RetrieveRenderKnowledge();
            Assert.True(agQueryExpr.RenderKnowledge != null);
            Assert.True(agQueryExpr.RenderKnowledge.Count == 1);

            var answerExpr = agQueryExpr.RenderKnowledge[0] as AGPropertyExpr;
            Assert.NotNull(answerExpr);
            Assert.True(answerExpr.Goal.Rhs.Equals(5.0));
            Assert.Null(answerExpr.AutoTrace);

            answerExpr.IsSelected = true;
            answerExpr.GenerateSolvingTrace();
            Assert.NotNull(answerExpr.AutoTrace);

            Reasoner.Instance.Reset();
        }

        [Test]
        public void Test_Problem_01_Constraint()
        {
            const string input1 = "A(2,0)";
            const string input2 = "B(5,4)";
            const string query1 = "AB";

            Reasoner.Instance.Load(input1);
            Reasoner.Instance.Load(input2);
            var obj = Reasoner.Instance.Load(query1);
            Assert.NotNull(obj);
            var agQueryExpr = obj as AGQueryExpr;
            Assert.NotNull(agQueryExpr);
            var query = agQueryExpr.QueryTag as Query;
            Assert.NotNull(query);
            Assert.False(query.Success);
            var shapeTypes = query.FeedBack as List<ShapeType>;
            Assert.NotNull(shapeTypes);

            query.Constraint2 = ShapeType.LineSegment;
            var queryNode = Reasoner.Instance.RelationGraph.RetrieveQueryNode(query);
            Assert.NotNull(queryNode);
            Assert.True(queryNode.Query.Success);
            Assert.Null(query.FeedBack);
            Assert.True(queryNode.InternalNodes.Count == 1);
            var sn = queryNode.InternalNodes[0] as ShapeNode;
            Assert.NotNull(sn);
            var ls = sn.ShapeSymbol as LineSegmentSymbol;
            Assert.NotNull(ls);
            //Assert.True(ls.ToString().Equals("x-y+1=0"));

            const string query2 = "d=";
            var obj2 = Reasoner.Instance.Load(query2);

            Assert.True(query.CachedEntities.Count == 1);

            Reasoner.Instance.Reset();
        }

        [Test]
        public void Test_Problem_01_RealTime_dynamicGeometry()
        {
            /*
             * Problem 1: Find the distance betweeen A(2,0) and B(5,4)?
             */
            const string input1 = "A(2,0)";
            const string input2 = "B(5,4)";
            const string query = "d=";

            Reasoner.Instance.Load(input1);
            Reasoner.Instance.Load(input2);

            var obj = Reasoner.Instance.Load(query);

            var agQueryExpr = obj as AGQueryExpr;
            Assert.NotNull(agQueryExpr);

            const string input2Update = "B(5,6)";
            Reasoner.Instance.Unload(input2);

            agQueryExpr.RetrieveRenderKnowledge();
            Assert.True(agQueryExpr.RenderKnowledge == null);
            
            Reasoner.Instance.Load(input2Update);

            agQueryExpr.RetrieveRenderKnowledge();
            Assert.True(agQueryExpr.RenderKnowledge != null);
            Assert.True(agQueryExpr.RenderKnowledge.Count == 1);

            var agPropExpr = agQueryExpr.RenderKnowledge[0] as AGPropertyExpr;
            Assert.NotNull(agPropExpr != null);

            Reasoner.Instance.Reset();
        }
       
        [Test]
        public void Test_Problem_01_Reify()
        {
            // general solving ability
            const string input1 = "A(2,0)";
            const string input2 = "B(5,v)";
            const string input0 = "v=3";
            const string query  = "d=";

            Reasoner.Instance.Load(input1);
            Reasoner.Instance.Load(input2);
            Reasoner.Instance.Load(input0);

            var obj = Reasoner.Instance.Load(query);
            Assert.NotNull(obj);
            var agQueryExpr = obj as AGQueryExpr;
            Assert.NotNull(agQueryExpr);
            var queryTag = agQueryExpr.QueryTag;
            Assert.NotNull(queryTag);
            Assert.True(queryTag.CachedEntities.Count == 1);

            Reasoner.Instance.Reset();
        }

        #region Behavior Validate 

        [Test]
        public void Test_Problem_01_BehaviorValidate_01()
        {
            // general solving ability
            const string input1 = "A(2,0)";
            const string input2 = "B(5,4)";
            const string query = "d=";

            Reasoner.Instance.Load(input1);
            Reasoner.Instance.Load(input2);

            var obj = Reasoner.Instance.Load(query);
            Assert.NotNull(obj);
            var agQueryExpr = obj as AGQueryExpr;
            Assert.NotNull(agQueryExpr);
            Assert.True(agQueryExpr.RenderKnowledge == null);
            agQueryExpr.RetrieveRenderKnowledge();
            Assert.True(agQueryExpr.RenderKnowledge != null);
            Assert.True(agQueryExpr.RenderKnowledge.Count == 1);

            var answerExpr = agQueryExpr.RenderKnowledge[0] as AGPropertyExpr;
            Assert.NotNull(answerExpr);
            Assert.True(answerExpr.Goal.Rhs.Equals(5.0));
            Assert.Null(answerExpr.AutoTrace);

            answerExpr.IsSelected = true;
            answerExpr.GenerateSolvingTrace();
            Assert.NotNull(answerExpr.AutoTrace);
            Assert.True(answerExpr.AutoTrace.Count == 3);

            //d^2 = 16 + 9
            object obj44 = ExprMock.Mock2();
            var userKnowledge = Reasoner.Instance.Load(obj44, null, true) as AGEquationExpr;
            Assert.NotNull(userKnowledge);

 /*           var rTemp = Reasoner.Instance.ExprValidate(userKnowledge.Expr);
            Assert.NotNull(rTemp);*/

            Reasoner.Instance.Reset();
        }

        [Test]
        public void Test_Problem_01_BehaviorValidate_02()
        {
            // general solving ability
            const string input1 = "A(2,0)";
            const string input2 = "B(5,4)";
            const string query = "d=";

            Reasoner.Instance.Load(input1);
            Reasoner.Instance.Load(input2);

            var obj = Reasoner.Instance.Load(query);
            Assert.NotNull(obj);
            var agQueryExpr = obj as AGQueryExpr;
            Assert.NotNull(agQueryExpr);
            Assert.True(agQueryExpr.RenderKnowledge == null);
            agQueryExpr.RetrieveRenderKnowledge();
            Assert.True(agQueryExpr.RenderKnowledge != null);
            Assert.True(agQueryExpr.RenderKnowledge.Count == 1);

            var answerExpr = agQueryExpr.RenderKnowledge[0] as AGPropertyExpr;
            Assert.NotNull(answerExpr);
            Assert.True(answerExpr.Goal.Rhs.Equals(5.0));
            Assert.Null(answerExpr.AutoTrace);

            answerExpr.IsSelected = true;
            answerExpr.GenerateSolvingTrace();
            Assert.NotNull(answerExpr.AutoTrace);
            Assert.True(answerExpr.AutoTrace.Count == 3);

            //d = Sqrt(16+9)
            object obj44 = ExprMock.Mock3();
            var userKnowledge = Reasoner.Instance.Load(obj44, null, true) as AGEquationExpr;
            Assert.NotNull(userKnowledge);

            var rTemp = Reasoner.Instance.ExprValidate(userKnowledge.Expr) as AGPropertyExpr;
            Assert.NotNull(rTemp);
            Assert.True(rTemp.Goal.Traces.Count != 0);

            Reasoner.Instance.Reset();
        }

        [Test]
        public void Test_Problem_01_BehaviorValidate_03()
        {
            #region Problem Input

            // general solving ability
            const string input1 = "A(2,0)";
            const string input2 = "B(5,4)";
            const string query = "d=";

            Reasoner.Instance.Load(input1);
            Reasoner.Instance.Load(input2);

            var obj = Reasoner.Instance.Load(query);
            Assert.NotNull(obj);
            var agQueryExpr = obj as AGQueryExpr;
            Assert.NotNull(agQueryExpr);
            Assert.True(agQueryExpr.RenderKnowledge == null);
            agQueryExpr.RetrieveRenderKnowledge();
            Assert.True(agQueryExpr.RenderKnowledge != null);
            Assert.True(agQueryExpr.RenderKnowledge.Count == 1);

            var answerExpr = agQueryExpr.RenderKnowledge[0] as AGPropertyExpr;
            Assert.NotNull(answerExpr);
            Assert.True(answerExpr.Goal.Rhs.Equals(5.0));
            Assert.Null(answerExpr.AutoTrace);

            answerExpr.IsSelected = true;
            answerExpr.GenerateSolvingTrace();
            Assert.NotNull(answerExpr.AutoTrace);
            Assert.True(answerExpr.AutoTrace.Count == 3);

            Assert.True(Reasoner.Instance.RelationGraph.Nodes.Count == 3);

            #endregion

            //user input a line segment in geometry side
            //it should validate its correctness

            var lss = MockSegment();
            var userKnowledge = Reasoner.Instance.Load(lss, null, true) as AGShapeExpr;
            Assert.NotNull(userKnowledge);

            Assert.True(Reasoner.Instance.RelationGraph.Nodes.Count == 3);

            //deep understanding
            object output;

            var rTemp = Reasoner.Instance.RelationValidate(userKnowledge.ShapeSymbol, out output);
            Assert.NotNull(rTemp);

            /////////////////////////////////////////
            Reasoner.Instance.Reset();
        }

        private LineSegmentSymbol MockSegment()
        {
            var pt1 = new Point("A", 2, 0);
            var pt2 = new Point("B", 5, 4);
            var seg = new LineSegment("AB", pt1, pt2);
            var segSymbol = new LineSegmentSymbol(seg);
            return segSymbol;
        }

        #endregion
    }
}

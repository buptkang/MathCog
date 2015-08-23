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
        public void Test_Problem_1()
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
            Assert.True(agQueryExpr.RenderKnowledge.Count == 2);

            var answerExpr = agQueryExpr.RenderKnowledge[1] as AGPropertyExpr;
            Assert.NotNull(answerExpr);
            Assert.True(answerExpr.Goal.Rhs.Equals(5.0));
            Assert.Null(answerExpr.AutoTrace);

            answerExpr.IsSelected = true;
            answerExpr.GenerateSolvingTrace();
            Assert.NotNull(answerExpr.AutoTrace);
        }

        [Test]
        public void Test_Problem_1_Sequence()
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
            Assert.True(agQueryExpr.RenderKnowledge.Count == 2);

            var answerExpr = agQueryExpr.RenderKnowledge[1] as AGPropertyExpr;
            Assert.NotNull(answerExpr);
            Assert.True(answerExpr.Goal.Rhs.Equals(5.0));
            Assert.Null(answerExpr.AutoTrace);

            answerExpr.IsSelected = true;
            answerExpr.GenerateSolvingTrace();
            Assert.NotNull(answerExpr.AutoTrace);
        }

        [Test]
        public void Test_Problem_1_Constraint()
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
        }

        [Test]
        public void Test_Problem_1_RealTime_dynamicGeometry()
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
            Assert.True(agQueryExpr.RenderKnowledge.Count == 2);

            var agPropExpr = agQueryExpr.RenderKnowledge[1] as AGPropertyExpr;
            Assert.NotNull(agPropExpr != null);
        }

        /*
         * Problem 1: Find the distance betweeen A(2,0) and B(5,v), when 
         * v = 2, 3, 4 respectively?
         */
        [Test]
        public void Test_Problem_1_Reify()
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
            Assert.True(queryTag.CachedEntities.Count == 2);
        }
    
    }
}

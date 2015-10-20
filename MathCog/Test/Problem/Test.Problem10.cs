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

using AlgebraGeometry;

namespace MathCog
{
    using System.Linq;
    using CSharpLogic;
    using NUnit.Framework;

    [TestFixture]
    public partial class TestProblems
    {
        /*
         * Problem 10: A line contains the point (0,5) and is perpendicular to the line 4y=x, what is the slope of this line? what is the general form of this line?
         */
        [Test]
        public void Test_Problem_10()
        {
            const string input1 = "(0,5)";
            const string input2 = "4y=x";
            const string constraint0 = "m1=";
            const string input3 = "m1*m=-1";
            const string query0 = "m=";
            const string query1 = "lineG";

            Reasoner.Instance.Load(input1);
            Reasoner.Instance.Load(input2);
            var agQueryExpr = Reasoner.Instance.Load(constraint0) as AGQueryExpr;
            Assert.NotNull(agQueryExpr);
            var queryTag = agQueryExpr.QueryTag;
            Assert.NotNull(queryTag);
            Assert.True(queryTag.Success);
            agQueryExpr.RetrieveRenderKnowledge();
            var answerExpr = agQueryExpr.RenderKnowledge[0] as AGPropertyExpr;
            Assert.NotNull(answerExpr);
            Assert.True(answerExpr.Goal.Rhs.Equals(0.25));
            Assert.Null(answerExpr.AutoTrace);
            answerExpr.IsSelected = true;
            answerExpr.GenerateSolvingTrace();
            Assert.NotNull(answerExpr.AutoTrace);

            Reasoner.Instance.Load(input3);
            Assert.True(Reasoner.Instance.RelationGraph.Nodes.Count == 5);

            var lst = Reasoner.Instance.RelationGraph.RetrieveGoalNodes();
            Assert.True(lst!=null);
            var goalNode = lst.ToList()[0];
            Assert.NotNull(goalNode);
            var synGoal = goalNode.Goal as EqGoal;
            Assert.NotNull(synGoal);
            Assert.True(synGoal.Rhs.ToString().Equals("-4"));
            Assert.True(synGoal.Traces.Count != 0);

            ////////////////////////////////////////////////////////////////

            var queryExpr0 = Reasoner.Instance.Load(query0) as AGQueryExpr;
            Assert.NotNull(queryExpr0);
            Assert.True(queryExpr0.RenderKnowledge == null);
            queryExpr0.RetrieveRenderKnowledge();
            Assert.True(queryExpr0.RenderKnowledge != null);
            Assert.True(queryExpr0.RenderKnowledge.Count == 2);

            var answerExpr0 = queryExpr0.RenderKnowledge[1] as AGPropertyExpr;
            Assert.NotNull(answerExpr0);
            Assert.Null(answerExpr0.AutoTrace);
            answerExpr0.IsSelected = true;
            answerExpr0.GenerateSolvingTrace();
            Assert.True(answerExpr0.AutoTrace.Count == 5);

            ////////////////////////////////////////////////////////////////

            var queryExpr2 = Reasoner.Instance.Load(query1) as AGQueryExpr;
            Assert.NotNull(queryExpr2);
            Assert.True(queryExpr2.RenderKnowledge == null);
            queryExpr2.RetrieveRenderKnowledge();
            Assert.True(queryExpr2.RenderKnowledge != null);
            Assert.True(queryExpr2.RenderKnowledge.Count == 2);
            var answerExpr2 = queryExpr2.RenderKnowledge[1] as AGShapeExpr;
            Assert.NotNull(answerExpr2);
            Assert.True(answerExpr2.ShapeSymbol.ToString().Equals("4x+y-5=0"));
            Assert.Null(answerExpr2.AutoTrace);
            answerExpr2.IsSelected = true;
            answerExpr2.GenerateSolvingTrace();
            Assert.True(answerExpr2.AutoTrace.Count == 6);

            Reasoner.Instance.Reset();
        }
    }
}

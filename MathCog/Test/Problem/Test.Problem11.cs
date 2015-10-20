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
    using System.Linq;
    using CSharpLogic;
    using NUnit.Framework;

    [TestFixture]
    public partial class TestProblems
    {
        /*
         * Line A passes through (-1,2) and (5,8). 
         * Line B is parallel to line A, and also crosses point (1,0), 
         * what is the general form of line B?
         */
        [Test]
        public void Test_Problem_11()
        {
            const string input1 = "(-1,2)";
            const string input2 = "(5,8)";
            const string constraint1 = "m1";
            const string input3 = "m1=m";
            const string query2 = "lineG";
            const string input4 = "(1,0)";

            Reasoner.Instance.Load(input1);
            Reasoner.Instance.Load(input2);
            Reasoner.Instance.Load(constraint1);
            Reasoner.Instance.Load(input3);
            var queryExpr2 = Reasoner.Instance.Load(query2) as AGQueryExpr;

            Assert.NotNull(queryExpr2);
            Assert.True(queryExpr2.RenderKnowledge == null);
            queryExpr2.RetrieveRenderKnowledge();
            Assert.True(queryExpr2.RenderKnowledge != null);
            Assert.True(queryExpr2.RenderKnowledge.Count == 1);

            Reasoner.Instance.Load(input4);

            /////////////////////////////////////////////////////////

            Assert.NotNull(queryExpr2);
            queryExpr2.RetrieveRenderKnowledge();
            Assert.True(queryExpr2.RenderKnowledge != null);
            Assert.True(queryExpr2.RenderKnowledge.Count == 5);

            var answerExpr2 = queryExpr2.RenderKnowledge[4] as AGShapeExpr;
            Assert.NotNull(answerExpr2);
            Assert.True(answerExpr2.ShapeSymbol.ToString().Equals("x-y-1=0"));
            Assert.Null(answerExpr2.AutoTrace);
            answerExpr2.IsSelected = true;
            answerExpr2.GenerateSolvingTrace();
            Assert.NotNull(answerExpr2.AutoTrace);

            Reasoner.Instance.Reset();
        }


        /*
         * Find the slope-intercept form of line equation parallel to y=5x-2 and passing through point (2,1). 
         * (use notation m, m1 to represent line slope)
         * 
         */
        [Test]
        public void Test_Problem_11_1()
        {
            const string input1 = "y=5x-2";
            const string input2 = "(2,1)";
            const string constraint1 = "m";
            const string input3 = "m=m1"; 
            const string input4 = "(1,0)";
            const string query = "lineG";
           
            Reasoner.Instance.Load(input1);
            Reasoner.Instance.Load(input2);

            var agQueryExpr = Reasoner.Instance.Load(constraint1) as AGQueryExpr;
            Assert.NotNull(agQueryExpr);
            var queryTag = agQueryExpr.QueryTag;
            Assert.NotNull(queryTag);
            Assert.True(queryTag.Success);
            agQueryExpr.RetrieveRenderKnowledge();

            var answerExpr = agQueryExpr.RenderKnowledge[0] as AGPropertyExpr;
            Assert.NotNull(answerExpr);
            Assert.True(answerExpr.Goal.Rhs.Equals(5.0));
            Assert.Null(answerExpr.AutoTrace);
            answerExpr.IsSelected = true;
            answerExpr.GenerateSolvingTrace();
            Assert.True(answerExpr.AutoTrace.Count == 2);
            
            Reasoner.Instance.Load(input3);
            Reasoner.Instance.Load(input4);

            ///////////////////////////////////////////////////////////////

            var queryExpr2 = Reasoner.Instance.Load(query) as AGQueryExpr;
            Assert.NotNull(queryExpr2);
            Assert.True(queryExpr2.RenderKnowledge == null);
            queryExpr2.RetrieveRenderKnowledge();
            Assert.True(queryExpr2.RenderKnowledge != null);
            Assert.True(queryExpr2.RenderKnowledge.Count == 4);

            var answerExpr1 = queryExpr2.RenderKnowledge[1] as AGShapeExpr;
            Assert.NotNull(answerExpr1);
            Assert.Null(answerExpr1.AutoTrace);
            answerExpr1.IsSelected = true;
            answerExpr1.GenerateSolvingTrace();
            Assert.NotNull(answerExpr1);
            Assert.True(answerExpr1.AutoTrace.Count == 5);

            Reasoner.Instance.Reset();
        }
    }
}
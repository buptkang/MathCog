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

using System.ComponentModel;
using CSharpLogic;

namespace MathCog
{
    using NUnit.Framework;

    [TestFixture]
    public partial class TestProblems
    {
        /*
         *  Line A passes through two points (4,3) and (2,v). 
         *  The line is perpendicular to line B in which the slope of line B is 1/2. 
         *  what is the value of v?
         */
        [Test]
        public void Test_Problem_29()
        {
            const string input1 = "(4,3)";
            const string input2 = "(2,v)";
            const string input4 = "m1=0.5";
            const string input3 = "m*m1=-1";
            const string query  = "v";

            Reasoner.Instance.Load(input1);
            Reasoner.Instance.Load(input2);
            Reasoner.Instance.Load(input4);
            Assert.True(Reasoner.Instance.RelationGraph.Nodes.Count == 4);
            Reasoner.Instance.Load(input3);
            Assert.True(Reasoner.Instance.RelationGraph.Nodes.Count == 7);

            var queryExpr2 = Reasoner.Instance.Load(query) as AGQueryExpr;
            Assert.NotNull(queryExpr2);
            Assert.True(queryExpr2.RenderKnowledge == null);
            queryExpr2.RetrieveRenderKnowledge();
            Assert.True(queryExpr2.RenderKnowledge != null);
            Assert.True(queryExpr2.RenderKnowledge.Count == 2);

            var agPropExpr = queryExpr2.RenderKnowledge[1] as AGPropertyExpr;
            Assert.NotNull(agPropExpr);
            var gGoal = agPropExpr.Goal;
            Assert.NotNull(gGoal);
            Assert.True(gGoal.Rhs.ToString().Equals("7"));
            agPropExpr.IsSelected = true;
            agPropExpr.GenerateSolvingTrace();
            Assert.True(agPropExpr.AutoTrace != null);
            Assert.True(agPropExpr.AutoTrace.Count == 4);

            bool result = Reasoner.Instance.RelationGraph.FindQuery("v");
            Assert.True(result);

            Reasoner.Instance.Reset();
        }

        public void Test_Problem_29_1()
        {
            //TODO
            const string input1 = "(4,3)";
            const string input2 = "(2,v)";
            //const string query1 = "m1";
            const string input3 = "m1*m2=-1";
            const string input4 = "m2=0.5";

            Reasoner.Instance.Load(input1);
            Reasoner.Instance.Load(input2);

            Reasoner.Instance.Load(input3);
            Assert.True(Reasoner.Instance.RelationGraph.Nodes.Count == 3);
            Reasoner.Instance.Load(input4);

            Assert.True(Reasoner.Instance.RelationGraph.Nodes.Count == 8);

            const string query2 = "v";
            //Reasoner.Instance.Load(query1);
            Reasoner.Instance.Reset();
        }

    }
}

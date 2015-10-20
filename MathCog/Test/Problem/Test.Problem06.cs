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
         * Problem 6: A line passes through points (2,3) and (4,y), the slope of this line is 5. What is the y-intercept of the line?
         */

        [Test]
        public void Test_Problem_06()
        {
            const string input1 = "(2,3)";
            const string input2 = "(4,y)";
            const string input3 = "m=5";
            const string query0 = "y";
            const string query  = "k";

            Reasoner.Instance.Load(input1);
            Reasoner.Instance.Load(input2);
            Reasoner.Instance.Load(input3);
            Assert.True(Reasoner.Instance.RelationGraph.Nodes.Count == 4);

            var obj = Reasoner.Instance.Load(query0);
            Assert.NotNull(obj);
            var agQueryExpr = obj as AGQueryExpr;
            Assert.NotNull(agQueryExpr);
            var queryTag = agQueryExpr.QueryTag;
            Assert.NotNull(queryTag);
            Assert.True(queryTag.Success);
            Assert.True(queryTag.CachedEntities.Count == 3);

            var gGoal3 = queryTag.CachedEntities.ToList()[2] as EqGoal;
            Assert.NotNull(gGoal3);
            Assert.True(gGoal3.Traces.Count == 2);

            var agQueryExpr1 = Reasoner.Instance.Load(query) as AGQueryExpr;
            Assert.NotNull(agQueryExpr1);
            var queryTag1 = agQueryExpr1.QueryTag;
            Assert.NotNull(queryTag1);
            Assert.True(queryTag1.Success);
            Assert.True(queryTag1.CachedEntities.Count == 1);
            var goal1 = queryTag1.CachedEntities.ToList()[0] as EqGoal;
            Assert.NotNull(goal1);
            Assert.True(goal1.ToString().Equals("k=-7"));
            Assert.True(goal1.Traces.Count != 0);

            Reasoner.Instance.Reset();
        }
    }
}

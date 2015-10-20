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

using System.Linq;
using CSharpLogic;

namespace MathCog
{
    using NUnit.Framework;

    [TestFixture]
    public partial class TestProblems
    {
        /*
         * Problem 28: There are two points A(2,y) and B(-1,4). The y-coordinate of point A is -1. What is the distance betweeen these two points?
         */
        [Test]
        public void Test_Problem_28()
        {
            const string input1 = "A(2,y)";
            const string input2 = "B(-1,4)";
            const string input3 = "y=-1";
            const string query  = "d=";

            Reasoner.Instance.Load(input1);
            Reasoner.Instance.Load(input2);
            Reasoner.Instance.Load(input3);
            var obj = Reasoner.Instance.Load(query);
            Assert.NotNull(obj);
            var agQueryExpr = obj as AGQueryExpr;
            Assert.NotNull(agQueryExpr);
            var queryTag = agQueryExpr.QueryTag;
            Assert.NotNull(queryTag);
            Assert.True(queryTag.Success);
            Assert.True(queryTag.CachedEntities.Count == 1);
            var goal1 = queryTag.CachedEntities.ToList()[0] as EqGoal;
            Assert.NotNull(goal1);
            Assert.True(goal1.ToString().Equals("d=5.831"));

            Assert.True(goal1.Traces.Count != 0);

            Reasoner.Instance.Reset();
        }
    }
}

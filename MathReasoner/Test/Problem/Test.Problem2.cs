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

namespace MathReason
{
    using AlgebraGeometry.Expr;
    using CSharpLogic;
    using NUnit.Framework;
    using System.Linq;

    [TestFixture]
    public partial class TestProblems
    {
        /*
         * There exists two points A(3,4) and B(4,v), the distance between A and B is 5. What is the value of v?
         */
        [Test]
        public void Test_Problem_2()
        {
            const string input1 = "A(3,4)";
            const string input2 = "B(7,v)";
            const string input3 = "d=5";
            const string query = "v=";

            Reasoner.Instance.Load(input1);
            Reasoner.Instance.Load(input2);
           
            var obj = Reasoner.Instance.Load(query);
            Assert.NotNull(obj);
            var agQueryExpr = obj as AGQueryExpr;
            Assert.NotNull(agQueryExpr);
            var queryTag = agQueryExpr.QueryTag;
            Assert.NotNull(queryTag);
            Assert.False(queryTag.Success);

            Reasoner.Instance.Load(input3);
            Assert.True(queryTag.Success);
            Assert.True(queryTag.CachedEntities.Count == 2);
            var goal1 = queryTag.CachedEntities.ToList()[0] as EqGoal;
            Assert.NotNull(goal1);
            Assert.True(goal1.Rhs.Equals(9));
            var goal2 = queryTag.CachedEntities.ToList()[1] as EqGoal;
            Assert.NotNull(goal2);
            Assert.True(goal2.Rhs.Equals(-1));
        }

    }
}

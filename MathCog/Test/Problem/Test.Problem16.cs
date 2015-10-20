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
    using NUnit.Framework;

    [TestFixture]
    public partial class TestProblems
    {
        /*
         * Find the midpoint of the line joining A(-1,2) and B(4,6).
         */
        [Test]
        public void Test_Problem_16()
        {
            const string input1 = "A(-1,2)";
            const string input2 = "B(5,8)";
            const string query0 = "MidP=";

            Reasoner.Instance.Load(input1);
            Reasoner.Instance.Load(input2);

            var obj = Reasoner.Instance.Load(query0);
            Assert.NotNull(obj);
            var agQueryExpr = obj as AGQueryExpr;
            Assert.NotNull(agQueryExpr);
            var queryTag = agQueryExpr.QueryTag;
            Assert.NotNull(queryTag);
            Assert.True(queryTag.Success);
            Assert.True(queryTag.CachedEntities.Count == 1);

            var cachedPt = queryTag.CachedEntities.ToList()[0] as PointSymbol;
            Assert.NotNull(cachedPt);
            Assert.True(cachedPt.Traces.Count != 0);
            Reasoner.Instance.Reset();
        }


        #region Behavior Test

        [Test]
        public void Test_Problem_16_BehaviorValidate_01()
        {
            #region Problem Input
            const string input1 = "A(-1,2)";
            const string input2 = "B(5,8)";
            const string query0 = "MidP=";

            Reasoner.Instance.Load(input1);
            Reasoner.Instance.Load(input2);

            var obj = Reasoner.Instance.Load(query0);
            Assert.NotNull(obj);
            var agQueryExpr = obj as AGQueryExpr;
            Assert.NotNull(agQueryExpr);
            var queryTag = agQueryExpr.QueryTag;
            Assert.NotNull(queryTag);
            Assert.True(queryTag.Success);
            Assert.True(queryTag.CachedEntities.Count == 1);

            var cachedPt = queryTag.CachedEntities.ToList()[0] as PointSymbol;
            Assert.NotNull(cachedPt);
            Assert.True(cachedPt.Traces.Count != 0);
            #endregion

            const string userInput = "A(2,4)";

            Reasoner.Instance.Reset();
        }

        #endregion
    }
}

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
         * 6. Determine the general form of this line: The line through (-2,0) with slope -2.
         */

        [Test]
        public void Test_Problem_6()
        {
            const string input1 = "(-2,0)";
            const string input2 = "m=-2";
            const string query = "LineGeneralForm=";

            var obj = Reasoner.Instance.Load(input1) as AGShapeExpr;
            Assert.NotNull(obj);

            var obj2 = Reasoner.Instance.Load(input2) as AGPropertyExpr;
            Assert.NotNull(obj2);

            var obj3 = Reasoner.Instance.Load(query) as AGQueryExpr;
            Assert.NotNull(obj3);

            var queryTag = obj3.QueryTag;
            Assert.True(queryTag.CachedEntities.Count == 1);
            var ls = queryTag.CachedEntities.ToList()[0] as LineSymbol;
            Assert.NotNull(ls);
            Assert.True(ls.SymA.Equals("-2"));
        }
    }
}

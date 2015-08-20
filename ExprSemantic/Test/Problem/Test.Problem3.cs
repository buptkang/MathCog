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
    using AlgebraGeometry;
    using AlgebraGeometry.Expr;
    using CSharpLogic;
    using NUnit.Framework;
    using System.Linq;

    [TestFixture]
    public partial class TestProblems
    {
        /*
         * A line passes through two points A(2,0), B(0,3) respectively.
         * 1) What is the slope of this line?  
         * 2) What is the standard form of this line?
         */

        [Test]
        public void Test_Problem_3()
        {
            /*
             * Problem 2: A line passes through two points A(2,0), B(0,3) respectively. 1) What is the slope of this line?  2) What is the standard form of this line?
             */
            const string input1 = "A(2,0)";
            const string input2 = "B(0,3)";
            //const string query1 = "m=";
            const string query2 = "lineG=";

            Reasoner.Instance.Load(input1);
            Reasoner.Instance.Load(input2);

            var obj = Reasoner.Instance.Load(query2);
            Assert.NotNull(obj);
            var agQueryExpr = obj as AGQueryExpr;
            Assert.NotNull(agQueryExpr);
            var query = agQueryExpr.QueryTag;
            Assert.NotNull(query);
            Assert.True(query.Success);
            Assert.True(query.CachedEntities.Count == 1);
            var lineSymbol = query.CachedEntities.ToList()[0] as LineSymbol;
            Assert.NotNull(lineSymbol);
            //            Assert.True(lineSymbol.ToString().Equals());

            //TODO build relations

        }
    }
}

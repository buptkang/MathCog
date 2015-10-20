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

using CSharpLogic;
using starPadSDK.MathExpr;

namespace MathCog
{
    using AlgebraGeometry;
    using NUnit.Framework;
    using System.Linq;
    using Text = starPadSDK.MathExpr.Text;

    [TestFixture]
    public partial class TestProblems
    {
        /*
         * There is a line, the slope of it is 3, the y-intercept of it is 2. 
         * What is the slope intercept form of this line? 
         * What is the general form of this line?
         */

        [Test]
        public void Test_Problem_04_0()
        {
            const string input1 = "m=3";
            const string input2 = "k=2";
            Reasoner.Instance.Load(input1);
            Reasoner.Instance.Load(input2);

            //Question 2:
            const string query1 = "lineG=";
            var obj = Reasoner.Instance.Load(query1);
            Assert.NotNull(obj);
            var agQueryExpr = obj as AGQueryExpr;
            Assert.NotNull(agQueryExpr);
            var query = agQueryExpr.QueryTag;
            Assert.NotNull(query);
            Assert.True(query.Success);
            Assert.True(query.CachedEntities.Count == 1);
            var lineSymbol = query.CachedEntities.ToList()[0] as LineSymbol;
            Assert.NotNull(lineSymbol);
            Assert.True(lineSymbol.ToString().Equals("3x-y+2=0"));

            Assert.True(lineSymbol.Traces.Count == 2);


            Reasoner.Instance.Reset();
        }


        [Test]
        public void Test_Problem_04_1()
        {
            const string input1 = "m=3";
            const string input2 = "k=2";
            Reasoner.Instance.Load(input1);
            Reasoner.Instance.Load(input2);

            //Question 1:
            const string query2 = "lineS=";
            var obj1 = Reasoner.Instance.Load(query2);
            Assert.NotNull(obj1);
            var agQueryExpr1 = obj1 as AGQueryExpr;
            Assert.NotNull(agQueryExpr1);
            var queryTag = agQueryExpr1.QueryTag;
            Assert.NotNull(queryTag);
            Assert.True(queryTag.Success);
            Assert.True(queryTag.CachedEntities.Count == 1);
            var lineSymbol1 = queryTag.CachedEntities.ToList()[0] as LineSymbol;
            Assert.NotNull(lineSymbol1);
            Assert.True(lineSymbol1.ToString().Equals("y=3x+2"));
            Assert.True(lineSymbol1.Traces.Count == 1);

            //Question 2:
            const string query1 = "lineG=";
            var obj = Reasoner.Instance.Load(query1);
            Assert.NotNull(obj);
            var agQueryExpr = obj as AGQueryExpr;
            Assert.NotNull(agQueryExpr);
            var query = agQueryExpr.QueryTag;
            Assert.NotNull(query);
            Assert.True(query.Success);
            Assert.True(query.CachedEntities.Count == 1);
            var lineSymbol = query.CachedEntities.ToList()[0] as LineSymbol;
            Assert.NotNull(lineSymbol);
            Assert.True(lineSymbol.ToString().Equals("3x-y+2=0"));
            Assert.True(lineSymbol.Traces.Count == 2);

            Reasoner.Instance.Reset();
        }


        #region Behavior Validate

        [Test]
        public void Test_Problem_04_BehaviorValidate_01()
        {
            #region Problem Setup

            const string input1 = "m=3";
            const string input2 = "k=2";
            Reasoner.Instance.Load(input1);
            Reasoner.Instance.Load(input2);

            //Question 2:
            const string query1 = "lineG=";
            var obj = Reasoner.Instance.Load(query1);
            Assert.NotNull(obj);
            var agQueryExpr = obj as AGQueryExpr;
            Assert.NotNull(agQueryExpr);
            var query = agQueryExpr.QueryTag;
            Assert.NotNull(query);
            Assert.True(query.Success);
            Assert.True(query.CachedEntities.Count == 1);
            var lineSymbol = query.CachedEntities.ToList()[0] as LineSymbol;
            Assert.NotNull(lineSymbol);
            Assert.True(lineSymbol.ToString().Equals("3x-y+2=0"));

            //Question 1:
            const string query2 = "lineS=";
            var obj1 = Reasoner.Instance.Load(query2);
            Assert.NotNull(obj1);
            var agQueryExpr1 = obj1 as AGQueryExpr;
            Assert.NotNull(agQueryExpr1);
            var queryTag = agQueryExpr1.QueryTag;
            Assert.NotNull(queryTag);
            Assert.True(queryTag.Success);
            Assert.True(queryTag.CachedEntities.Count == 1);
            var lineSymbol1 = queryTag.CachedEntities.ToList()[0] as LineSymbol;
            Assert.NotNull(lineSymbol1);
            Assert.True(lineSymbol1.ToString().Equals("y=3x+2"));

            #endregion

            string userInput = "y=mx+2";
            Expr expr = Text.Convert(userInput);

            var shapeExpr = Reasoner.Instance.ExprValidate(expr) as AGShapeExpr;
            Assert.NotNull(shapeExpr);

            object inferOutput;
            var boolResult = Reasoner.Instance.RelationValidate(shapeExpr.ShapeSymbol, out inferOutput) as bool?;
            Assert.NotNull(boolResult);
            Assert.True(boolResult.Value);

            Reasoner.Instance.Reset();
        }

        #endregion

    }
}

﻿/*******************************************************************************
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
    using AlgebraGeometry;
    using NUnit.Framework;
    using System.Linq;

    [TestFixture]
    public partial class TestProblems
    {
        /*
         * There is a line, the slope of it is 3, the intercept of it is 2. 
         * What is the slope intercept form of this line? 
         * What is the general form of this line? 
         */

        [Test]
        public void Test_Problem_4()
        {
            /*
             * Problem 4: There is a line, the slope of it is 3, the intercept of it is 2, what is the slope intercept form of this line? what is the general form of this line? 
             */
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

            //TODO Question1 and Question2 should point to the same line

            //obj = Reasoner.Instance.Load(lineDefault, ShapeType.Line);
            //Assert.Null(obj); 
            /*   List<AGPropertyExpr> props = Reasoner.Instance.TestGetProperties();
            Assert.True(props.Count == 2);
            List<AGShapeExpr> shapes = Reasoner.Instance.TestGetShapeFacts();
            Assert.True(shapes.Count == 0);
            List<AGQueryExpr> queries = Reasoner.Instance.TestGetQuerys();
            Assert.True(queries.Count == 1);
            var query = queries[0].QueryTag;
            Assert.True(query.Success);
            Assert.True(query.CachedEntities.Count == 1);
            var lineSymbol = query.CachedEntities.ToList()[0] as LineSymbol;
            Assert.NotNull(lineSymbol);
            Assert.True(lineSymbol.ToString().Equals("y=3x+2"));*/

            /*            const string lineG = "LineGeneralForm";
                        var obj1 = Reasoner.Instance.Load(lineG, ShapeType.Line);
                        var agQueryExpr1 = obj1 as AGQueryExpr;
                        Assert.NotNull(agQueryExpr1);
                        Assert.True(agQueryExpr1.QueryTag.Success);*/

            /*          
                        const string lineGeneralForm = "LineGeneralForm=";
                        //option 1:
                        obj = Reasoner.Instance.Load(lineGeneralForm);
                        //const string lineGF = "general";
                        //option 2 (preferred):
                        //obj = Reasoner.Instance.Load(lineGF, ShapeType.Line);
                        Assert.NotNull(obj);

                        const string lineSlopeInterceptForm = "mk=";
                        obj = Reasoner.Instance.Load(lineSlopeInterceptForm, ShapeType.Line);*/

            /*
            var agQueryExpr = result1 as AGQueryExpr;
            Assert.NotNull(agQueryExpr);
            var eqGoal = agQueryExpr.QueryTag as EqGoal;
            Assert.NotNull(eqGoal);
            Assert.True(eqGoal.Rhs == null);
            Assert.True(eqGoal.Lhs != null);
            Assert.True(eqGoal.CachedEntities.Count == 1);
            var cachedls1 = eqGoal.CachedEntities.ToList()[0] as LineSymbol;
            Assert.NotNull(cachedls1);
            Assert.True(cachedls1.ToString().Equals("y=3x+2"));

            agQueryExpr = result2 as AGQueryExpr;
            Assert.NotNull(agQueryExpr);
            eqGoal = agQueryExpr.QueryTag as EqGoal;
            Assert.NotNull(eqGoal);
            Assert.True(eqGoal.Rhs == null);
            Assert.True(eqGoal.Lhs != null);
            Assert.True(eqGoal.CachedEntities.Count == 1);
            var cachedls2 = eqGoal.CachedEntities.ToList()[0] as LineSymbol;
            Assert.NotNull(cachedls2);
            Assert.True(cachedls2.ToString().Equals("3x-y+2=0"));

            // cachedls1 and cachedls2
            Assert.True(cachedls1.Shape.Equals(cachedls2.Shape));
            Assert.AreNotEqual(cachedls1, cachedls2);*/
        }
    }
}

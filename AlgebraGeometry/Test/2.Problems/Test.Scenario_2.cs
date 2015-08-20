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

namespace AlgebraGeometry
{
    using CSharpLogic;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;

    [TestFixture]
    public partial class TestProblemSolvingScenarios
    {
        /*
         * There exists two points A(2,4) and B(5,v), the distance between A and B is 5. 
         *  What is the value of v?
         */
        [Test]
        public void TestScenario_2()
        {
            var pt1 = new Point("A", 2, 4);
            var pt1Symbol = new PointSymbol(pt1);
            var v = new Var("v");
            var pt2 = new Point("B", 5, v);
            var pt2Symbol = new PointSymbol(pt2);

            var graph = new RelationGraph();
            graph.AddNode(pt1Symbol);
            graph.AddNode(pt2Symbol);

            var d = new Var("d");
            var eqGoal = new EqGoal(d, 5);
            graph.AddNode(eqGoal);
            Assert.True(eqGoal.CachedEntities.Count == 2);

            var query = new Query("v");
            var queryNode = graph.AddNode(query) as QueryNode;
            Assert.Null(queryNode);
            Assert.True(query.Success);
            Assert.True(query.CachedEntities.Count == 2);
        }
    }
}

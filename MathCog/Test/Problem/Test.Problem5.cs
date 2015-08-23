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
    using AlgebraGeometry;    
    using NUnit.Framework;

    [TestFixture]
    public partial class TestProblems
    {
        [Test]
        public void Test_Problem_5()
        {
            /*
             * Given an equation 2y+2x-y+2x+4=0, graph this equation's corresponding shape?
             * What is the slope of this line? 
             */
            const string input1 = "2y+2x-y+2x+4=0";
            const string input2 = "m=";
            var obj = Reasoner.Instance.Load(input1);
            var shapeExpr = obj as AGShapeExpr;
            Assert.NotNull(shapeExpr);
            var lineSymbol = shapeExpr.ShapeSymbol as LineSymbol;
            Assert.NotNull(lineSymbol);
            Assert.True(lineSymbol.ToString().Equals("4x+y+4=0"));

            shapeExpr.GenerateSolvingTrace();
            Assert.Null(shapeExpr.AutoTrace);

            shapeExpr.IsSelected = true;
            shapeExpr.GenerateSolvingTrace();
            Assert.NotNull(shapeExpr.AutoTrace);

            obj = Reasoner.Instance.Load(input2);
            Assert.NotNull(obj);
            var agQueryExpr = obj as AGQueryExpr;
            Assert.NotNull(agQueryExpr);

            agQueryExpr.RetrieveRenderKnowledge();
            Assert.True(agQueryExpr.RenderKnowledge.Count == 1);

            var agPropertyExpr = agQueryExpr.RenderKnowledge[0] as AGPropertyExpr;
            Assert.True(agPropertyExpr != null);

            //Query answer
            agPropertyExpr.IsSelected = true;

            var eqGoal = agPropertyExpr.Goal;
            Assert.NotNull(eqGoal);
            Assert.True(eqGoal.Rhs.Equals(-4));

            //Trace
            agPropertyExpr.GenerateSolvingTrace();
            Assert.True(agPropertyExpr.AutoTrace != null);
        }
    }
}

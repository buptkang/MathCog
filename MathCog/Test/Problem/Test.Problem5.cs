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
        /*
         * Given an equation 2y+2x-y+2x+4=0, graph this equation's corresponding shape?
         * What is the slope of this line? 
         */
        [Test]
        public void Test_Problem_5()
        {
            const string input1 = "2y+2x-y+2x+4=0";

            var obj = Reasoner.Instance.Load(input1);
            var shapeExpr = obj as AGShapeExpr;
            Assert.NotNull(shapeExpr);
            var lineSymbol = shapeExpr.ShapeSymbol as LineSymbol;
            Assert.NotNull(lineSymbol);

            Assert.True(lineSymbol.Traces.Count == 7);
            Assert.True(lineSymbol.StrategyTraces.Count == 1);

            //demonstration mode : look at the semantic KR
            Assert.True(lineSymbol.ToString().Equals("4x+y+4=0"));
            shapeExpr.GenerateSolvingTrace();
            Assert.Null(shapeExpr.AutoTrace);
            shapeExpr.IsSelected = true;
            shapeExpr.GenerateSolvingTrace();
            Assert.NotNull(shapeExpr.AutoTrace);
            //tutor mode: refer to the syntax KR
            //shapeExpr.Expr 
        }

        [Test]
        public void Test_Problem_5_Question1()
        {
            //Tutor Mode to solve Question1
            const string input1 = "2y+2x-y+2x+4=0";
            var obj = Reasoner.Instance.Load(input1);
            var shapeExpr = obj as AGShapeExpr;
            Assert.NotNull(shapeExpr);
            var lineSymbol = shapeExpr.ShapeSymbol as LineSymbol;
            Assert.NotNull(lineSymbol);

            const string query1 = "graph=";
            var obj1 = Reasoner.Instance.Load(query1);
            var queryExpr1 = obj1 as AGQueryExpr;
            Assert.NotNull(queryExpr1);
            
            //Hidden Selected Render Knowledge 
            queryExpr1.RetrieveRenderKnowledge();
            Assert.True(queryExpr1.RenderKnowledge.Count == 1);
            var agShapeExpr1 = queryExpr1.RenderKnowledge[0] as AGShapeExpr;
            Assert.NotNull(agShapeExpr1);

            Assert.True(agShapeExpr1.ShapeSymbol.StrategyTraces.Count == 3);
            Assert.True(agShapeExpr1.ShapeSymbol.Traces.Count == 9);
            var glineSymbol = agShapeExpr1.ShapeSymbol as LineSymbol;
            Assert.NotNull(glineSymbol);
            Assert.True(glineSymbol.OutputType.Equals(LineType.SlopeIntercept));
            Assert.True(glineSymbol.ToString().Equals("y=-4x-4"));
        }

        [Test]
        public void Test_Problem_5_Question2_WithQ1()
        {
            //Solve Question2 based on solved Question1
            const string input1 = "2y+2x-y+2x+4=0";
            var obj = Reasoner.Instance.Load(input1);
            var shapeExpr = obj as AGShapeExpr;
            Assert.NotNull(shapeExpr);
            var lineSymbol = shapeExpr.ShapeSymbol as LineSymbol;
            Assert.NotNull(lineSymbol);

            const string query1 = "graph=";
            var obj1 = Reasoner.Instance.Load(query1);
            var queryExpr1 = obj1 as AGQueryExpr;
            Assert.NotNull(queryExpr1);

            //Hidden Selected Render Knowledge 
            queryExpr1.RetrieveRenderKnowledge();
            Assert.True(queryExpr1.RenderKnowledge.Count == 1);
            var agShapeExpr1 = queryExpr1.RenderKnowledge[0] as AGShapeExpr;
            Assert.NotNull(agShapeExpr1);

            Assert.True(agShapeExpr1.ShapeSymbol.StrategyTraces.Count == 3);
            Assert.True(agShapeExpr1.ShapeSymbol.Traces.Count == 9);
            var glineSymbol = agShapeExpr1.ShapeSymbol as LineSymbol;
            Assert.NotNull(glineSymbol);
            Assert.True(glineSymbol.OutputType.Equals(LineType.SlopeIntercept));
            Assert.True(glineSymbol.ToString().Equals("y=-4x-4"));

            //the above is the same as Test_Problem_5_2
            const string input2 = "m=";
            var obj2 = Reasoner.Instance.Load(input2);
            Assert.NotNull(obj2);
            var agQueryExpr = obj2 as AGQueryExpr;
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
            Assert.True(eqGoal.StrategyTraces.Count == 1);
            //Trace
            agPropertyExpr.GenerateSolvingTrace();
            Assert.True(agPropertyExpr.AutoTrace != null);
            Assert.True(agPropertyExpr.AutoTrace.Count == 1);
        }

        [Test]
        public void Test_Problem_5_Question2_WithoutQ1()
        {
            const string input1 = "2y+2x-y+2x+4=0";
            var obj = Reasoner.Instance.Load(input1);
            var shapeExpr = obj as AGShapeExpr;
            Assert.NotNull(shapeExpr);
            var lineSymbol = shapeExpr.ShapeSymbol as LineSymbol;
            Assert.NotNull(lineSymbol);

            //the above is the same as Test_Problem_5_2
            const string input2 = "m=";
            var obj2 = Reasoner.Instance.Load(input2);
            Assert.NotNull(obj2);
            var agQueryExpr = obj2 as AGQueryExpr;
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
            Assert.True(eqGoal.StrategyTraces.Count == 3);
            //Trace
            agPropertyExpr.GenerateSolvingTrace();
            Assert.True(agPropertyExpr.AutoTrace != null);
            Assert.True(agPropertyExpr.AutoTrace.Count == 9);
        }
    }
}

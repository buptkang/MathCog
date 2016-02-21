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

using System;
using starPadSDK.UnicodeNs;

namespace MathCog
{
    using AlgebraGeometry;
    using CSharpLogic;
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.Linq;

    using Text = starPadSDK.MathExpr.Text;
    using starPadSDK.MathExpr;

    [TestFixture]
    public partial class TestProblems
    {
        /*
         * Problem 1: Find the distance betweeen A(2,0) and B(5,4)?
         * 
         * Problem 2: There exists two points A(2,4) and B(5,v), the distance between A and B is 5. 
         *  What is the value of v?
         *  
         * Problem 4: There is a line, the slope of it is 3, the y-intercept of it is 2. 
         * What is the slope intercept form of this line? 
         * What is the general form of this line?
         * 
         * Problem 5: Given an equation 2y+2x-y+2x+4=0, graph this equation's corresponding shape?
         * What is the slope of this line? 
         * 
         * Problem 6: A line passes through points (2,3) and (4,y), the slope of this line is 5. 
         * What is the y-intercept of the line?
         * 
         * Problem 10: A line contains the point (0,5) and is perpendicular to the line 4y=x, what is the slope of this line? 
         * What is the general form of this line?
         * 
         * Problem 11: Line A passes through (-1,2) and (5,8). Line B is parallel to line A, and also crosses point (1,0), 
         * what is the general form of line B?
         * 
         * Problem 16: Find the midpoint of the line joining A(-1,2) and B(4,6).
         * 
         * Problem 29: Line A passes through two points (4,3) and (2,v).The line is perpendicular to line B in which the slope of line B is 1/2. 
         * What is the value of v?
         * 
         * Problem 56: Line A contains the point (2,3) and is perpendicular to the line B x-2y+1=0, what is the slope (use notation m) of line A? 
         * What is the general form of line A?
         * 
         * Problem 96: Simplify the expression x+2-1?
         * 
         * Problem 97: Simplify the expression x+1+x-4?
         * 
         * Problem 98: Simplify the expression 1+2-3?
         * 
         * Problem 99: Simplify the expression 1+2+3?
         * 
         * Problem 61: Line A is perpendicular to the line 2y=4x+8 and passes through (-2,-8). 
         * Draw line A on the geometric coordinate canvas. 
         * Write the equation of line A on the algebraic canvas.
         * 
         * 2y=4x+8
         * m*m1=-1
         * pass(A, (-2,-8))
         * (-2,-8)
         * A((-2,-8),)
         * 
         * Problem 62: Line C passes through the point (5,2). It is parallel to the line 4x+6y-12=0. 
         * Draw line C on the geometric coordinate canvas. 
         * Write the equation of line C on the algebraic canvas.
         * 
         * Problem 63: Find the minimum distance between the point (1,−3) and the line y = -x + 6. 
         * Draw this line, point, and the line segment between them onto the geometric coordinate canvas. 
         * Write the value of the distance, d, in the algebraic canvas. Round your answer to two decimal numbers.
         * 
         * Problem 64: Circle A is centered about the origin and has a radius of 5. Line B is tangent to circle A at the point (-3,4). 
         * Draw circle and line on the geometric coordinate canvas. 
         * What is the equation of the line that is tangent to circle A at the point (-3,4)?
         * 
         * Problem 65: The equation of line A is 2y-4x = 10. Line B is perpendicular to line A. 
         * Write the equation of line B, given that it passes through the point (1,2). 
         * Graph this equation.
         * 
         * Problem 66: Line D has the same slope as the line y=2x-6. Line D passes through the point (0,5). 
         * Draw line D on the geometric coordinate canvas. 
         * Write the equation of line D on the algebraic canvas.
         * 
         * Problem 67: Find the distance between the point (-2,-4) and the line 3y=-x+6. 
         * Draw this line, point, and the line segment between them onto the geometric coordinate canvas. 
         * Write the value of the distance, d, in the algebraic canvas. Round your answer to two decimal numbers.
         * 
         * Problem 68: Circle A is described by the equation (x-2)^2+(y-3)^2 = 25. 
         * What is the equation of a line that is tangent to this circle at the point (5,7)? 
         * Draw this circle and the tangent line on the geometric coordinate canvas. 
         * What is the equation of this tangent line?
         * 
         */

        #region Problem 1

        [Test]
        public void Test_Problem_01()
        {
            // general solving ability
            const string input1 = "A(2,0)"; 
            const string input2 = "B(5,4)";
            const string query = "d=";

            Reasoner.Instance.Load(input1);
            Reasoner.Instance.Load(input2);

            var obj = Reasoner.Instance.Load(query);
            Assert.NotNull(obj);
            var agQueryExpr = obj as AGQueryExpr;
            Assert.NotNull(agQueryExpr);
            Assert.True(agQueryExpr.RenderKnowledge == null);
            agQueryExpr.RetrieveRenderKnowledge();
            Assert.True(agQueryExpr.RenderKnowledge != null);
            Assert.True(agQueryExpr.RenderKnowledge.Count == 1);

            var answerExpr = agQueryExpr.RenderKnowledge[0] as AGPropertyExpr;
            Assert.NotNull(answerExpr);
            Assert.True(answerExpr.Goal.Rhs.Equals(5.0));
            Assert.Null(answerExpr.AutoTrace);

            answerExpr.IsSelected = true;
            answerExpr.GenerateSolvingTrace();
            Assert.NotNull(answerExpr.AutoTrace);
            Assert.True(answerExpr.AutoTrace.Count == 3);
          /*  
            int count = answerExpr.RetrieveStepsNumbers();
            Assert.True(count == 8);*/

            Reasoner.Instance.Reset();
        }

        [Test]
        public void Test_Problem_01_Sequence()
        {
            //sequence input test
            const string query = "d=";
            const string input1 = "A(2,0)";
            const string input2 = "B(5,4)";
            var obj = Reasoner.Instance.Load(query);
            Assert.NotNull(obj);
            var agQueryExpr = obj as AGQueryExpr;
            Assert.NotNull(agQueryExpr);
            agQueryExpr.RetrieveRenderKnowledge();
            Assert.True(agQueryExpr.RenderKnowledge == null);

            Reasoner.Instance.Load(input1);
            Reasoner.Instance.Load(input2);

            agQueryExpr.RetrieveRenderKnowledge();
            Assert.True(agQueryExpr.RenderKnowledge != null);
            Assert.True(agQueryExpr.RenderKnowledge.Count == 1);

            var answerExpr = agQueryExpr.RenderKnowledge[0] as AGPropertyExpr;
            Assert.NotNull(answerExpr);
            Assert.True(answerExpr.Goal.Rhs.Equals(5.0));
            Assert.Null(answerExpr.AutoTrace);

            answerExpr.IsSelected = true;
            answerExpr.GenerateSolvingTrace();
            Assert.NotNull(answerExpr.AutoTrace);

            Reasoner.Instance.Reset();
        }

        [Test]
        public void Test_Problem_01_Constraint()
        {
            const string input1 = "A(2,0)";
            const string input2 = "B(5,4)";
            const string query1 = "AB";

            Reasoner.Instance.Load(input1);
            Reasoner.Instance.Load(input2);
            var obj = Reasoner.Instance.Load(query1);
            Assert.NotNull(obj);
            var agQueryExpr = obj as AGQueryExpr;
            Assert.NotNull(agQueryExpr);
            var query = agQueryExpr.QueryTag as Query;
            Assert.NotNull(query);
            Assert.False(query.Success);
            var shapeTypes = query.FeedBack as List<ShapeType>;
            Assert.NotNull(shapeTypes);

            query.Constraint2 = ShapeType.LineSegment;
            var queryNode = Reasoner.Instance.RelationGraph.RetrieveQueryNode(query);
            Assert.NotNull(queryNode);
            Assert.True(queryNode.Query.Success);
            Assert.Null(query.FeedBack);
            Assert.True(queryNode.InternalNodes.Count == 1);
            var sn = queryNode.InternalNodes[0] as ShapeNode;
            Assert.NotNull(sn);
            var ls = sn.ShapeSymbol as LineSegmentSymbol;
            Assert.NotNull(ls);
            //Assert.True(ls.ToString().Equals("x-y+1=0"));

            const string query2 = "d=";
            var obj2 = Reasoner.Instance.Load(query2);

            Assert.True(query.CachedEntities.Count == 1);

            Reasoner.Instance.Reset();
        }

        [Test]
        public void Test_Problem_01_RealTime_dynamicGeometry()
        {
            /*
             * Problem 1: Find the distance betweeen A(2,0) and B(5,4)?
             */
            const string input1 = "A(2,0)";
            const string input2 = "B(5,4)";
            const string query = "d=";

            Reasoner.Instance.Load(input1);
            Reasoner.Instance.Load(input2);

            var obj = Reasoner.Instance.Load(query);

            var agQueryExpr = obj as AGQueryExpr;
            Assert.NotNull(agQueryExpr);

            const string input2Update = "B(5,6)";
            Reasoner.Instance.Unload(input2);

            agQueryExpr.RetrieveRenderKnowledge();
            Assert.True(agQueryExpr.RenderKnowledge == null);
            
            Reasoner.Instance.Load(input2Update);

            agQueryExpr.RetrieveRenderKnowledge();
            Assert.True(agQueryExpr.RenderKnowledge != null);
            Assert.True(agQueryExpr.RenderKnowledge.Count == 1);

            var agPropExpr = agQueryExpr.RenderKnowledge[0] as AGPropertyExpr;
            Assert.NotNull(agPropExpr != null);

            Reasoner.Instance.Reset();
        }
       
        [Test]
        public void Test_Problem_01_Reify()
        {
            // general solving ability
            const string input1 = "A(2,0)";
            const string input2 = "B(5,v)";
            const string input0 = "v=3";
            const string query  = "d=";

            Reasoner.Instance.Load(input1);
            Reasoner.Instance.Load(input2);
            Reasoner.Instance.Load(input0);

            var obj = Reasoner.Instance.Load(query);
            Assert.NotNull(obj);
            var agQueryExpr = obj as AGQueryExpr;
            Assert.NotNull(agQueryExpr);
            var queryTag = agQueryExpr.QueryTag;
            Assert.NotNull(queryTag);
            Assert.True(queryTag.CachedEntities.Count == 1);

            Reasoner.Instance.Reset();
        }

        #region Behavior Validate 

        [Test]
        public void Test_Problem_01_BehaviorValidate_01()
        {
            // general solving ability
            const string input1 = "A(2,0)";
            const string input2 = "B(5,4)";
            const string query = "d=";

            Reasoner.Instance.Load(input1);
            Reasoner.Instance.Load(input2);

            var obj = Reasoner.Instance.Load(query);
            Assert.NotNull(obj);
            var agQueryExpr = obj as AGQueryExpr;
            Assert.NotNull(agQueryExpr);
            Assert.True(agQueryExpr.RenderKnowledge == null);
            agQueryExpr.RetrieveRenderKnowledge();
            Assert.True(agQueryExpr.RenderKnowledge != null);
            Assert.True(agQueryExpr.RenderKnowledge.Count == 1);

            var answerExpr = agQueryExpr.RenderKnowledge[0] as AGPropertyExpr;
            Assert.NotNull(answerExpr);
            Assert.True(answerExpr.Goal.Rhs.Equals(5.0));
            Assert.Null(answerExpr.AutoTrace);

            answerExpr.IsSelected = true;
            answerExpr.GenerateSolvingTrace();
            Assert.NotNull(answerExpr.AutoTrace);
            Assert.True(answerExpr.AutoTrace.Count == 3);

            //d^2 = 16 + 9
            object obj44 = ExprMock.Mock2();
            var userKnowledge = Reasoner.Instance.Load(obj44, null, true) as AGEquationExpr;
            Assert.NotNull(userKnowledge);

 /*           var rTemp = Reasoner.Instance.ExprValidate(userKnowledge.Expr);
            Assert.NotNull(rTemp);*/

            Reasoner.Instance.Reset();
        }

        [Test]
        public void Test_Problem_01_BehaviorValidate_02()
        {
            // general solving ability
            const string input1 = "A(2,0)";
            const string input2 = "B(5,4)";
            const string query = "d=";

            Reasoner.Instance.Load(input1);
            Reasoner.Instance.Load(input2);

            var obj = Reasoner.Instance.Load(query);
            Assert.NotNull(obj);
            var agQueryExpr = obj as AGQueryExpr;
            Assert.NotNull(agQueryExpr);
            Assert.True(agQueryExpr.RenderKnowledge == null);
            agQueryExpr.RetrieveRenderKnowledge();
            Assert.True(agQueryExpr.RenderKnowledge != null);
            Assert.True(agQueryExpr.RenderKnowledge.Count == 1);

            var answerExpr = agQueryExpr.RenderKnowledge[0] as AGPropertyExpr;
            Assert.NotNull(answerExpr);
            Assert.True(answerExpr.Goal.Rhs.Equals(5.0));
            Assert.Null(answerExpr.AutoTrace);

            answerExpr.IsSelected = true;
            answerExpr.GenerateSolvingTrace();
            Assert.NotNull(answerExpr.AutoTrace);
            Assert.True(answerExpr.AutoTrace.Count == 3);

            //d = Sqrt(16+9)
            object obj44 = ExprMock.Mock3();
            var userKnowledge = Reasoner.Instance.Load(obj44, null, true) as AGEquationExpr;
            Assert.NotNull(userKnowledge);

            var rTemp = Reasoner.Instance.ExprValidate(userKnowledge.Expr) as AGPropertyExpr;
            Assert.NotNull(rTemp);
            Assert.True(rTemp.Goal.Traces.Count != 0);

            Reasoner.Instance.Reset();
        }

        [Test]
        public void Test_Problem_01_BehaviorValidate_03()
        {
            #region Problem Input

            // general solving ability
            const string input1 = "A(2,0)";
            const string input2 = "B(5,4)";
            const string query = "d=";

            Reasoner.Instance.Load(input1);
            Reasoner.Instance.Load(input2);

            var obj = Reasoner.Instance.Load(query);
            Assert.NotNull(obj);
            var agQueryExpr = obj as AGQueryExpr;
            Assert.NotNull(agQueryExpr);
            Assert.True(agQueryExpr.RenderKnowledge == null);
            agQueryExpr.RetrieveRenderKnowledge();
            Assert.True(agQueryExpr.RenderKnowledge != null);
            Assert.True(agQueryExpr.RenderKnowledge.Count == 1);

            var answerExpr = agQueryExpr.RenderKnowledge[0] as AGPropertyExpr;
            Assert.NotNull(answerExpr);
            Assert.True(answerExpr.Goal.Rhs.Equals(5.0));
            Assert.Null(answerExpr.AutoTrace);

            answerExpr.IsSelected = true;
            answerExpr.GenerateSolvingTrace();
            Assert.NotNull(answerExpr.AutoTrace);
            Assert.True(answerExpr.AutoTrace.Count == 3);

            Assert.True(Reasoner.Instance.RelationGraph.Nodes.Count == 3);

            #endregion

            //user input a line segment in geometry side
            //it should validate its correctness

            var lss = MockSegment();
            var userKnowledge = Reasoner.Instance.Load(lss, null, true) as AGShapeExpr;
            Assert.NotNull(userKnowledge);

            Assert.True(Reasoner.Instance.RelationGraph.Nodes.Count == 3);

            //deep understanding
            object output;

            var rTemp = Reasoner.Instance.RelationValidate(userKnowledge.ShapeSymbol, out output);
            Assert.NotNull(rTemp);

            /////////////////////////////////////////
            Reasoner.Instance.Reset();
        }

        private LineSegmentSymbol MockSegment()
        {
            var pt1 = new Point("A", 2, 0);
            var pt2 = new Point("B", 5, 4);
            var seg = new LineSegment("AB", pt1, pt2);
            var segSymbol = new LineSegmentSymbol(seg);
            return segSymbol;
        }

        #endregion

        #endregion

        #region Problem 2

        [Test]
        public void Test_Problem_02()
        {
            const string input1 = "A(2,4)";
            const string input2 = "B(5,v)";
            const string input3 = "d=5";
            const string query = "v=";

            Reasoner.Instance.Load(input1);
            Reasoner.Instance.Load(input2);
            Reasoner.Instance.Load(input3);

            Assert.True(Reasoner.Instance.RelationGraph.Nodes.Count == 5);


            var obj = Reasoner.Instance.Load(query);
            Assert.NotNull(obj);
            var agQueryExpr = obj as AGQueryExpr;
            Assert.NotNull(agQueryExpr);
            var queryTag = agQueryExpr.QueryTag;
            Assert.NotNull(queryTag);
            Assert.True(queryTag.Success);
            Assert.True(queryTag.CachedEntities.Count == 2);
            var goal1 = queryTag.CachedEntities.ToList()[0] as EqGoal;
            Assert.NotNull(goal1);
            Assert.True(goal1.Rhs.Equals(0));
            var goal2 = queryTag.CachedEntities.ToList()[1] as EqGoal;
            Assert.NotNull(goal2);
            Assert.True(goal2.Rhs.Equals(8));

            Reasoner.Instance.Reset();
        }

        #region Behavior Validate

        [Test]
        public void Test_Problem_02_BehaviorValidate_01()
        {
            const string input1 = "A(2,4)";
            const string input2 = "B(5,v)";
            const string query = "v=";

            bool? result = Reasoner.Instance.VerifyConcreteSymbol(input1);
            Assert.True(result != null);
            Assert.True(result.Value);

            result = Reasoner.Instance.VerifyConcreteSymbol(input2);
            Assert.True(result != null);
            Assert.False(result.Value);

            result = Reasoner.Instance.VerifyConcreteSymbol(query);
            Assert.Null(result);
        }

        #endregion


        #endregion

        #region Problem 4

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


        #endregion

        #region Problem 5

        [Test]
        public void Test_Problem_05()
        {
            const string input1 = "2y+2x-y+2x+4=0";

            var obj = Reasoner.Instance.Load(input1);
            var shapeExpr = obj as AGShapeExpr;
            Assert.NotNull(shapeExpr);
            var lineSymbol = shapeExpr.ShapeSymbol as LineSymbol;
            Assert.NotNull(lineSymbol);
            //demonstration mode : look at the semantic KR
            Assert.True(lineSymbol.ToString().Equals("4x+y+4=0"));
            shapeExpr.GenerateSolvingTrace();
            Assert.Null(shapeExpr.AutoTrace);
            shapeExpr.IsSelected = true;
            shapeExpr.GenerateSolvingTrace();
            Assert.NotNull(shapeExpr.AutoTrace);
            Assert.True(shapeExpr.AutoTrace.Count == 3);

            const string query1 = "graph=";
            var obj1 = Reasoner.Instance.Load(query1);
            var queryExpr1 = obj1 as AGQueryExpr;
            Assert.NotNull(queryExpr1);
            //Hidden Selected Render Knowledge 
            queryExpr1.RetrieveRenderKnowledge();
            Assert.True(queryExpr1.RenderKnowledge.Count == 1);
            var agShapeExpr1 = queryExpr1.RenderKnowledge[0] as AGShapeExpr;
            Assert.NotNull(agShapeExpr1);
            agShapeExpr1.IsSelected = true;
            agShapeExpr1.GenerateSolvingTrace();
            Assert.True(agShapeExpr1.AutoTrace.Count == 3);

            var glineSymbol = agShapeExpr1.ShapeSymbol as LineSymbol;
            Assert.NotNull(glineSymbol);
            /*            Assert.True(glineSymbol.OutputType.Equals(LineType.GeneralForm));
                        Assert.True(glineSymbol.ToString().Equals("y=-4x-4"));*/

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

            //Trace
            agPropertyExpr.GenerateSolvingTrace();
            Assert.True(agPropertyExpr.AutoTrace != null);
            Assert.True(agPropertyExpr.AutoTrace.Count == 4);


            Reasoner.Instance.Reset();
        }

        [Test]
        public void Test_Problem_05_Question2_WithoutQ1()
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
            //Trace
            agPropertyExpr.GenerateSolvingTrace();
            Assert.True(agPropertyExpr.AutoTrace != null);

            Reasoner.Instance.Reset();

        }


        #region Behavior Validate

        [Test]
        public void Test_Problem_05_BehaviorValidate_01()
        {
            #region Problem Setup

            const string input1 = "2y+2x-y+2x+4=0";

            var obj = Reasoner.Instance.Load(input1);
            var shapeExpr = obj as AGShapeExpr;
            Assert.NotNull(shapeExpr);
            var lineSymbol = shapeExpr.ShapeSymbol as LineSymbol;
            Assert.NotNull(lineSymbol);
            //demonstration mode : look at the semantic KR
            Assert.True(lineSymbol.ToString().Equals("4x+y+4=0"));
            shapeExpr.GenerateSolvingTrace();
            Assert.Null(shapeExpr.AutoTrace);
            shapeExpr.IsSelected = true;
            shapeExpr.GenerateSolvingTrace();
            Assert.NotNull(shapeExpr.AutoTrace);

            const string query1 = "graph=";
            var obj1 = Reasoner.Instance.Load(query1);
            var queryExpr1 = obj1 as AGQueryExpr;
            Assert.NotNull(queryExpr1);
            //Hidden Selected Render Knowledge 
            queryExpr1.RetrieveRenderKnowledge();
            Assert.True(queryExpr1.RenderKnowledge.Count == 1);
            var agShapeExpr1 = queryExpr1.RenderKnowledge[0] as AGShapeExpr;
            Assert.NotNull(agShapeExpr1);

            var glineSymbol = agShapeExpr1.ShapeSymbol as LineSymbol;
            Assert.NotNull(glineSymbol);
            //Assert.True(glineSymbol.OutputType.Equals(LineType.SlopeIntercept));
            //Assert.True(glineSymbol.ToString().Equals("y=-4x-4"));

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

            //Trace
            agPropertyExpr.GenerateSolvingTrace();
            Assert.True(agPropertyExpr.AutoTrace != null);

            #endregion

            string userInput = "(0,-4)";
            Expr expr = Text.Convert(userInput);
            var userExpr = Reasoner.Instance.ExprValidate(expr) as AGShapeExpr;
            Assert.NotNull(userExpr);

            object inferOutput;
            var boolResult = Reasoner.Instance.RelationValidate(userExpr.ShapeSymbol, out inferOutput) as bool?;
            Assert.NotNull(boolResult);
            Assert.True(boolResult.Value);
            Reasoner.Instance.Reset();
        }

        #endregion



        #endregion

        #region Problem 6

        [Test]
        public void Test_Problem_06()
        {
            const string input1 = "(2,3)";
            const string input2 = "(4,y)";
            const string input3 = "m=5";
            const string query0 = "y";
            const string query = "k";

            Reasoner.Instance.Load(input1);
            Reasoner.Instance.Load(input2);
            Reasoner.Instance.Load(input3);
            Assert.True(Reasoner.Instance.RelationGraph.Nodes.Count == 4);

            var obj = Reasoner.Instance.Load(query0);
            Assert.NotNull(obj);
            var agQueryExpr = obj as AGQueryExpr;
            Assert.NotNull(agQueryExpr);
            var queryTag = agQueryExpr.QueryTag;
            Assert.NotNull(queryTag);
            Assert.True(queryTag.Success);
            Assert.True(queryTag.CachedEntities.Count == 3);

            var gGoal3 = queryTag.CachedEntities.ToList()[2] as EqGoal;
            Assert.NotNull(gGoal3);
            Assert.True(gGoal3.Traces.Count == 2);

            var agQueryExpr1 = Reasoner.Instance.Load(query) as AGQueryExpr;
            Assert.NotNull(agQueryExpr1);
            var queryTag1 = agQueryExpr1.QueryTag;
            Assert.NotNull(queryTag1);
            Assert.True(queryTag1.Success);
            Assert.True(queryTag1.CachedEntities.Count == 1);
            var goal1 = queryTag1.CachedEntities.ToList()[0] as EqGoal;
            Assert.NotNull(goal1);
            Assert.True(goal1.ToString().Equals("k=-7"));
            Assert.True(goal1.Traces.Count != 0);

            Reasoner.Instance.Reset();
        }

        #endregion

        #region Problem 10

        [Test]
        public void Test_Problem_10()
        {
            const string input1 = "(0,5)";
            const string input2 = "4y=x";
            const string constraint0 = "m1=";
            const string input3 = "m1*m=-1";
            const string query0 = "m=";
            const string query1 = "lineG";

            Reasoner.Instance.Load(input1);
            Reasoner.Instance.Load(input2);
            var agQueryExpr = Reasoner.Instance.Load(constraint0) as AGQueryExpr;
            Assert.NotNull(agQueryExpr);
            var queryTag = agQueryExpr.QueryTag;
            Assert.NotNull(queryTag);
            Assert.True(queryTag.Success);
            agQueryExpr.RetrieveRenderKnowledge();
            var answerExpr = agQueryExpr.RenderKnowledge[0] as AGPropertyExpr;
            Assert.NotNull(answerExpr);
            Assert.True(answerExpr.Goal.Rhs.Equals(0.25));
            Assert.Null(answerExpr.AutoTrace);
            answerExpr.IsSelected = true;
            answerExpr.GenerateSolvingTrace();
            Assert.NotNull(answerExpr.AutoTrace);

            Reasoner.Instance.Load(input3);
            Assert.True(Reasoner.Instance.RelationGraph.Nodes.Count == 5);

            var lst = Reasoner.Instance.RelationGraph.RetrieveGoalNodes();
            Assert.True(lst != null);
            var goalNode = lst.ToList()[0];
            Assert.NotNull(goalNode);
            var synGoal = goalNode.Goal as EqGoal;
            Assert.NotNull(synGoal);
            Assert.True(synGoal.Rhs.ToString().Equals("-4"));
            Assert.True(synGoal.Traces.Count != 0);

            ////////////////////////////////////////////////////////////////

            var queryExpr0 = Reasoner.Instance.Load(query0) as AGQueryExpr;
            Assert.NotNull(queryExpr0);
            Assert.True(queryExpr0.RenderKnowledge == null);
            queryExpr0.RetrieveRenderKnowledge();
            Assert.True(queryExpr0.RenderKnowledge != null);
            Assert.True(queryExpr0.RenderKnowledge.Count == 2);

            var answerExpr0 = queryExpr0.RenderKnowledge[1] as AGPropertyExpr;
            Assert.NotNull(answerExpr0);
            Assert.Null(answerExpr0.AutoTrace);
            answerExpr0.IsSelected = true;
            answerExpr0.GenerateSolvingTrace();
            Assert.True(answerExpr0.AutoTrace.Count == 5);

            ////////////////////////////////////////////////////////////////

            var queryExpr2 = Reasoner.Instance.Load(query1) as AGQueryExpr;
            Assert.NotNull(queryExpr2);
            Assert.True(queryExpr2.RenderKnowledge == null);
            queryExpr2.RetrieveRenderKnowledge();
            Assert.True(queryExpr2.RenderKnowledge != null);
            Assert.True(queryExpr2.RenderKnowledge.Count == 2);
            var answerExpr2 = queryExpr2.RenderKnowledge[1] as AGShapeExpr;
            Assert.NotNull(answerExpr2);
            Assert.True(answerExpr2.ShapeSymbol.ToString().Equals("4x+y-5=0"));
            Assert.Null(answerExpr2.AutoTrace);
            answerExpr2.IsSelected = true;
            answerExpr2.GenerateSolvingTrace();
            Assert.True(answerExpr2.AutoTrace.Count == 6);

            Reasoner.Instance.Reset();
        }


        #endregion

        #region Problem 11

        [Test]
        public void Test_Problem_11()
        {
            const string input1 = "(-1,2)";
            const string input2 = "(5,8)";
            const string constraint1 = "m1";
            const string input3 = "m1=m";
            const string query2 = "lineG";
            const string input4 = "(1,0)";

            Reasoner.Instance.Load(input1);
            Reasoner.Instance.Load(input2);
            Reasoner.Instance.Load(constraint1);
            Reasoner.Instance.Load(input3);
            var queryExpr2 = Reasoner.Instance.Load(query2) as AGQueryExpr;

            Assert.NotNull(queryExpr2);
            Assert.True(queryExpr2.RenderKnowledge == null);
            queryExpr2.RetrieveRenderKnowledge();
            Assert.True(queryExpr2.RenderKnowledge != null);
            Assert.True(queryExpr2.RenderKnowledge.Count == 1);

            Reasoner.Instance.Load(input4);

            /////////////////////////////////////////////////////////

            Assert.NotNull(queryExpr2);
            queryExpr2.RetrieveRenderKnowledge();
            Assert.True(queryExpr2.RenderKnowledge != null);
            Assert.True(queryExpr2.RenderKnowledge.Count == 5);

            var answerExpr2 = queryExpr2.RenderKnowledge[4] as AGShapeExpr;
            Assert.NotNull(answerExpr2);
            Assert.True(answerExpr2.ShapeSymbol.ToString().Equals("x-y-1=0"));
            Assert.Null(answerExpr2.AutoTrace);
            answerExpr2.IsSelected = true;
            answerExpr2.GenerateSolvingTrace();
            Assert.NotNull(answerExpr2.AutoTrace);

            Reasoner.Instance.Reset();
        }


        /*
         * Find the slope-intercept form of line equation parallel to y=5x-2 and passing through point (2,1). 
         * (use notation m, m1 to represent line slope)
         * 
         */
        [Test]
        public void Test_Problem_11_1()
        {
            const string input1 = "y=5x-2";
            const string input2 = "(2,1)";
            const string constraint1 = "m";
            const string input3 = "m=m1";
            const string input4 = "(1,0)";
            const string query = "lineG";

            Reasoner.Instance.Load(input1);
            Reasoner.Instance.Load(input2);

            var agQueryExpr = Reasoner.Instance.Load(constraint1) as AGQueryExpr;
            Assert.NotNull(agQueryExpr);
            var queryTag = agQueryExpr.QueryTag;
            Assert.NotNull(queryTag);
            Assert.True(queryTag.Success);
            agQueryExpr.RetrieveRenderKnowledge();

            var answerExpr = agQueryExpr.RenderKnowledge[0] as AGPropertyExpr;
            Assert.NotNull(answerExpr);
            Assert.True(answerExpr.Goal.Rhs.Equals(5.0));
            Assert.Null(answerExpr.AutoTrace);
            answerExpr.IsSelected = true;
            answerExpr.GenerateSolvingTrace();
            Assert.True(answerExpr.AutoTrace.Count == 2);

            Reasoner.Instance.Load(input3);
            Reasoner.Instance.Load(input4);

            ///////////////////////////////////////////////////////////////

            var queryExpr2 = Reasoner.Instance.Load(query) as AGQueryExpr;
            Assert.NotNull(queryExpr2);
            Assert.True(queryExpr2.RenderKnowledge == null);
            queryExpr2.RetrieveRenderKnowledge();
            Assert.True(queryExpr2.RenderKnowledge != null);
            Assert.True(queryExpr2.RenderKnowledge.Count == 4);

            var answerExpr1 = queryExpr2.RenderKnowledge[1] as AGShapeExpr;
            Assert.NotNull(answerExpr1);
            Assert.Null(answerExpr1.AutoTrace);
            answerExpr1.IsSelected = true;
            answerExpr1.GenerateSolvingTrace();
            Assert.NotNull(answerExpr1);
            Assert.True(answerExpr1.AutoTrace.Count == 5);

            Reasoner.Instance.Reset();
        }


        #endregion

        #region Problem 16

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


        #endregion

        #region Problem 28

        /*
         * Problem 28: There are two points A(2,y) and B(-1,4). The y-coordinate of point A is -1. What is the distance betweeen these two points?
         */
        [Test]
        public void Test_Problem_28()
        {
            const string input1 = "A(2,y)";
            const string input2 = "B(-1,4)";
            const string input3 = "y=-1";
            const string query = "d=";

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

        #endregion

        #region Problem 29

        [Test]
        public void Test_Problem_29()
        {
            const string input1 = "(4,3)";
            const string input2 = "(2,v)";
            const string input4 = "m1=0.5";
            const string input3 = "m*m1=-1";
            const string query = "v";

            Reasoner.Instance.Load(input1);
            Reasoner.Instance.Load(input2);
            Reasoner.Instance.Load(input4);
            Assert.True(Reasoner.Instance.RelationGraph.Nodes.Count == 4);
            Reasoner.Instance.Load(input3);
            Assert.True(Reasoner.Instance.RelationGraph.Nodes.Count == 7);

            var queryExpr2 = Reasoner.Instance.Load(query) as AGQueryExpr;
            Assert.NotNull(queryExpr2);
            Assert.True(queryExpr2.RenderKnowledge == null);
            queryExpr2.RetrieveRenderKnowledge();
            Assert.True(queryExpr2.RenderKnowledge != null);
            Assert.True(queryExpr2.RenderKnowledge.Count == 2);

            var agPropExpr = queryExpr2.RenderKnowledge[1] as AGPropertyExpr;
            Assert.NotNull(agPropExpr);
            var gGoal = agPropExpr.Goal;
            Assert.NotNull(gGoal);
            Assert.True(gGoal.Rhs.ToString().Equals("7"));
            agPropExpr.IsSelected = true;
            agPropExpr.GenerateSolvingTrace();
            Assert.True(agPropExpr.AutoTrace != null);
            Assert.True(agPropExpr.AutoTrace.Count == 4);

            bool result = Reasoner.Instance.RelationGraph.FindQuery("v");
            Assert.True(result);

            Reasoner.Instance.Reset();
        }

        public void Test_Problem_29_1()
        {
            //TODO
            const string input1 = "(4,3)";
            const string input2 = "(2,v)";
            //const string query1 = "m1";
            const string input3 = "m1*m2=-1";
            const string input4 = "m2=0.5";

            Reasoner.Instance.Load(input1);
            Reasoner.Instance.Load(input2);

            Reasoner.Instance.Load(input3);
            Assert.True(Reasoner.Instance.RelationGraph.Nodes.Count == 3);
            Reasoner.Instance.Load(input4);

            Assert.True(Reasoner.Instance.RelationGraph.Nodes.Count == 8);

            const string query2 = "v";
            //Reasoner.Instance.Load(query1);
            Reasoner.Instance.Reset();
        }


        #endregion

        #region Problem 56

        [Test]
        public void Test_Problem_56()
        {
            const string input1 = "(2,3)";
            const string input2 = "x-2y+1=0";
            const string constraint0 = "m1=";
            const string input3 = "m1*m=-1";
            const string query0 = "m=";
            const string query1 = "lineG";

            Reasoner.Instance.Load(input1);
            Reasoner.Instance.Load(input2);
            var agQueryExpr = Reasoner.Instance.Load(constraint0) as AGQueryExpr;
            Assert.NotNull(agQueryExpr);
            var queryTag = agQueryExpr.QueryTag;
            Assert.NotNull(queryTag);
            Assert.True(queryTag.Success);
            agQueryExpr.RetrieveRenderKnowledge();
            var answerExpr = agQueryExpr.RenderKnowledge[0] as AGPropertyExpr;
            Assert.NotNull(answerExpr);
            Assert.True(answerExpr.Goal.Rhs.Equals(0.5));
            Assert.Null(answerExpr.AutoTrace);
            answerExpr.IsSelected = true;
            answerExpr.GenerateSolvingTrace();
            Assert.NotNull(answerExpr.AutoTrace);

            Reasoner.Instance.Load(input3);
            Assert.True(Reasoner.Instance.RelationGraph.Nodes.Count == 5);

            var lst = Reasoner.Instance.RelationGraph.RetrieveGoalNodes();
            Assert.True(lst != null);
            var goalNode = lst.ToList()[0];
            Assert.NotNull(goalNode);
            var synGoal = goalNode.Goal as EqGoal;
            Assert.NotNull(synGoal);
            Assert.True(synGoal.Rhs.ToString().Equals("-2"));
            Assert.True(synGoal.Traces.Count != 0);

            ////////////////////////////////////////////////////////////////

            var queryExpr0 = Reasoner.Instance.Load(query0) as AGQueryExpr;
            Assert.NotNull(queryExpr0);
            Assert.True(queryExpr0.RenderKnowledge == null);
            queryExpr0.RetrieveRenderKnowledge();
            Assert.True(queryExpr0.RenderKnowledge != null);
            Assert.True(queryExpr0.RenderKnowledge.Count == 2);

            var answerExpr0 = queryExpr0.RenderKnowledge[1] as AGPropertyExpr;
            Assert.NotNull(answerExpr0);
            Assert.Null(answerExpr0.AutoTrace);
            answerExpr0.IsSelected = true;
            answerExpr0.GenerateSolvingTrace();
            Assert.True(answerExpr0.AutoTrace.Count == 5);

            ////////////////////////////////////////////////////////////////

            var queryExpr2 = Reasoner.Instance.Load(query1) as AGQueryExpr;
            Assert.NotNull(queryExpr2);
            Assert.True(queryExpr2.RenderKnowledge == null);
            queryExpr2.RetrieveRenderKnowledge();
            Assert.True(queryExpr2.RenderKnowledge != null);
            Assert.True(queryExpr2.RenderKnowledge.Count == 2);
            var answerExpr2 = queryExpr2.RenderKnowledge[1] as AGShapeExpr;
            Assert.NotNull(answerExpr2);
            Assert.True(answerExpr2.ShapeSymbol.ToString().Equals("2x+y-7=0"));
            Assert.Null(answerExpr2.AutoTrace);
            answerExpr2.IsSelected = true;
            answerExpr2.GenerateSolvingTrace();
            Assert.True(answerExpr2.AutoTrace.Count == 6);

            Reasoner.Instance.Reset();
        }


        #endregion

        #region Problem 96

        [Test]
        public void Test_Problem_96()
        {
            const string fact1 = "x+2-1=";
            var queryExpr = Reasoner.Instance.Load(fact1) as AGQueryExpr;
            Assert.NotNull(queryExpr);
            queryExpr.RetrieveRenderKnowledge();
            Assert.True(queryExpr.RenderKnowledge.Count == 1);

            var agEquationExpr = queryExpr.RenderKnowledge[0] as AGEquationExpr;
            Assert.True(agEquationExpr != null);
            agEquationExpr.IsSelected = true;

            agEquationExpr.GenerateSolvingTrace();
            Assert.True(agEquationExpr.AutoTrace != null);
            Assert.True(agEquationExpr.AutoTrace.Count == 1);

            var steps = agEquationExpr.AutoTrace[0].Item2 as List<TraceStepExpr>;
            Assert.NotNull(steps);
            Assert.True(steps.Count == 1);

            Reasoner.Instance.Reset();
        }

        #endregion

        #region Problem 97

        [Test]
        public void Test_Problem_97()
        {
            const string fact1 = "x+1+x-4=";
            var queryExpr = Reasoner.Instance.Load(fact1) as AGQueryExpr;
            Assert.NotNull(queryExpr);
            queryExpr.RetrieveRenderKnowledge();
            Assert.True(queryExpr.RenderKnowledge.Count == 1);

            var agEquationExpr = queryExpr.RenderKnowledge[0] as AGEquationExpr;
            Assert.True(agEquationExpr != null);
            agEquationExpr.IsSelected = true;

            agEquationExpr.GenerateSolvingTrace();
            Assert.True(agEquationExpr.AutoTrace != null);
            Assert.True(agEquationExpr.AutoTrace.Count == 1);

            var steps = agEquationExpr.AutoTrace[0].Item2 as List<TraceStepExpr>;
            Assert.NotNull(steps);
            Assert.True(steps.Count == 4);

            Reasoner.Instance.Reset();
        }

        [Test]
        public void Test_Problem_97_1()
        {
            const string fact1 = "x+x-3";
            object objOutput;
            var trace = Reasoner.Instance.RelationValidate(fact1, out objOutput) as List<Tuple<object, object>>;

            var query = objOutput as Query;
            Assert.NotNull(query);
            Assert.NotNull(trace);
            Assert.True(trace.Count == 1);
            var strategy = trace[0].Item1 as string;
            var tsLst = trace[0].Item2 as List<TraceStepExpr>;
            Assert.NotNull(strategy);
            Assert.NotNull(tsLst);
            var eqExpr = tsLst[1].TraceStep.Target as Term;
            Assert.NotNull(eqExpr);
            /* var term = eq.Rhs as Term;
             Assert.NotNull(term);
             Assert.True(term.ToString().Equals("((2*x)-3)"));*/

            Reasoner.Instance.Reset();
        }


        #endregion

        #region Problme 98

        [Test]
        public void Test_Problem_98()
        {
            const string fact1 = "1+2-3=";
            var queryExpr = Reasoner.Instance.Load(fact1) as AGQueryExpr;
            Assert.NotNull(queryExpr);
            queryExpr.RetrieveRenderKnowledge();
            Assert.True(queryExpr.RenderKnowledge.Count == 1);

            var agEquationExpr = queryExpr.RenderKnowledge[0] as AGEquationExpr;
            Assert.True(agEquationExpr != null);
            agEquationExpr.IsSelected = true;

            agEquationExpr.GenerateSolvingTrace();
            Assert.True(agEquationExpr.AutoTrace != null);
            Assert.True(agEquationExpr.AutoTrace.Count == 1);

            var steps = agEquationExpr.AutoTrace[0].Item2 as List<TraceStepExpr>;
            Assert.NotNull(steps);
            Assert.True(steps.Count == 2);

            Reasoner.Instance.Reset();
        }

        #endregion

        #region Problem 99

        [Test]
        public void Test_Problem_99()
        {
            const string fact1 = "1+2+3=";
            var queryExpr = Reasoner.Instance.Load(fact1) as AGQueryExpr;
            Assert.NotNull(queryExpr);
            queryExpr.RetrieveRenderKnowledge();
            Assert.True(queryExpr.RenderKnowledge.Count == 1);

            var agEquationExpr = queryExpr.RenderKnowledge[0] as AGEquationExpr;
            Assert.True(agEquationExpr != null);
            agEquationExpr.IsSelected = true;

            agEquationExpr.GenerateSolvingTrace();
            Assert.True(agEquationExpr.AutoTrace != null);
            Assert.True(agEquationExpr.AutoTrace.Count == 1);

            var steps = agEquationExpr.AutoTrace[0].Item2 as List<TraceStepExpr>;
            Assert.NotNull(steps);
            Assert.True(steps.Count == 2);

            Reasoner.Instance.Reset();
        }

        #endregion

        #region Problem 61

        [Test]
        public void Test_Problem_61()
        {
            const string input0 = "is(line, A)";
            const string input1 = "perpendicular(A, line)";
            const string input2 = "map(line, 2y=4x+8)"; //2y=4x+8
            const string input3 = "compose(lineA, (-2,-8))";
            const string query1 = "geometry(lineA)=";
            const string query2 = "algebra(lineA)=";
        }

        #endregion

        #region Problme 62

        [Test]
        public void Test_Problem_62()
        {
            const string input0 = "is(line, C)";
            const string input1 = "compose(C, (5,2))";
            const string input2 = "map(line, 4x+6y-12=0)"; //4x+6y-12=0
            const string input3 = "parallel(C, line)";
            const string query1 = "geometry(C)=";
            const string query2 = "algebra(C)=";
        }

        #endregion

        #region Problem 63

        [Test]
        public void Test_Problem_63()
        {
            const string input0 = "(1,-3)"; //map(point, (1,-3))
            const string input1 = "y=-x+6"; //map(line, y=-x+6)
            const string input2 = "d=";     //distance(point, line, d), d=
        }

        #endregion

        #region Problem 64

        [Test]
        public void Test_Problem_64()
        {
            const string input0 = "is(circle, A)";
            const string input1 = "compose(A, r=5)";
            const string input2 = "is(line, B)";
            const string input3 = "tangent(A,B, (-3,4))";
            const string query1 = "geometry(B)=";
            const string query2 = "algebra(B)=";
        }

        #endregion

        #region Problem 65

        [Test]
        public void Test_Problem_65()
        {
            const string input1 = "map(A, 2y-4x=10)";
            const string input2 = "is(line, B)";
            const string input3 = "perpendicular(A,B)";
            const string query1 = "geometry(C)=";
            const string query2 = "algebra(C)=";
        }

        #endregion

        #region Problem 66

        [Test]
        public void Test_Problem_66()
        {
            const string input0 = "map(line, y=2x-6)";
            const string input1 = "compose(line, m)";
            const string input2 = "is(line, D)";
            const string input3 = "compose(D, m1)";
            const string input4 = "m1=m";
            const string input5 = "compose(D, (0,5))";
            const string query1 = "geometry(D)=";
            const string query2 = "algebra(D)=";
        }

        #endregion

        #region Problem 67

        [Test]
        public void Test_Problem_67()
        {
            const string input0 = "(-2,-4)"; //map(point, (1,-3))
            const string input1 = "3y=-x+6"; //map(line, y=-x+6)
            const string input2 = "d=";     //distance(point, line, d), d=
        }

        #endregion

        #region Problem 68

        [Test]
        public void Test_Problem_68()
        {
            const string input0 = "is(circle,A)"; //optional
            const string input1 = "map(A, (x-2)^2+(y-3)^2=25)";
            const string input2 = "is(line, B)";
            const string input3 = "tangent(A,B,(5,7))";
            const string query1 = "geometry(B)=";
            const string query2 = "algebra(B)=";
        }

        #endregion
    }
}

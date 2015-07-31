using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlgebraGeometry;
using AlgebraGeometry.Expr;
using CSharpLogic;
using ExprSemantic;
using NUnit.Framework;

namespace ExprSemanticTest
{
    [TestFixture]
    public class Problems
    {
        [Test]
        public void Test_Problem_1()
        {
            /*
             * Problem 1: Find the distance betweeen A(2,3) and B(7,4)?
             */
            const string input1 = "A(2,3)";
            const string input2 = "B(7,4)";
            const string query = "d=";
        }

        [Test]
        public void Test_Problem_2()
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

        [Test]
        public void Test_Problem_3()
        {
            /*
             * There exists two points A(3,4) and B(4,v), the distance between A and B is 5. What is the value of v?
             */
            const string input1 = "A(3,4)";
            const string input2 = "B(4,v)";
            const string input3 = "d=5";
            const string query  = "v=";
        }

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

        [Test]
        public void Test_Problem_5()
        {
            /*
             * Given an equation 2y+2x-y+2x+4=0, graph this equation's corresponding shape? What is the slope of this line? 
             */
            const string input1 = "2y+2x-y+2x+4=0";
            const string input2 = "m=";
            var obj = Reasoner.Instance.Load(input1);
            var shapeExpr = obj as AGShapeExpr;
            Assert.NotNull(shapeExpr);
            var lineSymbol = shapeExpr.ShapeSymbol as LineSymbol;
            Assert.NotNull(lineSymbol);
            Assert.True(lineSymbol.ToString().Equals("4x+y+4=0"));

            obj = Reasoner.Instance.Load(input2);
            Assert.NotNull(obj);
            var agQueryExpr = obj as AGQueryExpr;
            Assert.NotNull(agQueryExpr);
            var query = agQueryExpr.QueryTag;
            Assert.NotNull(query);
            Assert.True(query.Success);
            Assert.True(query.CachedEntities.Count == 1);
            var eqGoal = query.CachedEntities.ToList()[0] as EqGoal;
            Assert.NotNull(eqGoal);
            Assert.True(eqGoal.Rhs.Equals(-4));
        }

        public void Test()
        {
            /*
             * Given an equation 2y+2x-y+2x+4=0, graph this equation's corresponding shape? What is the slope of this line? 
             */
        }
    }
}

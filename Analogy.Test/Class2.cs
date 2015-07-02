/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Analogy.Test
{
    class Class2
    {
        [Test]
        public void Problem1()
        {
            //1.Find the distance between A(2,3) and B(7,4).
            //1
            const string input0 = "A(2,3)"; // on geometry
            //After pattern matching
            const string gen_entity0 = "Point(A(3,4), \"geometry\")";
            //2
            const string input1 = "B(7,4)"; // on geometry
            //After pattern matching           
            const string gen_entity1 = "Point(B(6,8), \"geometry\")";
            /////////////////////  Option 1 ///////////////////////////////////////////
            //3
            //User draws a line passing through two points, user select Distance           
            const string gen_rel1 = "has-a(default, A)";
            const string gen_rel2 = "has-a(default, B)";
            const string gen_rel3 = "compose(A, default)";
            const string gen_rel4 = "compose(B, default)";
            const string gen_entity2 = "Length(default(A, B), \"geometry\")";
            //4
            //User draws a question mark on the line
            const string gen_query0 = "Is(default, \"algebra\")=?";
            /////////////////////  Option 2 ///////////////////////////////////////////
            const string input2 = "d=?";    // on algebra
            //After pattern matching
            const string gen_query1 = "Property(Length(default), \"value\")= ?";
            #region TODO
            try
            {
                AGKnowlegeReasoner.Instance.AddProblem(input1, input2);
            }
            catch (AGException e)
            {
                System.Console.Write(e.ToString());
            }

            Assert.Equals(AGKnowlegeReasoner.Instance.InputShapeExprs.Count, 2);
            foreach (KeyValuePair<ShapeExpr, bool> pair in AGKnowlegeReasoner.Instance.InputShapeExprs.ToDictionary())
            {
                Assert.IsInstanceOf(typeof(PointExpr), pair.Key);
            }

            AGKnowledgeAnswer answer = null;
            try
            {
                answer = AGKnowlegeReasoner.Instance.SolveProblem(query.Substring(0, query.IndexOf('=')));
            }
            catch (AGException e)
            {

            }
            Assert.IsNotNull(answer);
            Assert.IsInstanceOf(typeof(LengthExprs), answer.Result);
            var result = (LengthExprs)answer.Result;
            var length = result.AGRelation as Length;
            Assert.IsNotNull(length);
            Assert.AreEqual(length.Value, 5.0);
            Assert.AreEqual(result.LengthStringFormat, "d=5");
            Assert.Equals(answer.Solver.SolvingSummary, AGSolvingStrategy.DistTwoPoints);

            //User Input to solve the problem in the geometry canvas.
            const string userInput1 = "(3,5)";
            const string userInput2 = "(6,7)";
            AGKnowlegeReasoner.Instance.AddProblemByUser(userInput1);
            int count = AGKnowlegeReasoner.Instance.InputShapeExprs.Sum(x => x.Value);
            Assert.Equals(count, 1);
            AGKnowlegeReasoner.Instance.AddProblemByUser(userInput2);
            count = AGKnowlegeReasoner.Instance.InputShapeExprs.Sum(x => x.Value);
            Assert.Equals(count, 2);

            //Draw a line segment and write a question mark to ask for the distance
            //Label two points with A and B respectively, ask for the distance

            #endregion
        }

        [Test]
        public void Problem2()
        {
            // 2. A line passes through two points (2,0) and (0,4) respectively. 
            // 1) What is the standard form of this equation? 2) What is the slope of this line?

            //1
            const string input0 = "(2,0)"; // on geometry
            //pattern matching
            const string gen_entity0 = "Point(default1(2,0), \"geometry\")";

            //2
            const string input1 = "(0,4)"; // on geometry
            //pattern matching
            const string gen_entity1 = "Point(default2(0,4), \"geometry\")";

            //3
            //user draws a line which approximately passes through two points
            // approximatge geometry pattern matching, user distinguish between line and length            
            const string gen_rel1 = "has-a(default3, default1)";
            const string gen_rel2 = "has-a(default3, default2)";
            const string gen_rel3 = "compose(default1, default3)";
            const string gen_rel4 = "compose(default2, default3)";
            const string gen_entity2 = "Line(default3(default1,default2), \"geometry\")";

            ////////////////  Option 1  /////////////////////////////////////////////////////////

            //4
            //user draw a question mark onto the geometric Line, it means to ask its algebraic expression
            const string gen_query1 = "Is(default3, \"algebra\") = ?";

            //5
            const string input2 = "S=?";
            //pattern matching
            const string gen_query2 = "Property(Line(default3, \"algebra\"), \"slope\")=?";

            #region TODO

            /*
 *
            try
            {
                AGKnowlegeReasoner.Instance.AddProblem(input1, input2);
            }
            catch (AGException e)
            {
                System.Console.Write(e.ToString());
            }
            Assert.Equals(AGKnowlegeReasoner.Instance.InputShapeExprs.Count, 2);
            foreach (KeyValuePair<ShapeExpr, bool> pair in AGKnowlegeReasoner.Instance.InputShapeExprs.ToDictionary())
            {
                Assert.IsInstanceOf(typeof(PointExpr), pair.Key);
            }

            AGKnowledgeAnswer answer = null;
            try
            {
                answer = AGKnowlegeReasoner.Instance.SolveProblem(query1.Substring(0, query1.IndexOf('=')));
            }
            catch (AGException e)
            {
                System.Console.Write(e.ToString());
            }

            Assert.IsNotNull(answer.Result);
            Assert.IsInstanceOf(typeof(LineExpr), answer.Result);
            var le = answer.Result as LineExpr;
            var line = le.AGShape as Line;
            Assert.IsNotNull(line);
            Assert.Equals(line.LineGeneralForm, "2x+y-4=0");
            Assert.Equals(answer.Solver.SolvingSummary, AGSolvingStrategy.TwoPointsFormLine);

            try
            {
                answer = AGKnowlegeReasoner.Instance.SolveProblem(query2.Substring(0, query2.IndexOf('=')));
            }
            catch (AGException e)
            {
                System.Console.Write(e.ToString());
            }
            Assert.IsNotNull(answer.Result);
            Assert.IsInstanceOf(typeof(double), answer.Result);
#1#
            #endregion
        }

        [Test]
        public void Problem3()
        {
            //3. There is a line whose slope is 1 and whose y-intercept is -4. 
            //What is its equation in slope-intercept form?

            const string input1 = "S=1";




            const string input2 = "I=-4";
            const string query = "L(S,I)=?";
            const string query2 = "SI(query)=?";




        }

        public void Problem4()
        {
            //4. Rewrite the following equation in a general form. y = 3/2 x - 3/ 10. 

            const string input = "y = 3/2 x - 3/ 10";
            const string query = "Line(input)=?";
        }

        public void Problem5()
        {
            //5.The points (3,v) and (2,0) fall on a line with a slope of 10. What is the value of v?

            const string input1 = "(2,0)";
            const string input2 = "S=10";
            const string query = "(3,v)";
        }

        [Test]
        public void Problem6()
        {
            //6.Determine the equation of the line AB if it is given that AB is perpendicular to the line 2y = 4x + 8 and passes through (–2; –8).
            const string input1 = "2y = 4x + 8";
            const string input2 = "Line(AB)";
            const string input3 = "perpendicular(input1, AB)";
            const string input4 = "pass(AB, (-2,-8))";
            const string query = " AB = ?";
        }

        [Test]
        public void Problem7()
        {
            //7.Find an equation of the line through the point (5,2) that is parallel to the line 4x + 6y + 5 = 0.
            const string input1 = "4x + 6y + 5 = 0";
            const string input2 = "Line(default)";
            const string input3 = "pass(default, (5,2))";
            const string input4 = "parallel(default, input1)";

            const string query = "default=?";
        }

        [Test]
        public void Problem8()
        {
            //8.What is the center and radius of the circle given by 8x2 + 8y2 – 16x – 32y + 24 = 0?

            const string input1 = "8x2 + 8y2 – 16x – 32y + 24 = 0?";
            const string query1 = "center(input1)";
            const string query2 = "radius(input1)";
        }

        [Test]
        public void Problem9()
        {
            //9. The point(1,4) is on a circle whose center is at (-2,-3). Write the standard form of the equation of the circle.

            const string input1 = "Circle(default)";
            const string input2 = "On(default, (1,4))";
            const string input3 = "Center(default, (-2,-3))";
            const string query = "StandardForm(default)=?";
        }

        [Test]
        public void Problem10()
        {
            //10.A circle is centered at (0,0) and has a radius of 2 units. 
            //A line with a slope of 2 passes through point (0,1) and intersects the cirlce in two places. 
            //Where does the line intersect the circle?

            const string input1 = "Circle(default)";
            const string input2 = "Center(default, (0,0))";
            const string input3 = "Radius(default, 2.0)";

            const string input4 = "Line(default1)";
            const string input5 = "Slope(default1, 2)";
            const string input6 = "Pass(default1, (0,1))";

            const string query = "Intersect(default, default1)=?";
        }

        public void Problem11()
        {
            //22. Assume a circle C's central point is CP(-1,-1), and its radius is 3. There is a line L of which the standard form is x-y-1=0. 
            //1) What is the relation between C and L? 2) If they intersect at two points, what are the value of two intersect points coordinates? 
            //3) Find the distance between the center of circle CP and line L?

            const string input1 = "Circle(C)";
            const string input2 = "Point(CP)";
            const string input3 = "CP(-1,-1)";
            const string input4 = "Center(C, CP)";
            const string input5 = "Radius(C, 3.0)";

            const string input6 = "Line(L)";
            const string input7 = "L=x-y-1=0";

            const string query1 = "Relation(L,C)=?";

        }


        [Test]
        public void MiscTest()
        {
            //string str = "FP = ?";
            string str = "FP1 = (-3.0; 0.0)";
            Expr currExpr = starPadSDK.MathExpr.Text.Convert(str);

            var composit = currExpr as CompositeExpr;

            Expr leftExpr = composit.Args[0];
            Expr rightExpr = composit.Args[1];

            var letter = rightExpr as LetterSym;
            if (letter.Letter.Equals('?'))
            {
                Console.Read();
            }
        }
    }
}
*/

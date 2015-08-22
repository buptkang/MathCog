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
    using NUnit.Framework;
    using starPadSDK.MathExpr;

    [TestFixture]
    public class Test
    {
        public void Test1()
        {
            //x+1=1 simulation

            //var x = new Var('x');            
            //var rhs = new Term(Expression.Substract, )
        }

        [Test]
        public void Test_Line_1()
        {
            var line = new Line(3.0, 1.0, 1.0);
            line.Label = "A";
            var lineSymbol = new LineSymbol(line);
            string str = lineSymbol.ToString();

            Assert.True(str.Equals("A(3x+y+1=0)"));

            Expr expr = lineSymbol.ToExpr();



        }

        [Test]
        public void Test_Point_1()
        {
            var point = new Point(1.0, 2.0);
            var pointSymbol = new PointSymbol(point);
            string str = pointSymbol.ToString();
            Assert.True(str.Equals("(1,2)"));
            Expr expr = pointSymbol.ToExpr();

            var str1 = expr.ToString();
            Assert.True(str.Equals("(1,2)"));
        }
    }
}

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
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;

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

        [Test]
        public void Test_Generate_XML_1()
        {
            string expr1 = "1+2=3";
            Expr expr = starPadSDK.MathExpr.Text.Convert(expr1);
            var mathml = new MathML();
            XmlDocument xml = mathml.Convert(expr);
            xml.Save("a.xml");
            Console.WriteLine(xml);
        }

        [Test]
        public void Test_Generate_XML_2()
        {
            var xml = new XmlDocument();
            xml.Load("a.xml");
            Console.WriteLine(xml.InnerXml);
        }

        [Test]
        public void Test_Generate_XML_3()
        {
            // Serialize
            string txt1 = "x+1=1";
            Expr expr1 = starPadSDK.MathExpr.Text.Convert(txt1);

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream("MyFile.bin",
                                     FileMode.Create,
                                     FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, expr1);
            stream.Close();
        }

        [Test]
        public void Test_Generate_XML_4()
        {
            // Deserialize
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream("MyFile.bin",
                                      FileMode.Open,
                                      FileAccess.Read,
                                      FileShare.Read);
            Expr obj = (Expr)formatter.Deserialize(stream);
            stream.Close();
            Console.WriteLine(obj);
        }

        [Test]
        public void Test_Generate_XML_5()
        {
            // Serialize
            string txt1 = "x+1=1";
            Expr expr1 = starPadSDK.MathExpr.Text.Convert(txt1);

            string txt2 = "x=0";
            Expr expr2 = starPadSDK.MathExpr.Text.Convert(txt2);

            var lst = new List<Expr> {expr1, expr2};

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream("MyFile2.bin",
                                     FileMode.Create,
                                     FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, lst);
            stream.Close();
        }

        [Test]
        public void Test_Generate_XML_6()
        {
            // Deserialize
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream("MyFile2.bin",
                                      FileMode.Open,
                                      FileAccess.Read,
                                      FileShare.Read);
            var obj = (List<Expr>)formatter.Deserialize(stream);
            stream.Close();
            Console.WriteLine(obj.Count);
        }
    }
}
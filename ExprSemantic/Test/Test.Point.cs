using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlgebraGeometry;
using ExprSemantic;
using NUnit.Framework;
using starPadSDK.MathExpr;
using Text = starPadSDK.MathExpr.Text;

namespace ExprSemanticTest
{
    [TestFixture]
    public class Test
    {
        [Test]
        public void Test1()
        {
            //Batch Mode
            const string fact1 = "A(x,2)";
            Reasoner.Instance.Load(fact1);

            const string fact2 = "x = 2.1";
            Reasoner.Instance.Load(fact2);

            List<ShapeSymbol> shapes = Reasoner.Instance.TestGetShapeFacts();
            Assert.True(shapes.Count == 1);
            var ss = shapes[0] as PointSymbol;
            Assert.NotNull(ss);
            Assert.True(ss.ToString().Equals("A(2.1,2)")); 
        }

        public void Test1_1()
        {
            //Reasoner.Instance.KnowledgeUpdated += Instance_KnowledgeUpdated;

            //real-time mode
        }

        /*
        void Instance_KnowledgeUpdated(object sender, object args)
        {
            Assert.IsInstanceOf(typeof(PointSymbol), args);
            var ps = args as PointSymbol;
            Assert.NotNull(ps);
            Assert.True(ps.ToString().Equals("A(2.1,2)"));
        }*/

        //Test Remove Action
        public void Test1_2()
        {
            //Batch Mode
            const string fact1 = "A(x,2)";
            Reasoner.Instance.Load(fact1);

            const string fact2 = "x = 2.1";
            Reasoner.Instance.Load(fact2);

            List<ShapeSymbol> shapes = Reasoner.Instance.TestGetShapeFacts();
            Assert.True(shapes.Count == 1);
            var ss = shapes[0] as PointSymbol;
            Assert.NotNull(ss);
            Assert.True(ss.ToString().Equals("A(2.1,2)"));

            Reasoner.Instance.Unload(fact2);

            shapes = Reasoner.Instance.TestGetShapeFacts();
            Assert.True(shapes.Count == 1);
            ss = shapes[0] as PointSymbol;
            Assert.NotNull(ss);
            Assert.True(ss.ToString().Equals("A(x,2)"));

        }



        [Test]
        public void Test3()
        {
            string txt = "B(x,y)";



            string txt1 = "x = 2.0";

            string txt2 = "y = -3.0";
        }

        [Test]
        public void Test4()
        {
            string txt = "B(x,y)";

            string txt1 = "x = 2.0";

            string txt2 = "y = -3.0";

            string txt3 = "x = 4.4";
        }


        [Test]
        public void Test11()
        {
            const string fact = "(2,3)";
            const string query = "x=";
            Reasoner.Instance.Load(fact);
            Reasoner.Instance.Load(query);


            Tuple<object, Tracer> result
                = Reasoner.Instance.Answer(query);
        }
    }
}

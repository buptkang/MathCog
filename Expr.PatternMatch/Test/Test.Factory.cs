using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using ExprSemantic;
using AlgebraGeometry;

namespace ExprPatternMatchTest
{
    [TestFixture]
    public class TestPointCreation
    {
        [Test]
        public void Test_Point()
        {
            object x = 3;
            object y = -3.9;

            PointSymbol ps = ExprKnowledgeFactory.CreatePointSymbol(x, y);
            Assert.NotNull(ps);
            Assert.True(ps.SymXCoordinate.Equals("3"));
            Assert.True(ps.SymYCoordinate.Equals("-3.9"));
            Assert.True(ps.ToString().Equals("(3,-3.9)"));
            var pt = ps.Shape as Point;
            Assert.NotNull(pt);
            Assert.True(pt.Concrete);

            string label = "A";
            ps = ExprKnowledgeFactory.CreatePointSymbol(label, x, y);
            Assert.NotNull(ps);
            Assert.True(ps.SymXCoordinate.Equals("3"));
            Assert.True(ps.SymYCoordinate.Equals("-3.9"));
            Assert.True(ps.ToString().Equals("A(3,-3.9)"));
            pt = ps.Shape as Point;
            Assert.NotNull(pt);
            Assert.True(pt.Concrete);
            
            x = "X";
            y = "2";
            ps = ExprKnowledgeFactory.CreatePointSymbol(x, y);
            Assert.NotNull(ps);
            Assert.True(ps.SymXCoordinate.Equals("X"));
            Assert.True(ps.SymYCoordinate.Equals("2"));
            Assert.True(ps.ToString().Equals("(X,2)"));
            pt = ps.Shape as Point;
            Assert.NotNull(pt);
            Assert.False(pt.Concrete);
            
            var dict  = new KeyValuePair<object, object>("m", 4);
            var dict2 = new KeyValuePair<object, object>("n", 5);

            x = dict;
            y = dict2;
            ps = ExprKnowledgeFactory.CreatePointSymbol(x, y);
            Assert.NotNull(ps);
            Assert.True(ps.SymXCoordinate.Equals("4"));
            Assert.True(ps.SymYCoordinate.Equals("5"));
            pt = ps.Shape as Point;
            Assert.NotNull(pt);
            Assert.True(pt.XCoordinate.Token.Equals("m"));
            Assert.True(pt.YCoordinate.Token.Equals("n"));
            Assert.True(ps.ToString().Equals("(4,5)"));
            Assert.True(pt.Concrete); 
        }
    }
}

using System.Collections.Generic;
using ExprGenerator;
using CSharpLogic;

namespace AlgebraGeometry.Expr
{
    public class AGShapeExpr : IKnowledge
    {
        private ShapeSymbol _shapeSymbol;
        public ShapeSymbol ShapeSymbol
        {
            get { return _shapeSymbol; }
            set { _shapeSymbol = value; }
        }

        public AGShapeExpr(starPadSDK.MathExpr.Expr expr, ShapeSymbol ss)
            :base(expr)
        {
            _shapeSymbol = ss;
            GenerateTrace(ss.Shape);
        }

        public IEnumerable<IKnowledge> RetrieveGeneratedShapes()
        {
            IEnumerable<ShapeSymbol> symbols = 
               _shapeSymbol.RetrieveGeneratedShapes();
            var shapes = new List<AGShapeExpr>();
            if (symbols != null)
            {
                foreach (ShapeSymbol symbol in symbols)
                {
                    starPadSDK.MathExpr.Expr expr = ExprG.Generate(symbol);
                    var agShape = new AGShapeExpr(expr, symbol);
                    shapes.Add(agShape);
                }
                return shapes;
            }
            else
            {
                return null;
            }
        }
    }
}

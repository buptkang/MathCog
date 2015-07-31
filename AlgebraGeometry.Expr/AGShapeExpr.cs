using System.Collections.Generic;
using System.Linq;
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

        public List<IKnowledge> RetrieveRenderKnowledge()
        {
            var symbols = _shapeSymbol.RetrieveConcreteShapes();
            var shapes = new List<IKnowledge>();
            if (symbols != null)
            {
                var shapeSymbol = symbols as ShapeSymbol;
                var ssLst       = symbols as IEnumerable<ShapeSymbol>;

                if (shapeSymbol != null)
                {
                    starPadSDK.MathExpr.Expr expr = ExprG.Generate(shapeSymbol);
                    var agShape = new AGShapeExpr(expr, shapeSymbol);
                    shapes.Add(agShape);   
                }

                if (ssLst != null)
                {
                    shapes.AddRange(from symbol in ssLst 
                                    let expr = ExprG.Generate(symbol) 
                                    select new AGShapeExpr(expr, symbol));
                }
                return shapes;
            }
            return null;
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using ExprGenerator;
using CSharpLogic;

namespace AlgebraGeometry.Expr
{
    public class AGShapeExpr : AGEquationExpr
    {
        #region Properties and Regions

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
        }

        #endregion

        #region Override functions

        public override void RetrieveRenderKnowledge()
        {
            var symbols = _shapeSymbol.RetrieveConcreteShapes();
            var shapes = new List<IKnowledge>();
            RenderKnowledge = null;
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
                RenderKnowledge = shapes;
            }
        }

        public override void GenerateSolvingTrace()
        {
            if (IsSelected)
            {
                var traces = _shapeSymbol.Shape.Traces;
                if (traces.Count == 0) return;
                var lst = new List<TraceStepExpr>();
                TraceStepExpr tse;
                for (int i = 0; i < traces.Count; i++)
                {
                    var ts = traces[i];
                    tse = new TraceStepExpr(ts);
                    lst.Add(tse);
                }
                AutoTrace = lst;
                return;
            }

            if (RenderKnowledge == null) return;

            foreach (var temp in RenderKnowledge)
            {
                if (temp.IsSelected)
                {
                    temp.GenerateSolvingTrace();
                }
            }
        }

        #endregion
    }
}

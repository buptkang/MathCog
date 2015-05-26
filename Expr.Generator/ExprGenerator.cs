using AlgebraGeometry;
using CSharpLogic;
using starPadSDK.MathExpr;

namespace ExprGenerator
{
    public static class ExprG
    {
        public static Expr Generate(ShapeSymbol ss)
        {
            if (ss is PointSymbol)
            {
                var ps = ss as PointSymbol;
                return ps.ToExpr();
            }
            else
            {
                return null;
            }
        }
    }

    public static class PointExpExtension
    {
        public static Expr ToExpr(this PointSymbol ps)
        {
            var xExpr = ToCoord(ps.SymXCoordinate);
            var yExpr = ToCoord(ps.SymYCoordinate);
            if (ps.Shape.Label == null)
            {
                var comp = new CompositeExpr(new WordSym("comma"), new Expr[] { xExpr, yExpr});
                var cc = new CompositeExpr(new WordSym(""), new Expr[] { comp });
                return cc;                
            }
            else
            {
                var comp = new CompositeExpr(new WordSym(ps.Shape.Label), new Expr[] {xExpr, yExpr});
                return comp;
            }
            //return Text.Convert(form);
        }

        public static Expr ToCoord(string coord)
        {
            int number;
            bool result = LogicSharp.IsInt(coord, out number);
            if(result) return new IntegerNumber(coord);

            double dNumber;
            result = LogicSharp.IsDouble(coord, out dNumber);
            if(result) return new DoubleNumber(dNumber);

            return new WordSym(coord);            
        }
    }
}

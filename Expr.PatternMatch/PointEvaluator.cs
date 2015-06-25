using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlgebraGeometry;
using CSharpLogic;

namespace ExprSemantic
{
    public static class PointEvaluator
    {
        public static PointSymbol CreatePointSymbol(object coord1, object coord2)
        {
            object setCoord1 = null;
            object setCoord2 = null;

            if (coord1 is string)
            {
                coord1 = new Var(coord1);
            }
            else if (coord1 is KeyValuePair<object, object>)
            {
                var dict = (KeyValuePair<object, object>)coord1;
                coord1 = new Var(dict.Key);
                setCoord1 = dict.Value;
            }

            if (coord2 is string)
            {
                coord2 = new Var(coord2);
            }
            else if (coord2 is KeyValuePair<object, object>)
            {
                var dict = (KeyValuePair<object, object>)coord2;
                coord2 = new Var(dict.Key);
                setCoord2 = dict.Value;
            }

            var pt = new Point(coord1, coord2);
            if (setCoord1 != null)
            {
                pt.AddXCoord(setCoord1);
            }

            if (setCoord2 != null)
            {
                pt.AddYCoord(setCoord2);
            }
            return new PointSymbol(pt);
        }

        public static PointSymbol CreatePointSymbol(string label, object coord1, object coord2)
        {
            PointSymbol ps = CreatePointSymbol(coord1, coord2);
            if (ps != null)
            {
                ps.Shape.Label = label;
            }
            return ps;
        }    
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSharpLogic;

namespace AlgebraGeometry
{
    public static class ShapeExtension
    {
        public static bool Unify(this ShapeSymbol shapeSymbol, object constraint, out object output)
        {
            output = null;
            var line = shapeSymbol as LineSymbol;
            if (line != null)
            {
                output = line.Unify(constraint);
                return output != null;
            }
            return false;
        }
    }
}

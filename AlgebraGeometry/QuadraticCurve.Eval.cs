using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSharpLogic;

namespace AlgebraGeometry
{
    public partial class QuadraticCurve : Shape
    {
    }

    public static class QuadraticCurveEquationExtension
    {
        public static bool IsQuadraticCurveEquation(this Equation eq, out QuadraticCurveSymbol qcs)
        {
            qcs = null;
            return false;
        }
    }
}

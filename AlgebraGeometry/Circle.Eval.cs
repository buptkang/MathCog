using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSharpLogic;

namespace AlgebraGeometry
{
    public partial class Circle : QuadraticCurve
    {

    }

    public static class CircleEquationExtension
    {
        public static bool IsCircleEquation(this QuadraticCurveSymbol qcs, out CircleSymbol cs)
        {
            cs = null;
            return false;
        }
    }
}

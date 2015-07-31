using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSharpLogic;

namespace AlgebraGeometry
{
    public partial class Ellipse : QuadraticCurve
    {

    }

    public static class EllipseEquationExtension
    {
        public static bool IsEllipseEquation(this QuadraticCurveSymbol qcs, out EllipseSymbol es)
        {
            es = null;
            return false;
        }
    }
}

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

        //case "R" : case "r":
        //            return circle.Radius;
        //        case "C":  case "c":
        //            return circle.CentralPt;
        //        case "P": case "p":
        //            return circle.Perimeter;
        //        case "S": case "s":
        //            return circle.Area;                 


    /*                    case "R":
                case "r":
                    propertyExpr = circleExpr.CircleRadiusExpr;
                    tracer = circleExpr.CircleRadiusTrace;
                    return true;
                case "CP":
                case "Cp":
                case "cp":
                    propertyExpr = circleExpr.CircleCenterPtExpr;
                    tracer = circleExpr.CircleCentralPtTrace;
                    return true;

                case "PR":
                case "Pr":
                case "pr":
                    propertyExpr = circleExpr.CircleStandardFormExpr;
                    tracer = circleExpr.CircleStandardFormTrace;
                    return true;
*/
}

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

    #region Ellipse
    /*
                    case "CP":
                    case "Cp":
                    case "cp":
                        propertyExpr = ellipseExpr.EllipseCenterPtExpr;
                        tracer = ellipseExpr.EllipseCentralPtTrace;
                        return true;
                    case "A":
                    case "a":
                        propertyExpr = ellipseExpr.EllipseRadiusAExpr;
                        tracer = ellipseExpr.EllipseRadiusTrace;
                        return true;
                    case "B":
                    case "b":
                        propertyExpr = ellipseExpr.EllipseRadiusBExpr;
                        tracer = ellipseExpr.EllipseRadiusTrace;                    
                        return true;
                    case "C":
                    case "c":
                        propertyExpr = ellipseExpr.EllipseFociCExpr;
                        tracer = ellipseExpr.EllipseFociTrace;
                        return true;    
                    case "FP":
                    case "Fp":
                    case "fp":
                        propertyExpr = ellipseExpr.EllipseFociPtExpr;
                        tracer = ellipseExpr.EllipseFociPtTrace;
                        return true;
                    case "SF":
                    case "Sf":
                    case "sf":
                        propertyExpr = ellipseExpr.EllipseStandardFormExpr;
                        tracer = ellipseExpr.EllipseStandardFormTrace;
                        return true;
*/
    #endregion
}

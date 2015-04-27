using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using AGSemantic.KnowledgeBase;
using ExprSemantic.KnowledgeBase;
using ExprSemantic.KnowledgeRelation;
using starPadSDK.MathExpr;

namespace ExprSemantic.KnowledgeQueryEngine
{
    public static class AGShapePropertySelector
    {
        public static bool Transform(this IKnowledgeExpr expr, string gestureName,
            out Expr propertyExpr,
            out List<AGKnowledgeTracer> tracer)
        {
            propertyExpr = null;
            tracer = null;

            if (expr is LineExpr)
            {
                #region Line

                var lineExpr = expr as LineExpr;
                switch (gestureName)
                { 
                    #region Line
                    case "S":
                    case "s":
                    case "K":
                    case "k":
                    case "m":
                    case "M":
                        propertyExpr = lineExpr.LineSlopeExpr;
                        tracer = lineExpr.LineSlopeTrace;
                        return true;
 /*                   
                    case "I":
                    case "i":
                    case "B":
                    case "b":
                        propertyExpr = lineExpr.LineInterceptExpr;
                        tracer = lineExpr.LineInterceptTrace;
                        return true;
                    case "SI":
                    case "si":
                    case "Si":
                        propertyExpr = lineExpr.LineSlopeInterceptFormExpr;
                        tracer = lineExpr.LineSlopeInterceptTrace;
                        return true;
  */
                    case "p":
                    case "P":
                        propertyExpr = lineExpr.LinePointSlopeFormExpr;
                        tracer = lineExpr.LinePointSlopeFormTrace;
                        return true;
                    #endregion
                }

                #endregion
            }
            else if (expr is CircleExpr)
            {
                #region Circle
                var circleExpr = expr as CircleExpr;
                switch (gestureName)
                {
                    #region Circle
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
                    #endregion 
                }
                #endregion
            }
            else if (expr is EllipseExpr)
            {
                #region Ellipse

                var ellipseExpr = expr as EllipseExpr;
                switch (gestureName)
                {
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

                #endregion
            }
            else if (expr is PointPointExpr)
            {
                var distanceExpr = expr as PointPointExpr;
                switch (gestureName)
                {
                    case "V":
                        propertyExpr = distanceExpr.FakeV;
                        tracer = distanceExpr.Tracers;
                        return true;
                        break;
                    case "v":
                        propertyExpr = distanceExpr.FakeV;
                        tracer = distanceExpr.Tracers;
                        return true;
                        break;
                }
            }
            return false;
        }

        public static object Select(this Circle circle, string gestureName)
        {
            switch (gestureName)
            {
                case "R" : case "r":
                    return circle.Radius;
                case "C":  case "c":
                    return circle.CentralPt;
                case "P": case "p":
                    return circle.Perimeter;
                case "S": case "s":
                    return circle.Area;                 
            }
            return null;
        }

        public static object Select(this Ellipse ellipse, string gestureName)
        {
            switch(gestureName)
            {
                case "C" : case "c":
                    return ellipse.CentralPt;
                case "A" : case "a":
                    return ellipse.RadiusAlongXAxis;
                case "B" :  case "b":
                    return ellipse.RadiusAlongYAxis;
                case "F" :  case "f":
                    return ellipse.FociDistance;
                case "F1" : case "f1":
                    return ellipse.LeftFoci;
                case "F2" : case "f2":
                    return ellipse.RightFoci;
            }
            return null;
        }

        public static object Select(this TwoPoints twoPoints, string gestureName)
        {
            switch (gestureName)
            {
                case "L" :  case "l":
                    return twoPoints.Distance;
            }
            return null;
        }

        public static object Select(this PointLine pointLine, string gestureName)
        {
            switch (gestureName)
            {
                case "D" : case "d":
                    return pointLine.PtoLDistance;
            }
            return null;
        }

        public static object Select(this Angle angle, string gestureName)
        {
            switch (gestureName)
            {
                case "alpha": case "beta":
                    return angle.Degree;
            }
            return null;
        }

    }
}

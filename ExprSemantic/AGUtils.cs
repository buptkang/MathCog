using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using starPadSDK.MathExpr;

namespace ExprSemantic
{
    public static class AGUtils
    {
        public static double ComputeDistance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(Math.Pow(y2 - y1, 2.0) + Math.Pow(x2 - x1, 2.0));
        }

        public static Expr GenerateLineParametricExpr(string str1, string str2)
        {
            Expr root = new WordSym("ParametricLine");
            return new CompositeExpr(root , new Expr[] { Text.Convert(str1), Text.Convert(str2) }); 
        }

        public static Expr GenerateCircleParametricExpr(string str1, string str2)
        {
            Expr root = new WordSym("ParametricCircle");
            return new CompositeExpr(root, new Expr[] { Text.Convert(str1), Text.Convert(str2)});            
        }

        public static Expr GenerateEllipseParametricExpr(string str1, string str2)
        {
            Expr root = new WordSym("ParametricEllipse");
            return new CompositeExpr(root, new Expr[] { Text.Convert(str1), Text.Convert(str2) });
        }
    }

    public static class AGExpression
    {
        public const string ImplicitLineHint = "Implicit Line Form: aX + bY + c = 0";
        public const string ExplicitLineHint = "Explicit Line Form: Y = aX + b";
        public const string ParametricLineHint = "Parametric Line Form: X = x0 + a * T, Y = y0 + b * T";
        public const string QueryImplicitLineHint = "Querying the Implicit form of Line...";
        public const string QueryExplicitLineHint = "Querying the Explicit form of Line...";
        public const string QueryParametricLineHint = "Querying the Parametric form of Line...";
        public const string QueryLineSlopeHint = "Line Slope = - A / B";
        public const string QueryLineInterceptHint = "Line Intercept = - C / B";

        public const string LineImplicitRepr = "a * X + b * Y + c = 0";
        public const string LineExplicitRepr = "Y = a * X + b";
        public const string LineParametricRepr1 = "X = x0 + a * T";
        public const string LineParametricRepr2 = "Y = y0 + b * T";

        public const string QueryLineImplicitRepr1 = "LI = ?";
        public const string QueryLineImplicitRepr2 = "li = ?";
        public const string QueryLineExplicitRepr1 = "LE = ?";
        public const string QueryLineExplicitRepr2 = "le = ?";
        public const string QueryLineParametricRepr1 = "LP = ?";
        public const string QueryLineParametricRepr2 = "lp = ?";
        public const string QueryLineSlope1 = "S = ?";
        public const string QueryLineSlope2 = "s = ?";
        public const string QueryLineIntercept1 = "I = ?";
        public const string QueryLineIntercept2 = "i = ?";

        //////////////////////////////////////////////////////////////////////////////////////

        public const string ImplicitCircleHint = "Implicit Circle Form: (X-a)^2 + (Y-b)^2 = r^2";
        public const string ParametricCircleHint = "Parametric Circle Form: X = r * cos(t), Y = r * sin(t)";
        public const string QueryImplicitCircleHint = "Querying the Implicit form of Circle...";
        public const string QueryParametricCircleHint = "Querying the Parametric form of Circle...";
        public const string QueryCircleRadiusHint = "Circle Radius R";
        public const string QueryCircleCentralPointHint = "Circle Central Point P";
        public const string QueryCirclePerimeterHint = "Circle Perimeter = 2 * PI * R, where R is the radius of the circle";
        public const string QueryCircleSizeHint = "Circle Size = PI * R * R, where R is the radius of the circle";

        public const string CircleImplicitRepr = "(X-a)^2 + (Y-b)^2 = r^2";
        public const string CircleParametricRepr1 = "X = r * cos(t)";
        public const string CircleParametricRepr2 = "Y = r * sin(t)";

        public const string QueryCircleImplicitRepr1 = "CI = ?";
        public const string QueryCircleImplicitRepr2 = "ci = ?";
        public const string QueryCircleParametricRepr1 = "CP = ?";
        public const string QueryCircleParametricRepr2 = "cp = ?";
        public const string QueryCircleRadius1 = "R = ?";
        public const string QueryCircleRadius2 = "r = ?";
        public const string QueryCircleCentralPt1 = "C = ?";
        public const string QueryCircleCentralPt2 = "c = ?";
        public const string QueryCirclePerimeter1 = "P = ?";
        public const string QueryCirclePerimeter2 = "p = ?";
        public const string QueryCircleSize1 = "S = ?";
        public const string QueryCircleSize2 = "s = ?";

        //////////////////////////////////////////////////////////////////////////////////////
        public const string ImplicitEllipseHint = "Implicit Ellipse Form: (X-h)^2/a^2 + (Y-k)^2/b^2 = 1";
        public const string ParametricEllipseHint = "Parametric Ellipse Form: X = a * cos(t) + h, Y = b * sin(t) + k";
        public const string QueryImplicitEllipseHint = "Querying the Implicit form of Ellipse...";
        public const string QueryParametricEllipseHint = "Querying the Parametric form of Ellipse...";
        public const string QueryEllipseCentralPtHint = "Ellipse Center Point"; 
        public const string QueryEllipseRadiusAlongXAxisHint = "Ellipse Radius Along X Axis A";
        public const string QueryEllipseRadiusAlongYAxisHint = "Ellipse Radius Along Y Axis B";
        public const string QueryEllipseFociDistanceHint = "Foci Location is calculated through F^2 = |a^2 - b^2| ,\n" +
           "where a and b are major axis and minor axis respectively.";
        public const string QueryEllipseLeftFociHint = "Left Foci ";
        public const string QueryEllipseRightFociHint = "Right Foci ";

        public const string EllipseImplicitRepr = "(X-h)^2/a^2 + (Y-k)^2/b^2 = 1";
        public const string EllipseParametricRepr1 = "X = a * cos(t) + h";
        public const string EllipseParametricRepr2 = "Y = b * sin(t) + k";

        public const string QueryEllipseImplicitRepr1 = "EI = ?";
        public const string QueryEllipseImplicitRepr2 = "ei = ?";
        public const string QueryEllipseParametricRepr1 = "EP = ?";
        public const string QueryEllipseParametricRepr2 = "ep = ?";
        public const string QueryEllipseCentralPt1 = "C = ?";
        public const string QueryEllipseCentralPt2 = "c = ?";
        public const string QueryEllipseRadiusAlongXAxis1 = "A = ?";
        public const string QueryEllipseRadiusAlongXAxis2 = "a = ?";
        public const string QueryEllipseRadiusAlongYAxis1 = "B = ?";
        public const string QueryEllipseRadiusAlongYAxis2 = "b = ?";
        public const string QueryEllipseFociDistance1 = "F = ?";
        public const string QueryEllipseFociDistance2 = "f = ?";
        public const string QueryLeftFoci1 = "F1 = ?";
        public const string QueryLeftFoci2 = "f1 = ?";
        public const string QueryRightFoci1 = "F2 = ?";
        public const string QueryRightFoci2 = "f2 = ?";        
        //////////////////////////////////////////////////////////////////////////////////////



    }
}

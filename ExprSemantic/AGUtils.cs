using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using AGSemantic.KnowledgeBase;
using ExprSemantic.KnowledgeRelation;
using starPadSDK.MathExpr;

namespace ExprSemantic.KnowledgeBase
{
    public class AGShapeUtils
    {
        public static string RetrieveNumberValueString(double num)
        {
            if ((num % 1).Equals(0))
            {
                return Int32.Parse(num.ToString()).ToString();
            }
            else
            {
                return num.ToString();
            }
        }

        public static Expr[] ExtractTerms(Expr expr)
        {
            var composite = expr as CompositeExpr;
            Expr[] exprs = composite.Args;
            var compositeExpr = exprs[0] as CompositeExpr;
            return compositeExpr.Args;
        }

        public static string ExtractQuestionVariable(Expr expr)
        {
            var compositeExpr = expr as CompositeExpr;
            var variableExpr = compositeExpr.Args[0];

            if (variableExpr is LetterSym)
            {
                var letter = variableExpr as LetterSym;
                return letter.Letter.ToString();
            }

            if (variableExpr is WordSym)
            {
                var word = variableExpr as WordSym;
                return word.Word.ToString();
            }

            return null;
        }

        public static double GCD(double a, double b)
        {
            if (b > a)
            {
                double swap = a;
                a = b;
                b = swap;
            }

            while (!b.Equals(0.0))
            {
                double remainder = a % b;
                a = b;
                b = remainder;
            }
            return a;
        }

        #region Expression Generation

        /// <summary>
        /// Generate Shape's corresponding Expr
        /// </summary>
        /// <param name="shape"></param>
        /// <returns></returns>
        public static Expr GenerateShapeGeneralForm(Shape shape)
        {
            if (shape is Line)
            {
                var line = shape as Line;
                return starPadSDK.MathExpr.Text.Convert(line.LineGeneralForm);
            }
            else if (shape is Circle)
            {
                var circle = shape as Circle;
                return GenerateCircleGeneralForm(circle);
            }
            else if (shape is Ellipse)
            {
                var ellipse = shape as Ellipse;
                return GenerateEllipseGeneralForm(ellipse);
            }
            else if (shape is Point)
            {
                var point = shape as Point;
                return starPadSDK.MathExpr.Text.Convert(point.SymPoint);
            }
            else
            {
                return null;
            }
        }
  
        #region Circle Expr Generation

        public static Expr GenerateCircleGeneralForm(Circle circle)
        {
            string str = String.Format("(x{0})^2 + (y{1})^2 = {2}^2",
                circle.CentralPt.XCoordinate > 0
                    ? String.Format(" - {0}", circle.CentralPt.XCoordinate)
                    : String.Format(" + {0}", - circle.CentralPt.XCoordinate),
                circle.CentralPt.YCoordinate > 0
                    ? String.Format(" - {0}", circle.CentralPt.YCoordinate)
                    : String.Format(" + {0}", - circle.CentralPt.YCoordinate),
                circle.Radius);
            return starPadSDK.MathExpr.Text.Convert(str);
        }

        public static Expr GenerateCircleTrace2(Circle circle)
        {
            string str = String.Format("CP = ({0}, {1}), R = {2}", circle.CentralPt.XCoordinate,
                circle.CentralPt.YCoordinate, circle.Radius);

            return starPadSDK.MathExpr.Text.Convert(str);
        }

        #endregion

        #region Ellipse Expr Generation

        public static Expr GenerateEllipseGeneralForm(Ellipse ellipse)
        {
            string str = String.Format("(x{0})^2 / {2}^2 + (y{1})^2 / {3}^2 = 1",
                ellipse.CentralPt.XCoordinate > 0
                    ? String.Format(" - {0}", ellipse.CentralPt.XCoordinate)
                    : String.Format(" + {0}", -ellipse.CentralPt.XCoordinate),
                ellipse.CentralPt.YCoordinate > 0
                    ? String.Format(" - {0}", ellipse.CentralPt.YCoordinate)
                    : String.Format(" + {0}", -ellipse.CentralPt.YCoordinate),
                ellipse.RadiusAlongXAxis,
                ellipse.RadiusAlongYAxis);

            return starPadSDK.MathExpr.Text.Convert(str);
        }

        public static Expr GenerateEllipseTrace2(Ellipse ellipse)
        {
            string str = String.Format("CP = ({0}, {1}), a = {2}, b ={3}", ellipse.CentralPt.XCoordinate,
                ellipse.CentralPt.YCoordinate, ellipse.RadiusAlongXAxis, ellipse.RadiusAlongYAxis);

            return starPadSDK.MathExpr.Text.Convert(str);
        }

        public static Expr GenerateEllipseTrace3(Ellipse ellipse)
        {
            string str = String.Format("c^2 = {0}^2 - {1}^2",
                ellipse.RadiusAlongXAxis >= ellipse.RadiusAlongYAxis
                    ? ellipse.RadiusAlongXAxis
                    : ellipse.RadiusAlongYAxis,
                ellipse.RadiusAlongXAxis < ellipse.RadiusAlongYAxis
                    ? ellipse.RadiusAlongXAxis
                    : ellipse.RadiusAlongYAxis);

            return starPadSDK.MathExpr.Text.Convert(str);
        }

        public static Expr GenerateEllipseTrace4(Ellipse ellipse)
        {
            var l = ellipse.RadiusAlongXAxis >= ellipse.RadiusAlongYAxis
                ? ellipse.RadiusAlongXAxis
                : ellipse.RadiusAlongYAxis;
            var s = ellipse.RadiusAlongXAxis < ellipse.RadiusAlongYAxis
                ? ellipse.RadiusAlongXAxis
                : ellipse.RadiusAlongYAxis;

            var c = Math.Pow(l, 2d) - Math.Pow(s, 2d);
            string str = String.Format("c^2 = {0}", c);

            return starPadSDK.MathExpr.Text.Convert(str);
        }

        public static Expr GenerateEllipseTrace5(Ellipse ellipse)
        {
            var l = ellipse.RadiusAlongXAxis >= ellipse.RadiusAlongYAxis
                ? ellipse.RadiusAlongXAxis
                : ellipse.RadiusAlongYAxis;
            var s = ellipse.RadiusAlongXAxis < ellipse.RadiusAlongYAxis
                ? ellipse.RadiusAlongXAxis
                : ellipse.RadiusAlongYAxis;

            var c = Math.Sqrt(Math.Pow(l, 2d) - Math.Pow(s, 2d));
            string str = String.Format("c = {0}", c);

            return starPadSDK.MathExpr.Text.Convert(str);
        }

        public static Expr GenerateEllipseTrace6(Ellipse ellipse)
        {
            bool isXLong = true;
            if (ellipse.RadiusAlongXAxis < ellipse.RadiusAlongYAxis)
                isXLong = false;

            var l = ellipse.RadiusAlongXAxis >= ellipse.RadiusAlongYAxis
                ? ellipse.RadiusAlongXAxis
                : ellipse.RadiusAlongYAxis;
            var s = ellipse.RadiusAlongXAxis < ellipse.RadiusAlongYAxis
                ? ellipse.RadiusAlongXAxis
                : ellipse.RadiusAlongYAxis;

            var c = Math.Sqrt(Math.Pow(l, 2d) - Math.Pow(s, 2d));
            string str;

            if (isXLong)
            {
                //str = String.Format("FP = ({0}, {1})", ellipse.CentralPt.XCoordinate - c,
                //    ellipse.CentralPt.YCoordinate, ellipse.CentralPt.XCoordinate + c, ellipse.CentralPt.YCoordinate);
              str = String.Format("FP1 = ({0}; {1}); FP2 = ({2}; {3})", ellipse.CentralPt.XCoordinate - c,
                    ellipse.CentralPt.YCoordinate, ellipse.CentralPt.XCoordinate + c, ellipse.CentralPt.YCoordinate);
            }
            else
            {
                str = String.Format("FP1 = ({0}; {1}); FP2 = ({2}; {3})", ellipse.CentralPt.XCoordinate,
                   ellipse.CentralPt.YCoordinate - c, ellipse.CentralPt.XCoordinate, ellipse.CentralPt.YCoordinate + c);
            }

            return starPadSDK.MathExpr.Text.Convert(str);
        }

        public static Expr Generate2PointsTrace1(TwoPoints tp)
        {
            string str = String.Format("P1({0}, {1}), P2({2}, {3})", tp.P1.XCoordinate, tp.P1.YCoordinate,
                tp.P2.XCoordinate, tp.P2.YCoordinate);
            return starPadSDK.MathExpr.Text.Convert(str);
        }

        public static Expr Generate2PointsTrace2(TwoPoints tp)
        {
            string str = String.Format("d = (({0} - {1})^2 + ({2} - {3})^2)^(1/2)", tp.P2.XCoordinate, tp.P1.XCoordinate,
                tp.P2.YCoordinate, tp.P1.YCoordinate);

            return starPadSDK.MathExpr.Text.Convert(str);
        }

        public static Expr Generate2PointsTrace3(TwoPoints tp)
        {
            var d =
                Math.Sqrt(Math.Pow(tp.P2.XCoordinate - tp.P1.XCoordinate, 2d) +
                          Math.Pow(tp.P2.YCoordinate - tp.P1.YCoordinate, 2d));
            string str = String.Format("d = {0}", d);

            return starPadSDK.MathExpr.Text.Convert(str);
        }
      
        #endregion

        #region TODO
       
        public static Expr GenerateLineParametricExpr(string str1, string str2)
        {
            Expr root = new WordSym("ParametricLine");
            return new CompositeExpr(root, new Expr[] { Text.Convert(str1), Text.Convert(str2) });
        }

        public static Expr GenerateCircleParametricExpr(string str1, string str2)
        {
            Expr root = new WordSym("ParametricCircle");
            return new CompositeExpr(root, new Expr[] { Text.Convert(str1), Text.Convert(str2) });
        }

        public static Expr GenerateEllipseParametricExpr(string str1, string str2)
        {
            Expr root = new WordSym("ParametricEllipse");
            return new CompositeExpr(root, new Expr[] { Text.Convert(str1), Text.Convert(str2) });
        }

        #endregion

        #endregion
    }
}

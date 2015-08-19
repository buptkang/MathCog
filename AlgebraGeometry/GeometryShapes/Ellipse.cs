using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSharpLogic;

namespace AlgebraGeometry
{
    public partial class Ellipse : QuadraticCurve
    {
        //        public Ellipse(string label, double a, double b, double d, double e, double f)
        //            : base(label, ShapeType.Ellipse, a, b, 0.0, d, e, f)
        //        {
        //        }

        //        public Ellipse(double a, double b, double d, double e, double f)
        //            : this(null, a, b, d, e, f)
        //        {
        //            CentralPt = new Point(0.0,0.0);
        //            RadiusAlongXAxis = Math.Sqrt(-f/a);
        //            RadiusAlongYAxis = Math.Sqrt(-f/b);
        //            FociDistance = Math.Sqrt(Math.Pow(RadiusAlongXAxis, 2.0) - Math.Pow(RadiusAlongYAxis, 2.0));
        //        }

        //        public Ellipse(string label, Point center, double _radiusAlongXAxis, double _radiusAlongYAxis)
        //        {
        //            Label = label;
        //            CentralPt = center;
        //            RadiusAlongXAxis = _radiusAlongXAxis;
        //            RadiusAlongYAxis = _radiusAlongYAxis;
        //        }

        //        public Ellipse(Point center, double _radiusAlongXAxis, double _radiusAlongYAxis)
        //        {
        //            CentralPt = center;
        //            RadiusAlongXAxis = _radiusAlongXAxis;
        //            RadiusAlongYAxis = _radiusAlongYAxis;
        //        }
    }

    public class EllipseSymbol : ShapeSymbol
    {
        public EllipseSymbol(Shape _shape) : base(_shape)
        {
        }

        public override object RetrieveConcreteShapes()
        {
            throw new NotImplementedException();
        }

        public override object GetOutputType()
        {
            throw new NotImplementedException();
        }

        public override bool UnifyProperty(string label, out object obj)
        {
            throw new NotImplementedException();
        }


/*        public static Expr GenerateEllipseGeneralForm(Ellipse ellipse)
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
        }*/
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSharpLogic;

namespace AlgebraGeometry
{
    public partial class Circle : QuadraticCurve
    {
        //        public Circle(string label, double a, double b, double d, double e, double f)
        //            : base(label, ShapeType.Circle, a, b, 0.0, d, e, f)
        //        {
        //        }

        //        public Circle(double a, double b, double d, double e, double f)
        //            : this(null, a, b, d, e, f)
        //        {
        //            CentralPt = new Point(-d/2*a, -e/2*a);
        //            Radius = Math.Sqrt((Math.Pow(d, 2.0) + Math.Pow(e, 2.0) - 4*f*a)/(4*Math.Pow(a, 2.0)));
        //        }

        //        public Circle(string label, Point center, double radius)
        //        {
        //            Label = label;
        //            Radius = radius;
        //            CentralPt = center;
        //        }

        //        public Circle(Point center, double radius)
        //        {
        //            Radius    = radius;
        //            CentralPt = center;
        //        }

        #region IEquatable
        //TODO
        #endregion
    }

    public partial class CircleSymbol : ShapeSymbol
    {
        public CircleSymbol(Shape _shape)
            : base(_shape)
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

        //        #region Symbolic Format

        //        public string SymRadius
        //        {
        //            get
        //            {
        //                if ((Radius % 1).Equals(0))
        //                {
        //                    return Int32.Parse(Radius.ToString()).ToString();
        //                }
        //                else
        //                {
        //                    return Radius.ToString();
        //                }
        //            }
        //        }

        //        public string NegSymRadius
        //        {
        //            get
        //            {
        //                double rad = -Radius;
        //                if ((rad % 1).Equals(0))
        //                {
        //                    return Int32.Parse(rad.ToString()).ToString();
        //                }
        //                else
        //                {
        //                    return rad.ToString();
        //                }              
        //            }
        //        }

        //        public string RadiusTrace
        //        {
        //            get
        //            {
        //                return string.Format("r= {0}", SymRadius);
        //            }
        //        }

        //        public string CircleStandardForm
        //        {
        //            get
        //            {
        //                if (CentralPt.XCoordinate.Equals(0.0) && CentralPt.YCoordinate.Equals(0.0))
        //                {
        //                    return string.Format("x^2+y^2={0}^2", Radius > 0d ? SymRadius : NegSymRadius);
        //                }
        //                else if ((!CentralPt.XCoordinate.Equals(0.0)) && CentralPt.YCoordinate.Equals(0.0))
        //                {
        //                    return string.Format("(x{0})^2+y^2={1}^2",
        //                        CentralPt.XCoordinate > 0d ? string.Format("-{0}", CentralPt.SymXCoordinate) : string.Format("+{0}", CentralPt.NegSymXCoordinate),
        //                        Radius > 0d ? SymRadius : NegSymRadius);                    
        //                }
        //                else if (CentralPt.XCoordinate.Equals(0.0) && (!CentralPt.YCoordinate.Equals(0.0)))
        //                {
        //                    return string.Format("x^2+(y{0})^2={1}^2",
        //                        CentralPt.YCoordinate > 0d ? string.Format("-{0}", CentralPt.SymYCoordinate) : string.Format("+{0}", CentralPt.NegSymYCoordinate),
        //                        Radius > 0d ? SymRadius : NegSymRadius);                    
        //                }
        //                else
        //                {
        //                    return string.Format("(x{0})^2+(y{1})^2={2}^2",
        //                        CentralPt.XCoordinate > 0d ? string.Format("-{0}", CentralPt.SymXCoordinate) : string.Format("+{0}", CentralPt.NegSymXCoordinate),
        //                        CentralPt.YCoordinate > 0d ? string.Format("-{0}", CentralPt.SymYCoordinate) : string.Format("+{0}", CentralPt.NegSymYCoordinate),
        //                        Radius > 0d ? SymRadius : NegSymRadius);
        //                }                
        //            }
        //        }
        //        /*
        //                public string CircleGeneralForm
        //                {
        //                    get 
        //                    {
        //                        return string.Format("{0}x^2 {1}x {2}y^2 {3}y {4} = 0", SymA,
        //                              D > 0d ? string.Format("+{0}", SymD) : string.Format("-{0}", NegSymD),
        //                              B > 0d ? string.Format("+{0}", SymB) : string.Format("-{0}", NegSymB),
        //                              E > 0d ? string.Format("+{0}", SymE) : string.Format("-{0}", NegSymE),
        //                              F > 0d ? string.Format("+{0}", SymF) : string.Format("-{0}", NegSymF)); 
        //                    }
        //                }
        //         */
        //        #endregion


     /*   public static Expr GenerateCircleGeneralForm(Circle circle)
        {
            string str = String.Format("(x{0})^2 + (y{1})^2 = {2}^2",
                circle.CentralPt.XCoordinate > 0
                    ? String.Format(" - {0}", circle.CentralPt.XCoordinate)
                    : String.Format(" + {0}", -circle.CentralPt.XCoordinate),
                circle.CentralPt.YCoordinate > 0
                    ? String.Format(" - {0}", circle.CentralPt.YCoordinate)
                    : String.Format(" + {0}", -circle.CentralPt.YCoordinate),
                circle.Radius);
            return starPadSDK.MathExpr.Text.Convert(str);
        }

        public static Expr GenerateCircleTrace2(Circle circle)
        {
            string str = String.Format("CP = ({0}, {1}), R = {2}", circle.CentralPt.XCoordinate,
                circle.CentralPt.YCoordinate, circle.Radius);

            return starPadSDK.MathExpr.Text.Convert(str);
        }*/

    }

    //namespace AGSemantic.KnowledgeBase
    //{
    //    public sealed class Circle : QuadraticCurve
    //    {
    //        public Point CentralPt { get; set; }
    //        public double Radius { get; set; }
    //        public double Perimeter { get; set; }
    //        public double Area { get; set; }
    //    }

    //    public sealed class Ellipse : QuadraticCurve
    //    {
    //        public Point CentralPt { get; set; }

    //        public double RadiusAlongXAxis { get; set; }
    //        public double RadiusAlongYAxis { get; set; }
    //        public double FociDistance { get; set; }

    //        public Point LeftFoci { get; set; }
    //        public Point RightFoci { get; set; }
    //        //public double Perimeter { get; set; }
    //        //public double Area { get; set; }

    //        #region Symbolic Format

    //        public string SymRadiusA
    //        {
    //            get
    //            {
    //                if ((RadiusAlongXAxis % 1).Equals(0))
    //                {
    //                    return Int32.Parse(RadiusAlongXAxis.ToString()).ToString();
    //                }
    //                else
    //                {
    //                    return RadiusAlongXAxis.ToString();
    //                }
    //            }
    //        }

    //        public string SymRadiusB
    //        {
    //            get
    //            {
    //                if ((RadiusAlongYAxis % 1).Equals(0))
    //                {
    //                    return Int32.Parse(RadiusAlongYAxis.ToString()).ToString();
    //                }
    //                else
    //                {
    //                    return RadiusAlongYAxis.ToString();
    //                }                
    //            }
    //        }

    //        public string SymFociC
    //        {
    //            get
    //            {
    //                if ((FociDistance % 1).Equals(0))
    //                {
    //                    return Int32.Parse(FociDistance.ToString()).ToString();
    //                }
    //                else
    //                {
    //                    return FociDistance.ToString();
    //                }  
    //            }
    //        }

    //        public string EllipseStandardForm
    //        {
    //            get
    //            {
    //                return string.Format("x^2/{0}^2+y^2/{1}^2=1",SymRadiusA,SymRadiusB);
    //            }
    //        }

    //        public string SymRadiusAProperty
    //        {
    //            get
    //            {
    //                return string.Format("A = {0}", SymRadiusA);
    //            }
    //        }

    //        public string SymRadiusBProperty
    //        {
    //            get
    //            {
    //                return string.Format("B = {0}", SymRadiusB);
    //            }
    //        }

    //        public string SymFociCProperty
    //        {
    //            get { return string.Format("C = {0}", SymFociC); }
    //        }


    //        public string FociTrace1
    //        {
    //            get
    //            {
    //                return string.Format("C^2 = {0}^2 - {1}^2", SymRadiusA, SymRadiusB);
    //            }
    //        }

    //        public string FociTrace2
    //        {
    //            get
    //            {
    //                return string.Format("C^2 = {0}", Math.Pow(FociDistance, 2.0));
    //            }
    //        }

    //        public string FocalPoint1
    //        {
    //            get
    //            {
    //                return string.Format("FP1 = (-{0},0)", SymFociC);
    //            }
    //        }

    //        public string FocalPoint2
    //        {
    //            get { return string.Format("FP2 = ({0},0)", SymFociC); }
    //        }

    //        #endregion
    //    }
    //}






}

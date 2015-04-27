// /*******************************************************************************
//  * Analytical Geometry Semantic Parsing 
//  * <p>
//  * Copyright (C) 2014  Bo Kang, Hao Hu
//  * <p>
//  * This program is free software; you can redistribute it and/or modify it under
//  * the terms of the GNU General Public License as published by the Free Software
//  * Foundation; either version 2 of the License, or any later version.
//  * <p>
//  * This program is distributed in the hope that it will be useful, but WITHOUT
//  * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
//  * FOR A PARTICULAR PURPOSE. See the GNU General Public License for more
//  * details.
//  * <p>
//  * You should have received a copy of the GNU General Public License along with
//  * this program; if not, write to the Free Software Foundation, Inc., 51
//  * Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.
//  ******************************************************************************/

using System.Text;

namespace AGSemantic.KnowledgeBase
{
    using System;
   
    public abstract class Shape : IEquatable<Shape>
    {
        public string Label { get; set; }
        public ShapeType ShapeType { get; set; }
        public CoordinateSystemType Coordinate { get; set; }
        public RepresentationType Repr { get; set; }

        protected Shape(ShapeType shapeType, string label)
            : this()
        {
            this.Label = label;
            this.ShapeType = shapeType; 
        }

        protected Shape()
        {
            Coordinate = CoordinateSystemType.Cartesian;
            Repr = RepresentationType.Explicit;
        }

        #region IEquatable

        public virtual bool Equals(Shape other)
        {
            if (this.Label != null)
            {
                return this.Label.Equals(other.Label) 
                   && this.ShapeType.Equals(other.ShapeType)
                   && this.Coordinate.Equals(other.Coordinate);                
            }
            else
            {
                return this.ShapeType.Equals(other.ShapeType)
                   && this.Coordinate.Equals(other.Coordinate);
            }
        }

        public override int GetHashCode()
        {
            if (Label != null)
            {
                return Label.GetHashCode() ^ Coordinate.GetHashCode() ^ ShapeType.GetHashCode();                
            }
            else
            {
                return Coordinate.GetHashCode() ^ ShapeType.GetHashCode();
            }
        }

        #endregion
    }

    public sealed class Point : Shape
    {
        public double XCoordinate { get; set; }
        public double YCoordinate { get; set; }

        public Point(string label, double xcoordinate, double ycoordinate)
            : base(ShapeType.Point, label)
        {
            XCoordinate = Math.Round(xcoordinate,2);
            YCoordinate = Math.Round(ycoordinate,2);

        }

        public Point(double xcoordinate, double ycoordinate)
            : this(null, xcoordinate, ycoordinate)
        {
            XCoordinate = Math.Round(xcoordinate, 2);
            YCoordinate = Math.Round(ycoordinate, 2);
        }

        #region Symbolic Format

        public string SymXCoordinate
        {
            get
            {
                if ((XCoordinate % 1).Equals(0))
                {
                    return Int32.Parse(XCoordinate.ToString()).ToString();
                }
                else
                {
                    return XCoordinate.ToString();
                }                
            }
        }

        public string NegSymXCoordinate
        {
            get 
            { 
                double negXCoordinate = -XCoordinate;

                if ((negXCoordinate % 1).Equals(0))
                {
                    return Int32.Parse(negXCoordinate.ToString()).ToString();
                }
                else
                {
                    return negXCoordinate.ToString();
                }
            }
        }

        public string SymYCoordinate
        {
            get
            {
                if ((YCoordinate % 1).Equals(0))
                {
                    return Int32.Parse(YCoordinate.ToString()).ToString();
                }
                else
                {
                    return YCoordinate.ToString();
                }
            }
        }

        public string NegSymYCoordinate
        {
            get
            {
                double negYCoordinate = -YCoordinate;

                if ((negYCoordinate % 1).Equals(0))
                {
                    return Int32.Parse(negYCoordinate.ToString()).ToString();
                }
                else
                {
                    return negYCoordinate.ToString();
                }
            }
        }

        public string SymPoint
        {
            get
            {
                if (Label != null)
                {
                    return String.Format("{0} = ({1}, {2})", Label, SymXCoordinate, SymYCoordinate);
                }
                else
                {
                    return String.Format("({0},{1})", SymXCoordinate, SymYCoordinate);
                }
            }
        }

        #endregion

        #region IEqutable

        public override bool Equals(Shape other)
        {
            if (other is Point)
            {
                var pt = other as Point;
                if (!(XCoordinate.Equals(pt.XCoordinate) && YCoordinate.Equals(pt.YCoordinate)))
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            return base.Equals(other);
        }

        public override int GetHashCode()
        {
            return XCoordinate.GetHashCode() ^ YCoordinate.GetHashCode();
        }

        #endregion
    }

    public sealed class Line : Shape
    {
        public double A { get; set; }
        public double B { get; set; }
        public double C { get; set; }

        public double Slope { get; set; }
        public double Intercept { get; set; }

        public Point YIntercept { get; set; }
        public Point XIntercept { get; set; }

        public Line(string label, double a, double b, double c)
            : base(ShapeType.Line, label)
        {
            A = Math.Round(a,2);
            B = Math.Round(b,2);
            C = Math.Round(c,2);

            Slope = -A / B;
            Intercept = -C / B;
            YIntercept = new Point("Y-Intercept", 0.0, Intercept);
            XIntercept = new Point("X-Intercept", -C / A, 0.0);
        }

        public Line(double a, double b, double c)           
        {
            A = Math.Round(a, 2);
            B = Math.Round(b, 2);
            C = Math.Round(c, 2);

            Slope = -A / B;
            Intercept = -C / B;
            YIntercept = new Point("Y-Intercept", 0.0, Intercept);
            XIntercept = new Point("X-Intercept", -C / A, 0.0);
        }

        public Line(string label, Point p1, Point p2)
        {
            Label = label;
            Slope = (p1.YCoordinate - p2.YCoordinate)/(p1.XCoordinate - p2.YCoordinate);
            Intercept = p1.YCoordinate - Slope*p1.XCoordinate;
            YIntercept = new Point("Y-Intercept", 0.0, Intercept);

            A = Slope;
            B = -1;
            C = Intercept;
        }

        public Line(Point p1, Point p2)
        {
            ShapeType = ShapeType.Line;
        }

        #region Symbolic Format

        public string SymA
        {
            get
            {
                if ((A % 1).Equals(0))
                {
                    return Int32.Parse(A.ToString()).ToString();
                }
                else
                {
                    return  Math.Round(A,2).ToString();
                }
            }
        }

        public string SymB
        {
            get
            {
                if ((B % 1).Equals(0))
                {
                    return Int32.Parse(B.ToString()).ToString();
                }
                else
                {
                    return B.ToString();
                }
            }
        }

        private string NegSymB
        {
            get 
            { 
                double negB = -B;

                if ((negB % 1).Equals(0))
                {
                    return Int32.Parse(negB.ToString()).ToString();
                }
                else
                {
                    return negB.ToString();
                }
            }
        }

        public string SymC
        {
            get
            {
                if (C.Equals(0d))
                {
                    return "0";
                }

                if ((C % 1).Equals(0))
                {
                    return Int32.Parse(C.ToString()).ToString();
                }
                else
                {
                    return C.ToString();
                }
            }
        }

        private string NegSymC
        {
            get
            {
                if (C.Equals(0d))
                {
                    return null;
                }

                double negC = -C;

                if ((negC % 1).Equals(0))
                {
                    return Int32.Parse(negC.ToString()).ToString();
                }
                else
                {
                    return negC.ToString();
                }
            }
        }

        public string SymSlope
        {
            get
            {
                if ((Slope % 1).Equals(0))
                {
                    return Int32.Parse(Slope.ToString()).ToString();
                }
                else
                {
                    return Slope.ToString();
                }
            }
        }

        public string SymIntercept
        {
            get
            {
                if ((Intercept % 1).Equals(0))
                {
                    return Int32.Parse(Intercept.ToString()).ToString();
                }
                else
                {
                    return Intercept.ToString();
                }
            } 
        }

        private string NegSymIntercept
        {
            get
            {
                double negIntercept = -Intercept;
                if ((negIntercept % 1).Equals(0))
                {
                    return Int32.Parse(negIntercept.ToString()).ToString();
                }
                else
                {
                    return negIntercept.ToString();
                }
            }
        }

        public string LineGeneralForm
        {
            get
            {
                if (C.Equals(0d))
                {
                    return string.Format("{0}x {1}y = 0", SymA,
                        B > 0d ? String.Format("+ {0}", SymB) : String.Format("- {0}", NegSymB));                    
                }
                else
                {
                    return String.Format("{0}x {1}y {2} = 0", SymA,
                        B > 0d ? String.Format("+ {0}", SymB) : String.Format("- {0}", NegSymB),
                        C > 0d ? String.Format("+ {0}", SymC) : String.Format("- {0}", NegSymC));   
                }
            }
        }

        /// <summary>
        /// the format is "A = 3" 
        /// </summary>
        public string SymAProperty
        {
            get { return string.Format("A＝{0}", SymA); }
        }

        /// <summary>
        /// the format is "B = 3" 
        /// </summary>
        public string SymBProperty
        {
            get { return string.Format("B＝{0}", SymB); }
        }

        /// <summary>
        /// the format is "C = 3" 
        /// </summary>
        public string SymCProperty
        {
            get { return string.Format("C＝{0}", SymC); }
        }

        public string SlopeTrace1
        {
            get
            {
                if (A * B > 0 )
                {
                    return String.Format("m = - {0} / {1}", SymA, SymB);
                }
                else
                {
                    return String.Format("m = {0} / {1}", SymA, NegSymB);                        
                }
            }
        }

        public string SlopeTrace2
        {
            get
            {
                return String.Format("m = {0}", SymSlope);
            }
        }

        public string InterceptTrace1
        {
            get
            {
                if (B > 0 && C > 0)
                {
                    return String.Format("I = - {0} / {1}", SymC, SymB);
                }
                else if (C > 0 && B < 0)
                {
                    return String.Format("I = {0} / {1}", SymC, NegSymB);
                }
                else if (C < 0 && B > 0)
                {
                    return String.Format("I = {0} / {1}", NegSymC, SymB);
                }

                return null;               
            }
        }

        public string InterceptTrace2
        {
            get
            {
                return String.Format("I = {0}", SymIntercept);
            }
        }

        public string LineSlopeInterceptForm
        {
            get
            {
                if (Intercept.Equals(0d))
                {
                    return string.Format("y={0}x", SymSlope);
                }
                else
                {
                    return string.Format("y={0}x{1}", SymSlope,
                        Intercept > 0d ? String.Format("+ {0}", SymIntercept) : String.Format("- {0}", NegSymIntercept));
                }
            }
        }

        public string LinePointSlopeForm
        {
            get
            {
                if (Intercept.Equals(0d))
                {
                    return string.Format("y={0}x", SymSlope);
                }
                else
                {
                    return string.Format("y{0}={1}（x{2}）",
                        YIntercept.YCoordinate > 0d ? String.Format("- {0}", YIntercept.YCoordinate) : String.Format("+ {0}", YIntercept.NegSymYCoordinate), 
                        SymSlope,
                        YIntercept.XCoordinate > 0d ? String.Format("- {0}", YIntercept.XCoordinate) : String.Format("+ {0}", YIntercept.NegSymXCoordinate));
                }               
            }
        }

        #endregion

        #region IEquatable

        public override bool Equals(Shape other)
        {
            if (other is Line)
            {
                var line = other as Line;
                if (!(A.Equals(line.A) && B.Equals(line.B) && C.Equals(line.C)))
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            return base.Equals(other);
        }

        public override int GetHashCode()
        {
            return A.GetHashCode() ^ B.GetHashCode() ^ C.GetHashCode();
        }

        #endregion
    }

    public abstract class QuadraticCurve : Shape
    {
        public double A { get; set; } // coefficient for the X^2
        public double B { get; set; } // coefficient for the Y^2
        public double C { get; set; } // coefficient for the XY
        public double D { get; set; } // coefficient for the X
        public double E { get; set; } // coefficient for the Y
        public double F { get; set; } // coefficient for the constant

        public QuadraticCurve() { }

        public QuadraticCurve(string label, ShapeType shapeType, double a, double b, double c, double d, double e, double f)
            : base(shapeType, label)
        {
            A = Math.Round(a,2);
            B = Math.Round(b,2);
            C = Math.Round(c,2);
            D = Math.Round(d,2);
            E = Math.Round(e,2);
            F = Math.Round(f,2);
        }

        public QuadraticCurve(ShapeType shapeType, double a, double b, double c, double d, double e, double f)
            : this(null, shapeType, a, b, c, d, e, f)
        {
        }

        #region Symbolic Format

        public string SymA
        {
            get
            {
                if ((A % 1).Equals(0))
                {
                    return Int32.Parse(A.ToString()).ToString();
                }
                else
                {
                    return A.ToString();
                }
            }
        }

        public string SymB
        {
            get
            {
                if ((B % 1).Equals(0))
                {
                    return Int32.Parse(B.ToString()).ToString();
                }
                else
                {
                    return B.ToString();
                }
            }
        }

        public string NegSymB
        {
            get
            {
                double negB = -B;
                if ((negB % 1).Equals(0))
                {
                    return Int32.Parse(negB.ToString()).ToString();
                }
                else
                {
                    return negB.ToString();
                }
            }
        }

        public string SymC
        {
            get
            {
                if ((C % 1).Equals(0))
                {
                    return Int32.Parse(C.ToString()).ToString();
                }
                else
                {
                    return C.ToString();
                }
            }
        }

        public string SymD
        {
            get
            {
                if ((D % 1).Equals(0))
                {
                    return Int32.Parse(D.ToString()).ToString();
                }
                else
                {
                    return D.ToString();
                }
            }
        }

        public string NegSymD
        {
            get
            {
                double negD = -D;
                if ((negD % 1).Equals(0))
                {
                    return Int32.Parse(negD.ToString()).ToString();
                }
                else
                {
                    return negD.ToString();
                }                
            }
        }

        public string SymE
        {
            get
            {
                if ((E % 1).Equals(0))
                {
                    return Int32.Parse(E.ToString()).ToString();
                }
                else
                {
                    return E.ToString();
                }
            }
        }

        public string NegSymE
        {
            get
            {
                double negE = -E;
                if ((negE % 1).Equals(0))
                {
                    return Int32.Parse(negE.ToString()).ToString();
                }
                else
                {
                    return negE.ToString();
                }
            }            
        }

        public string SymF
        {
            get
            {
                if ((F % 1).Equals(0))
                {
                    return Int32.Parse(F.ToString()).ToString();
                }
                else
                {
                    return F.ToString();
                }
            }
        }

        public string NegSymF
        {
            get 
            { 
                double negF = -F;
                if ((negF % 1).Equals(0))
                {
                    return Int32.Parse(negF.ToString()).ToString();
                }
                else
                {
                    return negF.ToString();
                }
            }
        }

        #endregion

        #region IEquatable

        public override bool Equals(Shape other)
        {
            if (other is QuadraticCurve)
            {
                var qc = other as QuadraticCurve;
                if (!(A.Equals(qc.A) && B.Equals(qc.B) && C.Equals(qc.C) && D.Equals(qc.D) && E.Equals(qc.E) && F.Equals(qc.F)))
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            return base.Equals(other);
        }

        public override int GetHashCode()
        {
            return A.GetHashCode() ^ B.GetHashCode() ^ C.GetHashCode();
        }

        #endregion
    }

    public sealed class Circle : QuadraticCurve
    {
        public Point CentralPt { get; set; }
        public double Radius { get; set; }
        public double Perimeter { get; set; }
        public double Area { get; set; }

        public Circle(string label, double a, double b, double d, double e, double f)
            : base(label, ShapeType.Circle, a, b, 0.0, d, e, f)
        {
        }

        public Circle(double a, double b, double d, double e, double f)
            : this(null, a, b, d, e, f)
        {
            CentralPt = new Point(-d/2*a, -e/2*a);
            Radius = Math.Sqrt((Math.Pow(d, 2.0) + Math.Pow(e, 2.0) - 4*f*a)/(4*Math.Pow(a, 2.0)));
        }

        public Circle(string label, Point center, double radius)
        {
            Label = label;
            Radius = radius;
            CentralPt = center;
        }

        public Circle(Point center, double radius)
        {
            Radius    = radius;
            CentralPt = center;
        }

        #region Symbolic Format

        public string SymRadius
        {
            get
            {
                if ((Radius % 1).Equals(0))
                {
                    return Int32.Parse(Radius.ToString()).ToString();
                }
                else
                {
                    return Radius.ToString();
                }
            }
        }

        public string NegSymRadius
        {
            get
            {
                double rad = -Radius;
                if ((rad % 1).Equals(0))
                {
                    return Int32.Parse(rad.ToString()).ToString();
                }
                else
                {
                    return rad.ToString();
                }              
            }
        }

        public string RadiusTrace
        {
            get
            {
                return string.Format("r= {0}", SymRadius);
            }
        }

        public string CircleStandardForm
        {
            get
            {
                if (CentralPt.XCoordinate.Equals(0.0) && CentralPt.YCoordinate.Equals(0.0))
                {
                    return string.Format("x^2+y^2={0}^2", Radius > 0d ? SymRadius : NegSymRadius);
                }
                else if ((!CentralPt.XCoordinate.Equals(0.0)) && CentralPt.YCoordinate.Equals(0.0))
                {
                    return string.Format("(x{0})^2+y^2={1}^2",
                        CentralPt.XCoordinate > 0d ? string.Format("-{0}", CentralPt.SymXCoordinate) : string.Format("+{0}", CentralPt.NegSymXCoordinate),
                        Radius > 0d ? SymRadius : NegSymRadius);                    
                }
                else if (CentralPt.XCoordinate.Equals(0.0) && (!CentralPt.YCoordinate.Equals(0.0)))
                {
                    return string.Format("x^2+(y{0})^2={1}^2",
                        CentralPt.YCoordinate > 0d ? string.Format("-{0}", CentralPt.SymYCoordinate) : string.Format("+{0}", CentralPt.NegSymYCoordinate),
                        Radius > 0d ? SymRadius : NegSymRadius);                    
                }
                else
                {
                    return string.Format("(x{0})^2+(y{1})^2={2}^2",
                        CentralPt.XCoordinate > 0d ? string.Format("-{0}", CentralPt.SymXCoordinate) : string.Format("+{0}", CentralPt.NegSymXCoordinate),
                        CentralPt.YCoordinate > 0d ? string.Format("-{0}", CentralPt.SymYCoordinate) : string.Format("+{0}", CentralPt.NegSymYCoordinate),
                        Radius > 0d ? SymRadius : NegSymRadius);
                }                
            }
        }
        /*
                public string CircleGeneralForm
                {
                    get 
                    {
                        return string.Format("{0}x^2 {1}x {2}y^2 {3}y {4} = 0", SymA,
                              D > 0d ? string.Format("+{0}", SymD) : string.Format("-{0}", NegSymD),
                              B > 0d ? string.Format("+{0}", SymB) : string.Format("-{0}", NegSymB),
                              E > 0d ? string.Format("+{0}", SymE) : string.Format("-{0}", NegSymE),
                              F > 0d ? string.Format("+{0}", SymF) : string.Format("-{0}", NegSymF)); 
                    }
                }
         */
        #endregion

        #region IEquatable

        //TODO

        #endregion
    }

    public sealed class Ellipse : QuadraticCurve
    {
        public Point CentralPt { get; set; }

        public double RadiusAlongXAxis { get; set; }
        public double RadiusAlongYAxis { get; set; }
        public double FociDistance { get; set; }

        public Point LeftFoci { get; set; }
        public Point RightFoci { get; set; }
        //public double Perimeter { get; set; }
        //public double Area { get; set; }

        public Ellipse(string label, double a, double b, double d, double e, double f)
            : base(label, ShapeType.Ellipse, a, b, 0.0, d, e, f)
        {
        }

        public Ellipse(double a, double b, double d, double e, double f)
            : this(null, a, b, d, e, f)
        {
            CentralPt = new Point(0.0,0.0);
            RadiusAlongXAxis = Math.Sqrt(-f/a);
            RadiusAlongYAxis = Math.Sqrt(-f/b);
            FociDistance = Math.Sqrt(Math.Pow(RadiusAlongXAxis, 2.0) - Math.Pow(RadiusAlongYAxis, 2.0));
        }

        public Ellipse(string label, Point center, double _radiusAlongXAxis, double _radiusAlongYAxis)
        {
            Label = label;
            CentralPt = center;
            RadiusAlongXAxis = _radiusAlongXAxis;
            RadiusAlongYAxis = _radiusAlongYAxis;
        }

        public Ellipse(Point center, double _radiusAlongXAxis, double _radiusAlongYAxis)
        {
            CentralPt = center;
            RadiusAlongXAxis = _radiusAlongXAxis;
            RadiusAlongYAxis = _radiusAlongYAxis;
        }

        #region Symbolic Format

        public string SymRadiusA
        {
            get
            {
                if ((RadiusAlongXAxis % 1).Equals(0))
                {
                    return Int32.Parse(RadiusAlongXAxis.ToString()).ToString();
                }
                else
                {
                    return RadiusAlongXAxis.ToString();
                }
            }
        }

        public string SymRadiusB
        {
            get
            {
                if ((RadiusAlongYAxis % 1).Equals(0))
                {
                    return Int32.Parse(RadiusAlongYAxis.ToString()).ToString();
                }
                else
                {
                    return RadiusAlongYAxis.ToString();
                }                
            }
        }

        public string SymFociC
        {
            get
            {
                if ((FociDistance % 1).Equals(0))
                {
                    return Int32.Parse(FociDistance.ToString()).ToString();
                }
                else
                {
                    return FociDistance.ToString();
                }  
            }
        }

        public string EllipseStandardForm
        {
            get
            {
                return string.Format("x^2/{0}^2+y^2/{1}^2=1",SymRadiusA,SymRadiusB);
            }
        }

        public string SymRadiusAProperty
        {
            get
            {
                return string.Format("A = {0}", SymRadiusA);
            }
        }

        public string SymRadiusBProperty
        {
            get
            {
                return string.Format("B = {0}", SymRadiusB);
            }
        }

        public string SymFociCProperty
        {
            get { return string.Format("C = {0}", SymFociC); }
        }


        public string FociTrace1
        {
            get
            {
                return string.Format("C^2 = {0}^2 - {1}^2", SymRadiusA, SymRadiusB);
            }
        }

        public string FociTrace2
        {
            get
            {
                return string.Format("C^2 = {0}", Math.Pow(FociDistance, 2.0));
            }
        }

        public string FocalPoint1
        {
            get
            {
                return string.Format("FP1 = (-{0},0)", SymFociC);
            }
        }

        public string FocalPoint2
        {
            get { return string.Format("FP2 = ({0},0)", SymFociC); }
        }

        #endregion
    }

    public sealed class Parabola : QuadraticCurve
    {
        #region Properties

        public Point Vertex { get; set; }
        public double FocalParameterP { get; set; }
        public double DistanceFromVertexToFocusA { get; set; }

        #endregion

        #region constructor

        public Parabola(string label, double a, double b, double d, double e, double f)
            : base(label, ShapeType.Parabola, a, b, 0.0, d, e, f)
        {
        }

        public Parabola(double a, double b , double d, double e, double f)
            : this(null, a, b, d, e, f)
        {
        }

        #endregion
    }

    public sealed class Hyperbola : QuadraticCurve
    {
        #region constructor

        public Hyperbola(string label, double a, double b, double f)
            : base(label, ShapeType.Hyperbola, a, b, 0.0, 0.0, 0.0, f)
        {
        }

        public Hyperbola(double a, double b, double d, double e, double f)
            : this(null, a, b, f)
        {
        }

        #endregion
    }
}






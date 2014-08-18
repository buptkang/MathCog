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

namespace ExprSemantic.KnowledgeBase
{
    using System;

    public abstract class Shape : IEquatable<Shape>
    {
        #region constructor

        protected Shape(ShapeType shapeType, string label)
            : this()
        {
            this.Label = label;
            this.ShapeType = shapeType;
        }

        protected Shape()
        {
            Coordinate = CoordinateSystemType.Cartesian;
        }

        #endregion

        #region IEquatable

        public virtual bool Equals(Shape other)
        {
            return this.Label.Equals(other.Label)
                   && this.ShapeType.Equals(other.ShapeType)
                   && this.Coordinate.Equals(other.Coordinate);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Shape);
        }

        public override int GetHashCode()
        {
            return Label.GetHashCode() ^ Coordinate.GetHashCode() ^ ShapeType.GetHashCode();
        }

        #endregion

        #region Properties

        public string Label { get; set; }
        public ShapeType ShapeType { get; set; }
        public CoordinateSystemType Coordinate { get; set; }

        #endregion
    }

    public sealed class Point : Shape
    {
        #region Constructors

        public Point(string label, double xcoordinate, double ycoordinate)
            : base(ShapeType.Point, label)
        {
            XCoordinate = xcoordinate;
            YCoordinate = ycoordinate;
        }

        public Point(double xcoordinate, double ycoordinate)
            : this(null, xcoordinate, ycoordinate)
        {
        }

        #endregion

        #region Properties

        public double XCoordinate { get; set; }
        public double YCoordinate { get; set; }

        #endregion
    }

    public sealed class Line : Shape
    {
        #region Constructors 

        public Line(string label, double a, double b, double c)
            : base(ShapeType.Line, label)
        {
            A = a;
            B = b;
            C = c;
        }

        public Line(double a, double b, double c)
            : this(null, a, b, c)
        {
            A = a;
            B = b;
            C = c;
        }

        #endregion

        #region Properties

        public double A { get; set; }
        public double B { get; set; }
        public double C { get; set; }

        public double Slope { get; set; }
        public double Intercept { get; set; }

        public string ImplicitRepr
        {
            get { return string.Format("{0}X+{1}Y+{2}=0", A, B, C); }
        }

        public string ExplicitRepr
        {
            get { return string.Format("Y = {0}X + {1}", Slope, Intercept); }
        }

        #endregion
    }

    public abstract class QuadraticCurve : Shape
    {
        #region Constructors 

        public QuadraticCurve(string label, ShapeType shapeType, double a, double b, double c, double d, double e, double f)
            : base(shapeType, label)
        {
            A = a;
            B = b;
            C = c;
            D = d;
            E = e;
            F = f;
        }

        public QuadraticCurve(ShapeType shapeType, double a, double b, double c, double d, double e, double f)
            : this(null, shapeType, a, b, c, d, e, f)
        {
        }

        #endregion

        #region Properties

        public double A { get; set; } // coefficient for the X^2
        public double B { get; set; } // coefficient for the Y^2
        public double C { get; set; } // coefficient for the XY
        public double D { get; set; } // coefficient for the X
        public double E { get; set; } // coefficient for the Y
        public double F { get; set; } // coefficient for the constant

        #endregion
    }

    public sealed class Circle : QuadraticCurve
    {
        #region constructor

        public Circle(string label, double a, double b, double d, double e, double f)
            : base(label, ShapeType.Circle, a, b, 0.0, d, e, f)
        {
        }

        public Circle(double a, double b, double d, double e, double f)
            : this(null, a, b, d, e, f)
        {
        }

        #endregion

        #region Properties

        public Point CentralPt { get; set; }
        public double Radius { get; set; }
        public double Perimeter { get; set; }
        public double Area { get; set; }

        public string ImplcitRepr
        {
            get { return string.Format("{0}X^2+{1}Y^2+{2}X+{3}Y+{4}=0", A, B, D, E, F); }
        }

        public string CenterRadiusRepr
        {
            get
            {
                return string.Format("(X+{0})^2+(Y+{1})^2= {2}^2", CentralPt.XCoordinate, CentralPt.YCoordinate, Radius);
            }
        }

        #endregion
    }

    public sealed class Ellipse : QuadraticCurve
    {
        #region constructor

        public Ellipse(string label, double a, double b, double d, double e, double f)
            : base(label, ShapeType.Ellipse, a, b, 0.0, d, e, f)
        {
        }

        public Ellipse(double a, double b, double d, double e, double f)
            : this(null, a, b, d, e, f)
        {
        }

        #endregion

        #region Properties

        public Point CentralPt { get; set; }

        public double RadiusAlongXAxis { get; set; }
        public double RadiusAlongYAxis { get; set; }
        public double SemiMajorAxisDistanceA { get; set; }
        public double SemiMinorAxisDistanceB { get; set; }
        public double FociDistance { get; set; }

        public Point LeftFoci { get; set; }
        public Point RightFoci { get; set; }
        public double Perimeter { get; set; }
        public double Area { get; set; }

        #endregion
    }

    public sealed class Parabola : QuadraticCurve
    {
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

        #region Properties

        public Point Vertex { get; set; }
        public double FocalParameterP { get; set; }
        public double DistanceFromVertexToFocusA { get; set; }

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






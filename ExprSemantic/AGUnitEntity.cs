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

namespace ExprSemantic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;
    using System.Runtime.Remoting;
    using System.Security;
    using System.Text;
    using System.Xml.XPath;
    using starPadSDK.MathExpr;
    using System.Dynamic;
    using Microsoft.FSharp.Text.Parsing;

    public interface IAxiom 
    {
        void ComputeAttributes();
        void ExlainAttributes();
        string Parse(string repr);
    }

    public abstract class Axiom : IAxiom, IEquatable<Axiom>
    {
        #region Constructors

        public Axiom(Enum entityType, RepresentationType reprType,  string label)
            : this(entityType, reprType)
        {
            Label = label;
        }

        public Axiom(Enum entityType, RepresentationType reprType) : this()
        {
            EntityType = entityType;
            ReprType   = reprType;
        }

        public Axiom(Enum entityType) : this()
        {
            EntityType = entityType;
        }

        public Axiom()
        {
            CoordinateType = CoordinateSystemType.Cartesian; //TODO
            HintDict = new Dictionary<Tuple<Expr, Expr>, string>();
        }

        #endregion

        #region Methods

        public virtual void ComputeAttributes() { }
        public virtual void ExlainAttributes() { }
        public abstract string Parse(string repr);

        #endregion

        #region Properties

        public string Label { get; set; }
        public Enum EntityType { get; set; } // AxiomEnum, TwoRelationEnum, TripleRelationEnum
        public RepresentationType ReprType { get; set; }
        public CoordinateSystemType CoordinateType { get; set; }
        public Expr AST { get; set; }

        /// Key.Tuple.item1 represents the original object (string or Tuple(string, string)) 
        /// Key.Tuple.items2 represents the target object  (string or Tuple(string, string))
        /// Value represents the explanation string
        public IDictionary<Tuple<Expr, Expr>, string> HintDict;

        #endregion

        #region IEquatable

        public virtual bool Equals(Axiom other)
        {
            return this.EntityType.Equals(other.EntityType) && this.Label.Equals(other.Label);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Axiom);
        }

        public override int GetHashCode()
        {
            return EntityType.GetHashCode() ^ Label.GetHashCode();
        }

        #endregion
    }

    public sealed partial class Point : Axiom
    {
        #region Constructors

        public Point(string label, double xcoordinate, double ycoordinate)
            : base(AxiomEnum.Point, RepresentationType.Explicit, label)
        {
            XCoordinate = xcoordinate;
            YCoordinate = ycoordinate;
        }

        public Point(double xcoordinate, double ycoordinate)
            : this(null, xcoordinate, ycoordinate)
        {
        }

        #endregion

        public override string Parse(string repr)
        {
            Expr expr;
            try
            {
                expr = Text.Convert(repr);
            }
            catch (TextParseException)
            {
                throw new AGTextParseException(repr, this);
            }

            //expr.MatchForm(Point.)

            //C(4.1,2.0)
            //X = 6.0
            //AT
            //y = 3.0
            //X = ?
            return null;
        }

        #region Properties
        public double XCoordinate { get; set; }
        public double YCoordinate { get; set; }
        #endregion

    }

    public sealed partial class Line : Axiom
    {
        public Line(string label, double slope, double intercept)
            : base(AxiomEnum.Line, RepresentationType.Explicit, label)
        {
            Slope = slope;
            Intercept = intercept;
        }

        public Line(double slope, double intercept)
            : this(null, slope, intercept)
        {
        }

        public Line(string label, double a, double b, double c)
            : base(AxiomEnum.Line, RepresentationType.Implicit, label)
        {
            A = a;
            B = b;
            C = c;
           
        }

        public override void ExlainAttributes()
        {           
            HintDict.Add(Tuple.Create(LineImplicitExpr, LineExplicitExpr), AGExpression.ExplicitLineHint);
            HintDict.Add(Tuple.Create(LineExplicitExpr, LineImplicitExpr), AGExpression.ImplicitLineHint);
            HintDict.Add(Tuple.Create(LineImplicitExpr, LineParametricExpr), AGExpression.ParametricLineHint);
            HintDict.Add(Tuple.Create(LineParametricExpr,LineImplicitExpr), AGExpression.ImplicitLineHint);

            foreach(Expr expr in QueryLineExplicitExprs)
                HintDict.Add(Tuple.Create(LineExplicitExpr, expr), AGExpression.QueryExplicitLineHint);
            foreach(Expr expr in QueryLineImplcitExprs)
                HintDict.Add(Tuple.Create(LineImplicitExpr, expr), AGExpression.QueryImplicitLineHint);
            foreach(Expr expr in QueryLineParametricExprs)
                HintDict.Add(Tuple.Create(LineParametricExpr, expr), AGExpression.QueryParametricLineHint);

            foreach(Expr expr in QueryLineSlopeExprs)
                HintDict.Add(Tuple.Create(LineExplicitExpr, expr), AGExpression.QueryLineSlopeHint);
            foreach(Expr expr in QueryLineInterceptExprs)
                HintDict.Add(Tuple.Create(LineExplicitExpr, expr), AGExpression.QueryLineInterceptHint);
        }

        public override void ComputeAttributes()
        {
            if (ReprType == RepresentationType.Explicit)
            {
                // Calculate the implicit form
                A = Slope; B = -1; C = Intercept;
                //Calculate the parametric form
                //TODO
            }
            else if (ReprType == RepresentationType.Implicit)
            {
                // Calculate the explicit form
                Slope = -A / B;
                Intercept = -C / B;
                //Calculate the parametric form
                //TODO
            }
        }

        //Implicit Line Form
        public double A { get; set; }
        public double B { get; set; }
        public double C { get; set; }

        //Explicit Line Form
        public double Slope { get; set; }
        public double Intercept { get; set; }

        //Parametric Line Form
        //TODO
    }

    public sealed partial class Circle : Axiom
    {
        public Circle(string label, Point centralPt, double radius)
            : base(AxiomEnum.Circle, RepresentationType.Implicit, label)
        {
            CentralPt = centralPt;
            Radius = radius;
           
        }

        public Circle(Point centralPt, double radius)
            : this(null, centralPt, radius)
        {
        }

        public override void ExlainAttributes()
        {
            HintDict.Add(Tuple.Create(CircleImplicitExpr, CircleParametricExpr), AGExpression.ParametricCircleHint);
            HintDict.Add(Tuple.Create(CircleParametricExpr, CircleImplicitExpr), AGExpression.ImplicitCircleHint);

            foreach (Expr expr in QueryCircleImplicitExprs)
                HintDict.Add(Tuple.Create(CircleImplicitExpr, expr), AGExpression.QueryImplicitCircleHint);
            foreach (Expr expr in QueryCircleParametricExprs)
                HintDict.Add(Tuple.Create(CircleParametricExpr, expr), AGExpression.QueryParametricCircleHint);
            foreach (Expr expr in QueryCircleRadiusExprs)
                HintDict.Add(Tuple.Create(CircleImplicitExpr, expr), AGExpression.QueryCircleRadiusHint);
            foreach (Expr expr in QueryCircleCentralPtExprs)
                HintDict.Add(Tuple.Create(CircleImplicitExpr, expr), AGExpression.QueryCircleCentralPointHint);
            foreach (Expr expr in QueryCirclePerimeterExprs)
                HintDict.Add(Tuple.Create(CircleImplicitExpr, expr), AGExpression.QueryCirclePerimeterHint);
            foreach (Expr expr in QueryCircleSizeExprs)
                HintDict.Add(Tuple.Create(CircleImplicitExpr, expr), AGExpression.QueryCircleSizeHint);
        }

    
        public override void ComputeAttributes()
        {
            if (ReprType == RepresentationType.Implicit)
            {
                Perimeter = 2.0*Math.PI*Radius;
                Area      = Math.PI * Math.Pow(Radius, 2.0);
            }
            else if (ReprType == RepresentationType.Parametric)
            {
                
            }
        }

        public Point CentralPt { get; set; }
        public double Radius { get; set; }
        public double Perimeter { get; set; }
        public double Area { get; set; }

        //Parametric Form
        //TODO
    }

    public partial class Ellipse : Axiom
    {
        public Ellipse(string label, Point centralPt, double x, double y)
            : base(AxiomEnum.Ellipse, RepresentationType.Implicit, label)
        {
            CentralPt = centralPt;
            RadiusAlongXAxis = x;
            RadiusAlongYAxis = y;
        }

        public Ellipse(Point centralPt, double x, double y)
            : this(null, centralPt, x, y)
        {
        }

        public override void ExlainAttributes()
        {
            HintDict.Add(Tuple.Create(EllipseImplicitExpr, EllipseParametricExpr), AGExpression.ParametricEllipseHint);
            HintDict.Add(Tuple.Create(EllipseParametricExpr, EllipseImplicitExpr), AGExpression.ImplicitEllipseHint);

            foreach (Expr expr in QueryEllipseImplicitExprs)
                HintDict.Add(Tuple.Create(EllipseImplicitExpr, expr), AGExpression.QueryImplicitEllipseHint);
            foreach (Expr expr in QueryEllipseParametricExprs)
                HintDict.Add(Tuple.Create(EllipseParametricExpr, expr), AGExpression.QueryParametricEllipseHint);

            //TODO
        }

        public override string Parse(string repr)
        {
            throw new NotImplementedException();
        }

        public override void ComputeAttributes()
        {
            
        }

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

        //TODO Parametric Form
    }

    /// <summary>
    /// TODO Parabola
    /// </summary>
    public class Parabola : Axiom
    {
        public Parabola(string label) 
            :base(AxiomEnum.Parabola, RepresentationType.Implicit, label)
        { }

        public Point Vertex { get; set; }
        public double FocalParameterP { get; set; }
        public double DistanceFromVertexToFocusA { get; set; }
      
        public override string Parse(string repr)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// TODO Hyperbola
    /// </summary>
    public class Hyperbola : Axiom
    {
        public Hyperbola(string label) 
            :base(AxiomEnum.Hyperbola, RepresentationType.Implicit, label)
        { }

     

        public override string Parse(string repr)
        {
            throw new NotImplementedException();
        }
    }
}

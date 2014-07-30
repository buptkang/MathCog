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
    using System.Linq;
    using System.Collections.Generic; 
    using starPadSDK.MathExpr;

    public sealed partial class Point : Axiom
    {
        private static readonly List<string> PointRepr = new List<string>()
        {
            "A(1,2)",
            "(1,2)"
        };

        private static readonly List<string> QueryPoint = new List<string>()
        {
            "(?)"
        };

        static List<Expr> PointForm { get; set; }
        static List<Expr> QueryPointExprs { get; set; }

        static Point()
        {
            PointForm = PointRepr.Select(Text.Convert).ToList();
            QueryPointExprs = QueryPoint.Select(Text.Convert).ToList();
        }

    }


    public partial class Line : Axiom
    {
        public const string LineImplicitRepr = AGExpression.LineImplicitRepr;
        public const string LineExplicitRepr = AGExpression.LineExplicitRepr;
        public static readonly string[] LineParametricRepr = new string[]
        {
            AGExpression.LineParametricRepr1,
            AGExpression.LineParametricRepr2
        };

        public static readonly List<string> QueryLineImplicitRepr = new List<string>()
        {
            AGExpression.QueryLineImplicitRepr1,
            AGExpression.QueryLineImplicitRepr2
        };

        public static readonly List<string> QueryLineExplicitRepr = new List<string>()
        {
            AGExpression.QueryLineExplicitRepr1,
            AGExpression.QueryLineExplicitRepr2
        };

        public static readonly List<string> QueryLineParametricRepr = new List<string>()
        {
            AGExpression.QueryLineParametricRepr1,
            AGExpression.QueryLineParametricRepr2
        };

        public static readonly List<string> QueryLineSlope = new List<string>()
        {
            AGExpression.QueryLineSlope1,
            AGExpression.QueryLineSlope2
        };

        public static readonly List<string> QueryLineIntercept = new List<string>()
        {
            AGExpression.QueryLineIntercept1,
            AGExpression.QueryLineIntercept2
        };

        public static Expr LineImplicitExpr { get; set; }
        public static Expr LineExplicitExpr { get; set; }
        public static Expr LineParametricExpr { get; set; }

        public static List<Expr> QueryLineImplcitExprs { get; set; }
        public static List<Expr> QueryLineExplicitExprs { get; set; }
        public static List<Expr> QueryLineParametricExprs { get; set; }

        public static List<Expr> QueryLineSlopeExprs { get; set; }
        public static List<Expr> QueryLineInterceptExprs { get; set; }

        static Line()
        {
            LineImplicitExpr = Text.Convert(LineImplicitRepr);
            LineExplicitExpr = Text.Convert(LineExplicitRepr);
            LineParametricExpr = AGUtils.GenerateLineParametricExpr(LineParametricRepr[0], LineParametricRepr[1]);

            QueryLineImplcitExprs = QueryLineImplicitRepr.Select(Text.Convert).ToList();
            QueryLineExplicitExprs = QueryLineExplicitRepr.Select(Text.Convert).ToList();
            QueryLineParametricExprs = QueryLineParametricRepr.Select(Text.Convert).ToList();

            QueryLineSlopeExprs = QueryLineSlope.Select(Text.Convert).ToList();
            QueryLineInterceptExprs = QueryLineIntercept.Select(Text.Convert).ToList();
        }

        public override string Parse(string repr)
        {
            throw new NotImplementedException();
        }
    }

    public partial class Circle : Axiom
    {
        public const string CircleImplicitRepr = AGExpression.CircleImplicitRepr;                    
        public static readonly string[] CircleParametricRepr = new string[]
        {
            AGExpression.CircleParametricRepr1,
            AGExpression.CircleParametricRepr2
        };

        public static readonly List<string> QueryCircleImplicitRepr = new List<string>()
        {
            AGExpression.QueryCircleImplicitRepr1,
            AGExpression.QueryCircleImplicitRepr2
        };

        public static readonly List<string> QueryCircleParametricRepr = new List<string>()
        {
            AGExpression.QueryCircleParametricRepr1,
            AGExpression.QueryCircleParametricRepr2
        };

        public static readonly List<string> QueryCircleRadius = new List<string>()
        {
            AGExpression.QueryCircleRadius1,
            AGExpression.QueryCircleRadius2
        };

        public static readonly List<string> QueryCircleCentralPt = new List<string>()
        {
            AGExpression.QueryCircleCentralPt1,
            AGExpression.QueryCircleCentralPt2
        };

        public static readonly List<string> QueryCirclePerimeter = new List<string>()
        {
            AGExpression.QueryCirclePerimeter1,
            AGExpression.QueryCirclePerimeter2
        };

        public static readonly List<string> QueryCircleSize = new List<string>()
        {
            AGExpression.QueryCircleSize1,
            AGExpression.QueryCircleSize2
        };

        public static Expr CircleImplicitExpr { get; set; }
        public static Expr CircleParametricExpr { get; set; }

        public static List<Expr> QueryCircleImplicitExprs { get; set; }
        public static List<Expr> QueryCircleParametricExprs { get; set; }
        public static List<Expr> QueryCircleRadiusExprs { get; set; }
        public static List<Expr> QueryCircleCentralPtExprs { get; set; }
        public static List<Expr> QueryCirclePerimeterExprs { get; set; }
        public static List<Expr> QueryCircleSizeExprs { get; set; }

        static Circle()
        {
            CircleImplicitExpr = Text.Convert(CircleImplicitRepr);
            CircleParametricExpr = AGUtils.GenerateCircleParametricExpr(CircleParametricRepr[0], CircleParametricRepr[1]);

            QueryCircleImplicitExprs = QueryCircleImplicitRepr.Select(Text.Convert).ToList();
            QueryCircleParametricExprs = QueryCircleParametricRepr.Select(Text.Convert).ToList();

            QueryCircleRadiusExprs = QueryCircleRadius.Select(Text.Convert).ToList();
            QueryCircleCentralPtExprs = QueryCircleCentralPt.Select(Text.Convert).ToList();
            QueryCirclePerimeterExprs = QueryCirclePerimeter.Select(Text.Convert).ToList();
            QueryCircleSizeExprs = QueryCircleSize.Select(Text.Convert).ToList();
        }

        public override string Parse(string repr)
        {
            throw new NotImplementedException();
        }
    }

    public partial class Ellipse : Axiom
    {
        public const string EllipseImplicitRepr = AGExpression.CircleImplicitRepr;

        public static readonly string[] EllipseParametricRepr = new string[]
        {
            AGExpression.EllipseParametricRepr1,
            AGExpression.EllipseParametricRepr2
        };

        public static readonly List<string> QueryEllipseImplicitRepr = new List<string>()
        {
            AGExpression.QueryEllipseImplicitRepr1,
            AGExpression.QueryEllipseImplicitRepr2
        };

        public static readonly List<string> QueryEllipseParametricRepr = new List<string>()
        {
            AGExpression.QueryEllipseParametricRepr1,
            AGExpression.QueryEllipseParametricRepr2
        };

        public static readonly List<string> QueryEllipseCentralPt = new List<string>()
        {
            AGExpression.QueryEllipseCentralPt1,
            AGExpression.QueryEllipseCentralPt2
        };

        public static readonly List<string> QueryEllipseRadiusAlongXAxis = new List<string>()
        {
            AGExpression.QueryEllipseRadiusAlongXAxis1,
            AGExpression.QueryEllipseRadiusAlongXAxis2
        };

        public static readonly List<string> QueryEllipseRadiusAlongYAxis = new List<string>()
        {
            AGExpression.QueryEllipseRadiusAlongYAxis1,
            AGExpression.QueryEllipseRadiusAlongYAxis2,
        };

        public static readonly List<string> QueryEllipseFoci = new List<string>()
        {
            AGExpression.QueryEllipseFociDistance1,
            AGExpression.QueryEllipseFociDistance2
        };

        public static readonly List<string> QueryEllipseLeftFoci = new List<string>()
        {
            AGExpression.QueryLeftFoci1,
            AGExpression.QueryLeftFoci2
        };

        public static readonly List<string> QueryEllipseRightFoci = new List<string>()
        {
            AGExpression.QueryRightFoci1,
            AGExpression.QueryRightFoci2
        };

        public static Expr EllipseImplicitExpr { get; set; }
        public static Expr EllipseParametricExpr { get; set; }

        public static List<Expr> QueryEllipseImplicitExprs { get; set; }
        public static List<Expr> QueryEllipseParametricExprs { get; set; }
        public static List<Expr> QueryEllipseCentralPtExprs { get; set; }
        public static List<Expr> QueryEllipseRadiusAlongXAxisExprs { get; set; }
        public static List<Expr> QueryEllipseRadiusAlongYAxisExprs { get; set; }
        public static List<Expr> QueryEllipseFociExprs { get; set; }
        public static List<Expr> QueryEllipseLeftFociExprs { get; set; }
        public static List<Expr> QueryEllipseRightFociExprs { get; set; }

        static Ellipse()
        {
            EllipseImplicitExpr = Text.Convert(EllipseImplicitRepr);
            EllipseParametricExpr = AGUtils.GenerateEllipseParametricExpr(EllipseParametricRepr[0],
                EllipseParametricRepr[1]);
            QueryEllipseImplicitExprs = QueryEllipseImplicitRepr.Select(Text.Convert).ToList();
            QueryEllipseParametricExprs = QueryEllipseParametricRepr.Select(Text.Convert).ToList();
            QueryEllipseCentralPtExprs = QueryEllipseCentralPt.Select(Text.Convert).ToList();
            QueryEllipseRadiusAlongXAxisExprs = QueryEllipseRadiusAlongXAxis.Select(Text.Convert).ToList();
            QueryEllipseRadiusAlongYAxisExprs = QueryEllipseRadiusAlongYAxis.Select(Text.Convert).ToList();
            QueryEllipseFociExprs = QueryEllipseFoci.Select(Text.Convert).ToList();
            QueryEllipseLeftFociExprs = QueryEllipseLeftFoci.Select(Text.Convert).ToList();
            QueryEllipseRightFociExprs = QueryEllipseRightFoci.Select(Text.Convert).ToList();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExprSemantic
{

    public enum RepresentationType
    {
        Explicit, Implicit, Parametric
    }

    public enum CoordinateSystemType
    {
        Cartesian, Polar
    }

    public enum TripleRelationEnum
    {
        //TODO Three Lines                                      
    }

    public enum TwoRelationEnum
    {
        PointPoint  = 0,       
        PointLine   = 1,
        PointCircle = 2,        
        LineLine    = 3,
        LineCircle  = 4,
        CircleCircle = 5,

        PEllipse            = 6,
        PHyperbola          = 7,
        PParabola           = 8,
        LineEllipsee        = 9,
        LineHyperbola       = 10,
        LineParabola        = 11,
        CircleEllipse       = 12,
        CircleHyperbola     = 13,
        CircleParabola      = 14,
        EllipseEllipse      = 15,
        EllipseHyperbola    = 16,
        EllipseParabola     = 17,
        HyperbolaHyperbola  = 18,
        HyperbolaParabola   = 19,
        ParabolaParabola    = 20
    }

    public enum AxiomEnum 
    {
        Point = 0,
        Line = 1,
        Circle = 2,
        Ellipse = 3,
        Parabola = 4,
        Hyperbola = 5,
        None = -1
        //TODO 
    }
}

namespace AlgebraGeometry
{
    public enum RepresentationType
    {
        Explicit, Implicit, Parametric
    }

    public enum CoordinateSystemType
    {
        Cartesian, Polar
    }
 
    public enum ShapeType
    {
        Point = 0,
        Line = 1,
        QuadraticCurve = 2,
        Circle = 3,
        Ellipse = 4,
        Parabola = 5,
        Hyperbola = 6,
        None = -1       
    }
}

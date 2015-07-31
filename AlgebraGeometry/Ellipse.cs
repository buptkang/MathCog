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
    }

}

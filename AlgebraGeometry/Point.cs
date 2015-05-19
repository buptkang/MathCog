using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSharpLogic;

namespace AlgebraGeometry
{
    public class Point : Shape
    {
        public Var XCoordinate { get; set; }
        public Var YCoordinate { get; set; }

        public Point(string label, object xcoordinate, object ycoordinate)
            : base(ShapeType.Point, label)
        {
            var xCord = xcoordinate as Var;
            if (xCord != null)
            {
                XCoordinate = xCord;                
            }
            else
            {
                XCoordinate = new Var('x');
                Properties.Add(XCoordinate, xcoordinate);
            }

            var yCord = ycoordinate as Var;
            if (yCord != null)
            {
                YCoordinate = yCord;
            }
            else
            {
                YCoordinate = new Var('y');
                Properties.Add(YCoordinate, ycoordinate);
            }
        }

        public Point(object xcoordinate, object ycoordinate)
            : this(null, xcoordinate, ycoordinate)
        { 
        }

        public bool AddXCoord(object x)
        {
            if (LogicSharp.IsNumeric(x))
            {
                Properties.Add(XCoordinate, x);
                return true;
            }
            return false;
        }

        public bool AddYCoord(object y)
        {
            if (LogicSharp.IsNumeric(y))
            {
                Properties.Add(YCoordinate, y);
                return true;
            }
            return false;
        }

        public override bool Concrete
        {
            get
            {
                return Properties.ContainsKey(XCoordinate) &&
                Properties.ContainsKey(YCoordinate);
            }
        }

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

    public class PointSymbol : ShapeSymbol
    {
        public PointSymbol(Point pt) : base(pt)
        {

        }

        public string SymXCoordinate
        {
            get
            {
                var pt = Shape as Point;
                if (pt.Properties.ContainsKey(pt.XCoordinate))
                {
                    object value = pt.Properties[pt.XCoordinate];
                    return value.ToString();
                }
                else
                {
                    return pt.XCoordinate.ToString();
                }
            }
        }

        public string SymYCoordinate
        {
            get
            {
                var pt = Shape as Point;
                if (pt.Properties.ContainsKey(pt.YCoordinate))
                {
                    object value = pt.Properties[pt.YCoordinate];
                    return value.ToString();
                }
                else
                {
                    return pt.YCoordinate.ToString();
                }
            }
        }

        public override string ToString()
        {
            if (Shape.Label != null)
            {
                return String.Format("{0}({1},{2})", Shape.Label, SymXCoordinate, SymYCoordinate);
            }
            else
            {
                return String.Format("({0},{1})", SymXCoordinate, SymYCoordinate);
            }
        }
    }
}

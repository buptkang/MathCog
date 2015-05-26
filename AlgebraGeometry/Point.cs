using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSharpLogic;

namespace AlgebraGeometry
{
    public partial class Point : Shape
    {
        public object XCoordinate { get; set; }
        public object YCoordinate { get; set; }

        public Point(string label, object xcoordinate, object ycoordinate)
            : base(ShapeType.Point, label)
        {
            XCoordinate = xcoordinate;
            YCoordinate = ycoordinate;
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
            get { return !Var.ContainsVar(XCoordinate) && !Var.ContainsVar(YCoordinate); }
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
        public override IEnumerable<ShapeSymbol> RetrieveGeneratedShapes()
        {
            if (Shape.Concrete) return null;
            if (Shape.CachedSymbols.Count == 0) return null;
            var lst = new List<PointSymbol>();
            foreach (Shape s in Shape.CachedSymbols)
            {
                var pt = s as Point;
                lst.Add(new PointSymbol(pt));
            }
            return lst;
        }

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

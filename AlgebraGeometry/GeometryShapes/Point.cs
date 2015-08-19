using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSharpLogic;

namespace AlgebraGeometry
{
    public partial class Point : Shape
    {
        private object _xCoord;
        private object _yCoord;

        public object XCoordinate
        {
            get
            {
                return _xCoord;
            }
            set
            {
                _xCoord = value;
                NotifyPropertyChanged("XCoordinate");
            }
        }

        public object YCoordinate
        {
            get
            {
                return _yCoord;
            }
            set
            {
                _yCoord = value;
                NotifyPropertyChanged("YCoordinate");
            }
        }

        public Point(string label, object xcoordinate, object ycoordinate)
            : base(ShapeType.Point, label)
        {
            _xCoord = xcoordinate;
            _yCoord = ycoordinate;

            double d; 
            if (LogicSharp.IsDouble(xcoordinate, out d))
            {
                _xCoord = Math.Round(d, 1);
            }

            if (LogicSharp.IsDouble(ycoordinate, out d))
            {
                _yCoord = Math.Round(d, 1);
            }
        }

        public Point(object xcoordinate, object ycoordinate)
            : this(null, xcoordinate, ycoordinate)
        { 
        }

        #region Pass by reference experiment, not used in the upper level

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

        #endregion 

        public override bool Concrete
        {
            get { return !Var.ContainsVar(XCoordinate) && !Var.ContainsVar(YCoordinate); }
        }

        public override List<Var> GetVars()
        {
            var lst = new List<Var>();
            lst.Add(Var.GetVar(XCoordinate));
            lst.Add(Var.GetVar(YCoordinate));
            return lst;
        }

        #region IEqutable

        public override bool Equals(object obj)
        {
            var shape = obj as Shape;
            if (shape != null)
            {
                return Equals(shape);
            }
            else
            {
                return false;
            }
        }

        public override bool Equals(Shape other)
        {
            if (other is Point)
            {
                var pt = other as Point;
                if (XCoordinate.Equals(pt.XCoordinate) && YCoordinate.Equals(pt.YCoordinate))
                {
                    return true;
                }
                if (!XCoordinate.Equals(pt.XCoordinate))
                {
                    return false;
                }
                if(!YCoordinate.Equals(pt.YCoordinate))
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

        public PointType InputType { get; set; } 
        public override object GetInputType() { return InputType;  }        
    }

    public enum PointType
    {
        Relation,
        General
    }

    public partial class PointSymbol : ShapeSymbol
    {
        public override bool UnifyProperty(string label, out object obj)
        {
            return this.Unify(label, out obj);
        }

        public override object RetrieveConcreteShapes()
        {
            if (CachedSymbols.Count == 0) return this;
            return CachedSymbols.ToList();
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

        public PointType OutputType { get; set; }
        public override object GetOutputType() { return OutputType; }
    }
}

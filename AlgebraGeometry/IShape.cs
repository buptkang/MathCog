using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSharpLogic;

namespace AlgebraGeometry
{
    public abstract class Shape : DyLogicObject, IEquatable<Shape>
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

        public virtual bool Concrete
        {
            get
            {
                throw new NotImplementedException();
            }
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

    public abstract class ShapeSymbol
    {
        public Shape Shape { get; set; }

        protected ShapeSymbol(Shape _shape)
        {
            Shape = _shape;
        } 
    }
}

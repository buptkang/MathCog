using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSharpLogic;
using System.ComponentModel;

namespace AlgebraGeometry
{
    public abstract partial class Shape : DyLogicObject,
        IEquatable<Shape>, INotifyPropertyChanged
    {
        public string Label { get; set; }
        public ShapeType ShapeType { get; set; }
        public CoordinateSystemType Coordinate { get; set; }
        public RepresentationType Repr { get; set; }

        #region Interaction Purpuse
        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property. 
        // The CallerMemberName attribute that is applied to the optional propertyName 
        // parameter causes the property name of the caller to be substituted as an argument. 
        protected void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        protected Shape(ShapeType shapeType, string label)
            : this()
        {
            this.Label = label;
            this.ShapeType = shapeType;
        }

        protected Shape()
        {
            CachedSymbols = new HashSet<Shape>();
            CachedGoals = new HashSet<KeyValuePair<object,EqGoal>>();
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

        public ShapeSymbol(Shape _shape)
        {
            Shape = _shape;
        }

        public abstract IEnumerable<ShapeSymbol> RetrieveGeneratedShapes();
    }
}

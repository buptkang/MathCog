using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlgebraGeometry
{
    public class ShapeNode : GraphNode
    {
        private Shape _shape;
        public Shape Shape
        {
            get { return _shape; }
            set
            {
                NotifyPropertyChanged("Shape");
                _shape = value;
            }
        }

        public ShapeNode(Shape shape) : base()
        {
            _shape = shape;
        }
    }
}

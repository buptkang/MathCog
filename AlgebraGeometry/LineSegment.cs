using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSharpLogic;

namespace AlgebraGeometry
{
    public partial class LineSegment : Shape
    {
        private Point _pt1;
        public Point Pt1
        {
            get { return _pt1;  }
            set { _pt1 = value; }
        }
        private Point _pt2;
        public Point Pt2
        {
            get { return _pt2; }
            set { _pt2 = value;}
        }

        public LineSegment(string label) : base(ShapeType.LineSegment, label)
        {
            RelationStatus = true;
        }

        public LineSegment(string label, Point pt1, Point pt2) : base(ShapeType.LineSegment, label)
        {
            if(pt1.Equals(pt2)) 
                throw new Exception("Two points are identical");
            RelationStatus = true;
            ExtractRelationLabel(pt1.Label, pt2.Label);
        }

        public LineSegment(Point pt1, Point pt2) : this(null, pt1, pt2)
        {            
        }

        private void ExtractRelationLabel(string label1, string label2)
        {
            if (Label != null) return;
            if (label1 == null || label2 == null)
            {
                Label = "Line Segment.";
            }
            else
            {
                Label = string.Concat(label1, label2);
            }
        }
    }


    public class LineSegmentSymbol : ShapeSymbol
    {
        public LineSegmentSymbol(LineSegment _seg) : base(_seg)
        {

        }

        public override IEnumerable<ShapeSymbol> RetrieveGeneratedShapes()
        {
            throw new NotImplementedException();
        }
    }
}

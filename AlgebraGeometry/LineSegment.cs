using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public override bool Concrete
        {
            get
            {
                return !Var.ContainsVar(_pt1.XCoordinate) &&
                       !Var.ContainsVar(_pt1.YCoordinate) &&
                       !Var.ContainsVar(_pt2.XCoordinate) &&
                       !Var.ContainsVar(_pt2.YCoordinate);
            }
        }

        public override List<Var> GetVars()
        {
            var lst = new List<Var>();
            lst.Add(Var.GetVar(_pt1.XCoordinate));
            lst.Add(Var.GetVar(_pt1.YCoordinate));
            lst.Add(Var.GetVar(_pt2.XCoordinate));
            lst.Add(Var.GetVar(_pt2.YCoordinate));
            return lst;
        }

        #region IEqutable

        public override bool Equals(Shape other)
        {
            if (other == null) return false;
            if (other is LineSegment)
            {
                var lineSeg = other as LineSegment;
                bool equalPt1 = Pt1.Equals(lineSeg.Pt1);
                bool equalPt2 = Pt2.Equals(lineSeg.Pt2);
                if (!(equalPt1 || equalPt2)) return false;
            }
            return base.Equals(other);
        }

        public override int GetHashCode()
        {
            Debug.Assert(Label != null);
            if (Pt1 != null)
            {
                if (Pt2 != null)
                {
                    return Pt1.GetHashCode() ^ Pt2.GetHashCode() ^ Label.GetHashCode();
                }
                else
                {
                    return Pt1.GetHashCode() ^ Label.GetHashCode();
                }
            }
            else
            {
                if (Pt2 != null)
                {
                    return Pt2.GetHashCode() ^ Label.GetHashCode();
                }
                else
                {
                    return Label.GetHashCode();
                }
            }
        }

        #endregion
    }

    public class LineSegmentSymbol : ShapeSymbol
    {
        public override IEnumerable<ShapeSymbol> RetrieveGeneratedShapes()
        {
            if (Shape.CachedSymbols.Count == 0) return null;
            return Shape.CachedSymbols.Select(s => s as LineSegment)
                .Select(lineSeg => new LineSegmentSymbol(lineSeg)).ToList();
        }

        public LineSegmentSymbol(LineSegment _seg) : base(_seg)
        {
        }

        public override string ToString()
        {
            return "LineSegment";
        }
    }
}

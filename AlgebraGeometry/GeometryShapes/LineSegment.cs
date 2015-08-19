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
        #region Properties and Constructors

        #region Properties

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

        private object _distance;

        public object Distance
        {
            get { return _distance; }
            set { _distance = value; }
        }

        public LineSegmentType InputType { get; set; }
        public override object GetInputType() { return InputType; }

        #endregion

        #region Constructors

        public LineSegment(string label) : base(ShapeType.LineSegment, label)
        {
            InputType = LineSegmentType.Relation;
        }

        public LineSegment(string label, Point pt1, Point pt2) : base(ShapeType.LineSegment, label)
        {
            _pt1 = pt1;
            _pt2 = pt2;
            if (pt1.Equals(pt2))
                throw new Exception("Two points are identical");
            InputType = LineSegmentType.Relation;
            ExtractRelationLabel(pt1.Label, pt2.Label);
            Calc_Distance();
        }

        public LineSegment(Point pt1, Point pt2) : this(null, pt1, pt2)
        {           
        }

        #endregion

        #endregion

        #region Utils

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

        #endregion

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

        #region Transformations

        private void Calc_Distance()
        {
            if (_pt1.Concrete && _pt2.Concrete)
            {
                double yDiff = Math.Abs((double)_pt1.YCoordinate - (double)_pt2.YCoordinate);
                double xDiff = Math.Abs((double)_pt1.XCoordinate - (double)_pt2.XCoordinate);
                Distance = Math.Sqrt(Math.Pow(xDiff, 2.0) + Math.Pow(yDiff, 2.0));
            }
        }

/*        public static Expr Generate2PointsTrace1(TwoPoints tp)
        {
            string str = String.Format("P1({0}, {1}), P2({2}, {3})", tp.P1.XCoordinate, tp.P1.YCoordinate,
                tp.P2.XCoordinate, tp.P2.YCoordinate);
            return starPadSDK.MathExpr.Text.Convert(str);
        }

        public static Expr Generate2PointsTrace2(TwoPoints tp)
        {
            string str = String.Format("d = (({0} - {1})^2 + ({2} - {3})^2)^(1/2)", tp.P2.XCoordinate, tp.P1.XCoordinate,
                tp.P2.YCoordinate, tp.P1.YCoordinate);

            return starPadSDK.MathExpr.Text.Convert(str);
        }

        public static Expr Generate2PointsTrace3(TwoPoints tp)
        {
            var d =
                Math.Sqrt(Math.Pow(tp.P2.XCoordinate - tp.P1.XCoordinate, 2d) +
                          Math.Pow(tp.P2.YCoordinate - tp.P1.YCoordinate, 2d));
            string str = String.Format("d = {0}", d);

            return starPadSDK.MathExpr.Text.Convert(str);
        }*/


        #endregion
    }

    public enum LineSegmentType
    {
        Relation
    }

    public partial class LineSegmentSymbol : ShapeSymbol
    {
        public override bool UnifyProperty(string label, out object obj)
        {
            obj = this.Unify(label);
            if (obj == null) return false;
            return true;
        }

        public override object RetrieveConcreteShapes()
        {
            var lineSeg = Shape as LineSegment;
            Debug.Assert(lineSeg != null);
            if (lineSeg.Concrete) return this;
            if (CachedSymbols.Count == 0) return null;
            return CachedSymbols.ToList();
        }

        public LineSegmentSymbol(LineSegment _seg) : base(_seg)
        {
        }

        public override string ToString()
        {
            return "LineSegment";
        }

        public LineSegmentType OutputType { get; set; }

        public override object GetOutputType() { return OutputType; }
    }
}

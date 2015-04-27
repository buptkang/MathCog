using AGSemantic.KnowledgeBase;

namespace ExprSemantic.KnowledgeRelation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public abstract class Relation
    {
        public int NumberOfEntities { get; set; }
    }

    public class TwoPoints : Relation
    {
        #region Properties
        public Point P1 { get; set; }
        public Point P2 { get; set; }

        public bool IsIdentity { get; set; }
        public double? Distance { get; set; }

        #endregion

        #region Constructor

        public TwoPoints(Point p1, Point p2)
        {
            P1 = p1;
            P2 = p2;
        }

        #endregion
    }

    public class PointLine : Relation
    {
        #region Properties

        public Point P { get; set; }
        public Line L { get; set; }

        // Check Point on the Line or not. PonL true, POffL false.  
        public bool PonL { get; set; }
        // Calculate the distance from the point to the line.
        private double _ptoLDistance;
        public double PtoLDistance
        {
            get { return _ptoLDistance; }
            set
            {
                _ptoLDistance = Math.Abs(L.A*P.XCoordinate + L.B*P.YCoordinate + L.C)/
                                Math.Sqrt(Math.Pow(L.A, 2.0) + Math.Pow(L.B, 2.0));
            }
        }

        #endregion

        #region constructors

        public PointLine(Point p, Line l)
        {
            P = p;
            L = l;
        }

        #endregion

        #region IEquatable

        public override bool Equals(object obj)
        {
            if (obj is PointLine)
            {
                var other = obj as PointLine;

                if (P.Equals(other.P) && L.Equals(other.L))
                {
                    return base.Equals(obj);                    
                }
                else
                {
                    return false;
                }                
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return P.GetHashCode() ^ L.GetHashCode();
        }

        #endregion


    }

    public class TwoLines : Relation
    {
        #region Properties

        public Line Line1 { get; set; }
        public Line Line2 { get; set; }

        public bool TwoLineParallel { get; set; } // true: parallel; false: intersect
        public double? Distance { get; set; } // if two line parallel, distance exists. 

        #endregion

        #region Constructor

        public TwoLines(Line line1, Line line2)
        {
            Line1 = line1;
            Line2 = line2;
        }

        #endregion 
    }

    public class Angle : TwoLines
    {
        public double Degree { get; set; }
        public Point IntersectPoint { get; set; }

        public Angle(Line line1, Line line2) : base(line1, line2)
        {
            
        }
    }

    public class LineAndCircle : Relation
    {
        #region Properties

        public Circle Circle { get; set; }
        public Line Line { get; set; }

        public IntersectType IntersectEnum { get; set; }
        public Point IntersectPt1 { get; set; }
        public Point IntersectPt2 { get; set; }

        public enum IntersectType
        {
            Tangent, Miss, TwoPoints
        };

        #endregion

        #region Constructor

        public LineAndCircle(Line line, Circle circle)
        {
            Line = line;
            Circle = circle;
        }

        #endregion

    }

    public class ThreeLines : Relation
    {
        #region Properties

        public TwoLines TwoLine { get; set; }
        public Line Line { get; set; }

        public bool IntersectOnePt { get; set; } // intersect in one point or two points
        public Point IntersectPt1 { get; set; }
        public Point IntersectPt2 { get; set; }

        public double Perimeter { get; set; }
        public double Size { get; set; }
       
        #endregion

        #region Constructor

        public ThreeLines(TwoLines twoLines, Line line)
        {
            TwoLine = twoLines;
            Line = line;
        }

        #endregion
    }
}

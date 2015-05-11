using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AGSemantic.KnowledgeBase;
using starPadSDK.MathExpr;

namespace ExprSemantic.KnowledgeRelation
{
    public abstract class AGRelationExprs : IKnowledgeExpr
    {
        public List<ShapeExpr> ShapeExprs { get; set; }

        public AGRelationExprs()
        {
               
        }

        public AGRelationExprs(List<ShapeExpr> exprs)
        {
            ShapeExprs = exprs;
        }
    }

    public class PointPointExpr : AGRelationExprs
    {
        public TwoPoints PointPoint { get; set; }

        public PointPointExpr(TwoPoints tp)
        {
            PointPoint = tp;
        }

        public Expr FakeV
        {
            get
            {
                return starPadSDK.MathExpr.Text.Convert("v=8, v=0");        
            }
        }
    }

    public class PointLineExpr : AGRelationExprs
    {
        public PointLine PLRelation { get; set; }

        public PointLineExpr(PointLine pl)
        {
            PLRelation = pl;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is PointLineExpr)) return false;

            var otherPL = obj as PointLineExpr;
            return this.PLRelation.Equals(otherPL.PLRelation);
        }

        public override int GetHashCode()
        {
            return GeneralExpr.GetHashCode() ^ PLRelation.GetHashCode();
        }
    }

    public class TwoLinesExprs : AGRelationExprs
    {
        public LineExpr LineExpr1 { get; set; }
        public LineExpr LineExpr2 { get; set; }

        public TwoLines TwoLine { get; set; }

        public TwoLinesExprs(LineExpr expr1, LineExpr expr2)
            : base(new List<ShapeExpr>() { expr1, expr2})
        {
            LineExpr1 = expr1;
            LineExpr2 = expr2;

            //TwoLine = RelationFactory.Cre
        }
    }

    public class ThreeLinesExprs : AGRelationExprs
    {
        public TwoLinesExprs TwoLineExpr { get; set; }
        public LineExpr LineExpr { get; set; }

        public ThreeLinesExprs(TwoLinesExprs twoLineExprs, LineExpr lineExpr)
            : base(new List<ShapeExpr>() { twoLineExprs.LineExpr1, twoLineExprs.LineExpr2, lineExpr})
        {
            TwoLineExpr = twoLineExprs;
            LineExpr = lineExpr;
        }
    }

    public class LineCircleExprs : AGRelationExprs
    {
        public LineExpr LineExpr { get; set; }
        public CircleExpr CircleExpr { get; set; }

        public LineCircleExprs(LineExpr lineExpr, CircleExpr circleExpr)
            : base(new List<ShapeExpr>() { lineExpr, circleExpr})
        {
            LineExpr = lineExpr;
            CircleExpr = circleExpr;
        }
    }


}

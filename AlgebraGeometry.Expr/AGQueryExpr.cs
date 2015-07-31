using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using CSharpLogic;
using ExprGenerator;

namespace AlgebraGeometry.Expr
{
    /// <summary>
    /// Data structure to convert QueryResult to
    /// </summary>
    public class AGQueryExpr : IKnowledge
    {
        private Query _query;
        public Query QueryTag
        {
            get { return _query; }
            set { _query = value; }
        }

        public AGQueryExpr(starPadSDK.MathExpr.Expr expr, Query queryTag) : base(expr)
        {
            QueryTag = queryTag;
        }

        public List<IKnowledge> RetrieveRenderKnowledge()
        {
            if (_query.CachedEntities.Count == 0) return null;
            var lst = new List<IKnowledge>();

            foreach (var cacheObj in _query.CachedEntities)
            {
                var cacheShapeSymbol = cacheObj as ShapeSymbol;
                var cacheGoal = cacheObj as EqGoal;
                if (cacheShapeSymbol != null)
                {
                    starPadSDK.MathExpr.Expr expr = ExprG.Generate(cacheShapeSymbol);
                    var agShape = new AGShapeExpr(expr, cacheShapeSymbol);
                    lst.Add(agShape);   
                }
                if (cacheGoal != null)
                {
                    starPadSDK.MathExpr.Expr expr = ExprG.Generate(cacheGoal);
                    var agGoal = new AGPropertyExpr(expr, cacheGoal);
                    lst.Add(agGoal);
                }
            }
            return lst;
        }
    }
}

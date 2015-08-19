using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        #region Properties and Constructors

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

        #endregion

        #region Override Functions

        public override void RetrieveRenderKnowledge()
        {
            if (_query.CachedEntities.Count == 0) return;
            var lst = new ObservableCollection<IKnowledge>();

            foreach (var cacheObj in _query.CachedEntities)
            {
                var cacheShapeSymbol = cacheObj as ShapeSymbol;
                var cacheGoal        = cacheObj as EqGoal;
                var cacheEq          = cacheObj as Equation;
                if (cacheShapeSymbol != null)
                {
                    starPadSDK.MathExpr.Expr expr = ExprG.Generate(cacheShapeSymbol);
                    var agShape = new AGShapeExpr(expr, cacheShapeSymbol);
                    lst.Add(agShape);   
                }
                else if (cacheGoal != null)
                {
                    starPadSDK.MathExpr.Expr expr = ExprG.Generate(cacheGoal);
                    var agGoal = new AGPropertyExpr(expr, cacheGoal);
                    lst.Add(agGoal);
                }
                else if (cacheEq != null)
                {
                    starPadSDK.MathExpr.Expr expr = ExprG.Generate(cacheEq);
                    var agEq = new AGEquationExpr(expr, cacheEq);
                    lst.Add(agEq);
                }
            }
            RenderKnowledge = lst;
        }

        public override void GenerateSolvingTrace()
        {
            foreach (var temp in RenderKnowledge)
            {
                if (temp.IsSelected)
                {
                    temp.GenerateSolvingTrace();
                }
            }
        }

        #endregion
    }
}

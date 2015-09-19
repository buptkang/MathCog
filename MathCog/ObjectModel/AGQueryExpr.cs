/*******************************************************************************
 * Copyright (c) 2015 Bo Kang
 *   
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *  
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *******************************************************************************/

namespace MathCog
{
    using AlgebraGeometry;
    using CSharpLogic;
    using System.Collections.ObjectModel;

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

        public AGQueryExpr(starPadSDK.MathExpr.Expr expr, Query queryTag)
            : base(expr)
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
                var cacheGoal = cacheObj as EqGoal;
                var cacheEq = cacheObj as Equation;
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

        public override bool Equals(object obj)
        {
            var queryExpr = obj as AGQueryExpr;
            if (queryExpr == null) return false;

            return QueryTag.Equals(queryExpr.QueryTag);
        }

        #endregion
    }
}

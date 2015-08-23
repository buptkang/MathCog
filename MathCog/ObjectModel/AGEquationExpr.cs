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
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using AlgebraGeometry;
    using CSharpLogic;

    public class AGEquationExpr : IKnowledge
    {
        #region Properties and Constructors

        private Equation _equation;

        public Equation Equation
        {
            get { return _equation; }
            set { _equation = value; }
        }

        protected AGEquationExpr(starPadSDK.MathExpr.Expr expr)
            : base(expr)
        {
        }

        public AGEquationExpr(starPadSDK.MathExpr.Expr expr, Equation eq)
            : base(expr)
        {
            _equation = eq;
        }

        #endregion

        #region Override functions

        public override void RetrieveRenderKnowledge()
        {
            if (_equation.CachedEntities.Count == 0) return;
            var lst = new ObservableCollection<IKnowledge>();

            foreach (var cacheObj in _equation.CachedEntities)
            {
                var cacheShapeSymbol = cacheObj as ShapeSymbol;
                var cacheGoal = cacheObj as EqGoal;
                var cacheEq = cacheObj as Equation;
                if (cacheShapeSymbol != null)
                {
                    throw new Exception("TODO");
                    /*                    starPadSDK.MathExpr.Expr expr = ExprG.Generate(cacheShapeSymbol);
                                        var agShape = new AGShapeExpr(expr, cacheShapeSymbol);
                                        lst.Add(agShape);*/
                }
                else if (cacheGoal != null)
                {
                    throw new Exception("TODO");
                    /*                    starPadSDK.MathExpr.Expr expr = ExprG.Generate(cacheGoal);
                                        var agGoal = new AGPropertyExpr(expr, cacheGoal);
                                        lst.Add(agGoal);*/
                }
                else if (cacheEq != null)
                {
                    throw new Exception("TODO");
                    /*
                                        starPadSDK.MathExpr.Expr expr = ExprG.Generate(cacheEq);
                                        var agEq = new AGEquationExpr(expr, cacheEq);
                                        lst.Add(agEq);
                     */
                }
                else
                {
                    var boolValue = cacheObj as bool?;
                    if (boolValue != null)
                    {
                        starPadSDK.MathExpr.Expr expr = ExprG.Generate(boolValue);
                        var agEq = new IKnowledge(expr);
                        lst.Add(agEq);
                    }
                }
            }
            RenderKnowledge = lst;
        }

        public override void GenerateSolvingTrace()
        {
            if (RenderKnowledge == null)
            {
                //rendered knowledge itself
                if (IsSelected)
                {
                    var traces = _equation.Traces;
                    if (traces.Count == 0) return;
                    var lst = new List<TraceStepExpr>();
                    for (var i = 0; i < traces.Count; i++)
                    {
                        var ts = traces[i];
                        var tse = new TraceStepExpr(ts);
                        lst.Add(tse);
                    }
                    AutoTrace = lst;
                }
            }
            else
            {
                foreach (var temp in RenderKnowledge)
                {
                    if (temp.IsSelected)
                    {
                        temp.GenerateSolvingTrace();
                    }
                }
            }
        }

        #endregion
    }
}

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

using System.Diagnostics;

namespace MathCog
{
    using AlgebraGeometry;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;
    using CSharpLogic;

    public class AGShapeExpr : AGEquationExpr
    {
        #region Properties and Regions

        private ShapeSymbol _shapeSymbol;
        public ShapeSymbol ShapeSymbol
        {
            get { return _shapeSymbol; }
            set { _shapeSymbol = value; }
        }

        public AGShapeExpr(starPadSDK.MathExpr.Expr expr, ShapeSymbol ss)
            : base(expr)
        {
            _shapeSymbol = ss;
        }

        #endregion

        #region Override functions

        public override bool HasSolvingTrace()
        {
            var traces = _shapeSymbol.Traces;
            if (traces.Count == 0) return false;
            return true;
        }

        public override void RetrieveRenderKnowledge()
        {
            var symbols = _shapeSymbol.RetrieveConcreteShapes();
            var shapes = new ObservableCollection<IKnowledge>();
            RenderKnowledge = null;
            if (symbols != null)
            {
                var shapeSymbol = symbols as ShapeSymbol;
                var ssLst = symbols as IEnumerable<ShapeSymbol>;

                if (shapeSymbol != null)
                {
                    starPadSDK.MathExpr.Expr expr = ExprG.Generate(shapeSymbol);
                    var agShape = new AGShapeExpr(expr, shapeSymbol);
                    shapes.Add(agShape);
                }

                if (ssLst != null)
                {
                    foreach (ShapeSymbol ss in ssLst.ToList())
                    {
                        var expr = ExprG.Generate(ss);
                        var agExpr = new AGShapeExpr(expr, ss);
                        shapes.Add(agExpr);
                    }
                }
                RenderKnowledge = shapes;
                RenderKnowledge.CollectionChanged += RenderKnowledge_CollectionChanged;
            }
        }

        void RenderKnowledge_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                Console.Write("TODO");
            }
        }

        public override void GenerateSolvingTrace()
        {
            if (IsSelected)
            {
                var traces = _shapeSymbol.Traces;
                if (traces.Count == 0) return;
                var lstTuples = new List<Tuple<object, object>>();
                for (var i = 0; i < traces.Count; i++)
                {
                    var strategy = traces[i].Item1 as string;
                    var steps = traces[i].Item2 as List<TraceStep>;
                    Debug.Assert(steps != null);
                    var gSteps = new List<TraceStepExpr>();
                    foreach (TraceStep ts in steps)
                    {
                        var tse = new TraceStepExpr(ts);
                        gSteps.Add(tse);
                    }

                    var gTuple = new Tuple<object, object>(strategy, gSteps);
                    lstTuples.Add(gTuple);
                }
                AutoTrace = lstTuples;
                return;
            }

            if (RenderKnowledge == null) return;

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
            var shapeExpr = obj as AGShapeExpr;
            if (shapeExpr != null)
            {
                return ShapeSymbol.Equals(shapeExpr.ShapeSymbol);                
            }
            var shapeSym = obj as ShapeSymbol;
            if (shapeSym != null)
            {
                return ShapeSymbol.Equals(shapeSym);
            }

            return false;
        }

        #endregion
    }
}

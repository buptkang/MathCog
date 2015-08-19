﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using ExprGenerator;
using CSharpLogic;

namespace AlgebraGeometry.Expr
{
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
            :base(expr)
        {
            _shapeSymbol = ss;
        }

        #endregion

        #region Override functions

        public override void RetrieveRenderKnowledge()
        {
            var symbols = _shapeSymbol.RetrieveConcreteShapes();
            var shapes = new ObservableCollection<IKnowledge>();
            RenderKnowledge = null;
            if (symbols != null)
            {
                var shapeSymbol = symbols as ShapeSymbol;
                var ssLst       = symbols as IEnumerable<ShapeSymbol>;

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
                var traces = _shapeSymbol.Shape.Traces;
                if (traces.Count == 0) return;
                var lst = new List<TraceStepExpr>();
                TraceStepExpr tse;
                for (int i = 0; i < traces.Count; i++)
                {
                    var ts = traces[i];
                    tse = new TraceStepExpr(ts);
                    lst.Add(tse);
                }
                AutoTrace = lst;
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

        #endregion
    }
}

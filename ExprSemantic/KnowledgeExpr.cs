using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ExprSemantic;
using ExprSemantic.KnowledgeBase;
using ExprSemantic.KnowledgeUnification;

namespace AGSemantic.KnowledgeBase
{
    using starPadSDK.MathExpr;

    public abstract class IKnowledgeExpr
    {
        public Expr GeneralExpr { get; set; }

        public List<AGKnowledgeTracer> Tracers;
        private int _traceIndex = 0;
        public int TraceIndex
        {
            get{return _traceIndex;}
            set 
            {
                _traceIndex = value; 
            }
        }
        public string RetrieveCurrWhyHint(Expr currExpr)
        {
            int index = Tracers.FindIndex(x => x.Source.Equals(currExpr));

            if (index != -1)
            {
                TraceIndex = index;
                if (index + 1 < Tracers.Count)
                {
                    AGKnowledgeTracer tracer = Tracers[index+1];
                    return tracer.WhyHints;                    
                }
                else
                {
                    return null;
                }
            }
            return null;
        }
        public string RetrieveCurrStrategyHint(Expr currExpr)
        {
            int index = Tracers.FindIndex(x => x.Source.Equals(currExpr));

            if (index != -1)
            {
                TraceIndex = index;
                AGKnowledgeTracer tracer = Tracers[index];
                return tracer.StrategyHints;
            }

            if (currExpr.Equals(Tracers[Tracers.Count - 1].Target))
            {
                TraceIndex = Tracers.Count - 1;
                AGKnowledgeTracer tracer = Tracers[TraceIndex];
                return tracer.StrategyHints;
            }

            return null;
        }
        public Expr RetrieveCurrExpr()
        {
            return Tracers[TraceIndex].Source;
        }
        public Expr RetrieveHow(Expr currExpr, out string ruleApplied, out bool isLastTrace)
        {
            isLastTrace = false;
            int index = Tracers.FindIndex(x => x.Source.Equals(currExpr));

            bool lastOne = false;
            lastOne = currExpr.Equals(Tracers[Tracers.Count - 1].Target);

            if (index != -1 )
            {
                TraceIndex = index;
                isLastTrace = (index == (Tracers.Count - 1));
                AGKnowledgeTracer tracer = Tracers[index];
                ruleApplied = tracer.AppliedRule;             
                return tracer.Target;
            }

            if (lastOne)
            {
                isLastTrace = true;
                AGKnowledgeTracer tracer = Tracers[Tracers.Count - 1];
                ruleApplied = tracer.AppliedRule;
                return tracer.Target;
            }


            ruleApplied = null;
            return null;
        }
    }

    public abstract class ShapeExpr : IKnowledgeExpr
    {
        //True:  geometry input
        //False: algebra  input
        public bool GeometryInput { get; set; }

        protected ShapeExpr(Shape shape)
        {
            AGShape = shape;
            GeneralExpr = AGShapeUtils.GenerateShapeGeneralForm(AGShape);
        }

        protected ShapeExpr(Shape shape, Expr _expr)
        {
            AGShape = shape;
            GeneralExpr = _expr;
        }
        
        public Shape AGShape { get; set; }

        #region IEquatable

        public override bool Equals(object obj)
        {
            if (obj is ShapeExpr)
            {
                var otherShape = obj as ShapeExpr;
                return this.AGShape.Equals(otherShape.AGShape);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return GeneralExpr.GetHashCode() ^ AGShape.GetHashCode();
        }

        #endregion
    }

    public sealed class PointExpr : ShapeExpr
    {
        public PointExpr(Point point) : base(point)
        {
            
        }

        public PointExpr(Point point, Expr expr)
            :base(point, expr)
        {
                        
        }
    }

    public sealed class LineExpr : ShapeExpr
    {
        public LineExpr(Line shape)
            :base(shape)
        {
            //shape.
        }

        public LineExpr(Line shape, Expr expr)
            :base(shape, expr)
        {

        }

        #region What

        public Expr DrawLineExpr
        {
            get
            {
                return new CompositeExpr(new WordSym("l:"), new Expr[]
                {
                    LineSlopeInterceptFormExpr
                });
            }
        }

        public Expr TwoInterceptsExpr
        {
            get
            {
                var line = AGShape as Line;

                return new CompositeExpr(new WordSym(""), new Expr[]
                {
                    new CompositeExpr(new WordSym(line.XIntercept.Label), new Expr[]
                    {   
                        Text.Convert(line.XIntercept.SymXCoordinate),
                        Text.Convert(line.XIntercept.SymYCoordinate),

                        //new IntegerNumber(0), 
                        //new IntegerNumber(-line.C / line.A), 
                    }), 
                    new CompositeExpr(new WordSym(line.YIntercept.Label), new Expr[]
                    {
                        Text.Convert(line.YIntercept.SymXCoordinate),
                        Text.Convert(line.YIntercept.SymYCoordinate),
                    }), 
                });
                
            }
        }

        public Expr LineSlopeExpr
        {
            get
            {
                var line = AGShape as Line;
                return starPadSDK.MathExpr.Text.Convert(line.SlopeTrace2);                
            }
        }

        public Expr LineInterceptExpr
        {
            get
            {
                var line = AGShape as Line;
                return starPadSDK.MathExpr.Text.Convert(line.InterceptTrace2);
            }            
        }

        public Expr LineSlopeInterceptFormExpr
        {
            get 
            { 
                var line = AGShape as Line;
                return starPadSDK.MathExpr.Text.Convert(line.LineSlopeInterceptForm);
            }
        }

        public Expr LinePointSlopeFormExpr
        {
            get
            {
                var line = AGShape as Line;
                Expr trace30 = starPadSDK.MathExpr.Text.Convert(line.LinePointSlopeForm);
                Expr trace3 = new CompositeExpr(new WordSym("P:"), new Expr[]
                {
                    trace30
                });
                return trace3;
            }
        }

        public Expr LineABCExpr
        {
            get
            {
                var line = AGShape as Line;
                Expr a = starPadSDK.MathExpr.Text.Convert(line.SymAProperty);
                Expr b = starPadSDK.MathExpr.Text.Convert(line.SymBProperty);
                Expr c = starPadSDK.MathExpr.Text.Convert(line.SymCProperty);

                return new CompositeExpr(new WordSym("Line:"), new Expr[]
                {
                    a,b,c
                });
            }
        }

        #endregion

        #region How and Why

        public List<AGKnowledgeTracer> LineSlopeTrace
        {
            get
            {
                var line = AGShape as Line;
                Expr slopeTracer1 = starPadSDK.MathExpr.Text.Convert(line.SlopeTrace1);
                Expr slopeTracer2 = starPadSDK.MathExpr.Text.Convert(line.SlopeTrace2);
                var lst = new List<AGKnowledgeTracer>()
                {
                    new AGKnowledgeTracer(GeneralExpr, LineABCExpr, "Hello World", AGStrategyHint.LineSlopeStrategy, AGAppliedRule.ConceptUnderstanding),
                    new AGKnowledgeTracer(LineABCExpr, slopeTracer2, AGKnowledgeHints.LineCoefficientHint, AGStrategyHint.LineSlopeStrategy, AGAppliedRule.LineSlopeFromABC),
                    new AGKnowledgeTracer(slopeTracer2, null, AGKnowledgeHints.LineSlopeHint, AGStrategyHint.LineSlopeStrategy, AGAppliedRule.LineSlopeFromABC)
                };
                return lst;
            }
        }

        public List<AGKnowledgeTracer> LinePointSlopeFormTrace
        {
            get
            {
                var line = AGShape as Line;

                Expr trace1 = starPadSDK.MathExpr.Text.Convert(line.SlopeTrace2);
                //Expr trace2 = starPadSDK.MathExpr.Text.Convert(line.YIntercept.SymPoint);

                Expr trace2 = new CompositeExpr(new WordSym(line.YIntercept.Label), new Expr[]
                {
                    starPadSDK.MathExpr.Text.Convert(line.YIntercept.SymXCoordinate),
                    starPadSDK.MathExpr.Text.Convert(line.YIntercept.SymYCoordinate)
                });

                Expr trace30 = starPadSDK.MathExpr.Text.Convert(line.LinePointSlopeForm);
                Expr trace3 = new CompositeExpr(new WordSym("P:"), new Expr[]
                {
                    trace30
                });

                var lst = new List<AGKnowledgeTracer>()
                {
                    new AGKnowledgeTracer(GeneralExpr, trace1, AGStrategyHint.QueryLinePointSlopeFormStrategy, AGStrategyHint.QueryLinePointSlopeFormStrategy, AGAppliedRule.LineSlopeFromABC),
                    new AGKnowledgeTracer(trace1, trace2, AGKnowledgeHints.LineSlopeHint, AGStrategyHint.QueryLinePointSlopeFormStrategy, AGAppliedRule.CalculateYIntercept),
                    new AGKnowledgeTracer(trace2, trace3, AGKnowledgeHints.FindlineYInterceptPoint, AGStrategyHint.QueryLinePointSlopeFormStrategy, AGAppliedRule.FitYInterceptIntoPointSlopeForm),
                    new AGKnowledgeTracer(trace3, null, AGKnowledgeHints.FitYInterceptIntoPointSlopeForm, AGStrategyHint.QueryLinePointSlopeFormStrategy, ""),                    
                };
                return lst;
            }
        }

//        public List<AGKnowledgeTracer>  
/*
        public List<AGKnowledgeTracer> LineABCPropertyTrace
        {
            get
            {
                var line = AGShape as Line;
                var lst = new List<AGKnowledgeTracer>()
                {
                    new AGKnowledgeTracer(GeneralExpr,
                        LineABCExpr,
                        AGKnowledgeHints.LineCoefficientHint)
                };
                return lst;
            }
        }
*/

/*
        public List<AGKnowledgeTracer> LineInterceptTrace
        {
            get
            {
                var line = AGShape as Line;
                Expr interceptTracer1 = starPadSDK.MathExpr.Text.Convert(line.InterceptTrace1);
                Expr interceptTracer2 = starPadSDK.MathExpr.Text.Convert(line.InterceptTrace2);
                var lst = new List<AGKnowledgeTracer>()
                {
                    new AGKnowledgeTracer(GeneralExpr,LineABCExpr, AGKnowledgeHints.LineCoefficientHint),
                    new AGKnowledgeTracer(LineABCExpr, interceptTracer1, AGKnowledgeHints.LineSlopeHint),
                    new AGKnowledgeTracer(LineABCExpr, interceptTracer2, AGKnowledgeHints.LineSlopeHint),
                };
                return lst;
            }
        }
*/
       

        #endregion
    }

    public abstract class QuadraticCurveExpr : ShapeExpr
    {
        protected QuadraticCurveExpr(Shape shape)
            :base(shape)
        {            

        }

        protected QuadraticCurveExpr(Shape shape, Expr expr)
            :base(shape, expr)
        {

        }
    }

    public sealed class CircleExpr : QuadraticCurveExpr
    {
        public CircleExpr(Circle shape, Expr expr)
            :base(shape, expr)
        {

        }

        public CircleExpr(Circle shape)
            : base(shape)
        {
            
        }

        #region What

        public Expr CircleCenterPtExpr
        {
            get
            {
                var circle = AGShape as Circle;
                return starPadSDK.MathExpr.Text.Convert(circle.CentralPt.SymPoint);               
            }
        }

        public Expr CircleRadiusExpr
        {
            get
            {
                var circle = AGShape as Circle;
                return starPadSDK.MathExpr.Text.Convert(circle.SymRadius);                
            }
        }

        public Expr CircleStandardFormExpr
        {
            get
            {
                var circle = AGShape as Circle;
                return starPadSDK.MathExpr.Text.Convert(circle.CircleStandardForm);                
            }
        }

        #endregion

        #region How and Why

        /*
        public List<AGKnowledgeTracer> CircleCentralPtTrace
        {
            get
            {
                var lst = new List<AGKnowledgeTracer>()
                {
                   new AGKnowledgeTracer(GeneralExpr, CircleCenterPtExpr, AGKnowledgeHints.CircleCentralPtHint)
                };
                return lst;
            }
        }

        public List<AGKnowledgeTracer> CircleRadiusTrace
        {
            get
            {
                var lst = new List<AGKnowledgeTracer>()
                {
                   new AGKnowledgeTracer(GeneralExpr, CircleRadiusExpr, AGKnowledgeHints.CircleRadiusHint)
                };
                return lst;                
            }
        }

        public List<AGKnowledgeTracer> CircleStandardFormTrace
        {
            get
            {
                var circle = AGShape as Circle;
                return AGUnifier.Instance.TransformCircleFromGeneralFormToStandardForm(this);
            }
        }
*/
        #endregion
    }

    public sealed class EllipseExpr : QuadraticCurveExpr
    {
        public EllipseExpr(Ellipse shape)
            : base(shape)
        {
            
        }

        public EllipseExpr(Ellipse shape, Expr expr)
            : base(shape, expr)
        {
            
        }

        #region What
/*
        public Expr EllipseCenterPtExpr
        {
            get
            {
                var ellipse = AGShape as Ellipse;

                var expr = new CompositeExpr(new WordSym("Ellipse CentralPoint:"), new Expr[]
                {
                    new WordSym(ellipse.CentralPt.SymXCoordinate),
                    new WordSym(ellipse.CentralPt.SymYCoordinate),
                });

                //return starPadSDK.MathExpr.Text.Convert(ellipse.CentralPt.SymPoint);
                return expr;
            }
        }

        public Expr EllipseRadiusAExpr
        {
            get
            {
                var ellipse = AGShape as Ellipse;
                return starPadSDK.MathExpr.Text.Convert(ellipse.SymRadiusAProperty);                
            }
        }

        public Expr EllipseRadiusBExpr 
        {
            get
            {
                var ellipse = AGShape as Ellipse;
                return starPadSDK.MathExpr.Text.Convert(ellipse.SymRadiusBProperty);                
            }
        }

        public Expr EllipseFociCExpr
        {
            get
            {
                var ellipse = AGShape as Ellipse;
                return starPadSDK.MathExpr.Text.Convert(ellipse.SymFociCProperty);                
            }
        }

        public Expr EllipseFociPtExpr
        {
            get
            {
                var ellipse = AGShape as Ellipse;
                Expr leftFoci  = starPadSDK.MathExpr.Text.Convert(ellipse.FocalPoint1);
                Expr rightFoci = starPadSDK.MathExpr.Text.Convert(ellipse.FocalPoint2);
                return new CompositeExpr(new WordSym("FPs:"), new Expr[]
                {
                    leftFoci,rightFoci
                });               
            }
        }

        public Expr EllipseStandardFormExpr
        {
            get 
            { 
                var ellipse = AGShape as Ellipse;
                return starPadSDK.MathExpr.Text.Convert(ellipse.EllipseStandardForm);
            }
        }
*/
        #endregion

        #region How and why
/*
        public List<AGKnowledgeTracer> EllipseCentralPtTrace
        {
            get
            {
                var lst = new List<AGKnowledgeTracer>()
                {
                   new AGKnowledgeTracer(GeneralExpr, EllipseCenterPtExpr, AGKnowledgeHints.EllipseCenterHint)
                };
                return lst;
            }
        }

        public List<AGKnowledgeTracer> EllipseRadiusTrace
        {
            get
            {
                var composite = new CompositeExpr(new WordSym("Ellipse Radius:"), new Expr[]
                {
                    EllipseRadiusAExpr, EllipseRadiusBExpr
                });

                var lst = new List<AGKnowledgeTracer>()
                {
                   new AGKnowledgeTracer(GeneralExpr, composite, AGKnowledgeHints.EllipseRadiusHint)
                };
                return lst;
            }
        }

        public List<AGKnowledgeTracer> EllipseFociTrace
        {
            get
            {
                var ellipse = AGShape as Ellipse;
                var composite = new CompositeExpr(new WordSym("Ellipse Radius:"), new Expr[]
                {
                    EllipseRadiusAExpr, EllipseRadiusBExpr
                });

                Expr fociTraceExpr1 = starPadSDK.MathExpr.Text.Convert(ellipse.FociTrace1);
                Expr fociTraceExpr2 = starPadSDK.MathExpr.Text.Convert(ellipse.FociTrace2);

                var lst = new List<AGKnowledgeTracer>()
                {
                    new AGKnowledgeTracer(GeneralExpr, EllipseCenterPtExpr, AGKnowledgeHints.EllipseCenterHint),
                    new AGKnowledgeTracer(GeneralExpr, composite, AGKnowledgeHints.EllipseRadiusHint),
                    new AGKnowledgeTracer(composite, fociTraceExpr1, AGKnowledgeHints.EllipseFociHint),
                    new AGKnowledgeTracer(fociTraceExpr1, fociTraceExpr2, AGKnowledgeHints.EllipseFociHint),
                    new AGKnowledgeTracer(fociTraceExpr2, EllipseFociCExpr, AGKnowledgeHints.EllipseFociHint),
                };

                return lst;
            }
        }

        public List<AGKnowledgeTracer> EllipseFociPtTrace
        {
            get
            {
                var ellipse = AGShape as Ellipse;
                var composite = new CompositeExpr(new WordSym("Ellipse Radius:"), new Expr[]
                {
                    EllipseRadiusAExpr, EllipseRadiusBExpr
                });

                Expr fociTraceExpr1 = starPadSDK.MathExpr.Text.Convert(ellipse.FociTrace1);
                Expr fociTraceExpr2 = starPadSDK.MathExpr.Text.Convert(ellipse.FociTrace2);

                var lst = new List<AGKnowledgeTracer>()
                {
                    new AGKnowledgeTracer(GeneralExpr, EllipseCenterPtExpr, AGKnowledgeHints.EllipseCenterHint),
                    new AGKnowledgeTracer(GeneralExpr, composite, AGKnowledgeHints.EllipseRadiusHint),
                    new AGKnowledgeTracer(composite, fociTraceExpr1, AGKnowledgeHints.EllipseFociHint),
                    new AGKnowledgeTracer(fociTraceExpr1, fociTraceExpr2, AGKnowledgeHints.EllipseFociHint),
                    new AGKnowledgeTracer(fociTraceExpr2, EllipseFociCExpr, AGKnowledgeHints.EllipseFociHint),
                    new AGKnowledgeTracer(EllipseFociCExpr, EllipseFociPtExpr, AGKnowledgeHints.EllipseFociPoint)
                };

                return lst;                                
            }
        }

        public List<AGKnowledgeTracer> EllipseStandardFormTrace
        {
            get
            {
                var ellipse = AGShape as Ellipse;
                return AGUnifier.Instance.TransformEllipseFromGeneralFormToStandardForm(this);
            }
        }
*/
        #endregion
    }

}
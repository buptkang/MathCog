using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using AGSemantic.KnowledgeBase;
using ExprSemantic.KnowledgeBase;
using ExprSemantic.KnowledgeRelation;
using starPadSDK.MathExpr;

namespace ExprSemantic.KnowledgeQueryEngine
{
    public static class AGKnowledgePropertyCalculator
    {
        public static List<AGKnowledgeTracer> CalculatePropertyTrace(this Shape shape, Expr source)
        {
            if (shape is Line)
            {
                //var line = shape as Line;
                //return line.CalculatePropertyTrace(source);
            }
            else if(shape is Circle)
            {
                var circle = shape as Circle;
                return circle.CalculatePropertyTrace(source);
            }
            else if (shape is Ellipse)
            {
                var ellipse = shape as Ellipse;
                return ellipse.CalculatePropertyTrace(source);
            }

            return null;
        }


        private static List<AGKnowledgeTracer> CalculatePropertyTrace(this Line line, Expr source)
        {
/*            var tracers = new List<AGKnowledgeTracer>();
           
            //Here line.A line.B line.C is known variables
            Expr target = AGShapeUtils.GenerateLineGeneralForm(line);
            var tracer = new AGKnowledgeTracer(source, target, AGKnowledgeHints.LineGeneralFormHint);
            tracers.Add(tracer);

            source = target;
            target = AGShapeUtils.GenerateThreeCoefficientLineExpr(line);
            tracer = new AGKnowledgeTracer(source, target, AGKnowledgeHints.LineGeneralFormHint);
            tracers.Add(tracer);

            source = target;
            target = AGShapeUtils.GenerateLineTrace3(line);
            tracer = new AGKnowledgeTracer(source, target, AGKnowledgeHints.LineSlopeHint);
            tracers.Add(tracer);

            source = target;
            target = AGShapeUtils.GenerateLineTrace4(line);
            tracer = new AGKnowledgeTracer(source, target, AGKnowledgeHints.LineSlopeHint);
            tracers.Add(tracer);

            source = target;
            target = AGShapeUtils.GenerateLineTrace5(line);
            tracer = new AGKnowledgeTracer(source, target, AGKnowledgeHints.LineYInterceptHints);
            tracers.Add(tracer);

            source = target;
            target = AGShapeUtils.GenerateLineTrace6(line);
            tracer = new AGKnowledgeTracer(source, target, AGKnowledgeHints.LineYInterceptHints);
            tracers.Add(tracer);

            return tracers;
 */ 
            return null;
        }

        private static List<AGKnowledgeTracer> CalculatePropertyTrace(this Circle circle, Expr source)
        {
/*
            var tracers = new List<AGKnowledgeTracer>();
            Expr target;

            target = AGShapeUtils.GenerateCircleGeneralForm(circle);
            var tracer = new AGKnowledgeTracer(source, target, AGKnowledgeHints.CircleStandardFormHint);
            tracers.Add(tracer);

            source = target;
            target = AGShapeUtils.GenerateCircleTrace2(circle);
            tracer = new AGKnowledgeTracer(source, target, AGKnowledgeHints.CircleRadiusHint);
            tracers.Add(tracer);
*/
            return null;
        }

        public static List<AGKnowledgeTracer> CalculatePropertyTrace(this PointLine pointLine)
        {
            var tracers = new List<AGKnowledgeTracer>();
            return tracers;
        }

        private static List<AGKnowledgeTracer> CalculatePropertyTrace(this Ellipse ellipse, Expr source)
        {
/*            
 *          var tracers = new List<AGKnowledgeTracer>();

            Expr target;

            target = AGShapeUtils.GenerateEllipseGeneralForm(ellipse);
            var tracer = new AGKnowledgeTracer(source, target, AGKnowledgeHints.EllipseStandarFormHint);
            tracers.Add(tracer);

            source = target;
            target = AGShapeUtils.GenerateEllipseTrace2(ellipse);
            tracer = new AGKnowledgeTracer(source, target, AGKnowledgeHints.EllipseCenterHint);
            tracers.Add(tracer);

            source = target;
            target = AGShapeUtils.GenerateEllipseTrace3(ellipse);
            tracer = new AGKnowledgeTracer(source, target, AGKnowledgeHints.EllipseFociHint);
            tracers.Add(tracer);

            source = target;
            target = AGShapeUtils.GenerateEllipseTrace4(ellipse);
            tracer = new AGKnowledgeTracer(source, target, AGKnowledgeHints.EllipseFociHint);
            tracers.Add(tracer);

            source = target;
            target = AGShapeUtils.GenerateEllipseTrace5(ellipse);
            tracer = new AGKnowledgeTracer(source, target, AGKnowledgeHints.EllipseFociHint);
            tracers.Add(tracer);

            source = target;
            target = AGShapeUtils.GenerateEllipseTrace6(ellipse);
            tracer = new AGKnowledgeTracer(source, target, AGKnowledgeHints.EllipseFociPoint);
            tracers.Add(tracer);

            return tracers;
 */

            return null;
        }



    }
}

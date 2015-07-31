using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using CSharpLogic;

namespace AlgebraGeometry
{
    public partial class Line : Shape
    {
        private void Calc_SlopeIntercept_General()
        {
            //slope and intercept known
            Debug.Assert(Slope != null);
            Debug.Assert(Intercept != null);
            A = Slope;
            B = -1;
            C = Intercept;
        }

        private void Calc_General_SlopeIntercept()
        {
            //A, B, C known ax+by+c=0
            //Slope     = (-1*a)/b
            //Intercept = (-1*c)/b
            /*            
             * Debug.Assert(A != null);
            Debug.Assert(B != null);*/
            Debug.Assert(C != null);

            if (B == null)
            {
                Slope = double.NaN;
                Intercept = double.NaN;
                return;
            }
            if (A == null)
            {
                //by+c=0
                Slope = 0.0d;
                var term31 = new Term(Expression.Multiply, new List<object>() { -1, C });
                var term41 = new Term(Expression.Divide, new List<object>() { term31, B });
                Intercept = term41.Eval();
                return;
            }

            var term1 = new Term(Expression.Multiply, new List<object>() {-1, A});
            var term2 = new Term(Expression.Divide, new List<object>() {term1, B});
            var term3 = new Term(Expression.Multiply, new List<object>() {-1, C});
            var term4 = new Term(Expression.Divide, new List<object>() {term3, B});
            Slope     = term2.Eval();
            Intercept = term4.Eval();
        }

        void Line_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "A":
                    break;
                case "B":
                    break;
                case "C":
                    break;
                case "Slope":
                    break;
                case "Intercept":
                    break;
            }
        }
    }

    public static class LineTransformExtension
    {
        public static object Unify(this LineSymbol ls, object constraint)
        {
            var refObj = constraint as string;
            Debug.Assert(refObj != null);

            switch (refObj)
            {
                case LineAcronym.A:
                case LineAcronym.A1:
                    break;
                case LineAcronym.B:
                case LineAcronym.B1:
                    break;
                case LineAcronym.C:
                case LineAcronym.C1:
                    break;
                case LineAcronym.Slope1:
                case LineAcronym.Slope2:
                case LineAcronym.Slope3:
                    return ls.InferSlope(refObj);
                case LineAcronym.Intercept1:
                case LineAcronym.Intercept2:
                case LineAcronym.Intercept3:
                    return true;
                case LineAcronym.GeneralForm1:
                case LineAcronym.GeneralForm2:
                case LineAcronym.GeneralForm3:
                case LineAcronym.GeneralForm4:
                    return ls.InferGeneralForm(refObj);
                case LineAcronym.SlopeInterceptForm1:
                case LineAcronym.SlopeInterceptForm2:
                    return ls.InferSlopeInterceptForm(refObj);
            }

            return null;
        }

        private static LineSymbol InferSlopeInterceptForm(this LineSymbol inputLineSymbol, string label)
        {
            //TODO
            var line = inputLineSymbol.Shape as Line;
            Debug.Assert(line != null);
            var ls = new LineSymbol(line);
            ls.OutputType = LineType.SlopeIntercept;
            return ls;
        }

        private static LineSymbol InferGeneralForm(this LineSymbol inputLineSymbol, string label)
        {
            var line = inputLineSymbol.Shape as Line;
            Debug.Assert(line != null);
            var ls = new LineSymbol(line);
            ls.OutputType = LineType.GeneralForm;
            return ls;
        }

        private static EqGoal InferSlope(this LineSymbol inputLineSymbol, string label)
        {
            var line = inputLineSymbol.Shape as Line;
            Debug.Assert(line != null);
            return new EqGoal(new Var(label), line.Slope);

            if (line.InputType == LineType.SlopeIntercept)
            {
                Debug.Assert(line.Slope != null);
/*                var newGoal = new EqGoal(egGoal.Lhs, line.Slope);
                egGoal.CachedEntities.Add(newGoal);*/
                //TODO
                
            }
            else if (line.InputType == LineType.GeneralForm)
            {
                #region Infer Procedure

                /*Debug.Assert(line.A != null);
                Debug.Assert(line.B != null);
                Debug.Assert(line.C != null);

                if (line.Concrete)
                {
                    double slope = (-1*(double)line.A)/(double)line.B;
                    var newGoal = new EqGoal(egGoal.Lhs, slope);
                    egGoal.CachedEntities.Add(newGoal);
                }
                else
                {
                    throw new Exception("TODO");
                    //ax+by+c=0
                    var term1 = new Term(Expression.Multiply, new List<object>() { -1, line.A });
                    var term2 = new Term(Expression.Divide, new List<object>() { term1, line.B });
                    object obj = term2.Eval();
                    if (LogicSharp.IsNumeric(obj))
                    {
                        var newGoal = new EqGoal(egGoal.Lhs, line.Slope);
                        egGoal.CachedEntities.Add(newGoal);
                        return;
                    }
                }*/
                #endregion
            }
            else if (line.InputType == LineType.Relation)
            {
              /*  var newGoal = new EqGoal(egGoal.Lhs, line.Slope);
                egGoal.CachedEntities.Add(newGoal);*/
            }
        }   
    }


    //Step 1: Move x and constant term to the right side of equation.
    //Step 2: Normalize the coefficient of y term.

    //TODO
    /*            LineSymbol siForm = ls.TransformTo(LineFormType.SlopeIntercept);
                Assert.NotNull(siForm);
                Assert.True(siForm);
                var line = siForm.Shape as Line;
                Assert.NotNull(line);*/


/*    	Label = label;
            Slope = (p1.YCoordinate - p2.YCoordinate) / (p1.XCoordinate - p2.YCoordinate);
            Intercept = p1.YCoordinate - Slope * p1.XCoordinate;
            YIntercept = new Point("Y-Intercept", 0.0, Intercept);

            A = Slope;
            B = -1;
            C = Intercept;*/

    //        public string SlopeTrace1
    //        {
    //            get
    //            {
    //                if (A * B > 0 )
    //                {
    //                    return String.Format("m = - {0} / {1}", SymA, SymB);
    //                }
    //                else
    //                {
    //                    return String.Format("m = {0} / {1}", SymA, NegSymB);                        
    //                }
    //            }
    //        }

    //        public string SlopeTrace2
    //        {
    //            get
    //            {
    //                return String.Format("m = {0}", SymSlope);
    //            }
    //        }

    //        public string InterceptTrace1
    //        {
    //            get
    //            {
    //                if (B > 0 && C > 0)
    //                {
    //                    return String.Format("I = - {0} / {1}", SymC, SymB);
    //                }
    //                else if (C > 0 && B < 0)
    //                {
    //                    return String.Format("I = {0} / {1}", SymC, NegSymB);
    //                }
    //                else if (C < 0 && B > 0)
    //                {
    //                    return String.Format("I = {0} / {1}", NegSymC, SymB);
    //                }

    //                return null;               
    //            }
    //        }

    //        public string InterceptTrace2
    //        {
    //            get
    //            {
    //                return String.Format("I = {0}", SymIntercept);
    //            }
    //        }

     

    /*public static LineSymbol TransformTo(this LineSymbol ls, LineFormType type)
        {
            if (ls.GeneralFormInput)
            {
                if (type == LineFormType.General)
                {
                    return ls;
                }
                else if (type == LineFormType.SlopeIntercept)
                {
                    //General -> slope-intercept
                                        
                }
                else
                {
                    throw new Exception("TODO");
                }
            }
            else if (ls.SlopeInterceptInput)
            {
                if (type == LineFormType.General)
                {
                    return ls;
                }
                else if (type == LineFormType.SlopeIntercept)
                {
                    return ls;
                }
                else
                {
                    throw new Exception("TODO");
                }
            }
            else
            {
                throw new Exception("TODO");
            }

            return null;
        }*/
}

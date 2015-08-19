using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using CSharpLogic;
using System.ComponentModel;
using System.Linq.Expressions;

namespace AlgebraGeometry
{
    public partial class Line : Shape
    {
        #region Properties and Constructors

        #region Properties
        private object _a;
        public object A
        {
            get
            {
                return _a;
            }
            set
            {
                _a = value;
                NotifyPropertyChanged("A");
            }
        }

        private object _b;
        public object B
        {
            get
            {
                return _b;
            }
            set
            {
                _b = value;
                NotifyPropertyChanged("B");
            }
        }

        private object _c;
        public object C
        {
            get
            {
                return _c;
            }
            set
            {
                _c = value;
                NotifyPropertyChanged("C");
            }
        }

        private object _slope;
        public object Slope
        {
            get
            {
                return _slope;
            }
            set
            {
                _slope = value;
                NotifyPropertyChanged("Slope");
            }
        }
        private object _intercept;
        public object Intercept
        {
            get { return _intercept; }
            set
            {
                _intercept = value;
                NotifyPropertyChanged("Intercept");
            }
        }

        /*
                public Point XIntercept { get; set; }
                public Point YIntercept { get; set; }
        */

        public LineType InputType { get; set; }
        public override object GetInputType() { return InputType; } 

        #endregion

        #region Entity Constructors

        public Line(string label, object a, object b, object c)
            : base(ShapeType.Line, label)
        {
            InputType = LineType.GeneralForm;
            _a = a;
            _b = b;
            _c = c;

            double d;
            if (LogicSharp.IsDouble(a, out d))
            {
                _a = Math.Round(d, 1);
            }

            if (LogicSharp.IsDouble(b, out d))
            {
                _b = Math.Round(d, 1);
            }

            if (LogicSharp.IsDouble(c, out d))
            {
                _c = Math.Round(d, 1);
            }

            if (_a is string)
            {
                _a = new Var(_a);
            }
            if (_b is string)
            {
                _b = new Var(_b);
            }
            if (_c is string)
            {
                _c = new Var(_c);
            }

            if (_c == null)
            {
                _c = 0.0d;
            }
            Calc_General_SlopeIntercept();
            PropertyChanged += Line_PropertyChanged;
        }

        public Line(object a, object b, object c): 
            this(null, a,b,c)
        {
        }

        public Line(object slope, object intercept)
            : this(null, slope, intercept)
        {
        }

        public Line(string label, object slope, object intercept)
            : base(ShapeType.Line, label)
        {
            InputType = LineType.SlopeIntercept;

            _slope = slope;
            _intercept = intercept;

            double d;
            if (LogicSharp.IsDouble(_slope, out d))
            {
                _slope = Math.Round(d, 1);
            }
            if (LogicSharp.IsDouble(_intercept, out d))
            {
                _intercept = Math.Round(d, 1);
            }

            if (_slope is string)
            {
                _slope = new Var(_slope);
            }
            if (_intercept is string)
            {
                _intercept = new Var(_intercept);
            }
            if (_intercept == null)
            {
                _intercept = 0.0d;
            }

            Calc_SlopeIntercept_General();
            PropertyChanged += Line_PropertyChanged;
        }

        #endregion

        #region Relation based
        // relation based line
        public Line(string label)
            : base(ShapeType.Line, label)
        {
            InputType = LineType.Relation;
        }

        public Line(Point p1, Point p2)
        {
            InputType = LineType.Relation;
        }

        public Line(string label, Point p1, Point p2)
        {
            InputType = LineType.Relation;
        }

        #endregion

        #endregion

        #region Utils

        public override bool Concrete
        {
            get
            {
                return !Var.ContainsVar(A) &&
                       !Var.ContainsVar(B) &&
                       !Var.ContainsVar(C);
            }
        }

        public override List<Var> GetVars()
        {
            var lst = new List<Var>();
            lst.Add(Var.GetVar(A));
            lst.Add(Var.GetVar(B));
            lst.Add(Var.GetVar(C));
            return lst;
        }

        #endregion

        #region IEqutable

        public override bool Equals(object obj)
        {
            var shape = obj as Shape;
            if (shape != null)
            {
                return Equals(shape);
            }
            return false;
        }

        public override bool Equals(Shape other)
        {
            if (other == null) return false;
            if (other is Line)
            {
                var line = other as Line;
                bool equalA = A == null || A.Equals(line.A);
                bool equalB = B == null || B.Equals(line.B);
                bool equalC = C == null || C.Equals(line.C);
                if (!(equalA || equalB || equalC)) return false;
            }
            return base.Equals(other);
        }

        public override int GetHashCode()
        {
            if (A == null)
            {
                return B.GetHashCode() ^ C.GetHashCode();
            }

            if (B == null)
            {
                return A.GetHashCode() ^ C.GetHashCode();
            }

            return A.GetHashCode() ^ B.GetHashCode()
                   ^ C.GetHashCode();
        }

        #endregion       

        #region Transformations

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

            var term1 = new Term(Expression.Multiply, new List<object>() { -1, A });
            var term2 = new Term(Expression.Divide, new List<object>() { term1, B });
            var term3 = new Term(Expression.Multiply, new List<object>() { -1, C });
            var term4 = new Term(Expression.Divide, new List<object>() { term3, B });
            Slope = term2.Eval();
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



        #endregion
    }

    public enum LineType
    {
        Relation,
        GeneralForm,
        SlopeIntercept,
        PointSlope
    }

    public partial class LineSymbol : ShapeSymbol
    {
        //IEnumerable<ShapeSymbol>
        public override object RetrieveConcreteShapes()
        {
            var line = Shape as Line;
            Debug.Assert(line != null);
            //if (line.Concrete) return this;
            if (CachedSymbols.Count == 0) return this;
            return CachedSymbols.ToList();
        }

        public override bool UnifyProperty(string label, out object obj)
        {
            return this.Unify(label, out obj);
        }

        public LineSymbol(Line line) : base(line)
        {
            OutputType = line.InputType;
        }

        #region Symbolic Elements

        public string SymA
        {
            get
            {
                var line = Shape as Line;
                Debug.Assert(line!=null);
                return line.A != null ? line.A.ToString() : null;
            }
        }

        public string SymB
        {
            get
            {
                var line = Shape as Line;
                Debug.Assert(line != null);
                return line.B != null ? line.B.ToString() : null;
            }
        }

        private string NegSymB
        {
            get
            {
                var line = Shape as Line;
                Debug.Assert(line != null);
                if (LogicSharp.IsNumeric(line.B))
                {
                    double d;
                    LogicSharp.IsDouble(line.B, out d);
                    double negB = -d;
                    if ((negB % 1).Equals(0))
                    {
                        return Int32.Parse(negB.ToString(CultureInfo.InvariantCulture))
                            .ToString(CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        return negB.ToString(CultureInfo.InvariantCulture);
                    }
                }
                else
                {
                    return line.B.ToString();
                }
            }
        }

        public string SymC
        {
            get
            {
                var line = Shape as Line;
                Debug.Assert(line != null);
                return line.C != null ? line.C.ToString() : null;
            }
        }

        public string NegSymC
        {
            get
            {
                var line = Shape as Line;
                Debug.Assert(line != null);
                if (LogicSharp.IsNumeric(line.C))
                {
                    double d;
                    LogicSharp.IsDouble(line.C, out d);
                    double negC = -d;
                    if ((negC % 1).Equals(0))
                    {
                        return Int32.Parse(negC.ToString(CultureInfo.InvariantCulture))
                            .ToString(CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        return negC.ToString(CultureInfo.InvariantCulture);
                    }
                }
                else
                {
                    return line.C.ToString();
                }
            }
        }

        //ax
        public string TermX
        {
            get 
            { 
                var line = Shape as Line;
                Debug.Assert(line != null);
                if (LogicSharp.IsNumeric(line.A))
                {
                    double d;
                    LogicSharp.IsDouble(line.A, out d);

                    //d = 0, d = -1, d= 1, others

                    double dd = Math.Abs(d);

                    if (dd - 0.0 < 0.0000001) //d = 0
                    {
                        return "";
                    }
                    else if (dd - 1.0 < 0.0000001) //d = 1 or -1
                    {
                        if (d < 0)
                        {
                            return string.Format("-x");
                        }
                        else
                        {
                            return string.Format("x");
                        }
                        
                    }
                    else
                    {
                        return string.Format("{0}x", SymA);
                    }
                }
                else
                {
                    return string.Format("{0}x", SymA);                     
                }
            }
        }

        //by
        public string TermY
        {
            get
            {
                var line = Shape as Line;
                Debug.Assert(line != null);
                if (LogicSharp.IsNumeric(line.B))
                {
                    #region Numerics

                    double d;
                    LogicSharp.IsDouble(line.B, out d);

                    double dd = Math.Abs(d);
                    if(dd - 0.0 < 0.0000001) return "";

                    if (d > 0.0)
                    {
                        if (d - 1.0 < 0.0001)
                        {
                            return (SymA == null || SymA.Equals("0")) ? 
                                string.Format("y") : 
                                string.Format("+y");
                        }
                        else
                        {
                            return (SymA == null || SymA.Equals("0")) ?
                                string.Format("{0}y", SymB) :
                                string.Format("+{0}y", SymB);
                        }                        
                    }
                    else
                    {
                        if (dd - 1.0 < 0.0001)
                        {
                            return (SymA == null || SymA.Equals("0")) ?
                               string.Format("y") :
                               string.Format("-y");
                        }
                        else
                        {
                            return (SymA == null || SymA.Equals("0")) ?
                               string.Format("{0}y", NegSymB) :
                               string.Format("-{0}y", NegSymB);
                        }
                    }

                    #endregion
                }
                else
                {
                    return (SymA == null || SymA.Equals("0")) ?
                              string.Format("{0}y", SymB) :
                              string.Format("+{0}y", SymB);
                }
            }            
        }

        //c
        public string TermC
        {
            get
            {
                var line = Shape as Line;
                Debug.Assert(line != null);
                if (LogicSharp.IsNumeric(line.C))
                {
                    #region Numerics 

                    double d;
                    LogicSharp.IsDouble(line.C, out d);

                    double dd = Math.Abs(d);

                    if (dd - 0.0 < 0.0000001)
                    {
                        return "";
                    }

                    if (d > 0.0)
                    {
                        return string.Format("+{0}", SymC);
                    }
                    else
                    {
                        return string.Format("-{0}", NegSymC);
                    }

                    #endregion
                }
                else
                {
                    return string.Format("+{0}", SymC);
                }                
            }
        }

        //ax+by+c=0
        public string GeneralForm
        {
            get
            {
                if(SymA == null)
                    return string.Format("{0}{1}=0", TermY, TermC);

                if(SymB == null)
                    return string.Format("{0}{1}=0", TermX, TermC);

                return string.Format("{0}{1}{2}=0", TermX, TermY, TermC);
            }
        }

        #endregion

        #region Line Format

        public string SymSlope
        {
            get
            {
                var line = Shape as Line;
                Debug.Assert(line != null);
                return line.Slope != null ? line.Slope.ToString() : null;
            }
        }

        public string TermSlope
        {
            get
            {
                var line = Shape as Line;
                Debug.Assert(line != null);
                if (LogicSharp.IsNumeric(line.Slope))
                {
                    double d;
                    LogicSharp.IsDouble(line.Slope, out d);

                    //d = 0, d = -1, d= 1, others

                    double dd = Math.Abs(d);

                    if (dd - 0.0 < 0.0000001) //d = 0
                    {
                        return "";
                    }
                    else if (Math.Abs(dd - 1.0) < 0.0000001) //d = 1 or -1
                    {
                        if (d < 0)
                        {
                            return string.Format("-x");
                        }
                        else
                        {
                            return string.Format("x");
                        }

                    }
                    else
                    {
                        return string.Format("{0}x", SymSlope);
                    }
                }
                else
                {
                    return string.Format("{0}x", SymSlope);
                }
            }
        }

        public string SymIntercept
        {
            get
            {
                var line = Shape as Line;
                Debug.Assert(line != null);
                return line.Intercept != null ? line.Intercept.ToString() : null;
            }
        }

        private string NegSymIntercept
        {
            get
            {
                var line = Shape as Line;
                Debug.Assert(line != null);
                if (LogicSharp.IsNumeric(line.Intercept))
                {
                    double d;
                    LogicSharp.IsDouble(line.Intercept, out d);
                    double negC = -d;
                    if ((negC % 1).Equals(0))
                    {
                        return Int32.Parse(negC.ToString(CultureInfo.InvariantCulture))
                            .ToString(CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        return negC.ToString(CultureInfo.InvariantCulture);
                    }
                }
                else
                {
                    return line.Intercept.ToString();
                }
            }
        }

        public string TermIntercept
        {
            get
            {
                var line = Shape as Line;
                Debug.Assert(line != null);
                if (LogicSharp.IsNumeric(line.Intercept))
                {
                    #region Numerics

                    double d;
                    LogicSharp.IsDouble(line.Intercept, out d);

                    double dd = Math.Abs(d);

                    if (dd - 0.0 < 0.0000001)
                    {
                        return "";
                    }

                    if (d > 0.0)
                    {
                        return string.Format("+{0}", SymIntercept);
                    }
                    else
                    {
                        return string.Format("-{0}", NegSymIntercept);
                    }

                    #endregion
                }
                else
                {
                    return string.Format("+{0}", SymIntercept);
                }
            }
        }

        public string SlopeInterceptForm
        {
            get
            {
                if (SymIntercept != null)
                {
                    return string.Format("y={0}{1}", TermSlope, TermIntercept);
                }
                else
                {
                    return string.Format("y={0}", TermSlope);
                }
            }
        }

        //        public string LinePointSlopeForm
        //        {
        //            get
        //            {
        //                if (Intercept.Equals(0d))
        //                {
        //                    return string.Format("y={0}x", SymSlope);
        //                }
        //                else
        //                {
        //                    return string.Format("y{0}={1}（x{2}）",
        //                        YIntercept.YCoordinate > 0d ? String.Format("- {0}", YIntercept.YCoordinate) : String.Format("+ {0}", YIntercept.NegSymYCoordinate), 
        //                        SymSlope,
        //                        YIntercept.XCoordinate > 0d ? String.Format("- {0}", YIntercept.XCoordinate) : String.Format("+ {0}", YIntercept.NegSymXCoordinate));
        //                }               
        //            }
        //        }

        #endregion

        #region Utils

        public LineType OutputType { get; set; }

        public override object GetOutputType() { return OutputType; }

        public override string ToString()
        {
            var line = this.Shape as Line;
            Debug.Assert(line != null);
            if (OutputType == LineType.SlopeIntercept)
            {
                return String.Format("{0}", SlopeInterceptForm);                
            }
            else
            {
                if (Shape.Label != null)
                {
                    return String.Format("{0}({1})", Shape.Label, GeneralForm);
                }
                else
                {
                    return String.Format("{0}", GeneralForm);
                }                
            }
        }

        #endregion
    }
}

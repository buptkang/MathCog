using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using CSharpLogic;

namespace AlgebraGeometry
{
    /// <summary>
    /// Augmenting concepts of line
    /// </summary>
    public partial class Line : Shape
    {
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
    }

    public partial class LineSymbol : ShapeSymbol
    {
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
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using CSharpLogic;

namespace AlgebraGeometry
{
    public partial class Line : Shape
    {
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

        public Line(string label, object a, object b, object c)
            : base(ShapeType.Line, label)
        {
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
        }

        public Line(object a, object b, object c): 
            this(null, a,b,c)
        {
        }

        public Line(string label) : base(ShapeType.Line, label)
        {
            RelationStatus = true;
        }

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

        #region IEqutable

        public override bool Equals(Shape other)
        {
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
    }

    public class LineSymbol : ShapeSymbol
    {
        public override IEnumerable<ShapeSymbol> RetrieveGeneratedShapes()
        {
            if (Shape.Concrete) return null;
            if (Shape.CachedSymbols.Count == 0) return null;
            var lst = new List<LineSymbol>();
            foreach (Shape s in Shape.CachedSymbols)
            {
                var line = s as Line;
                lst.Add(new LineSymbol(line));
            }
            return lst;
        }

        public LineSymbol(Line line) : base(line)
        {
        }

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
                return line.C.ToString();
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

        public override string ToString()
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
}

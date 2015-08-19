using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSharpLogic;

namespace AlgebraGeometry
{
    public partial class QuadraticCurve : Shape
    {
        public object A { get; set; } // coefficient for the X^2
        public object B { get; set; } // coefficient for the Y^2
        public object C { get; set; } // coefficient for the XY
        public object D { get; set; } // coefficient for the X
        public object E { get; set; } // coefficient for the Y
        public object F { get; set; } // coefficient for the constant

        public QuadraticCurve() { }

        public QuadraticCurve(string label, ShapeType shapeType, double a, double b, double c, double d, double e, double f)
            : base(shapeType, label)
        {
            /*
            A = Math.Round(a, 2);
            B = Math.Round(b, 2);
            C = Math.Round(c, 2);
            D = Math.Round(d, 2);
            E = Math.Round(e, 2);
            F = Math.Round(f, 2);
             */ 
        }

        #region IEquatable

        //        public override bool Equals(Shape other)
        //        {
        //            if (other is QuadraticCurve)
        //            {
        //                var qc = other as QuadraticCurve;
        //                if (!(A.Equals(qc.A) && B.Equals(qc.B) && C.Equals(qc.C) && D.Equals(qc.D) && E.Equals(qc.E) && F.Equals(qc.F)))
        //                {
        //                    return false;
        //                }
        //            }
        //            else
        //            {
        //                return false;
        //            }

        //            return base.Equals(other);
        //        }

        //        public override int GetHashCode()
        //        {
        //            return A.GetHashCode() ^ B.GetHashCode() ^ C.GetHashCode();
        //        }

        #endregion

        public QuadraticCurveType InputType { get; set; } 
        public override object GetInputType() { return InputType; }
    }

    public enum QuadraticCurveType
    {
        Relation
    }

    public class QuadraticCurveSymbol : ShapeSymbol
    {
        public QuadraticCurveSymbol(Shape _shape) : base(_shape)
        {
        }

        public override bool UnifyProperty(string label, out object obj)
        {
            throw new NotImplementedException();
        }

        public override object RetrieveConcreteShapes()
        {
            throw new NotImplementedException();
        }

        public QuadraticCurveType OutputType { get; set; }
        public override object GetOutputType() { return OutputType; }

        //        #region Symbolic Format

        //        public string SymA
        //        {
        //            get
        //            {
        //                if ((A % 1).Equals(0))
        //                {
        //                    return Int32.Parse(A.ToString()).ToString();
        //                }
        //                else
        //                {
        //                    return A.ToString();
        //                }
        //            }
        //        }

        //        public string SymB
        //        {
        //            get
        //            {
        //                if ((B % 1).Equals(0))
        //                {
        //                    return Int32.Parse(B.ToString()).ToString();
        //                }
        //                else
        //                {
        //                    return B.ToString();
        //                }
        //            }
        //        }

        //        public string NegSymB
        //        {
        //            get
        //            {
        //                double negB = -B;
        //                if ((negB % 1).Equals(0))
        //                {
        //                    return Int32.Parse(negB.ToString()).ToString();
        //                }
        //                else
        //                {
        //                    return negB.ToString();
        //                }
        //            }
        //        }

        //        public string SymC
        //        {
        //            get
        //            {
        //                if ((C % 1).Equals(0))
        //                {
        //                    return Int32.Parse(C.ToString()).ToString();
        //                }
        //                else
        //                {
        //                    return C.ToString();
        //                }
        //            }
        //        }

        //        public string SymD
        //        {
        //            get
        //            {
        //                if ((D % 1).Equals(0))
        //                {
        //                    return Int32.Parse(D.ToString()).ToString();
        //                }
        //                else
        //                {
        //                    return D.ToString();
        //                }
        //            }
        //        }

        //        public string NegSymD
        //        {
        //            get
        //            {
        //                double negD = -D;
        //                if ((negD % 1).Equals(0))
        //                {
        //                    return Int32.Parse(negD.ToString()).ToString();
        //                }
        //                else
        //                {
        //                    return negD.ToString();
        //                }                
        //            }
        //        }

        //        public string SymE
        //        {
        //            get
        //            {
        //                if ((E % 1).Equals(0))
        //                {
        //                    return Int32.Parse(E.ToString()).ToString();
        //                }
        //                else
        //                {
        //                    return E.ToString();
        //                }
        //            }
        //        }

        //        public string NegSymE
        //        {
        //            get
        //            {
        //                double negE = -E;
        //                if ((negE % 1).Equals(0))
        //                {
        //                    return Int32.Parse(negE.ToString()).ToString();
        //                }
        //                else
        //                {
        //                    return negE.ToString();
        //                }
        //            }            
        //        }

        //        public string SymF
        //        {
        //            get
        //            {
        //                if ((F % 1).Equals(0))
        //                {
        //                    return Int32.Parse(F.ToString()).ToString();
        //                }
        //                else
        //                {
        //                    return F.ToString();
        //                }
        //            }
        //        }

        //        public string NegSymF
        //        {
        //            get 
        //            { 
        //                double negF = -F;
        //                if ((negF % 1).Equals(0))
        //                {
        //                    return Int32.Parse(negF.ToString()).ToString();
        //                }
        //                else
        //                {
        //                    return negF.ToString();
        //                }
        //            }
        //        }

        //        #endregion
    }
}

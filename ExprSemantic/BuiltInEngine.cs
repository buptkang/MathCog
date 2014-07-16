using System;
using System.Collections;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using FSBigInt = System.Numerics.BigInteger;

namespace starPadSDK.MathExpr {

    public partial class BuiltInEngine : Engine {

        public override Expr _Simplify(Expr e)
        {
         //   Expr ret = Canonicalize(e);
         //   return Reformat(ret);
        }

        public override Expr _Approximate(Expr e)
        {
         //   Expr ret = Numericize(_Simplify(e));
         //   return Reformat(ret);
        }

    
        public override Expr _Approximate(Expr e)
        {

        }

        #region substitution

        public override Expr _Substitute(Expr e, Expr orig, Expr replacement)
        {
            throw new NotImplementedException();
        }

        public override Expr _Replace(Expr e, Expr orig, Expr replacement)
        {
            throw new NotImplementedException();
        }

        #endregion

        public override string Name
        {
            get { return "Built-in mathematics engine"; }
        }

        public override void Activate()
        {
            // TODO:  Add BuiltInEngine.Activate implementation
        }

        public override void Deactivate()
        {
            // TODO:  Add BuiltInEngine.Deactivate implementation
        }
    }
}
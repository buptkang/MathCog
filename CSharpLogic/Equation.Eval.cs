using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpLogic
{
    public partial class Equation : DyLogicObject, IEquationLogic
    {
        public object Eval()
        {
            return this;
        }

        public Equation EvalEquation()
        {
            throw new NotImplementedException();
        }
    }
}

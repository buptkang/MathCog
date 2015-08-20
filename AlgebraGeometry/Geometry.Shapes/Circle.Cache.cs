using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSharpLogic;

namespace AlgebraGeometry
{
    public partial class CircleSymbol : ShapeSymbol
    {
        public override void UndoGoal(EqGoal goal, object parent)
        {
            throw new NotImplementedException();
        }

        public override bool UnifyProperty(string label, out object obj)
        {
            throw new NotImplementedException();
        }
    }
}

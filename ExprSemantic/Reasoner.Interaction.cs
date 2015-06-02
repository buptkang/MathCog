using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using AlgebraGeometry.Expr;

namespace ExprSemantic
{
    public partial class Reasoner
    {
        #region Delegate for the interaction purpose

        public delegate void UpdateKnowledgeHandler(object sender, object args);

        public event UpdateKnowledgeHandler KnowledgeUpdated;

        #endregion
    }
}

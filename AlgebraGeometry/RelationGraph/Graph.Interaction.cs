using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlgebraGeometry
{
    public partial class RelationGraph
    {
        #region Delegate for the interaction purpose

        public delegate void UpdateKnowledgeHandler(object sender, object args);

        public event UpdateKnowledgeHandler KnowledgeUpdated;

        #endregion
    }
}

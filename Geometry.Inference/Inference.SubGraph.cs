using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlgebraGeometry;

namespace GeometryLogicInference
{
    public partial class GeometryInference
    {
        #region Graph Node Selections

        public void SelectNode(GraphNode gn)
        {
            if (!_relationGraph.Nodes.Contains(gn))
                throw new Exception("Root graph needs to contain select node!");

            _selectedNode.Add(gn);
        }

        public void DeSelectNode(GraphNode gn)
        {
            if (!_relationGraph.Nodes.Contains(gn))
            {
                throw new Exception("Root graph needs to contain select node!");
            }

            if (!_selectedNode.Contains(gn))
            {
                throw new Exception("The node should be selected before.");
            }

            _selectedNode.Remove(gn);
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlgebraGeometry
{
    public class GraphEdge
    {
        private GraphNode _node1;
        public GraphNode Source
        {
            get { return _node1; }
            set { _node1 = value; }
        }

        private GraphNode _node2;

        public GraphNode Target
        {
            get { return _node2; }
            set { _node2 = value; }
        }

        public GraphEdge(GraphNode _source, GraphNode _target)
        {
            _node1 = _source;
            _node2 = _target;
        }
    }
}

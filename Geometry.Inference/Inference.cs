using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlgebraGeometry;

namespace GeometryLogicInference
{
    public class GeometryInference
    {
        #region Singleton

        private static GeometryInference _instance;

        private GeometryInference()
        {
            
        }

        public static GeometryInference Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GeometryInference();
                }
                return _instance;
            }
        }

        private RelationGraph _relationGraph;
        public RelationGraph Graph
        {
            get { return _relationGraph; }
            set { _relationGraph = value;}
        }

        private List<GraphNode> _selectedNode;

        #endregion

        public void SelectNode(GraphNode gn)
        {
            if(!_relationGraph.Nodes.Contains(gn)) 
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

        public RelationGraph SelectedGraph
        {
            get
            {
                return new RelationGraph(_selectedNode);
            }
        }
    }
}

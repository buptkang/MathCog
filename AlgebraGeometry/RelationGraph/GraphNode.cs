using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using NUnit.Framework;

namespace AlgebraGeometry
{
    /// <summary>
    /// A GraphNode can be Shape or Goal
    /// </summary>
    public abstract class GraphNode : INotifyPropertyChanged
    {
        private List<GraphEdge> _inEdges;
        public List<GraphEdge> InEdges {
            get { return _inEdges; }
            set { _inEdges = value; } 
        }

        private List<GraphEdge> _outEdges;

        public List<GraphEdge> OutEdges
        {
            get { return _outEdges; }
            set { _outEdges = value; }
        }

        protected GraphNode()
        {
            _inEdges = new List<GraphEdge>();
            _outEdges = new List<GraphEdge>();
        }

        public bool IsolatedVertex { get { return _inEdges.Count == 0 && _outEdges.Count == 0; } }
        public bool SourceVertex { get { return _inEdges.Count == 0; } }
        public bool SinkVertex { get { return _outEdges.Count == 0; } }

        #region Dynamic System

        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property. 
        // The CallerMemberName attribute that is applied to the optional propertyName 
        // parameter causes the property name of the caller to be substituted as an argument. 
        protected void NotifyPropertyChanged(String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}

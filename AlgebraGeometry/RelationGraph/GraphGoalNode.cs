using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using CSharpLogic;

namespace AlgebraGeometry
{
    public class GoalNode : GraphNode
    {
        private Goal _goal;

        public Goal Goal
        {
            get { return _goal; }
            set { _goal = value; }
        }

        public GoalNode(Goal goal) :base()
        {
            _goal = goal;
        }
    }
}

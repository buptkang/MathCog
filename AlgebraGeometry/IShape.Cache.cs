﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSharpLogic;

namespace AlgebraGeometry
{
    public abstract partial class Shape : DyLogicObject, IEquatable<Shape>
    {
        //Cached symbols for non-concrete objects
        public HashSet<Shape> CachedSymbols { get; set; }
        public HashSet<KeyValuePair<object, EqGoal>> CachedGoals { get; set; }

        public bool ContainGoal(EqGoal goal)
        {
            return CachedGoals.Any(pair => pair.Value.Equals(goal));
        }

        public void RemoveGoal(EqGoal goal)
        {
            CachedGoals.RemoveWhere(pair => pair.Value.Equals(goal));
        }
    }
}
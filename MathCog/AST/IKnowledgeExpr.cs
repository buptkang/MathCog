/*******************************************************************************
 * Copyright (c) 2015 Bo Kang
 *   
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *  
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *******************************************************************************/

using System;
using System.Diagnostics;

namespace MathCog
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    /// <summary>
    /// Knowledge Interface with parsing support(starPadSDK.MathExpr)
    /// </summary>
    public class IKnowledge
    {
        #region Properties and Constructors

        private starPadSDK.MathExpr.Expr _inputExpr;
        public starPadSDK.MathExpr.Expr Expr
        {
            get { return _inputExpr; }
            set { _inputExpr = value; }
        }

        public bool IsSelected { get; set; } // query handler

        public ObservableCollection<IKnowledge> RenderKnowledge { get; set; }

        public List<Tuple<object, object>> AutoTrace { get; set; } // outer and inner loop scaffolding
      
        public IKnowledge(starPadSDK.MathExpr.Expr exp)
        {
            _inputExpr = exp;
        }

        public object Tag { get; set; }

        public override bool Equals(object obj)
        {
            var knowledge = obj as IKnowledge;
            if (knowledge == null) return false;
            return Expr.Equals(knowledge.Expr);
        }

        #endregion

        #region Virtual Functions and Utils

        public int RetrieveStepsNumbers()
        {
            int count = 0;
            foreach (Tuple<object, object> tuple in AutoTrace)
            {
                var lst = tuple.Item2 as List<TraceStepExpr>;
                Debug.Assert(lst != null);
                count += lst.Count;
            }
            return count;
        }

        public List<string> RetrieveOuterLoopStrategies()
        {
            var lst = new List<string>();
            foreach (Tuple<object, object> tuple in AutoTrace)
            {
                var str = tuple.Item1 as string;
                Debug.Assert(str != null);
                lst.Add(str);
            }
            return lst;
        }

        public virtual bool HasSolvingTrace()
        {
            return false;
        }

        public virtual void GenerateSolvingTrace()
        {
        }

        public virtual void RetrieveRenderKnowledge()
        {
        }

        public IKnowledge FindSelectedKnowledge()
        {
            var result = RenderKnowledge.FirstOrDefault(tempKnowledge => tempKnowledge.IsSelected);
            if (result != null) return result;

            if (IsSelected) return this;
            return null;
        }

        #endregion
    }
}

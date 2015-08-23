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

namespace MathCog
{
    using System.Collections.Generic;
    using CSharpLogic;

    /// <summary>
    /// such as S= 5.0, m = 4, x=2+1
    /// </summary>
    public class AGPropertyExpr : AGEquationExpr
    {
        #region Properties and Constructors

        private EqGoal _goal;
        public EqGoal Goal
        {
            get { return _goal; }
            set { _goal = value; }
        }

        public AGPropertyExpr(starPadSDK.MathExpr.Expr expr, EqGoal goal)
            :base(expr)
        {
            _goal = goal;
        }

        #endregion

        #region Override functions

        public override void GenerateSolvingTrace()
        {
            if (IsSelected)
            {
                var traces = _goal.Traces;
                if (traces.Count == 0) return;
                var lst = new List<TraceStepExpr>();
                TraceStepExpr tse;
                for (int i = 0; i < traces.Count; i++)
                {
                    var ts = traces[i];
                    tse = new TraceStepExpr(ts);
                    lst.Add(tse);
                }
                AutoTrace = lst;
                return;
            }

            if (RenderKnowledge == null) return;

            foreach (var temp in RenderKnowledge)
            {
                if (temp.IsSelected)
                {
                    temp.GenerateSolvingTrace();
                }
            }
        }

        public override void RetrieveRenderKnowledge()
        {
            base.RetrieveRenderKnowledge();
        }

        #endregion
    }
}
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
    using CSharpLogic;
    using AlgebraGeometry;

    public class TraceStepExpr
    {
        #region Properties and Constructors

        public string MetaRule { get; set; } //Tutor Mode
        public string AppliedRule { get; set; } //Demonstration Mode

        public starPadSDK.MathExpr.Expr Source { get; set; }
        public starPadSDK.MathExpr.Expr Target { get; set; }
        public starPadSDK.MathExpr.Expr StepExpr { get; set; }

        public TraceStepExpr(TraceStep ts)
        {
            MetaRule = ts.Rule as string;
            AppliedRule = ts.AppliedRule as string;
            Source = ExprG.Generate(ts.Source);
            Target = ExprG.Generate(ts.Target);
            StepExpr = ExprG.Derive(Source, Target);
        }

        #endregion

        public override bool Equals(object obj)
        {
            var traceStep = obj as TraceStepExpr;
            if (traceStep == null) return false;

            return traceStep.AppliedRule.Equals(AppliedRule)
                   && traceStep.Source.Equals(Source)
                   && traceStep.Target.Equals(Target);
        }

        public override int GetHashCode()
        {
            return Source.GetHashCode() ^
                Target.GetHashCode() ^ AppliedRule.GetHashCode();
        }
    }
}

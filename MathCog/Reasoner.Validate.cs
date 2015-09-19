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

using System.Net.Configuration;

namespace MathCog
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using AlgebraGeometry;
    using CSharpLogic;
    using ExprPatternMatch;
    using starPadSDK.MathExpr;
    public partial class Reasoner
    {
        private void InternalValidate(Expr expr,object obj,out object output)
        {
            output = null;
            var query    = obj as Query;
            var equation = obj as Equation;
            var shape = obj as ShapeSymbol;
            if (query != null)
            {
                InternalValidate(expr, query, out output);
                return;
            }
            if (equation != null)
            {
                InternalValidate(expr, equation, out output);
                return;
            }
            if (shape != null)
            {
                InternalValidate(expr, shape, out output);
            }
        }

        private void InternalValidate(Expr expr, ShapeSymbol ss, out object output)
        {
            output = false;
            foreach (var gn in RelationGraph.Nodes)
            {
                var sn = gn as ShapeNode;
                if (sn == null) continue;
                bool result = sn.ShapeSymbol.ApproximateMatch(ss);
                if (result)
                {
                    output = true;
                    return;
                }
            }
        }

        private bool InternalValidate(Expr expr, Equation eq, out object trace)
        {
            trace = null;

            var rTemp = ExprVisitor.Instance.Match(expr, false); //input patter match

            return false;



/*            var eqExpr = new AGEquationExpr(expr, eq);
            eq.TransformTermTrace();
            eqExpr.RetrieveRenderKnowledge();
            if (eqExpr.RenderKnowledge == null || eqExpr.RenderKnowledge.Count == 0)
            {
                return false;
            }
            var agEquationExpr = eqExpr.RenderKnowledge[0] as AGEquationExpr;
            if (agEquationExpr == null)
            {
                return false;
            }

            agEquationExpr.IsSelected = true;
            agEquationExpr.GenerateSolvingTrace();

            if (agEquationExpr.AutoTrace == null) return false;
            trace = agEquationExpr.AutoTrace;
            return true;*/
        }

        private bool InternalValidate(Expr expr, Query query, out object trace)
        {
            trace = null;
            RelationGraph.AddNode(query);

            var queryExpr = new AGQueryExpr(expr, query);
            queryExpr.RetrieveRenderKnowledge();
            if (queryExpr.RenderKnowledge.Count == 0)
            {
                return false;
            }
            var agEquationExpr = queryExpr.RenderKnowledge[0] as AGEquationExpr;
            if (agEquationExpr == null)
            {
                return false;
            }
            agEquationExpr.IsSelected = true;
            agEquationExpr.GenerateSolvingTrace();

            if (agEquationExpr.AutoTrace == null) return false;
            trace = agEquationExpr.AutoTrace;

            RelationGraph.DeleteNode(query);
            return true;
        }
    }
}
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

using AlgebraGeometry;
using CSharpLogic;

namespace MathCog
{
    public static class IKnowledgeGenerator
    {
        public static IKnowledge Generate(object obj)
        {
            var shape = obj as ShapeSymbol;
            var goal = obj as EqGoal;
            var equation = obj as Equation;
            if (shape != null)
            {
                return new AGShapeExpr(ExprG.Generate(shape), shape);
            }

            if (goal != null)
            {
                return new AGPropertyExpr(ExprG.Generate(goal), goal);
            }

            if (equation != null)
            {
                return new AGEquationExpr(ExprG.Generate(equation), equation);
            }

            var knowledge = new IKnowledge(ExprG.Generate(obj));
            knowledge.Tag = obj;
            return knowledge;
        }
    }
}

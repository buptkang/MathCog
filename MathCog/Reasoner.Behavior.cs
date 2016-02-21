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
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using AlgebraGeometry;
    using CSharpLogic;
    using ExprPatternMatch;
    using starPadSDK.MathExpr;
    using Text = starPadSDK.MathExpr.Text;

    public partial class Reasoner
    {
        public bool? VerifyConcreteSymbol(string str)
        {
            Expr expr = Text.Convert(str);
            object rTemp = ExprVisitor.Instance.Match(expr);

            var ss = rTemp as ShapeSymbol;
            if (ss == null) return null;
            return ss.Shape.Concrete;
        }

        /*
         * Parse math equation to be goal or shape in higher syntax.
         *
         */
        public object ExprValidate(Expr expr)
        {
            //User Input Equation Pattern Match
            var rTemp = ExprVisitor.Instance.Match(expr);
            object output;
            var lst = rTemp as List<object>;
            if (lst != null)
            {
                var lst1 = new List<object>();
                foreach (object tempObj in lst)
                {
                    EvalExprPatterns(expr, tempObj, null, out output, true);
                    lst1.Add(output);
                }
                return lst1;
            }
            EvalExprPatterns(expr, rTemp, null, out output, true);
            var iKnowledge = output as IKnowledge;
            return iKnowledge;
        }

        public object RelationValidate(object obj, out object objOutput)
        {
            var expr = obj as Expr;
            var str = obj as string;     // text input
            var shapeSymbol = obj as ShapeSymbol;
            if (shapeSymbol != null)
            {
                expr = ExprG.Generate(shapeSymbol);
            }
            if (str != null)
            {
                expr = Text.Convert(str);
            }
            Debug.Assert(expr != null);
            var rTemp = ExprVisitor.Instance.UserMatch(expr); //input patter match
            Debug.Assert(rTemp != null);
            rTemp = ExprVisitor.Instance.Transform(rTemp);
            object output;
            if (shapeSymbol != null)
            {
                rTemp = shapeSymbol;
            }
            InternalValidate(expr, rTemp, out output);
            objOutput = rTemp;
            return output;
        }
    }
}

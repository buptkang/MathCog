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

namespace AlgebraGeometry
{
    using CSharpLogic;
    using starPadSDK.MathExpr;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Text = starPadSDK.MathExpr.Text;
    
    public static class ExprMock
    {
        /*
         * 1: d^2 = 9 + 16 
         * 2: d^2 = 16 + 9
         * 3: d = Sqrt(16+9)
         * 
         * 4: (y-3)/(4-2) = 5
         * 5: m = (y-3)/(4-2)
         */

        public static Expr Mock1()
        {
            var expr1 = new CompositeExpr(WellKnownSym.plus, new Expr[] {new IntegerNumber("9"), new IntegerNumber("16")});
            var expr2 = new CompositeExpr(WellKnownSym.power, new Expr[] {new LetterSym('d'), new IntegerNumber("2")});
            var compositeExpr = new CompositeExpr(WellKnownSym.equals, new Expr[] {expr2, expr1});
            return compositeExpr;
        }

        public static Expr Mock2()
        {
            var expr1 = new CompositeExpr(WellKnownSym.plus, new Expr[] { new IntegerNumber("16"), new IntegerNumber("9") });
            var expr2 = new CompositeExpr(WellKnownSym.power, new Expr[] { new LetterSym('d'), new IntegerNumber("2") });
            var compositeExpr = new CompositeExpr(WellKnownSym.equals, new Expr[] { expr2, expr1 });
            return compositeExpr;
        }

        public static Expr Mock3()
        {
            var expr1 = new CompositeExpr(WellKnownSym.plus,
                new Expr[] {new IntegerNumber("16"), new IntegerNumber("9")});
            var expr2 = new CompositeExpr(WellKnownSym.root, 
                new Expr[] {new IntegerNumber("2"), expr1});
            var expr3 = new CompositeExpr(WellKnownSym.equals,
                new Expr[] {new LetterSym('d'), expr2});
            return expr3;
        }

        public static Expr Mock3_1()
        {
            var expr1 = new CompositeExpr(WellKnownSym.plus,
                new Expr[] { new IntegerNumber("3"), new IntegerNumber("4") });
            var expr2 = new CompositeExpr(WellKnownSym.root,
                new Expr[] { new IntegerNumber("2"), expr1 });
            var expr3 = new CompositeExpr(WellKnownSym.equals,
                new Expr[] { new LetterSym('d'), expr2 });
            return expr3;
            
        }

        public static Expr Mock4()
        {
            //(y-3)/(4-2) = 5

            var expr0 = new CompositeExpr(WellKnownSym.minus,
                new Expr[] {new IntegerNumber("3")});
            var expr1 = new CompositeExpr(WellKnownSym.plus,
                new Expr[] {new LetterSym('y'), expr0});

            var expr2 = new CompositeExpr(WellKnownSym.minus,
                new Expr[] { new IntegerNumber("2") }); 
            var expr4 = new CompositeExpr(WellKnownSym.plus,
                new Expr[] {new IntegerNumber("4"), expr2});

            var expr5 = new CompositeExpr(WellKnownSym.divide,
                new Expr[] {expr4});
            var expr6 = new CompositeExpr(WellKnownSym.times,
                new Expr[] {expr1, expr5});
            var expr7 = new CompositeExpr(WellKnownSym.equals,
                new Expr[] {expr6, new IntegerNumber("5")});

            return expr7;
        }

        public static Expr Mock5()
        {
            //(y-3)/(4-2) = 5

            var expr0 = new CompositeExpr(WellKnownSym.minus,
                new Expr[] { new IntegerNumber("3") });
            var expr1 = new CompositeExpr(WellKnownSym.plus,
                new Expr[] { new LetterSym('y'), expr0 });

            var expr2 = new CompositeExpr(WellKnownSym.minus,
                new Expr[] { new IntegerNumber("2") });
            var expr4 = new CompositeExpr(WellKnownSym.plus,
                new Expr[] { new IntegerNumber("4"), expr2 });

            var expr5 = new CompositeExpr(WellKnownSym.divide,
                new Expr[] { expr4 });
            var expr6 = new CompositeExpr(WellKnownSym.times,
                new Expr[] { expr1, expr5 });
            var expr7 = new CompositeExpr(WellKnownSym.equals,
                new Expr[] { new LetterSym('m'), expr6});

            return expr7;
        }

        public static Expr Mock6()
        {
            var expr11 = new CompositeExpr(WellKnownSym.divide,
                new Expr[] {0.5});

            var expr1 = new CompositeExpr(WellKnownSym.times,
                new Expr[] {-1, expr11});
            var expr2 = new CompositeExpr(WellKnownSym.equals,
                new Expr[] {new LetterSym('m'), expr1});
            return expr2;
        }
    }
}

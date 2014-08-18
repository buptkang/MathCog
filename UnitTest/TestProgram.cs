using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExprSemantic.KnowledgeBase;
using NUnit.Framework.Constraints;
using starPadSDK.MathExpr;

namespace ParseUnitTest
{
    public class TestProgram
    {
        public static void Main()
        {
            //var shape = new Shape();
            //string label = shape.Label;
            
            string str4 = "AB(1.2,3.5)";
            Expr expr = 2.0;
            try
            {
                expr = Text.Convert(str4);
                bool flag = CheckPointExpr(expr);
            }
            catch (TextParseException ex)
            {
                Console.WriteLine(ex.GetType().ToString());
            }
            
            Console.ReadLine();
        }

        public static bool CheckPointExpr(Expr expr)
        {
            var composite = expr as CompositeExpr;
            var sym = composite.Head as Sym;
            string pointLabel = sym.ToString();
            
            return false;
        }
    }
}

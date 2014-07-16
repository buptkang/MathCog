using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using Microsoft.FSharp.Text.Lexing;
using starPadSDK.MathExpr;
using starPadSDK.MathExpr.TextInternals;

namespace ParseUnitTest
{
    public static class MyExtension
    { 
        public static int WordCount(this string str, bool testCond)
        {
            if (testCond)
            {
                return str.Split(new char[] { ' ', '.', '?' },
                             StringSplitOptions.RemoveEmptyEntries).Length;
            }
            else
            {
                return 0;
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            /*
            const string expr = "x+2*x-4+y-2x = 6";
            Expr astExpr = Text.Convert(expr);
            Expr simExpr = Engine.Simplify(astExpr);
            */

            String s = "Hello World";
            int i = s.WordCount(false);
            Console.WriteLine(i);

            Console.ReadLine();
        }
    }
}

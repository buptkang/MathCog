using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using starPadSDK.MathExpr;

namespace ParseUnitTest
{
    public class TestProgram
    {
        public static void Main()
        {
            string str4 = "(?)";
            Expr expr = 2.0;
            try
            {
                expr = Text.Convert(str4);
            }
            catch (TextParseException ex)
            {
                Console.WriteLine(ex.GetType().ToString());
            }

            Console.ReadLine();
        }
    }
}

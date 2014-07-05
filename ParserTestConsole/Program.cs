using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.FSharp.Text.Lexing;

using starPadSDK.MathExpr;

namespace ParserTestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            const string expr = "1+3";
            var lexbuff = LexBuffer<char>.FromString(expr);

            
        }
    }
}

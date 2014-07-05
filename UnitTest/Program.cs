using Microsoft.FSharp.Text.Lexing;

namespace ParseUnitTest
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

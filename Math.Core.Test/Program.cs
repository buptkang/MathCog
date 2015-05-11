using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MathRestAPI.Test
{
    public class MainClass
    {
        public static void Main(string[] args)
        {
            //string pattern = @"\b(\w+?)([\u00AE\u2122])";
            //string input = "Microsoft® Office Professional Edition combines several office " +
            //               "productivity products, including Word, Excel®, Access®, Outlook®, " +
            //               "PowerPoint®, and several others. Some guidelines for creating " +
            //               "corporate documents using these productivity tools are available " +
            //               "from the documents created using Silverlight™ on the corporate " +
            //               "intranet site.";

            //MatchCollection matches = Regex.Matches(input, pattern);
            //foreach (Match match in matches)
            //{
            //    GroupCollection groups = match.Groups;
            //    Console.WriteLine("{0}: {1}   - {2}", groups[2], groups[1], groups[0]);
            //}
            //Console.WriteLine();
            //Console.WriteLine("Found {0} trademarks or registered trademarks.", matches.Count);


            string pattern = @"(\b(\w+?)\s?)+[?.!]";
            string input = "This is one sentence. This is a second sentence.";

            Match match = Regex.Match(input, pattern);
            Console.WriteLine("Match: " + match.Value);
            int groupCtr = 0;
            foreach (Group group in match.Groups)
            {
                groupCtr++;
                Console.WriteLine("Group {0}: '{1}'", groupCtr, group.Value);
                int captureCtr = 0;
                foreach (Capture capture in group.Captures)
                {
                    captureCtr++;
                    Console.WriteLine("      Capture {0}: '{1}'", captureCtr, capture.Value);
                }
            }


            Match next = match.NextMatch();

            Console.WriteLine("Match: " + next.Value);
            groupCtr = 0;
            foreach (Group group in next.Groups)
            {
                groupCtr++;
                Console.WriteLine("Group {0}: '{1}'", groupCtr, group.Value);
                int captureCtr = 0;
                foreach (Capture capture in group.Captures)
                {
                    captureCtr++;
                    Console.WriteLine("      Capture {0}: '{1}'", captureCtr, capture.Value);
                }
            }

            Console.Read();
        }
    }
}

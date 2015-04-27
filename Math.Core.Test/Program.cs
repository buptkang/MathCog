using System;
using System.Collections.Generic;

namespace MathRestAPI.Test
{
	public class MainClass
	{
		public static void Main (string[] args)
		{
            //List<MathProblem> objs = Visitor.Instance.GetProblems();
		    MathProblem obj = VisitorLocal.Instance.GetProblem(1);
		    Console.Read();
		}
	}
}

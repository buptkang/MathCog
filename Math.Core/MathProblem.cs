using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathRestAPI
{
    public class MathProblem
    {
        public string problem { get; set; }
        public string id { get; set; }
        public List<Tag> Tags { get; set; }
        public List<int> InputIndexes { get; set; }
        public List<ExplainTag> ExplainTags { get; set; }
        public MathSolver Solver { get; set; }

        public MathProblem(string _problem, string _id)
        {
            problem = _problem;
            id = _id;
        }

        public class Tag
        {
            public int index { get; set; }
            public string tag { get; set; }
        }

        public class ExplainTag
        {
            public int index      { get; set; }
            public string Explain { get; set; }
            public string Drag    { get; set; }
        }
    }

    public class MathSolver
    {
        public string Strategy { get; set; }
        public string Answer   { get; set; }
        public List<MathTrace> Solution { get; set; }

        public MathSolver(string _strategy, string _answer, List<MathTrace> _solution)
        {
            Strategy = _strategy;
            Answer = _answer;
            Solution = _solution;
        }

        public class MathTrace
        {
            public string Step { get; set; }
            public string Rule { get; set; }
            public string ToHint { get; set; }

            public MathTrace(string _step, string _rule, string _toHint)
            {
                Step = _step;
                Rule = _rule;
                ToHint = _toHint;
            }
        }
    }
}

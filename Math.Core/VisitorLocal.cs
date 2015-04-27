using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MathRestAPI
{
    public class VisitorLocal
    {
        #region Singleton 

		private static VisitorLocal _api;

        private VisitorLocal()
		{
		}

        public static VisitorLocal Instance
		{
			get
			{
				if (_api == null) 
				{
                    _api = new VisitorLocal();	
				}

				return _api;
			}			
		}

		#endregion

        private DirectoryInfo _jsonFolder =
            Directory.GetParent(System.AppDomain.CurrentDomain.BaseDirectory)
                .Parent.Parent.Parent.GetDirectories("Test.JSONs")[0];

        #region Problem Parsing Functions

        public MathProblem GetProblem(int id)
        {
            StreamReader file = null;
            try
            {
                FileInfo fileInfo = _jsonFolder.GetFiles(id + ".json")[0];
                file = fileInfo.OpenText();
            }
            catch (Exception e)
            {
                return null;
            }

            var serializer = new JsonSerializer();
            using (var reader = new JsonTextReader(file))
            {
                var o2 = (JObject)JToken.ReadFrom(reader);

                var problem = new MathProblem(o2["problem"].Value<string>(), o2["id"].Value<string>());

                problem.Tags = o2["tags"].ToObject<MathProblem.Tag[]>().ToList();
                if (o2["inputTags"] != null)
                {
                    problem.InputIndexes = o2["inputTags"].Select(token => token.Value<int>()).ToList();                    
                }
                if (o2["explainTags"] != null)
                {
                    problem.ExplainTags = o2["explainTags"].ToObject<MathProblem.ExplainTag[]>().ToList();
                }

                #region solving tags

                if (o2["Strategy"] != null && o2["Answer"] != null && o2["Trace"] != null)
                {
                    var strategy = o2["Strategy"].Value<string>();
                    var answer = o2["Answer"].Value<string>();
                    List<MathSolver.MathTrace> trace = o2["Trace"].ToObject<MathSolver.MathTrace[]>().ToList();

                    var mathSolver = new MathSolver(strategy, answer, trace);
                    problem.Solver = mathSolver;
                }

                #endregion


                return problem;
            }
            return null;
        }

        #endregion

    }
}

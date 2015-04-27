using System;
using System.Collections.Generic;
using RestSharp;
using RestSharp.Deserializers;

namespace MathRestAPI
{
	public class Visitor
	{
        #region Singleton 

		private static Visitor _api;

		private Visitor()
		{
		}

		public static Visitor Instance
		{
			get
			{
				if (_api == null) 
				{
					_api = new Visitor ();	
				}

				return _api;
			}			
		}

		#endregion

        private RestClient client = new RestClient("http://104.236.111.21");


        #region Problem Parsing Functions

	    public List<MathProblem> GetProblems()
	    {
            var request = new RestRequest("problems", Method.GET);

            IRestResponse restResponse = client.Execute(request);

            var content = restResponse.ContentType;
            var deserial = new JsonDeserializer();

            return deserial.Deserialize<List<MathProblem>>(restResponse);
	    }


        #endregion

        #region Problem Solving Functions



        #endregion

    }
}


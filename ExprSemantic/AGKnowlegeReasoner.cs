using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExprSemantic
{
    /// <summary>
    /// Main Entry Class for the Analytical Geometry Semantic Parsing
    /// </summary>
    public class AGKnowlegeReasoner
    {
        #region Singleton Pattern

        private AGKnowlegeReasoner()
        {
            
        }

        private static AGKnowlegeReasoner _agKnowlegeReasoner;

        public static AGKnowlegeReasoner Instance
        {
            get
            {
                if (_agKnowlegeReasoner == null)
                {
                    _agKnowlegeReasoner = new AGKnowlegeReasoner();
                }
                return _agKnowlegeReasoner;
            }
        }

        #endregion

        public Axiom Parse(string repr) 
        {
            return null;
        } 
    }
}

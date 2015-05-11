using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExprSemantic
{
    public class ExprPatternMatchVisitor
    {
        private static ExprPatternMatchVisitor _instance;

        private ExprPatternMatchVisitor()
        {            
        }

        public static ExprPatternMatchVisitor Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ExprPatternMatchVisitor();                                        
                }

                return _instance;
            }
        }




    }
}

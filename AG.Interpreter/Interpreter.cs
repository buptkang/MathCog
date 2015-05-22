using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExprSemantic;
using GuiLabs.Undo;

namespace AG.Interpreter
{
    public class Interpreter : IInterpreter
    {
        #region public interface

        public static Interpreter _instance;

        public static Interpreter Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Interpreter();
                }
                return _instance;
            }
        }

        public ActionManager ActionManager { get; set; }

        private static Reasoner _memory = Reasoner.Instance;

        private Interpreter()
        {
            ActionManager = new ActionManager();
        }




        #endregion
    }
}

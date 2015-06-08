using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GuiLabs.Undo;

namespace CSharpLogic.Action
{
    /*
    public class ReifyLogicObjectAction : AbstractAction
    {
        private DyLogicObject _oldObj;
        private DyLogicObject _newObj;
        private EqGoal _substitute;

        public DyLogicObject CurrObj
        {
            get
            {
                return _newObj ?? _oldObj;
            }
        }

        public ReifyLogicObjectAction(DyLogicObject obj, EqGoal goal)
        {
            _oldObj = obj;
            _substitute = goal;
        }

        protected override void ExecuteCore()
        {
            _newObj = _oldObj.Reify(_substitute.ToDict());
        }

        protected override void UnExecuteCore()
        {
            _newObj = null;
        }
    }
     */ 
}

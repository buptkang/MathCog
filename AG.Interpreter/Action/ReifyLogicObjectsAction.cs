using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GuiLabs.Undo;
using NUnit.Framework;

namespace CSharpLogic.Action
{
    /*
    public class ReifyLogicObjectsAction : AbstractAction
    {
        private List<DyLogicObject> _oldObjs;
        private List<DyLogicObject> _newObjs;
        private Dictionary<object, object> _substitute;

        public ReifyLogicObjectsAction(List<DyLogicObject> objs,
            Dictionary<object, object> dict)
        {
            _oldObjs = objs;
            _substitute = dict;
        }

        protected override void ExecuteCore()
        {
            _newObjs = new List<DyLogicObject>();
            foreach (DyLogicObject obj in _oldObjs)
            {
                var newObj = obj.Reify(_substitute);
                _newObjs.Add(newObj);
            }
        }

        protected override void UnExecuteCore()
        {
            _newObjs = null;
        }
    }
     */
}

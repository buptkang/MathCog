using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GuiLabs.Undo;

namespace CSharpLogic.Undo
{
    public class DyLogicCachingObject : DyLogicObject
    {
        private ActionManager _actionManager;
        public DyLogicCachingObject()
        {
            _actionManager = new ActionManager();            
        }
    }

    public static class DyLogicCachingObjectExtension
    {
        public static void UnReify(this DyLogicCachingObject logicObj, Goal goal)
        {
            goal.Unify(logicObj.Properties);
        }

        public static void Reify(this DyLogicCachingObject logicObj, Goal goal)
        {
           goal.Unify(logicObj.Properties);
        }
    }
}

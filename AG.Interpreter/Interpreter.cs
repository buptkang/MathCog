using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExprSemantic;
using GuiLabs.Undo;

namespace AG.Interpreter
{
    public partial class Interpreter : IInterpreter
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
            _queryCache = new ObservableCollection<KeyValuePair<object, object>>();
        }

        #endregion

        private ObservableCollection<KeyValuePair<object, object>> _queryCache;

        public bool LoadQuery(object obj)
        {
            
        }

        public bool UnLoadQuery(object obj)
        {
            
        }

        public object SearchCurrentQueryResult(object key)
        {
            foreach (KeyValuePair<object, object> pair in _queryCache)
            {
                if (pair.Key.Equals(key))
                {
                    return pair.Value;
                }
            }
            return null;
        }

        public int GetNumberOfQueries()
        {
            return _queryCache.Count;
        }
    }
}

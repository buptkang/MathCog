using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Dynamic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Text = starPadSDK.MathExpr.Text;
using GuiLabs.Undo;

namespace ExprSemantic
{
    public partial class Reasoner
    {
        #region Instance

        private static Reasoner _instance;

        public static Reasoner Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Reasoner();
                }
                return _instance;
            }
        }

        private Reasoner()
        {
            ActionManager = new ActionManager();
            _cache = new ObservableCollection<KeyValuePair<string, object>>();
            _cache.CollectionChanged += _cache_CollectionChanged;
        }

        void _cache_CollectionChanged(object sender, 
            System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (KeyValuePair<string, object> pair in e.NewItems)
                {
               
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                
            }
        }

        #endregion

        #region Properties

        private ObservableCollection<KeyValuePair<string, object>> _cache;

        public ActionManager ActionManager { get; set; }

        #endregion

        #region Public Interface

        public void Load(string fact)
        {
            starPadSDK.MathExpr.Expr expr = Text.Convert(fact);
            object result = ExprVisitor.Instance.Match(expr);
            EvalPropogate(result);
            var pair = new KeyValuePair<string, object>(fact, result);
            _cache.Add(pair);                               
        }

        public void Load(IEnumerable<string> facts)
        {
            foreach (string f in facts)
            {
                Load(f);
            }
        }

        public void Unload(string key)
        {
            object fact = SearchFact(key);
            if (fact != null)
            {
                var temp = (KeyValuePair<string, object>) fact;
                UndoEvalPropogate(temp.Value);
                _cache.Remove(temp);
            }
        }

        public Tuple<object, Tracer> Answer(string fact)
        {
            return null;
        }

        #endregion

        #region Delegate for the interaction purpose

        public delegate void UpdateKnowledgeHandler(object sender, object args);

        public event UpdateKnowledgeHandler KnowledgeUpdated;

        #endregion
    }
}

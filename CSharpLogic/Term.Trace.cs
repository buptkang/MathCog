using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CSharpLogic
{
    /// <summary>
    /// Substitution or term
    /// </summary>
    public partial class Term : DyLogicObject
        , IArithmeticLogic, IAlgebraLogic
    {
        public object Substitute(object obj1, object obj2, object obj)
        {
            Term cloneTerm = Clone();
            var lst = cloneTerm.SearchList(obj1);
            if (lst != null)
            {
                var index = lst.FindIndex(x => x.Equals(obj1));
                lst[index] = obj;
                lst.Remove(obj2);
                if (lst.Count == 1) return lst[0];
                return cloneTerm;
            }
            else
            {
                return obj2;
            }       

        }

        //Arithmetic Usage
        public void GenerateTrace(object obj1, object obj2, object obj, string rule)
        {
            Term currentTerm;
            if (Traces.Count == 0)
            {
                currentTerm = this;
            }
            else
            {
                currentTerm = Traces[Traces.Count - 1].Target as Term;
                if(currentTerm == null) throw new Exception("Must be term here");
            }

            Term cloneTerm = currentTerm.Clone();
            List<object> lst;
            bool result = cloneTerm.SearchArithList(obj1, obj2, out lst);

            if (!result)
            {
                throw new Exception("Term.Trace.cs: Cannot be false");
            }
            var index = lst.FindIndex(x => x.Equals(obj1));
            lst[index] = obj;
            lst.Remove(obj2);
            object objj = cloneTerm.ReConstruct();
            var ts = new TraceStep(currentTerm, objj, rule);
            Traces.Add(ts);
        }

        //Algebraic Usage
        public void GenerateTrace(object source, object target, string rule)
        {
            Term currentTerm;
            if (Traces.Count == 0)
            {
                currentTerm = this;
            }
            else
            {
                currentTerm = Traces[Traces.Count - 1].Target as Term;
                if (currentTerm == null) throw new Exception("Must be term here");
            }
            Term cloneTerm = currentTerm.Clone();
            var lst = cloneTerm.SearchList(source);
            if (lst != null)
            {
                var index = lst.FindIndex(x => x.Equals(source));
                lst[index] = target;
                object objj = cloneTerm.ReConstruct();
                var ts = new TraceStep(currentTerm, objj, rule);
                Traces.Add(ts);
            }
            else
            {
                var targetTerm = target as Term;
                if (targetTerm != null)
                {
                    object objj = targetTerm.ReConstruct();
                    var ts = new TraceStep(currentTerm, objj, rule);
                    Traces.Add(ts);
                }
                else
                {
                    var ts = new TraceStep(currentTerm, target, rule);
                    Traces.Add(ts);                    
                }
            }
        }

        private bool SearchArithList(object obj1, object obj2, out List<object> output)
        {
            output = null;
            var lst = Args as List<object>;
            Debug.Assert(lst != null);

            for (int i = 0; i < lst.Count-1; i++)
            {
                var localTerm = lst[i] as Term;
                if (localTerm != null)
                {
                    bool result = localTerm.SearchArithList(obj1, obj2, out output);
                    if (result) return true;
                }

                localTerm = lst[i+1] as Term;
                if (localTerm != null)
                {
                    bool result = localTerm.SearchArithList(obj1, obj2, out output);
                    if (result) return true;
                }

                if (lst[i].Equals(obj1) && lst[i + 1].Equals(obj2))
                {
                    output = lst;
                    return true;
                }              
            }
            return false;
        }

        private List<object> SearchList(object obj)
        {
            var lst = Args as List<object>;
            Debug.Assert(lst!= null);
            foreach (var tempObj in lst)
            {
                if (tempObj.Equals(obj)) return lst;
                var localTerm = tempObj as Term;
                if (localTerm != null)
                {
                    return localTerm.SearchList(obj);
                }
            }
            return null;
        }
    }
}

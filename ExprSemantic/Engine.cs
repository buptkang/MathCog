using System;
using System.Collections;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

namespace starPadSDK.MathExpr {
	/// <summary>
	/// Summary description for Engine.
	/// </summary>
	public abstract class Engine {
		/// <summary>
		/// Try to simplify the expression. (For some meaning of "simplify".)
		/// The original expression should not be modified.
		/// </summary>
		public abstract Expr _Simplify(Expr e);
		/// <summary>
		/// Attempt to perform numeric conversion and evaluation similar to Mathematica's N[] function.
		/// The original expression should not be modified.
		/// </summary>
		public abstract Expr _Approximate(Expr e);
		/// <summary>
		/// Precise semantics of this are up to the Engine, but it should at least support literal replacement with orig a Sym.
		/// The original expression should not be modified.
		/// </summary>
        public abstract Expr _Substitute(Expr e, Expr orig, Expr replacement);
        /// <summary>
        /// Precise semantics of this are up to the Engine, but it should at least support literal replacement with orig a Sym.
        /// The original expression should not be modified.
        /// Replaces the expression using an Object refernce test
        /// </summary>
        public abstract Expr _Replace(Expr e, Expr orig, Expr replacement);
		/// <summary>
		/// Names of this engine's variants, for display to the user, or null for no variants.
		/// </summary>
        public virtual string[] Names { get { return null; } }
        /// <summary>
        /// Name of this engine, for display to the user.
        /// </summary>
        public abstract string Name { get; }
        /// <summary>
        /// Pick which variant to use, from the set named in Name
        /// </summary>
        public virtual int Variant { get { return 0; } set { Deactivate(); } }
        /// <summary>
		/// Perform any startup activation needed (connecting to an external program, etc.). Use Deactivate to deactivate.
		/// </summary>
		public abstract void Activate();
		public abstract void Deactivate();
        /// <summary>
        /// Does the underlying program or platform for this engine exist? If this returns false, the engine will be omitted from the Engines list.
        /// </summary>
        public virtual bool Exists { get { return true; } }

		private static Engine[] _engines;
		public static Engine[] Engines { get { return _engines; } }
		private static Engine _current;
		public static Engine Current { 
			get { return _current; }
            set { _current.Deactivate(); _current = value; _current.Activate(); }
		}
        private static Stack<Engine> _engineStack = new Stack<Engine>();
        public static void PushEngine(Engine e) {
            _engineStack.Push(_current);
            _current = e;
            _current.Activate();
        }
        public static void PopEngine() {
            if(_engineStack.Count != 0) {
                _current.Deactivate();
                _current = _engineStack.Pop();
            }
        }
		public static Expr Simplify(Expr e) { return Current._Simplify(e); }
		public static Expr Approximate(Expr e) { return Current._Approximate(e); }
        public static Expr Substitute(Expr e, Expr orig, Expr replacement) { return Current._Substitute(e, orig, replacement); }
        public static Expr Replace(Expr e, Expr orig, Expr replacement) { return Current._Replace(e, orig, replacement); }
		static Engine() {
			DirectoryInfo rundir = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory;
            List<Engine> engines = new List<Engine>();
			foreach(FileInfo fi in rundir.GetFiles("*.dll")) {
				try {
					Assembly assy = Assembly.LoadFrom(fi.FullName);
					foreach(Type t in assy.GetExportedTypes()) {
						if(!t.IsAbstract && typeof(Engine).IsAssignableFrom(t)) {
							Engine e = (Engine) Activator.CreateInstance(t);
							if(e.Exists) engines.Add(e);
						}
					}
				} catch (BadImageFormatException) {
					// do nothing. The dll isn't in the right format, so we'll ignore it.
				} catch (FileLoadException) {
					// do nothing. We tried to load the same assembly twice or the assembly name was longer than MAX_PATH characters.
					// In either case, we can't do anything with it.
				} catch (System.Reflection.ReflectionTypeLoadException) {
					// ignore ReflectionTypeLoadException
                } catch (TypeLoadException) {
                    // ignore
				} catch (Exception e) {
					throw new ApplicationException("Could not load math engine plugin " + fi.Name + ":\n\n" + e.Message, e);
				}
			}
            engines.Sort(delegate(Engine a, Engine b) { return a.Name.CompareTo(b.Name); });
			_engines = engines.ToArray();
			Trace.Assert(_engines.Length > 0);
			int ix = 0;
			for(int i = 0; i < _engines.Length; i++) {
                if (_engines[i] is BuiltInEngine) {				
                //if(_engines[i] is MMAEngine) {
					ix = i;
					break;
				}
			}
			_current = _engines[ix];
		}
	}
}

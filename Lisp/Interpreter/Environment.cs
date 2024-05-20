using Lisp.DataStructures;

namespace Lisp.Interpreter
{
    /// <summary>
    /// Stores dictionary of bindings (symbol, value).
    /// Symbols act as variable names
    /// </summary>
    public class Environment
    {
        private readonly Dictionary<Symbol, LispObject> bindings = new Dictionary<Symbol, LispObject>();
        public Environment? Parent { get; set; } = null;

        public void Assign(Symbol s, LispObject o)
        {
            bindings[s] = o;
        }

        public LispObject LookUp(Symbol s)
        {
            if (bindings.ContainsKey(s))
                return bindings[s];
            if (Parent != null)
                return Parent.LookUp(s);
            throw new EvalException($"Symbol {s} not found", s);
        }
    }
}

using Lisp.DataStructures;

namespace Lisp.Interpreter
{
    public record class PrimitiveSpecialForm : SpecialForm
    {
        private readonly Func<List<LispObject>, Environment, LispObject> func;
        public PrimitiveSpecialForm(Func<List<LispObject>, Environment, LispObject> func)
        {
            this.func = func;
        }

        public override LispObject Apply(List<LispObject> args, Environment env)
        {
            return func(args, env);
        }

        public override string ToString()
        {
            return $"#Procedure{GetHashCode()}";
        }
    }
}

using Lisp.DataStructures;

namespace Lisp.Intepreter
{
    public record class PrimitiveProcedure : Procedure
    {
        private readonly Func<List<LispObject>, LispObject> func;

        public PrimitiveProcedure(Func<List<LispObject>, LispObject> func)
        {
            this.func = func;
        }

        public override LispObject Apply(List<LispObject> args)
        {
            return func(args);
        }

        public override string ToString()
        {
            return $"#Procedure{GetHashCode()}";
        }
    }
}

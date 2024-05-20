using Lisp.DataStructures;
using Lisp.DataStructures.Virtual;

namespace Lisp.Interpreter
{
    /// <summary>
    /// Implements Eval functionality.
    /// Any expression can be evaluated given an environment,
    /// which contains values assigned to variables.
    /// </summary>
    public class Evaluator
    {
        public static LispObject Eval(LispObject o, Environment env)
        {
            try
            {
                return EvalInternal(o, env);
            }
            catch (LispException ex)
            {
                throw new EvalException("Evaluation failed", o, ex);
            }
        }

        private static LispObject EvalInternal(LispObject o, Environment env)
        {
            if (o is Symbol s)
            {
                return env.LookUp(s);
            }
            else if (o is ConsCell c)
            {
                if (!List.IsList(c.Cdr))
                    throw new EvalException("Expression is not a list", o);
                var first = Eval(c.Car, env);
                var args = List.FromLisp(c.Cdr);
                if (first is Procedure proc)
                    return proc.Apply(args.Select(x => Eval(x, env)).ToList());
                else if (first is SpecialForm specForm)
                    return specForm.Apply(args, env);
                throw new EvalException($"Object is not callable.", o);
            }
            return o;
        }
    }
}

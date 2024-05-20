using Lisp.DataStructures;
using Lisp.DataStructures.Virtual;

namespace Lisp.Interpreter
{
    public record class UserProcedure : Procedure
    {
        private LispObject parameters;
        private LispObject body;
        private Environment environment;

        public UserProcedure(LispObject parameters, LispObject body, Environment environment)
        {
            this.parameters = parameters;
            this.body = body;
            this.environment = environment;
        }

        public override LispObject Apply(List<LispObject> args)
        {
            var localEnv = new Environment { Parent = environment };
            MatchParameters(parameters, args, localEnv);
            LispObject result = Nil.Instance;
            foreach (var expr in List.FromLisp(body))
            {
                result = Evaluator.Eval(expr, localEnv);
            }
            return result;
        }

        private void MatchParameters(LispObject formal, IEnumerable<LispObject> actual, Environment env)
        {
            while (formal is ConsCell cell && actual.Any())
            {
                if (cell.Car is not Symbol s)
                    throw new EvalException("Invalid lambda parameters", this);
                env.Assign(s, actual.First());
                actual = actual.Skip(1);
                formal = cell.Cdr;
            }
            if (formal is Symbol s1)
            {
                env.Assign(s1, List.ToLisp(actual));
            }
            else if (formal is not Nil || actual.Any())
            {
                throw new EvalException("Mismatched arguments in user procedure", this);
            }
        }
    }
}

using Lisp.DataStructures;
using Lisp.DataStructures.Virtual;
using Lisp.Intepreter;

namespace Lisp.Interpreter
{
    /// <summary>
    /// Provides most basic functions for the interpreter.
    /// Most of them can't be implemented as library functions.
    /// For other built-in functions see stdlib.lisp
    /// </summary>
    public class InitialEnvironment
    {
        public static Environment CreateInitialEnvironment()
        {
            var env = new Environment();

            Assign("+", new PrimitiveProcedure(NumberOperator((x, y) => x + y, "+")), env);
            Assign("-", new PrimitiveProcedure(NumberOperator((x, y) => x - y, "-")), env);
            Assign("*", new PrimitiveProcedure(NumberOperator((x, y) => x * y, "*")), env);
            Assign("/", new PrimitiveProcedure(NumberOperator((x, y) =>
                y == 0 ? throw new PrimitiveException("Division by 0", "/") : x / y, "/")), env);
            Assign("%", new PrimitiveProcedure(NumberOperator((x, y) => x % y, "%")), env);
            Assign("=", new PrimitiveProcedure(NumberPredicate((x, y) => x == y, "=")), env);
            Assign(">=", new PrimitiveProcedure(NumberPredicate((x, y) => x >= y, ">=")), env);
            Assign("<=", new PrimitiveProcedure(NumberPredicate((x, y) => x <= y, "<=")), env);
            Assign(">", new PrimitiveProcedure(NumberPredicate((x, y) => x > y, ">")), env);
            Assign("<", new PrimitiveProcedure(NumberPredicate((x, y) => x < y, "<")), env);

            Assign("number?", new PrimitiveProcedure(IsType<Number>("number?")), env);
            Assign("pair?", new PrimitiveProcedure(IsType<ConsCell>("pair?")), env);
            Assign("nil?", new PrimitiveProcedure(IsType<Nil>("nil?")), env);
            Assign("procedure?", new PrimitiveProcedure(IsType<Procedure>("procedure?")), env);
            Assign("symbol?", new PrimitiveProcedure(IsType<Symbol>("symbol?")), env);
            Assign("atom?", new PrimitiveProcedure(IsType<Atom>("atom?")), env);
            Assign("bool?", new PrimitiveProcedure(IsType<Bool>("bool?")), env);
            Assign("eq?", new PrimitiveProcedure(SymbolEq), env);
            Assign("apply", new PrimitiveProcedure(Apply), env);

            Assign("print-char", new PrimitiveProcedure(PrintChar), env);
            Assign("to-string", new PrimitiveProcedure(ToString), env);
            Assign("print-native", new PrimitiveProcedure(PrintString), env);

            Assign("nil", Nil.Instance, env);
            Assign("#t", Bool.TrueInstance, env);
            Assign("#f", Bool.FalseInstance, env);
            Assign("cons", new PrimitiveProcedure(Cons), env);
            Assign("car", new PrimitiveProcedure(Car), env);
            Assign("cdr", new PrimitiveProcedure(Cdr), env);
            Assign("eval", new PrimitiveSpecialForm(EvalForm), env);

            Assign("lambda", new PrimitiveSpecialForm(MakeUserProcedure), env); // TODO: handle exceptions thrown by eval for better error reporting
            Assign("define", new PrimitiveSpecialForm(Define), env);
            Assign("if", new PrimitiveSpecialForm(IfThenElse), env);
            Assign("cond", new PrimitiveSpecialForm(Cond), env);
            Assign("let", new PrimitiveSpecialForm(Let), env);
            Assign("and", new PrimitiveSpecialForm(And), env);
            Assign("or", new PrimitiveSpecialForm(Or), env);
            Assign("quote", new PrimitiveSpecialForm(Quote), env);
            Assign("begin", new PrimitiveSpecialForm(Begin), env);
            return env;
        }

        private static void Assign(string name, LispObject value, Environment env)
        {
            env.Assign(new Symbol(name), value);
        }

        private static Func<List<LispObject>, LispObject> NumberOperator(Func<int, int, int> op, string opName)
        {
            return args =>
            {
                EnsureArity(2, args, opName);
                if (args.Count == 2 && args[0] is Number l && args[1] is Number r)
                    return new Number(op(l.Value, r.Value));
                throw new PrimitiveException("Expected 2 arguments of type number", opName);
            };
        }

        private static Func<List<LispObject>, LispObject> NumberPredicate(Func<int, int, bool> op, string opName)
        {
            return args =>
            {
                EnsureArity(2, args, opName);
                if (args.Count == 2 && args[0] is Number l && args[1] is Number r)
                    return new Bool(op(l.Value, r.Value));
                throw new PrimitiveException("Expected 2 arguments of type number", opName);
            };
        }

        private static LispObject Cons(List<LispObject> args)
        {
            EnsureArity(2, args, "cons");
            return new ConsCell(args[0], args[1]);
        }

        private static LispObject Car(List<LispObject> args)
        {
            EnsureArity(1, args, "car");
            if (args[0] is not ConsCell cell)
                throw new PrimitiveException("Expected argument of type pair", "car");
            return cell.Car;
        }

        private static LispObject Cdr(List<LispObject> args)
        {
            EnsureArity(1, args, "cdr");
            if (args[0] is not ConsCell cell)
                throw new PrimitiveException("Expected argument of type pair", "cdr");
            return cell.Cdr;
        }

        private static LispObject ToString(List<LispObject> args)
        {
            EnsureArity(1, args, "print-object");
            return DataStructures.Virtual.String.ToLisp(args[0].ToString());
        }

        private static LispObject PrintChar(List<LispObject> args)
        {
            EnsureArity(1, args, "print-char");
            if (args[0] is not Number n)
                throw new PrimitiveException("Expected argument of type number", "print-char");

            Console.Write((char)n.Value);
            return Nil.Instance;
        }

        private static LispObject PrintString(List<LispObject> args)
        {
            EnsureArity(1, args, "print-native");
            if (!DataStructures.Virtual.String.IsString(args[0]))
                throw new PrimitiveException("Expected argument of type string", "print-native");
            Console.WriteLine(DataStructures.Virtual.String.FromLisp(args[0]));
            return Nil.Instance;
        }

        private static LispObject SymbolEq(List<LispObject> args)
        {
            EnsureArity(2, args, "eq?");
            if (args[0] is not Symbol s1 || args[1] is not Symbol s2)
                throw new PrimitiveException("Expected argument of type symnbol", "eq?");

            return new Bool(s1.Name == s2.Name);
        }

        private static LispObject Apply(List<LispObject> args)
        {
            EnsureArity(2, args, "apply");

            if (args[0] is not Procedure proc)
                throw new PrimitiveException("First argument is not callable", "apply");
            if (!List.IsList(args[1]))
                throw new PrimitiveException("Second argument is not a list", "apply");
            return proc.Apply(List.FromLisp(args[1]));
        }

        private static UserProcedure MakeUserProcedure(List<LispObject> args, Environment env)
        {
            return new UserProcedure(args.First(), List.ToLisp(args.Skip(1)), env);
        }

        private static Nil Define(List<LispObject> args, Environment env)
        {
            EnsureArity(2, args, "define");
            if (args[0] is Symbol s)
            {
                env.Assign(s, Evaluator.Eval(args[1], env));
                return Nil.Instance;
            }
            throw new PrimitiveException("Argument mismatch", "define");
        }

        private static LispObject IfThenElse(List<LispObject> args, Environment env)
        {
            if (args.Count < 2 || args.Count > 3)
            {
                throw new PrimitiveException("Argument mismatch", "define");
            }
            var pred = Evaluator.Eval(args[0], env);
            if (pred is not Bool b || b.Value)
            {
                return Evaluator.Eval(args[1], env);
            }
            else if (args.Count == 3)
            {
                return Evaluator.Eval(args[2], env);
            }
            return Nil.Instance;

        }

        private static LispObject Let(List<LispObject> args, Environment env)
        {
            EnsureArity(2, args, "let");
            var bindings = args[0];
            var expression = args[1];
            if (!List.IsList(bindings))
                throw new PrimitiveException("Expected 2 arguments", "let");

            var environment = new Environment() { Parent = env };
            foreach (var def in List.FromLisp(bindings))
            {
                if (!List.IsList(def))
                    throw new PrimitiveException("Expected list of bindings", "let");
                var binding = List.FromLisp(def);
                if (binding.Count != 2 || binding[0] is not Symbol s)
                    throw new PrimitiveException("Expected symbol and its value in bindings", "let");

                environment.Assign(s, Evaluator.Eval(binding[1], environment));
            }

            return Evaluator.Eval(expression, environment);
        }

        private static LispObject Cond(List<LispObject> args, Environment env)
        {
            foreach (var condCase in args)
            {
                if (!List.IsList(condCase))
                    throw new PrimitiveException("Invalid cond arguments", "cond");
                var condCaseList = List.FromLisp(condCase);
                if (Evaluator.Eval(condCaseList[0], env) is not Bool b || b.Value)
                    return Evaluator.Eval(condCaseList[1], env);
            }
            return Nil.Instance;
        }

        private static LispObject And(List<LispObject> args, Environment env)
        {
            foreach (var arg in args)
            {
                var value = Evaluator.Eval(arg, env);
                if (value is Bool b && !b.Value)
                    return value;
            }
            return Bool.TrueInstance;
        }


        private static LispObject Or(List<LispObject> args, Environment env)
        {
            foreach (var arg in args)
            {
                var value = Evaluator.Eval(arg, env);
                if (value is not Bool b || b.Value)
                    return value;
            }
            return Bool.FalseInstance;
        }

        private static LispObject Quote(List<LispObject> args, Environment env)
        {
            EnsureArity(1, args, "quote");
            return args[0];
        }

        private static LispObject Begin(List<LispObject> args, Environment env)
        {
            LispObject returnValue = Nil.Instance;
            foreach (var arg in args)
            {
                returnValue = Evaluator.Eval(arg, env);
            }
            return returnValue;
        }
        private static LispObject EvalForm(List<LispObject> args, Environment env)
        {
            EnsureArity(1, args, "eval");
            return Evaluator.Eval(Evaluator.Eval(args[0], env), env);
        }

        private static Func<List<LispObject>, LispObject> IsType<T>(string predicateName)
        {
            return args =>
            {
                EnsureArity(1, args, predicateName);
                if (args[0] is T)
                    return Bool.TrueInstance;
                return Bool.FalseInstance;
            };
        }

        private static void EnsureArity(int expectedArity, List<LispObject> args, string primitiveName)
        {
            if (args.Count != expectedArity)
                throw new PrimitiveException($"Expected {expectedArity} arguments but got {args.Count}", primitiveName);
        }
    }
}

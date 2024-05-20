using Lisp.DataStructures;
using Lisp.DataStructures.Virtual;
using Lisp.Parser;

namespace Lisp.Interpreter
{
    /// <summary>
    /// Takes programs as input (either filename or code contained in string) and evaluates them
    /// </summary>
    public class Interpreter
    {
        private readonly Environment environment;

        private Interpreter(Environment environment)
        {
            this.environment = environment;
        }

        public static async Task<Interpreter> CreateInterpreter()
        {
            var initialEnvironment = InitialEnvironment.CreateInitialEnvironment();
            var stdlibEnvironment = new Environment() { Parent = initialEnvironment };
            var interpreter = new Interpreter(stdlibEnvironment);

            var stdlibPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Interpreter/stdlib.lisp");
            await interpreter.RunFromFile(stdlibPath);

            return interpreter;
        }

        public async Task RunFromUserString(string program, string directory)
        {
            try
            {
                await RunFromString(program, "<user-program>", directory);
            }
            catch (ParseException ex)
            {
                Console.WriteLine(ex);
            }
            catch (LispException ex)
            {
                Console.WriteLine(ex.GetTrace());
            }
        }

        public async Task RunFromFile(string file, string? overrideName = null)
        {
            try
            {
                var program = await File.ReadAllTextAsync(file);
                await RunFromString(program, overrideName ?? file, Path.GetDirectoryName(file) ?? "");
            }
            catch (ParseException ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            catch (LispException ex)
            {
                Console.WriteLine(ex.GetTrace());
                throw;
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error loading file {file}: {ex.Message}");
                throw;
            }
        }

        private async Task RunFromString(string program, string filename, string directory)
        {
            using var programParser = new Parser.Parser(program, filename);
            await RunParsed(programParser.ParseProgram(), directory);
        }

        private async Task RunParsed(List<LispObject> program, string directory)
        {

            foreach (var expression in program)
            {
                await EvalTopLevelExpression(expression, directory);
            }
        }

        private async Task EvalTopLevelExpression(LispObject expression, string directory)
        {
            if (IsRequire(expression))
                await Require(expression, directory);
            else
                Evaluator.Eval(expression, environment);
        }

        private bool IsRequire(LispObject expression)
        {
            return List.IsList(expression)
                && List.FromLisp(expression).FirstOrDefault() is Symbol s
                && s.Name == "require";
        }

        private async Task Require(LispObject expression, string directory)
        {
            var args = List.FromLisp(expression).Skip(1);
            if (!args.All(IsUnevaluatedString))
                throw new PrimitiveException("Expected file-paths as list of strings", "require");

            var files = args.Select(ExtractString);
            await Task.WhenAll(files.Select(file => RunFromFile(Path.Join(directory, file))));
        }

        private bool IsUnevaluatedString(LispObject expression)
        {
            if (!List.IsList(expression))
                return false;
            var lst = List.FromLisp(expression);
            return lst[0] == new Symbol("quote") && DataStructures.Virtual.String.IsString(lst[1]);
        }
        private string ExtractString(LispObject expression)
        {
            return DataStructures.Virtual.String.FromLisp(List.FromLisp(expression)[1]);
        }
    }
}

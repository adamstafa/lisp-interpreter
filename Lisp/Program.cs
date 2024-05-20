using Lisp.Interpreter;
using Lisp.Parser;

if (args.Length != 1)
{
    Console.WriteLine("Expected a single file to interpret as argument");
    return -1;
}

try
{
    var interpreter = await Interpreter.CreateInterpreter();
    await interpreter.RunFromFile(args[0]);
    return 0;
}
catch (Exception ex)
{
    if (ex is IOException or LispException or ParseException)
        return -1;
    throw;
}


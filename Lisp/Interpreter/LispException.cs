using Lisp.DataStructures;

namespace Lisp.Interpreter
{
    public abstract class LispException : Exception
    {
        protected LispException(string message, LispException? innerException) : base(message, innerException) { }

        public string GetTrace()
        {
            var innerTrace = (InnerException as LispException)?.GetTrace();
            if (innerTrace != null)
                return $"{innerTrace}\n{this.ToString()}";
            return this.ToString();
        }
    }

    public class EvalException : LispException
    {
        public readonly LispObject expression;

        public EvalException(string message, LispObject expression, LispException? innerException = null) : base(message, innerException)
        {
            this.expression = expression;
        }

        public override string ToString()
        {
            return $"Error: {Message}\n  in expression: {expression}";
        }
    }
    public class PrimitiveException : LispException
    {
        public readonly string primitiveName;

        public PrimitiveException(string message, string primitiveName, LispException? innerException = null) : base(message, innerException)
        {
            this.primitiveName = primitiveName;
        }

        public override string ToString()
        {
            return $"Error: {Message}\n  in primitive procedure/special form {primitiveName}";
        }
    }
}

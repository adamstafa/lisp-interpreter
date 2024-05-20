namespace Lisp.DataStructures
{
    /// <summary>
    /// Type of a function. The special form decides itself what arguments to evalute.
    /// </summary>
    public abstract record class SpecialForm : LispObject
    {
        public abstract LispObject Apply(List<LispObject> args, Interpreter.Environment env);

        public override string ToString()
        {
            return $"#procedure{GetHashCode()}";
        }
    }
}

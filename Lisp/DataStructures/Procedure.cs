namespace Lisp.DataStructures
{
    /// <summary>
    /// Type of a function. All arguments are evaluated before applying
    /// </summary>
    public abstract record class Procedure : LispObject
    {
        public override string ToString()
        {
            return $"#Procedure{GetHashCode()}";
        }

        public abstract LispObject Apply(List<LispObject> args);
    }
}

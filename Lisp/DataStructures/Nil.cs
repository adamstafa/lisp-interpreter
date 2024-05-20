namespace Lisp.DataStructures
{
    public record class Nil : Atom
    {
        public override string ToString()
        {
            return "()";
        }

        public static readonly Nil Instance = new Nil();
    }
}

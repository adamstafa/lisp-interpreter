using Lisp.DataStructures.Virtual;

namespace Lisp.DataStructures
{
    public record class ConsCell : LispObject
    {
        public LispObject Car { get; set; }

        public LispObject Cdr { get; set; }

        public ConsCell(LispObject car, LispObject cdr)
        {
            Car = car;
            Cdr = cdr;
        }

        public override string ToString()
        {
            if (List.IsList(this))
                return List.AsString(this);
            return $"({Car} . {Cdr})";
        }
    }
}

namespace Lisp.DataStructures
{
    public record class Bool : Atom
    {
        public bool Value { get; set; }

        public Bool(bool value)
        {
            Value = value;
        }

        public override string ToString()
        {
            if (Value)
                return "#t";
            return "#f";
        }

        public static readonly Bool TrueInstance = new Bool(true);
        public static readonly Bool FalseInstance = new Bool(false);
    }
}

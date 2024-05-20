namespace Lisp.DataStructures
{
    public record class Number : Atom
    {
        public int Value { get; set; }

        public Number(int value)
        {
            Value = value;
        }

        public override string ToString()
        {
            if (Value >= 0)
                return $"{Value}";
            else return $"(- {-Value})";
        }
    }
}

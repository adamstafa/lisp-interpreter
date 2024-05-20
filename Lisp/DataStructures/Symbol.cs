namespace Lisp.DataStructures
{
    /// <summary>
    /// Symbols are used as names for bindings/variables.
    /// </summary>
    public record class Symbol : Atom
    {
        public string Name { get; set; }

        public Symbol(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}

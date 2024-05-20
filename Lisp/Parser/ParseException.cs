namespace Lisp.Parser
{
    public class ParseException : Exception
    {
        public int Row { get; }
        public int Col { get; }
        public string Filename { get; }
        public ParseException(string message, string filename, int row, int col) : base(message)
        {
            Filename = filename;
            Row = row;
            Col = col;
        }

        public override string ToString()
        {
            return $"{Filename}:{Row}:{Col}: syntax error, {Message}";
        }
    }
}

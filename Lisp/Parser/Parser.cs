using System.Text;
using Lisp.DataStructures;
using Lisp.DataStructures.Virtual;

namespace Lisp.Parser
{
    /// <summary>
    /// Transforms a string into AST Composed of classes from DataStructures namespace.
    /// </summary>
    public class Parser : IDisposable
    {
        private readonly string filename;
        private int row = 1;
        private int col = 1;
        private readonly IEnumerator<char> chars;
        private bool hasNext;

        public Parser(string program, string filename)
        {
            this.filename = filename;
            chars = program.GetEnumerator();
            hasNext = chars.MoveNext();
        }


        public List<LispObject> ParseProgram()
        {
            var program = new List<LispObject>();
            while(hasNext)
                program.Add(ParseExpression());
            ParseEOF();
            return program;
        }

        private LispObject ParseExpression()
        {
            ParseWhiteSpaces();
            try
            {
                if (LookAhead('('))
                    return ParseList();
                else if (char.IsDigit(LookAhead()))
                    return ParseNumber();
                else if (LookAhead('"'))
                    return ParseString();
                else if (LookAhead('\''))
                    return ParseQuote();
                else
                    return ParseSymbol();
            }
            finally
            {
                ParseWhiteSpaces();
            }
        }

        private LispObject ParseList()
        {
            ParseChar('(');
            ParseWhiteSpaces();
            if (LookAhead(')'))
            {
                MoveNext();
                return Nil.Instance;
            }
            else
            {
                var car = ParseExpression();
                if (LookAhead('.'))
                {
                    MoveNext();
                    var cdr = ParseExpression();
                    ParseChar(')');
                    return new ConsCell(car, cdr);
                }
                else
                {
                    var cdr = ParseListTail();
                    return new ConsCell(car, cdr);
                }
            }
        }

        private LispObject ParseListTail()
        {
            ParseWhiteSpaces();
            if (LookAhead(')'))
            {
                MoveNext();
                ParseWhiteSpaces();
                return Nil.Instance;
            }
            var car = ParseExpression();
            return new ConsCell(car, ParseListTail());
        }

        private LispObject Quoted(LispObject obj)
        {
            return List.ToLisp(new[] { new Symbol("quote"), obj });
        }

        private LispObject ParseQuote()
        {
            ParseChar('\'');
            var obj = ParseExpression();
            return Quoted(obj);
        }


        private LispObject ParseString()
        {
            ParseChar('"');
            var chars = new StringBuilder();
            while (hasNext && !LookAhead('"'))
            {
                if (LookAhead('\\'))
                    chars.Append(ParseEscapedChar());
                else
                    chars.Append(ParseChar());
            }
            ParseChar('"');
            return Quoted(DataStructures.Virtual.String.ToLisp(chars.ToString()));
        }

        private char ParseEscapedChar()
        {
            ParseChar('\\');
            char c = '\0';
            if (chars.Current == 'n')
                c = '\n';
            else if (chars.Current == 't')
                c = '\t';
            else if (chars.Current == '\'')
                c = '\'';
            else if (chars.Current == '"')
                c = '"';
            else
                ThrowException($"Unknown escape sequence '\\{chars.Current}");
            MoveNext();
            return c;
        }

        private char ParseChar()
        {
            var c = chars.Current;
            MoveNext();
            return c;
        }

        private Number ParseNumber()
        {
            var sb = new StringBuilder();
            if (!char.IsDigit(LookAhead()))
                ThrowException("Expected a digit");

            while (hasNext && char.IsDigit(LookAhead()))
            {
                sb.Append(LookAhead());
                MoveNext();
            }

            if (hasNext)
            {
                var c = LookAhead();
                if (!(char.IsWhiteSpace(LookAhead()) || LookAhead() == ')' || LookAhead() == '('))
                    ThrowException("Unexpected character in integer literal");
            }

            return new Number(int.Parse(sb.ToString()));
        }

        private static bool IsAllowedSymbolChar(char c)
        {
            return !char.IsControl(c)
                && !char.IsWhiteSpace(c)
                && c != '('
                && c != ')'
                && c != '"'
                && c != '\'';
        }
        private Symbol ParseSymbol()
        {
            var sb = new StringBuilder();
            var first = LookAhead();
            if (!IsAllowedSymbolChar(first) || char.IsDigit(first))
                ThrowException("Invalid symbol name");
            while (hasNext && IsAllowedSymbolChar(LookAhead()))
            {
                sb.Append(LookAhead());
                MoveNext();
            }
            return new Symbol(sb.ToString());
        }

        private bool MoveNext()
        {
            if (!hasNext)
                ThrowException("Unexpected EOF");
            col++;
            if (chars.Current == '\n')
            {
                col = 1;
                row++;
            }
            return hasNext = chars.MoveNext();
        }

        private void ParseWhiteSpaces()
        {
            while (hasNext && char.IsWhiteSpace(LookAhead()))
                MoveNext();
            if (hasNext && chars.Current == ';')
                ParseComment();
        }

        private void ParseComment()
        {
            ParseChar(';');
            while (hasNext && chars.Current != '\n')
                MoveNext();
            ParseWhiteSpaces();
        }

        private void ParseEOF()
        {
            if (hasNext)
                ThrowException("Expected EOF");
        }

        private void ParseChar(char c)
        {
            if (!LookAhead(c))
                ThrowException($"Expected {c}");
            MoveNext();
        }

        private char LookAhead()
        {
            if (!hasNext)
                ThrowException("Unexpected EOF");
            return chars.Current;
        }

        private bool LookAhead(char c)
        {
            return LookAhead() == c;
        }

        private void ThrowException(string message)
        {
            throw new ParseException(message, filename, row, col);
        }

        public void Dispose()
        {
            chars.Dispose();
        }
    }
}

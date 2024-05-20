using System.Collections.Generic;
using System.Collections;
using Lisp.DataStructures;
using Lisp.DataStructures.Virtual;

namespace Lisp.Tests
{
    public class ParserTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { "123", new Number(123) };
            yield return new object[] { "(- 123)", List.ToLisp(new LispObject[] { new Symbol("-"), new Number(123) }) };
            yield return new object[] { "0", new Number(0) };
            yield return new object[] { "0", new Number(0) };
            yield return new object[] { "abc", new Symbol("abc") };
            yield return new object[] { "a0123b", new Symbol("a0123b") };
            yield return new object[] { "xd?cd", new Symbol("xd?cd") };
            yield return new object[] { "a0123b", new Symbol("a0123b") };
            yield return new object[] { "(1)", List.ToLisp(new[] { new Number(1) }) };
            yield return new object[] { "(  1  )", List.ToLisp(new[] { new Number(1) }) };
            yield return new object[] { "  (1)  ", List.ToLisp(new[] { new Number(1) }) };
            yield return new object[] { "(1 a)", List.ToLisp(new LispObject[] { new Number(1), new Symbol("a") }) };
            yield return new object[] { "(a . b)", new ConsCell(new Symbol("a"), new Symbol("b")) };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}

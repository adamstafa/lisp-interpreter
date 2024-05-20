using Lisp.Parser;
using Xunit;


namespace Lisp.Tests
{
    public class ParserTests
    {
        [Theory]
        [ClassData(typeof(ParserTestData))]
        public void ParserTest(string input, object expected)
        {
            var parser = new Parser.Parser(input, "test");
            var result = parser.ParseProgram()[0];
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ParseValidProgram()
        {
            var program =
@"(defun factorial (n &optional (acc 1))
    (if (zerop n) acc
        (factorial (-  1 n) (* acc n))))";
            var parser = new Parser.Parser(program, "test");
            var result = parser.ParseProgram();
            Assert.NotEmpty(result.ToString());
        }
        [Fact]
        public void RejectInvalidProgram()
        {
            var program =
@"(defun factorial (n &optional (acc 1))
    (if (zerop n) acc
        (factorial (-) 1 n) (* acc n))))";
            var parser = new Parser.Parser(program, "test");
            Assert.Throws<ParseException>(parser.ParseProgram);
        }
    }
}

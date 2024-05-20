namespace Lisp.DataStructures.Virtual
{
    /// <summary>
    /// Strings are implemented as lists of numbers.
    /// The numbers correspond to the codes of the characters of the string.
    /// </summary>
    public static class String
    {
        public static LispObject ToLisp(string str)
        {
            return List.ToLisp(str.Select(c => new Number(c)));
        }

        public static bool IsString(LispObject str)
        {
            if (!List.IsList(str))
                return false;
            var list = List.FromLisp(str);
            return list.All(x => x.GetType() == typeof(Number));
        }

        public static string FromLisp(LispObject str)
        {
            if (!IsString(str))
                throw new ArgumentException(nameof(str));
            return new string(List.FromLisp(str).Select(x => (char) ((Number)x).Value).ToArray());
        }
    }
}

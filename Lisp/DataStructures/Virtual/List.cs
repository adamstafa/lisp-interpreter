namespace Lisp.DataStructures.Virtual
{
    /// <summary>
    /// Lisp Lists are linked lists made of ConsCell and Nil
    /// </summary>
    public static class List
    {
        public static LispObject ToLisp(IEnumerable<LispObject> objects)
        {
            var reversed = objects.ToList();
            reversed.Reverse();
            LispObject result = Nil.Instance;
            foreach (var e in reversed)
            {
                result = new ConsCell(e, result);
            }
            return result;
        }

        public static bool IsList(LispObject ex)
        {
            if (ex is Nil)
                return true;
            if (ex is ConsCell c)
                return IsList(c.Cdr);
            return false;
        }

        public static List<LispObject> FromLisp(LispObject list)
        {
            if (!IsList(list))
                throw new ArgumentException(nameof(list));

            var result = new List<LispObject>();
            while (list is not Nil)
            {
                result.Add(((ConsCell)list).Car);
                list = ((ConsCell)list).Cdr;
            }
            return result;
        }

        public static string AsString(LispObject list)
        {
            if (!IsList(list))
                throw new ArgumentException(nameof(list));
            return "(" + string.Join(" ", FromLisp(list)) + ")";
        }
    }
}

namespace NetGrab.Extensions
{
    static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string s)
        {
            return s == null || s.Equals(string.Empty);
        }
    }
}

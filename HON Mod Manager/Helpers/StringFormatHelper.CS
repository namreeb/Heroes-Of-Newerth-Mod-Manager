namespace HON_Mod_Manager.Helpers
{
    internal static class StringFormatHelper
    {
        public static string RemoveNonDigits(string s)
        {
            if (s == "*") return int.MinValue.ToString();
            var myOutput = "";
            for (var i = 0; i <= s.Length - 1; i++)
                if (s[i] >= '0' && s[i] <= '9')
                    myOutput += s[i];
            return myOutput;
        }
    }
}
using static HON_Mod_Manager.Helpers.StringFormatHelper;

namespace HON_Mod_Manager.Helpers
{
    internal static class ArrayParseHelper
    {
        public static int[] StrArrayToIntArray(string[] s)
        {
            var myOutput = new int[s.Length];
            for (var i = 0; i < s.Length; i++)
            {
                int j;
                if (int.TryParse(RemoveNonDigits(s[i]), out j))
                    myOutput[i] = j;
                else
                    myOutput[i] = 0;
            }

            return myOutput;
        }
    }
}
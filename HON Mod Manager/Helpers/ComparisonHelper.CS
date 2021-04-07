using System;

namespace HON_Mod_Manager.Helpers
{
    internal static class ComparisonHelper
    {
        public static bool IsNewerVersion(string Old, string New)
        {
            if (Old == "") Old = "0";
            if (New == "") New = "0";

            var OldParts = ArrayParseHelper.StrArrayToIntArray(Old.Split('.'));
            var NewParts = ArrayParseHelper.StrArrayToIntArray(New.Split('.'));
            for (var i = 0; i < Math.Min(OldParts.Length, NewParts.Length); i++)
                if (OldParts[i] != NewParts[i])
                    return NewParts[i] > OldParts[i];
            if (OldParts.Length != NewParts.Length) return NewParts.Length > OldParts.Length;
            return false;
            //if the version strings are the same, return true
        }
    }
}
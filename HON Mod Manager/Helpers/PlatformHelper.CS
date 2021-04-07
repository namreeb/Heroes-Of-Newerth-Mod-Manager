using System;
using System.IO;

namespace HON_Mod_Manager.Helpers
{
    internal static class PlatformHelper
    {
        private static bool IsWindows()
        {
            return Environment.OSVersion.Platform == PlatformID.Win32NT;
            //includes XP, Vista and 7
        }

        public static bool IsLinux()
        {
            if (Environment.OSVersion.Platform != PlatformID.Unix) return false;
            //make sure mono isn't fooling us!
            try
            {
                return
                    !Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                        "Library/Application Support"));
            }
            catch
            {
                return true;
            }
        }

        public static bool IsMacOS()
        {
            if (Environment.OSVersion.Platform == PlatformID.MacOSX)
                return true;

            //might still be mac in disguise!
            if (Environment.OSVersion.Platform != PlatformID.Unix)
                return false;
            try
            {
                return
                    Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                        "Library/Application Support"));
            }
            catch
            {
                return false;
            }
        }
    }
}
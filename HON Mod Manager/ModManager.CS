using System;
using System.Windows.Forms;

namespace HON_Mod_Manager
{
    internal static class ModManager
    {
        [STAThread]
        private static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ModManagerForm());
        }
    }
}
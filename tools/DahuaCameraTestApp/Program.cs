using System;
using System.Windows.Forms;

namespace DahuaCameraTestApp
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new DahuaTestForm());
        }
    }
}

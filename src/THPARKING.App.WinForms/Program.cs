using System;
using System.Windows.Forms;
using THPARKING.App.WinForms.Bootstrap;

namespace THPARKING.App.WinForms
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ParkingAppRuntime runtime = null;

            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                var bootstrapper = new ParkingAppBootstrapper();

                runtime = bootstrapper.BuildDefaultRuntime(
                    operatorCode: "admin",
                    machineName: Environment.MachineName);

                runtime.Start();

                Application.Run(new Form1(runtime));
            }
            finally
            {
                if (runtime != null)
                {
                    runtime.Dispose();
                }
            }
        }
    }
}
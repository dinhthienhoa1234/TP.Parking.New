using System;
using THPARKING.App.WinForms.Bootstrap;

namespace THPARKING.RuntimeSmoke
{
    internal static class Program
    {
        private static int Main()
        {
            ParkingAppRuntime runtime = null;

            try
            {
                var bootstrapper = new ParkingAppBootstrapper();
                runtime = bootstrapper.BuildDefaultRuntime(
                    operatorCode: "smoke-test",
                    machineName: Environment.MachineName);

                if (runtime == null)
                    throw new InvalidOperationException("Runtime is null.");

                if (runtime.Context == null)
                    throw new InvalidOperationException("Runtime.Context is null.");

                if (runtime.Orchestrator == null)
                    throw new InvalidOperationException("Runtime.Orchestrator is null.");

                if (runtime.LicenseGate == null)
                    throw new InvalidOperationException("Runtime.LicenseGate is null.");

                if (runtime.SyncOutboxService == null)
                    throw new InvalidOperationException("Runtime.SyncOutboxService is null.");

                if (runtime.SyncWorker == null)
                    throw new InvalidOperationException("Runtime.SyncWorker is null.");

                runtime.Start();

                Console.WriteLine("Runtime smoke test passed.");
                return 0;
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine(exception.ToString());
                return 1;
            }
            finally
            {
                if (runtime != null)
                    runtime.Dispose();
            }
        }
    }
}

using System;
using System.Timers;

namespace THPARKING.Licensing.Core
{
    public class LicenseWorker : IDisposable
    {
        private readonly LicenseGate _licenseGate;
        private readonly Timer _timer;

        public LicenseWorker(LicenseGate licenseGate)
        {
            _licenseGate = licenseGate ?? throw new ArgumentNullException(nameof(licenseGate));

            _timer = new Timer();
            _timer.Interval = TimeSpan.FromMinutes(30).TotalMilliseconds;
            _timer.Elapsed += OnTimerElapsed;
        }

        public event EventHandler<LicenseCheckResult> LicenseChecked;

        public void Start()
        {
            _timer.Start();
            RaiseLicenseChecked(_licenseGate.Check());
        }

        public void Stop()
        {
            _timer.Stop();
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            RaiseLicenseChecked(_licenseGate.Check());
        }

        private void RaiseLicenseChecked(LicenseCheckResult result)
        {
            var handler = LicenseChecked;
            if (handler != null)
                handler(this, result);
        }

        public void Dispose()
        {
            Stop();
            _timer.Elapsed -= OnTimerElapsed;
            _timer.Dispose();
        }
    }
}
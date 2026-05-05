using System;
using System.IO;
using System.Text;
using THPARKING.Core.Enums;

namespace THPARKING.Licensing.Core
{
    public class LocalLicenseData
    {
        public string GmailOwner { get; set; }

        public string MachineId { get; set; }

        public string Fingerprint { get; set; }

        public LicenseMode Mode { get; set; }

        public DateTime ExpiredAt { get; set; }

        public DateTime LastCheckedAt { get; set; }

        public string SignedToken { get; set; }
    }

    public class LocalLicenseStore
    {
        private readonly string _filePath;

        public LocalLicenseStore(string filePath)
        {
            _filePath = filePath;
        }

        public LocalLicenseData Load()
        {
            if (string.IsNullOrWhiteSpace(_filePath))
                return null;

            if (!File.Exists(_filePath))
                return null;

            var lines = File.ReadAllLines(_filePath, Encoding.UTF8);

            var data = new LocalLicenseData();

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var parts = line.Split(new[] { '=' }, 2);
                if (parts.Length != 2)
                    continue;

                var key = parts[0].Trim();
                var value = parts[1].Trim();

                if (key == "GmailOwner")
                    data.GmailOwner = value;
                else if (key == "MachineId")
                    data.MachineId = value;
                else if (key == "Fingerprint")
                    data.Fingerprint = value;
                else if (key == "Mode")
                    data.Mode = ParseMode(value);
                else if (key == "ExpiredAt")
                    data.ExpiredAt = DateTime.Parse(value);
                else if (key == "LastCheckedAt")
                    data.LastCheckedAt = DateTime.Parse(value);
                else if (key == "SignedToken")
                    data.SignedToken = value;
            }

            return data;
        }

        public void Save(LocalLicenseData data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            var folder = Path.GetDirectoryName(_filePath);

            if (!string.IsNullOrWhiteSpace(folder) && !Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var builder = new StringBuilder();

            builder.AppendLine("GmailOwner=" + data.GmailOwner);
            builder.AppendLine("MachineId=" + data.MachineId);
            builder.AppendLine("Fingerprint=" + data.Fingerprint);
            builder.AppendLine("Mode=" + data.Mode);
            builder.AppendLine("ExpiredAt=" + data.ExpiredAt.ToString("o"));
            builder.AppendLine("LastCheckedAt=" + data.LastCheckedAt.ToString("o"));
            builder.AppendLine("SignedToken=" + data.SignedToken);

            File.WriteAllText(_filePath, builder.ToString(), Encoding.UTF8);
        }

        private static LicenseMode ParseMode(string value)
        {
            LicenseMode mode;
            if (Enum.TryParse(value, true, out mode))
                return mode;

            return LicenseMode.Unknown;
        }
    }
}
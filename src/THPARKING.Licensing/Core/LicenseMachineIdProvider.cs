using System;
using System.Security.Cryptography;
using System.Text;

namespace THPARKING.Licensing.Core
{
    public class LicenseMachineIdProvider
    {
        public string GetMachineId()
        {
            var raw = string.Join("|",
                Environment.MachineName,
                Environment.UserName,
                Environment.ProcessorCount,
                Environment.OSVersion.VersionString);

            return Sha256(raw);
        }

        private static string Sha256(string input)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(input ?? string.Empty);
                var hash = sha.ComputeHash(bytes);

                var builder = new StringBuilder();
                foreach (var b in hash)
                    builder.Append(b.ToString("x2"));

                return builder.ToString();
            }
        }
    }
}
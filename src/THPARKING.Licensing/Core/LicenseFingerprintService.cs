using System.Security.Cryptography;
using System.Text;

namespace THPARKING.Licensing.Core
{
    public class LicenseFingerprintService
    {
        public string CreateFingerprint(string gmailOwner, string machineId)
        {
            var raw = (gmailOwner ?? string.Empty).Trim().ToLowerInvariant()
                + "|"
                + (machineId ?? string.Empty).Trim().ToLowerInvariant();

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
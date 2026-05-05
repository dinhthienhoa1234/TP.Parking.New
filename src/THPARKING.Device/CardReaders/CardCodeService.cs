using System;

namespace THPARKING.Device.CardReaders
{
    public class CardCodeService
    {
        private readonly object _syncRoot = new object();

        private string _lastNormalizedCardCode;

        private DateTime _lastReadTime;

        public int DuplicateReadIntervalMilliseconds { get; set; } = 1500;

        public CardBusinessResult ProcessRawCode(string rawCardCode)
        {
            var normalized = Normalize(rawCardCode);

            if (string.IsNullOrWhiteSpace(normalized))
            {
                return CardBusinessResult.Fail(
                    rawCardCode,
                    "Mã thẻ rỗng hoặc không hợp lệ.");
            }

            lock (_syncRoot)
            {
                var now = DateTime.Now;

                if (string.Equals(_lastNormalizedCardCode, normalized, StringComparison.OrdinalIgnoreCase))
                {
                    var elapsed = now - _lastReadTime;

                    if (elapsed.TotalMilliseconds <= DuplicateReadIntervalMilliseconds)
                    {
                        return CardBusinessResult.Duplicate(rawCardCode, normalized);
                    }
                }

                _lastNormalizedCardCode = normalized;
                _lastReadTime = now;

                return CardBusinessResult.Ok(rawCardCode, normalized);
            }
        }

        public string Normalize(string rawCardCode)
        {
            if (string.IsNullOrWhiteSpace(rawCardCode))
                return null;

            return rawCardCode
                .Trim()
                .Replace("\r", string.Empty)
                .Replace("\n", string.Empty)
                .Replace("\t", string.Empty)
                .ToUpperInvariant();
        }

        public void ResetDuplicateCache()
        {
            lock (_syncRoot)
            {
                _lastNormalizedCardCode = null;
                _lastReadTime = DateTime.MinValue;
            }
        }
    }
}
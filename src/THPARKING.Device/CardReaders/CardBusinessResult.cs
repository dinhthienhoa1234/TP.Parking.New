namespace THPARKING.Device.CardReaders
{
    public class CardBusinessResult
    {
        public bool Accepted { get; private set; }

        public bool IsDuplicateRead { get; private set; }

        public string RawCardCode { get; private set; }

        public string NormalizedCardCode { get; private set; }

        public string Message { get; private set; }

        public static CardBusinessResult Ok(string rawCardCode, string normalizedCardCode)
        {
            return new CardBusinessResult
            {
                Accepted = true,
                RawCardCode = rawCardCode,
                NormalizedCardCode = normalizedCardCode,
                Message = "Mã thẻ hợp lệ."
            };
        }

        public static CardBusinessResult Duplicate(string rawCardCode, string normalizedCardCode)
        {
            return new CardBusinessResult
            {
                Accepted = false,
                IsDuplicateRead = true,
                RawCardCode = rawCardCode,
                NormalizedCardCode = normalizedCardCode,
                Message = "Bỏ qua do quẹt thẻ trùng trong thời gian ngắn."
            };
        }

        public static CardBusinessResult Fail(string rawCardCode, string message)
        {
            return new CardBusinessResult
            {
                Accepted = false,
                RawCardCode = rawCardCode,
                Message = message
            };
        }
    }
}
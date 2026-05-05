using System;

namespace THPARKING.Device.CardReaders
{
    public class CardReadResult
    {
        public bool Success { get; set; }

        public string RawCardCode { get; set; }

        public string NormalizedCardCode { get; set; }

        public string ReaderCode { get; set; }

        public string PortName { get; set; }

        public DateTime ReadTime { get; set; }

        public string ErrorMessage { get; set; }

        public static CardReadResult Ok(
            string rawCardCode,
            string normalizedCardCode,
            string readerCode,
            string portName)
        {
            return new CardReadResult
            {
                Success = true,
                RawCardCode = rawCardCode,
                NormalizedCardCode = normalizedCardCode,
                ReaderCode = readerCode,
                PortName = portName,
                ReadTime = DateTime.Now
            };
        }

        public static CardReadResult Fail(
            string readerCode,
            string portName,
            string errorMessage)
        {
            return new CardReadResult
            {
                Success = false,
                ReaderCode = readerCode,
                PortName = portName,
                ReadTime = DateTime.Now,
                ErrorMessage = errorMessage
            };
        }
    }
}
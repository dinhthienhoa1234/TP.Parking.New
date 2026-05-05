using System;

namespace THPARKING.Device.CardReaders
{
    public class CR208ReaderDevice : ICardReaderDevice
    {
        public CR208ReaderDevice(string readerCode, string portName)
        {
            ReaderCode = readerCode;
            PortName = portName;
        }

        public string ReaderCode { get; private set; }

        public string PortName { get; private set; }

        public bool IsOpen { get; private set; }

        public event EventHandler<CardReadResult> CardRead;

        public void Open()
        {
            IsOpen = true;
        }

        public void Close()
        {
            IsOpen = false;
        }

        protected void RaiseCardRead(CardReadResult result)
        {
            var handler = CardRead;
            if (handler != null)
                handler(this, result);
        }

        public void Dispose()
        {
            Close();
        }
    }
}
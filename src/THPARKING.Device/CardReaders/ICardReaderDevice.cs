using System;

namespace THPARKING.Device.CardReaders
{
    public interface ICardReaderDevice : IDisposable
    {
        string ReaderCode { get; }

        string PortName { get; }

        bool IsOpen { get; }

        event EventHandler<CardReadResult> CardRead;

        void Open();

        void Close();
    }
}
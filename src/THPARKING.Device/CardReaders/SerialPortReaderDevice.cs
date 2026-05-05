using System;
using System.IO.Ports;

namespace THPARKING.Device.CardReaders
{
    public class SerialPortReaderDevice : ICardReaderDevice
    {
        private readonly CardCodeService _cardCodeService;

        private SerialPort _serialPort;

        public SerialPortReaderDevice(
            string readerCode,
            string portName,
            int baudRate,
            CardCodeService cardCodeService)
        {
            if (string.IsNullOrWhiteSpace(readerCode))
                throw new ArgumentException("Reader code is required.", nameof(readerCode));

            if (string.IsNullOrWhiteSpace(portName))
                throw new ArgumentException("Port name is required.", nameof(portName));

            ReaderCode = readerCode;
            PortName = portName;
            BaudRate = baudRate <= 0 ? 9600 : baudRate;
            _cardCodeService = cardCodeService ?? new CardCodeService();
        }

        public string ReaderCode { get; private set; }

        public string PortName { get; private set; }

        public int BaudRate { get; private set; }

        public bool IsOpen
        {
            get { return _serialPort != null && _serialPort.IsOpen; }
        }

        public event EventHandler<CardReadResult> CardRead;

        public void Open()
        {
            if (IsOpen)
                return;

            _serialPort = new SerialPort(PortName, BaudRate);
            _serialPort.DataReceived += OnDataReceived;
            _serialPort.Open();
        }

        public void Close()
        {
            if (_serialPort == null)
                return;

            _serialPort.DataReceived -= OnDataReceived;

            if (_serialPort.IsOpen)
                _serialPort.Close();

            _serialPort.Dispose();
            _serialPort = null;
        }

        private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                var raw = _serialPort.ReadExisting();
                var businessResult = _cardCodeService.ProcessRawCode(raw);

                if (!businessResult.Accepted)
                {
                    RaiseCardRead(CardReadResult.Fail(
                        ReaderCode,
                        PortName,
                        businessResult.Message));

                    return;
                }

                RaiseCardRead(CardReadResult.Ok(
                    businessResult.RawCardCode,
                    businessResult.NormalizedCardCode,
                    ReaderCode,
                    PortName));
            }
            catch (Exception ex)
            {
                RaiseCardRead(CardReadResult.Fail(
                    ReaderCode,
                    PortName,
                    ex.Message));
            }
        }

        private void RaiseCardRead(CardReadResult result)
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
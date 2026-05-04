# Device Rules

## Card Reader

Supported card readers:

- CR501
- CR208
- SerialPort
- Other future devices

All card readers must implement:

- ICardReaderDevice

The UI must not call CR501, CR208, or SerialPort directly.

## Card Code

Card code processing must go through:

- CardCodeService

CardCodeService must:

- Normalize card id.
- Trim whitespace.
- Remove CR/LF.
- Reject empty card id.
- Reject invalid card id when configured.
- Ignore duplicate reads within configured interval.
- Return CardBusinessResult.

## Duplicate Read

Default duplicate read interval:

- 1500 ms

If the same card is read again within the duplicate interval, the system must ignore it.

## Serial Port

SerialPortReaderDevice must:

- Open configured COM port.
- Use configured baud rate.
- Raise CardRead event.
- Recover safely when COM port is disconnected.
- Log port open/close/error events.

## CR501

CR501ReaderDevice must:

- Wrap legacy CR501 code.
- Not expose legacy CR501 logic to UI.
- Raise CardRead event through ICardReaderDevice.
- Be replaceable by CR208ReaderDevice without changing business logic.

## CR208

CR208ReaderDevice must:

- Implement ICardReaderDevice.
- Follow the same CardReadResult format.
- Not require changes in UI or InOutBusinessService.

## Barrier

Barrier control must go through:

- IBarrierService

Rules:

- Do not open barrier before local database commit succeeds.
- Barrier opening must create AuditLog.
- Barrier failure must show warning but must not corrupt parking data.

## LED Display

LED display must go through:

- ILedDisplayService

Rules:

- Show fee after vehicle out calculation.
- Clear display when lane reset.
- LED failure must not block database save.

## Sound

Sound must go through:

- IParkingSoundService

Rules:

- Sound should be mapped from VehicleProcessStatus.
- Sound failure must not block vehicle operation.
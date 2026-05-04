# Sync Rules

## Storage Goal

Each vehicle in/out event should be stored in multiple places:

1. Server database
2. Entry machine local database
3. Exit machine local database or cache

The system must not depend on only one database to operate.

## Offline First

Vehicle operation must be offline-first.

Rules:

- Save local first.
- Create SyncOutbox event.
- Do not wait for server before responding to UI.
- Do not block barrier opening because server is offline.
- Sync to server in background.

## Vehicle In

Vehicle in flow:

1. Read card.
2. Capture images.
3. Recognize plate if ANPR is enabled.
4. Create ParkingSession locally.
5. Save ParkingImage locally.
6. Create SyncOutbox event VehicleEntered.
7. Commit local transaction.
8. Return success to UI.
9. Open barrier if allowed.
10. SyncWorker pushes event to server.

## Vehicle Out

Vehicle out lookup order:

1. Local exit machine database.
2. Server database.
3. Entry machine LAN API.
4. Emergency checkout.

Vehicle out flow:

1. Read card.
2. Find open ParkingSession.
3. Capture out images.
4. Calculate fee.
5. Update ParkingSession locally.
6. Save out images locally.
7. Create SyncOutbox event VehicleExited.
8. Commit local transaction.
9. Return result to UI.
10. Open barrier if allowed.
11. SyncWorker pushes event to server.

## SyncOutbox

Each important operation must create SyncOutbox event:

- VehicleEntered
- VehicleExited
- ImageSaved
- EmergencyExitCreated
- CardRegistered
- MonthlyPassUpdated
- LicenseEmergencyOut

Each event must have:

- EventId
- EventType
- AggregateId
- SourceNodeId
- Payload
- CreatedAt
- SyncStatus
- RetryCount
- LastError

## SyncInbox

SyncInbox is used for idempotency.

If an event has already been processed, it must not be applied again.

## Images

Images must sync after core event.

Priority:

1. VehicleEntered / VehicleExited event
2. ParkingImage metadata
3. Image file upload

Image sync failure must not block vehicle operation.

## Conflict

Conflict must be recorded when:

- Same card has two open sessions.
- VehicleExited arrives before VehicleEntered.
- Same session is updated by two nodes.
- Event payload conflicts with existing server data.

Conflict must be saved to ConflictLog.

## Emergency Exit

Emergency exit is used when no normal session can be found.

Emergency exit must:

- Capture images.
- Record card id if available.
- Record plate number if available.
- Record operator.
- Record reason.
- Create SyncOutbox event.
- Create AuditLog.
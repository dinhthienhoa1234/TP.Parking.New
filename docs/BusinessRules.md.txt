# Business Rules

## Vehicle In

- Card must be registered.
- Card must not be disabled.
- Card must not already have an open parking session.
- Vehicle in must save local database before opening barrier.
- Vehicle in must create SyncOutbox event.
- Vehicle in must create AuditLog.
- Duplicate plate must show warning.
- Monthly pass plate mismatch must show "MỜI KIỂM TRA".
- If license is expired, vehicle in is not allowed.

## Vehicle Out

- Vehicle not found in parking cannot exit normally.
- Vehicle out must capture out images.
- Fee must be calculated before barrier opens.
- Vehicle out must update local database before opening barrier.
- Vehicle out must create SyncOutbox event.
- Vehicle out must create AuditLog.
- If license is expired, normal vehicle out is not allowed.
- If license is expired and the vehicle is already in parking, emergency vehicle out is allowed.

## Emergency Out

- Emergency out is allowed only when license is expired or data cannot be found through normal lookup.
- Emergency out must capture vehicle out images.
- Emergency out must record operator information.
- Emergency out must create EmergencyExit record.
- Emergency out must create AuditLog.
- Emergency out must not allow browsing all parking data.

## Monthly Pass

- Monthly pass must be checked by card id.
- Monthly pass must be checked by valid date.
- Expired monthly pass must show warning.
- If ANPR plate does not match monthly pass plate, system must show "MỜI KIỂM TRA".
- Monthly pass mismatch must not be silently ignored.

## Duplicate Plate

- Duplicate plate detection must use normalized plate number.
- Duplicate plate warning must show current open parking session if available.
- Operator must confirm before continuing when duplicate plate is detected.

## Barrier

- Barrier must not open before local database commit succeeds.
- Barrier operation must be written to AuditLog.
- Barrier failure must not corrupt parking session data.

## Audit

Every important action must create AuditLog:

- Vehicle in
- Vehicle out
- Emergency out
- License expired emergency out
- Barrier open
- Manual fee override
- Manual plate correction
- Card registration
- Monthly pass update
- Configuration change
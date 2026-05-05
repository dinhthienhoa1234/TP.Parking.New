# License Rules

## Final License Principles

1. Gmail is the owner account, not the runtime key.
2. Every machine must have MachineId and fingerprint.
3. Customer can activate only the number of machines purchased.
4. No auto-renew.
5. Renewal requires signed confirmation code.
6. Worker checks periodically but does not renew.
7. Expired license locks safely and preserves data.

## Machine Identity

Each installed machine must have:

- MachineId
- Machine fingerprint
- Machine name
- Machine role

Machine roles:

- Server
- EntryClient
- ExitClient
- AllInOne

## License Account

Gmail is used as account owner.

Gmail alone must not allow software to run.

Runtime license must be bound to:

- Gmail
- MachineId
- Fingerprint
- Signed token

## Machine Limit

If customer buys 3 licenses, only 3 machines can be active.

If customer needs to replace a machine:

- Admin must deactivate old machine.
- Admin must issue a new activation code.
- The old machine token must be revoked or marked inactive.

## Activation

Online activation flow:

1. App creates MachineId.
2. App creates fingerprint.
3. User enters Gmail.
4. App sends activation request.
5. Server checks license slot.
6. Server or admin issues activation code.
7. User enters activation code.
8. Server verifies code.
9. Server returns signed license token.
10. App stores local license.

## Offline Temporary Activation

If there is no internet during installation:

- Online activation is not possible.
- Admin may issue temporary offline code.
- Temporary offline code must be valid only 1 to 3 days.
- Temporary code must be bound to InstallRequestCode.
- Temporary code must not be reusable on another machine.

## Renewal

License must not auto-renew.

Renewal flow:

1. App warns when license has less than 3 days remaining.
2. User requests renewal code.
3. Admin or server sends confirmation code.
4. User enters renewal code.
5. App verifies code.
6. App updates signed license token.

## Worker

LicenseWorker must:

- Check local token periodically.
- Detect expired license.
- Detect clock tamper.
- Send heartbeat when online.
- Check server suspend/revoke status when online.
- Update LicenseGate.
- Show warning when remaining days are less than or equal to 3.
- Not auto-renew license.

## Clock Tamper

The system must detect clock tampering.

Detection rules:

- If current local time is earlier than LastLocalCheckTime by more than configured tolerance, mark ClockTamper.
- If current local time is earlier than LastKnownServerTime by more than configured tolerance, mark ClockTamper.
- ClockTamper must not delete data.
- ClockTamper must lock system safely.

## Expired License Permissions

When license is expired, allowed:

- Open license screen.
- Enter renewal code.
- Sync pending data.
- Emergency vehicle out if vehicle is already in parking.

When license is expired, blocked:

- New vehicle in.
- Normal vehicle out.
- Data browsing.
- Report export.
- System configuration.
- Card registration.
- Monthly pass editing.
- Tariff editing.

## Safe Lock

When license is expired:

- Do not delete data.
- Do not lock database.
- Do not corrupt local sync queue.
- Do not prevent emergency vehicle out for vehicles already in parking.
- Create AuditLog for emergency vehicle out.
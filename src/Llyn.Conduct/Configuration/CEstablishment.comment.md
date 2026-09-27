# CEstablishment.cs

## `public sealed record CEstablishment(`

The workspace's size and unsaved work, as the status strip shows it.
Its verdicts are copied from the engine, so the strip judges nothing itself.

**Parameters**

- `CEstablishmentUnsaved`: how many drafts hold unsaved work.
- `CEstablishmentEntry`: how many entries the workspace holds.
- `CEstablishmentSize`: the workspace's size in bytes.
- `CEstablishmentPending`: whether any draft holds unsaved work.
- `CEstablishmentSingle`: whether the workspace holds exactly one entry.

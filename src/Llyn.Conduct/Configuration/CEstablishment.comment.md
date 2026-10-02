# CEstablishment.cs
Hash: `faa9ed337890ca0b`

## `public sealed record CEstablishment(`

The workspace's size and unsaved work, as the status strip shows it.
Its verdicts come from the engine and its keys from the atelier, so the strip judges nothing itself.

**Parameters**

- `CEstablishmentUnsaved`: how many drafts hold unsaved work.
- `CEstablishmentEntry`: how many entries the workspace holds.
- `CEstablishmentPending`: whether any draft holds unsaved work.
- `CEstablishmentEntryKey`: the entry count's wording, singular for exactly one entry.
- `CEstablishmentSizeKey`: the size's wording, megabytes or kilobytes.
- `CEstablishmentAmount`: the size in that unit, written for the current culture.

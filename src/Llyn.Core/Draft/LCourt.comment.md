# LCourt.cs

## `public sealed record LCourt(`

One tentative link, waiting for the record it points at to become real or be dropped.
It is plain data and knows nothing of files or the database.
The court is the register these links live in, so one row is one court entry.

**Parameters**

- `LCourtId` — Id of this link.
- `LCourtOwnerId` — Draft id of the draft holding the link.
- `LCourtTargetId` — Draft id of the tentative entry the link points at.
- `LCourtHeadword` — Headword of the target, shown before the target is real.
- `LCourtLanguage` — Language of the target.
- `LCourtVersion` — Shape of the file this link was read from, stamped by the archive on every write.
  A file of another shape is skipped and swept, as a draft file is.

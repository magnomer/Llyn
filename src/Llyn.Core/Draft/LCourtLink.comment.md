# LCourtLink.cs

## `public sealed record LCourtLink(`

One tentative link, waiting for the record it points at to become real or be dropped.
It is plain data and knows nothing of files or the database.
The court is the register these links live in.

**Parameters**

- `LCourtLinkId` — Id of this link.
- `LCourtLinkOwner` — Draft id of the draft holding the link.
- `LCourtLinkTarget` — Draft id of the tentative entry the link points at.
- `LCourtLinkHeadword` — Headword of the target, shown before the target is real.
- `LCourtLinkLanguage` — Language of the target.

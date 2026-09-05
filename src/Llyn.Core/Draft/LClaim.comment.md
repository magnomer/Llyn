# LClaim.cs

## `public sealed record LClaim(`

One running program's hold on a tentative record, written where every launch can read it.
A held draft is otherwise known only to the copy of the program that started it.
A second copy would report that live work as a leftover of a crash and offer to recover it.
It is plain data and knows nothing of files or of the operating system.

**Parameters**

- `LClaimDraft` — Id of the draft this claim holds.
- `LClaimProcess` — Operating-system process id of the program holding it.
- `LClaimMoment` — Instant that process started.
  An id is reused once its process is gone, so the start time is what tells the holder from its successor.

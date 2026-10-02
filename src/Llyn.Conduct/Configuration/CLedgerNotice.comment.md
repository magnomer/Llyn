# CLedgerNotice.cs
Hash: `34a2b3923db6163d`

## `public sealed record CLedgerNotice(string CLedgerNoticeKey, string? CLedgerNoticeLabel, string? CLedgerNoticePath);`

The detail a failure notice shows, as `CLedgerNoticeRead` answers it.

**Parameters**

- `CLedgerNoticeKey`: the wording key of the reason line.
- `CLedgerNoticeLabel`: the wording key of the audit file's caption, or null when no file was written.
- `CLedgerNoticePath`: the audit file a fault went to, or null.

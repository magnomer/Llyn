# LReceipt.cs
Hash: `a2abbd39386ba268`

## `public sealed record LReceipt(int LReceiptSaved, int LReceiptKept, int LReceiptRemoved, IReadOnlyList<string> LReceiptFailed);`

The tally of one push to Joplin, shown to the user when it ends.
A failed note does not stop the push, so failures are listed rather than raised.

**Parameters**

- `LReceiptSaved` — How many notes were pushed.
- `LReceiptKept` — How many notes were unchanged and skipped.
- `LReceiptRemoved` — How many notes were moved to Joplin's trash.
- `LReceiptFailed` — The headwords whose notes failed to push.

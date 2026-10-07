# LReceipt.cs
Hash: `a2abbd39386ba268`

## `public sealed record LReceipt(int LReceiptSaved, int LReceiptKept, int LReceiptRemoved, IReadOnlyList<string> LReceiptFailed);`

The tally of one push to Joplin, shown to the user when it ends.
A failed note does not stop the push, so failures are listed rather than raised.
A reconstruction note whose title itself failed is listed under its language name.

**Parameters**

- `LReceiptSaved` — How many entry and reconstruction notes were pushed, never the style note.
- `LReceiptKept` — How many entry and reconstruction notes were unchanged and skipped.
- `LReceiptRemoved` — How many notes were moved to Joplin's trash.
- `LReceiptFailed` — The entry headwords and reconstruction note titles whose notes failed to push.

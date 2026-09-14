# LMarkupOutcome.cs

## `public sealed record LMarkupOutcome(`

What one markup import produced: the entries now stored and everything the import left behind.
The whole import runs as one revision, so the list holds every entry or none.

**Parameters**

- `LMarkupOutcomeEntry` — The stored entries in the order of the file.
- `LMarkupOutcomeOmission` — What the read and the import skipped, in file order.

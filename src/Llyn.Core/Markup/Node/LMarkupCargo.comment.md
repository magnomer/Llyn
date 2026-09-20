# LMarkupCargo.cs

## `public sealed record LMarkupCargo(`

What one read of a markup file holds: the parsed entries and what the read skipped.
The shell shows the entries, asks how each enters, and hands the same cargo back to the import.
The file is read once, so what the user saw is what is imported.
A file swapped on disk between the two steps changes nothing.

**Parameters**

- `LMarkupCargoEntry` — The parsed entries in the order of the file.
- `LMarkupCargoOmission` — What the read skipped, in file order.

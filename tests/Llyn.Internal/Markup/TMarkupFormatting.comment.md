# TMarkupFormatting.cs

## `public sealed class TMarkupFormatting`

Covers the markup writer through the reader.
A parsed entry written and read again is the same entry, and the text written twice is the same text.
An unspecified value leaves no element behind and an unknown value leaves a marked empty one.

## Inline notes

### `public void MarkupFormat_ParsedEntry_RoundTripsEqual()`

Record equality covers every field, and text equality shows the writer is stable on its own output.

### `public void MarkupFormat_ControlCharacter_DropsIt()`

The writer strips what XML cannot carry, so the text parses again instead of the export throwing.

### `public void MarkupFormat_EmptyLocal_RoundTripsEmpty()`

An empty spelling and no spelling are different values, and the round trip keeps them apart.

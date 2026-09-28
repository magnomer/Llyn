# LDrillMarkup.cs

## `internal sealed class LDrillMarkup : LDrill`

Round-trips a large markup document: text to tree to entries, then back to text.
It exercises the `.llx` reader and writer on both sides of `LMarkupFile`.

## `private const string LDrillMarkupFixture = "Fixture/markup.xml";`

The one-entry sample the document is built from, copied beside the binary.

## `private const int LDrillMarkupCopies = 400;`

How many copies of the sample entries make up the document.
It keeps one cycle near a hundred milliseconds, long enough for steady samples.

## `private string _lDrillMarkupText = string.Empty;`

The document text every cycle parses.
It is formatted once in preparation, so the cycle measures the parse and format only.

# LQuillExample.cs
Hash: `9bb47d84a3103237`

## `public sealed class LQuillExample`

The edits of a held Example's own fields, each building exactly one request.
Its glosses are sentence rows, so they go through `LQuillSentence` with card and sentence zero.

## `private readonly LTenure _lQuillExampleTenure;`

The tenure every request is built for and handed to.

## `public LQuillExample(LTenure tenure)`

Builds the edits over one tenure, which they never swap.

## `public void LQuillExampleSet(string text)`

Defers the held Example's typed text, always as known.
Typing is what makes an unknown text known.

## `public void LExampleSpeakerSet(string language)`

Sends the held Example's chosen language at once, since it came from a click.

## `public void LExampleReferenceSet(long reference)`

Points the held Example at the picked Source, sent at once.

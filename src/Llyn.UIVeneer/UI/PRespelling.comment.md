# PRespelling.cs

## `internal sealed record PRespelling(bool PRespellingShown, bool PRespellingSlashed)`

How the readings of one language are shown right now, read once per render from the engine's switch and pack.
Every reading surface asks it which of the two stored forms to print and which brackets to draw around it.
The editor also asks it which form a typed change belongs to, so the edit side mirrors the read side.

**Parameters**

- `PRespellingShown` — Whether the respelled form is shown and edited.
  It is true only while the switch is on and the pack has groups.
- `PRespellingSlashed` — Whether the shown form stands between slashes, true only for a shown respelling of a phonemic pack.

## `internal static readonly PRespelling PRespellingPlain = new(false, false);`

The plain state, original reading between square brackets, for a surface with no language yet.

## `internal string PRespellingOpener`

The bracket drawn before the reading, a slash for a phonemic respelling and a square bracket otherwise.

## `internal string PRespellingCloser`

The bracket drawn after the reading, a slash for a phonemic respelling and a square bracket otherwise.

## `internal static PRespelling PRespellingRead(LEngine engine, string language)`

Reads the state for `language` from the engine, plain for a blank language.

## `internal string PRespellingTextRead(LPronunciationDraft spoken)`

The form of one draft row to print, its respelling while shown and filled and its reading otherwise.
A row stored before respellings were kept has none, and its reading stands in whatever the switch says.

## `internal string PRespellingTextRead(string phonetic, string? respelling)`

The same choice for a lookup candidate, which carries the two forms as bare strings.

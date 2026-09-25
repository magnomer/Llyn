# LRespellingMark.cs

## `public sealed record LRespellingMark(bool LRespellingMarkShown, bool LRespellingMarkSlashed)`

How the readings of one language are shown right now, read once per render from the engine's switch and pack.
Every reading surface asks it which of the two stored forms to print and which brackets to draw around it.
The editor also asks it which form a typed change belongs to, so the edit side mirrors the read side.

**Parameters**

- `LRespellingMarkShown` — Whether the respelled form is shown and edited.
  It is true only while the switch is on and the pack has groups.
- `LRespellingMarkSlashed` — Whether the shown form stands between slashes, true only for a shown respelling of a phonemic pack.

## `public static readonly LRespellingMark LRespellingMarkPlain = new(false, false);`

The plain state, original reading between square brackets, for a surface with no language yet.

## `public string LRespellingMarkOpener`

The bracket drawn before the reading, a slash for a phonemic respelling and a square bracket otherwise.

## `public string LRespellingMarkCloser`

The bracket drawn after the reading, a slash for a phonemic respelling and a square bracket otherwise.

## `public static LRespellingMark LRespellingMarkRead(LWindow window, string language)`

Reads the state for `language` from the engine, plain for a blank language.

## `public string LRespellingMarkResolve(LPronunciationDraft spoken)`

The form of one draft row to print, its respelling while shown and filled and its reading otherwise.
A row stored before respellings were kept has none, and its reading stands in whatever the switch says.

## `public string LRespellingMarkResolve(string phonetic, string? respelling)`

The same choice for a lookup candidate, which carries the two forms as bare strings.

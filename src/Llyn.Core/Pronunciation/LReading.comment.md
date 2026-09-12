# LReading.cs

## `public sealed record LReading(string LReadingVariety, string LReadingPhonetic)`

One transcription a source returned for a word, tagged with the regional variety it belongs to.
A page that lists a British and an American form yields two readings, not one joined string.
The tag travels with the form from the source to the candidate the menu shows.

**Parameters**

- `LReadingVariety` — The name of the regional variety, matching one the language pack declares.
  It is empty when the source does not say which variety the form belongs to.
- `LReadingPhonetic` — The bare phonetic form, without enclosing brackets.

## `public static string LReadingNormalize(string phonetic)`

The built-in, language-agnostic cleanup every reading passes through before it becomes a candidate.
It runs whatever the respelling switch says, so the trove caches cleaned text.
It NFC-normalizes, drops every syllable boundary and every whitespace character, and trims.
A boundary is the full stop, the middle dot Cambridge writes, or the hyphenation point.
Stress marks, length marks, tie bars, and delimiters survive.
So `ˈdɪf.ər.əns` and `ˈdɪf ər əns` both become `ˈdɪfərəns`.
A multi-word headword loses its inner space by design.
Site quirks this step cannot know are left to the pack's `cleanup` groups.

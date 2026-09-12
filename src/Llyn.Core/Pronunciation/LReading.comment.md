# LReading.cs

## `public sealed record LReading(string LReadingVariety, string LReadingPhonetic)`

One transcription a source returned for a word, tagged with the regional variety it belongs to.
A page that lists a British and an American form yields two readings, not one joined string.
The tag travels with the form from the source to the candidate the menu shows.

**Parameters**

- `LReadingVariety` — The name of the regional variety, matching one the language pack declares.
  It is empty when the source does not say which variety the form belongs to.
- `LReadingPhonetic` — The bare phonetic form, without enclosing brackets.

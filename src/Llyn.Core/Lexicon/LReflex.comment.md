# LReflex.cs

## `public sealed record LReflex(`

One reading of a Han-character entry in a neighbouring language that borrowed the character.
Korean 음훈, Mandarin IPA and Japanese on'yomi are reflexes of one Middle Chinese reading.
An entry owns an ordered list of these, grouped by language, each row one kind of reading.
It is not a Transcription, which spells the entry's own reading in one scheme.
`LReflexId` is the identity, an opaque and program-generated stable id.
The language and kind are stored as text, so a row stays readable after its pack changes.

**Parameters**

- `LReflexId` — Opaque, program-generated stable id.
- `LReflexEntryId` — Owning entry id.
- `LReflexPosition` — Order within the entry, zero first.
- `LReflexLanguage` — The borrowing language the reading belongs to, such as Korean or Japanese.
- `LReflexKind` — The kind of reading, printed before it, such as a Japanese Go-on or Kan-on.
  Empty when the language sorts its readings by no kind.
- `LReflexText` — The reading as stored, such as `롱(농)` or `nʊŋ⁵¹`.
  A pronunciation is stored bare, with no slash or bracket, so its onset, nucleus and coda can be read later.
  A page listing two readings gives two rows, never one row holding both.
- `LReflexNote` — The note printed after the reading: a Mandarin pinyin or a Korean 훈.
  Empty when the reading carries none.
- `LReflexMain` — True when this reading is the one in common use.
  The view then prints it in the accent colour.
- `LReflexRespelling` — The reading recast in its own language's respelling convention, such as `nuŋ⁵¹`.
  Empty when that language declares no respelling groups.
- `LReflexRegion` — The place the reading is taken from, such as `Shanghai` under Wu.
  The view shows it when the language name is hovered.
  Empty when the language names no place.
- `LReflexRemark` — What the source says of the reading, such as `literary` or `vernacular (“difficult”)`.
  The view shows it when the reading is hovered.
  Empty when the source says nothing.
- `LReflexAnatomy` — The onset, vowel, coda and tone of the reading, cut in IPA and in respelling alike.
  It is one [LAnatomy](../Pronunciation/LAnatomy.comment.md), derived by the engine under the entry's pack rules.
  Empty in every part when the entry's language declares no rule for the reading's language.
- `LReflexAnchors` — The ids of the fanqie rows the user tied this reading to, sorted, each once.
  Only through them does the reading count on a Diwei page.
  Empty until the user anchors the row.

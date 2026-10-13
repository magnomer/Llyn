# Japanese

The Japanese language pack and its display vocabulary, both declared as data.
The pack format is documented in `languages/English/English.comment.md`.

## `source.json`

Section heading anchors allow attributes before `id`, which the REST rendering writes there.
No varieties are declared, so every reading is the standard Tokyo one.
No respelling groups are declared, so every reading shows as its source wrote it.
Segoe UI has no kana or kanji glyphs, so the headword and example blocks name the Yu fonts instead.
The gloss block keeps Georgia, since the translations under an example are not Japanese.
Every Wiktionary page except the kana lookup is asked as the article page before the REST rendering.
The kana lookup reads wikitext through the action API.
Both hosts reject bursts with 429, and the article page recovers first.
A kana-form page carries no reading of its own.
It only points at its kanji form through `{{ja-see}}`, so each Wiktionary attempt follows that pointer.
Live, こんにちは led to 今日は and 有難う to ありがとう.

## `transcription[0]` Kana

The kana reading of the headword is read from the `{{ja-pron}}` template in the wikitext.
The rendered page splits that reading over the ruby of each kanji, so the wikitext is read instead.
The first template after the Japanese heading belongs to the first etymology.
Named parameters such as `only_show_acc` may precede the reading, so the pattern steps over them.
A reading is the first bare parameter followed by a bar or the closing braces.
A kana-only headword writes the template without one, so the lookup finds nothing and the row is typed by hand.
The template writes the reading as pronounced, so 今日は gave こんにちわ.

## `transcription[1]` Romaji

This is Hepburn romaji as Wiktionary prints it after the headword, inside the headword-tr span.
The match anchors on the Japanese section heading so a Chinese section listed before it is skipped.
The first span after the heading belongs to the first part of speech.
A romaji with its own entry is wrapped in a link, which the pattern steps into.

## `transcription[2]` Pitch

This is the Tokyo pitch accent as `{{ja-pron}}` renders it.
It is romaji with a grave or acute per mora and a downstep mark.
It sits in the first `<samp>` after the Japanese section heading.
The conjugation table of a verb prints one per form, so only the first is taken.

## `glyph` Kanji

It gives one transcription row under the scheme named here, and the language each character opens an entry in.
No source is listed, because a Japanese headword already carries its kanji and the reading view drops the kana itself.
The row exists so a kyūjitai form can be typed in by hand.

## `pronunciation[0]` Wiktionary

The ja-pron template derives the IPA from the kana, so the wikitext carries none and the rendered HTML is read.
The etymology section may print IPA spans of its own.
So the match anchors on the key link of the pronunciation line.
The article page writes the space after the colon as an entity and the REST rendering as a span.
So a short gap is allowed before the IPA span.
The reading is phonetic and stands between brackets, which `normalize` strips.

## `audio[0]` Naver

Naver's text-to-speech answers every headword, since a synthesizer has no missing entries.
The address is the recording, so the `link` strategy answers it once the fetch succeeded.
The speaker is `ja_jp` and the service `jadic`, and any other speaker code answers 400.

## `audio[1]` Wiktionary

Wiktionary embeds Commons recordings as protocol-relative addresses, so `prefix` supplies the scheme.
Only the transcoded mp3 rendition is taken, because the original is Ogg or WAV.
The first attempt takes a `Ja-` filename after the Japanese section heading.
The second takes a Lingua Libre `LL-Q5287_%28jpn%29-` one, Q5287 being Japanese.
Few Japanese entries carry a recording, so either attempt often yields nothing.

## `frequency[0]` Jisho

The Jisho word search API answers the JLPT levels and the JMdict common flag of each entry.
The search also lists compounds.
So the entry taken is the one whose first written or kana form is the headword.
A level is a grade with no count behind it, so pattern bands label it instead of an interval.
N5 and N4 are core, N3 and N2 everyday and N1 advanced.
An entry may list several levels in any order, so the bands try the easiest first.
The first attempt yields the level list as written, quotes included.
The second attempt reads the common flag when no level is listed.
JMdict marks roughly the top twenty thousand words common, so a common word without a level is advanced.
A word neither graded nor common yields nothing, as an unknown spelling does.

## `morphology`

The list stands empty until a source is pinned.

## `vocabulary.json`

The parts of speech an entry may carry are presets in dropdowns.
A user may still type a value no preset names, which is stored as typed.
Every id is an integer that never changes once published.
Renaming a preset keeps its id, so every entry linking it follows the rename.
Order is the order of the lists.
A language is added by creating `languages/<Name>/vocabulary.json`.

## `exampleOrder`

It states which of the two frame fields an example row writes first, `particle` or `dependence`.
It states nothing either may hold.

## `parts`

Each part is a bare role.
Valency and other subtypes belong to the dependence field and the morphology, not here.
A part may carry `parent`, the id of the part it specialises.
A paradigm declared on the parent applies to it.

## `retired`

It lists the parts once published and since removed, so no id is ever reused.
The import applies every row on each start, and a row whose part is already gone does nothing.
`into` names the part that takes over the removed part's links.
`unit` names the lexical unit set on a linking entry that has none yet.
A row with neither drops the links.

## Stamp

Commit: `c2f335c331fe`
- `source.json` `50d04de79fd65ac9`
- `vocabulary.json` `e4aecee2b8d7110a`

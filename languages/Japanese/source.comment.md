# source.json

## pack

Language pack for Japanese.
The pack format is documented in `languages/English/source.comment.md`.
No varieties are declared, so every reading is the standard Tokyo one.
No respelling groups are declared, so every reading shows as its source wrote it.
Segoe UI has no kana or kanji glyphs, so the headword and example blocks name the Yu fonts instead.
The gloss block keeps Georgia, since the translations under an example are not Japanese.
Every Wiktionary page is asked as the article page before the REST rendering.
Both hosts reject bursts with 429, and the article page recovers first.
A kana-form page carries no reading of its own.
It only points at its kanji form through `{{ja-see}}`, so each Wiktionary attempt follows that pointer.
Pinned live, こんにちは led to 今日は and 有難う to ありがとう.

## `transcription[0]` Kana

The kana reading of the headword as `{{ja-pron}}` names it in the wikitext.
The rendered page splits that reading over the ruby of each kanji, so the wikitext is read instead.
The first template after the Japanese heading belongs to the first etymology.
Named parameters such as `only_show_acc` may precede the reading, so the pattern steps over them.
A reading is the first bare parameter followed by a bar or the closing braces.
A kana-only headword writes the template without one, so the lookup finds nothing.
The row is then typed by hand.
The template writes the reading as pronounced, so 今日は gave こんにちわ.
Pinned live, 猫 gave ねこ, 食べる たべる, 学校 がっこう and 憂鬱 ゆううつ.

## `transcription[1]` Romaji

Hepburn romaji as Wiktionary prints it after the headword, inside the headword-tr span.
The match anchors on the Japanese section heading so a Chinese section listed before it is skipped.
The first span after the heading belongs to the first part of speech.
A romaji with its own entry is wrapped in a link, which the pattern steps into.
Pinned live, 猫 gave neko, 学校 gakkō, ありがとう arigatō and コーヒー kōhī.

## `transcription[2]` Pitch

The Tokyo pitch accent as `{{ja-pron}}` renders it, romaji with a grave or acute per mora and a downstep mark.
It sits in the first `<samp>` after the Japanese section heading.
The conjugation table of a verb prints one per form, so only the first is taken.
Pinned live, 猫 gave [néꜜkò], 食べる [tàbéꜜrù], 学校 [gàkkóó] and 水 [mìzú].

## `glyph` Kanji

The kanji of the headword.
It gives one transcription row under the scheme named here.
It also gives the language each character opens an entry in.
No source is listed.
A Japanese headword already carries its kanji and the reading view drops the kana itself.
The row exists so a kyūjitai form can be typed in by hand.

## `pronunciation[0]` Wiktionary

The IPA is derived from the kana by the ja-pron template, so the wikitext carries none.
The rendered HTML is therefore read.
The etymology section may print IPA spans of its own.
So the match anchors on the key link of the pronunciation line.
The article page writes the space after the colon as an entity and the REST rendering as a span.
So a short gap is allowed before the IPA span.
The reading is phonetic and stands between brackets, which `normalize` strips.
Pinned live, 猫 gave [ne̞ko̞], 食べる [ta̠be̞ɾɯ̟], 学校 [ɡa̠k̚ko̞ː] and 静か [ɕizɨka̠].

## `audio[0]` Naver

Naver's Japanese dictionary text-to-speech, one mp3 per request.
The address is the recording, so the `link` strategy answers it once the fetch succeeded.
The speaker is `ja_jp` and the service `jadic`, and any other speaker code answers 400.
Every headword answers, since a synthesizer has no missing entries.

## `audio[1]` Wiktionary

Wiktionary embeds Commons recordings as protocol-relative addresses, so `prefix` supplies the scheme.
Only the transcoded mp3 rendition is taken, because the original is Ogg or WAV.
The first attempt takes a `Ja-` filename after the Japanese section heading.
The second takes a Lingua Libre `LL-Q5287_%28jpn%29-` one, Q5287 being Japanese.
Few Japanese entries carry a recording, so either attempt often yields nothing.
Pinned live, 学校 and ありがとう gave a `Ja-` recording, 猫 a Lingua Libre one and 食べる none.

## `frequency[0]` Jisho

Jisho word search API, which answers the JLPT levels and the JMdict common flag of each entry.
The search also lists compounds.
So the entry taken is the one whose first written or kana form is the headword.
A level is a grade with no count behind it, so pattern bands label it instead of an interval.
N5 and N4 are core, N3 and N2 everyday and N1 advanced.
An entry may list several levels in any order, so the bands try the easiest first.
The first attempt yields the level list as written, quotes included.
The second attempt reads the common flag when no level is listed.
JMdict marks roughly the top twenty thousand words common, so a common word without a level is advanced.
A word neither graded nor common yields nothing, as an unknown spelling does.
Pinned live, 猫 gave N5, 勉強 N3 and N5, 憂鬱 N1 and 薔薇 common.

## `morphology`

The list stands empty until a source is pinned.

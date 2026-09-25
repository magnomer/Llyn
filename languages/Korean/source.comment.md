# source.json

## pack

Language pack for Korean.
The pack format is documented in `languages/English/source.comment.md`.
No varieties are declared, so every reading is the Seoul standard one.
No respelling groups are declared, so every reading shows as its source wrote it.
Segoe UI has no hangul glyphs, so the headword and example blocks name Malgun Gothic first.
Malgun Gothic ships with every Windows install since 7.
Georgia has no hangul either, so the example block does not name it.
The gloss block keeps Georgia, since the translations under an example are not Korean.
Krdict is the National Institute of Korean Language's learner dictionary.
Its search page lists every headword spelled exactly as typed, the most important homonym first.
Each Krdict attempt anchors on that exact headword span, which may carry a homonym superscript.
The attempt then reads within that entry's `<dt>` alone, so a following entry is never crossed.
A spelling Krdict does not know lists only near matches, so every Krdict attempt yields nothing.
Every Wiktionary page is asked as the REST rendering before the article page.

## `cleanup[0]` Variants

Wiktionary prints two free variants in one IPA span as `[a] ~ [b]`.
The built-in removal strips only the outer brackets, leaving the inner pair around the tilde.
So the rule turns that inner `] ~ [` into a plain tilde.
Pinned live, 맛있다 gave ma̠ɕʰit̚t͈a̠ ~ ma̠dit̚t͈a̠.

## `transcription[0]` Romanization

Revised Romanization as Wiktionary prints it after the headword, inside the headword-tr span.
The match anchors on the Korean section heading so a Jeju section listed before it is skipped.
The first span after the heading belongs to the first part of speech.
Pinned live, 학교 gave hakgyo, 먹다 meokda, 사과하다 sagwahada and 한국어 han'gugeo.

## `transcription[1]` Phonetic

The phonetic hangul, the headword respelled as it is pronounced.
Krdict prints it between brackets before the recording link of each entry.
It marks a long vowel with ː after the syllable.
A word with two free pronunciations prints each with its own link, and only the first is taken.
A loanword such as 컴퓨터 prints none, so the row falls to Wiktionary.
Pinned live, 학교 gave 학꾜, 먹다 먹따, 한국어 한ː구거 and 맛있다 마딛따.
Wiktionary prints its own inside the ko-pron__ph item, one span or bold per syllable.
The article page writes the underscores of that class as entities, so the pattern accepts both.
The `span` strategy strips the syllable tags and `normalize` the brackets.
A long vowel is marked as (ː) after the syllable, and two free variants are split on the slash.
Pinned live, 학교 gave 학꾜, 한국어 한(ː)구거 and 맛있다 마싣따 and 마딛따.

## `glyph` Hanja

The hanja of a Sino-Korean headword.
It gives one transcription row per hanja spelling under the scheme named here.
It also gives the language each character opens an entry in.
Batang is a serif face with hanja glyphs and Malgun Gothic the fallback.
Krdict prints the origin of a headword in parentheses right after it.
The origin of a loanword is its Latin spelling, so the pattern requires a first CJK character.
A Sino-Korean verb keeps its hangul ending, as 謝過하다.
Every exact homonym is taken, since each may carry a different hanja.
Pinned live, 학교 gave 學校, 사과하다 謝過하다 and 한국어 韓國語.
Wiktionary prints the hanja in the headword line of a noun, inside a link.
The headword line of a verb omits it, so the Krdict source comes first.
Pinned live, 학교 gave 學校 and 사과하다 nothing.

## `pronunciation[0]` Wiktionary

The IPA is derived from the hangul by the ko-IPA template, so the wikitext carries none.
The rendered HTML is therefore read.
The match anchors on the key link of the pronunciation line after the Korean section heading.
So the Jeju section of a page such as 물 is skipped.
The article page writes the space after the colon as an entity, so a short gap is allowed.
The reading is phonetic and stands between brackets, which `normalize` strips.
A long vowel is marked as (ː) after the vowel.
Pinned live, 학교 gave [ha̠k̚k͈jo], 먹다 [mʌ̹k̚t͈a̠], 물 [muɭ] and 한국어 [ˈha̠(ː)nɡuɡʌ̹].

## `audio[0]` Krdict

Krdict links a studio recording of each entry as an mp3 on its media host.
The link sits in the fnSoundPlay call after the phonetic hangul.
A loanword prints no phonetic hangul and so no recording.
Pinned live, 학교, 먹다, 사과하다 and 한국어 gave a recording and 컴퓨터 none.

## `audio[1]` Naver

Naver's Korean dictionary text-to-speech, one mp3 per request.
The address is the recording, so the `link` strategy answers it once the fetch succeeded.
The speaker is `ko_kr` and the service `krdic`, and any other speaker code answers 400.
Every headword answers, since a synthesizer has no missing entries.

## `audio[2]` Wiktionary

Wiktionary embeds Commons recordings as protocol-relative addresses, so `prefix` supplies the scheme.
Only the transcoded mp3 rendition is taken, because the original is Ogg or WAV.
The first attempt takes a `Ko-` filename after the Korean section heading.
The second takes a Lingua Libre `LL-Q9176_%28kor%29-` one, Q9176 being Korean.
Pinned live, 학교, 먹다 and 컴퓨터 gave a `Ko-` recording, 사랑 a Lingua Libre one and 한국어 none.

## `frequency[0]` Krdict

Krdict grades its learner vocabulary with one to three stars after the phonetic hangul.
Three stars is 초급, the beginner grade, two 중급, intermediate, and one 고급, advanced.
A grade is an editor's rank with no count behind it, so pattern bands label it instead of an interval.
The star span carries no text, so the pattern reads the grade's name off the legend table below the results.
Each alternative counts the stars of the first exact entry and captures the matching legend cell.
The alternatives share one named group, so `group` 1 is the grade whichever alternative fired.
The bands then label 초급 core, 중급 everyday and 고급 advanced.
An entry outside the learner vocabulary carries no stars, so the attempt yields nothing.
Pinned live, 학교, 먹다 and 컴퓨터 gave 초급, 사과하다 중급 and 한국어 nothing.

## `morphology`

The list stands empty until a source is pinned.

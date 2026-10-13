# Korean

The Korean language pack and its display vocabulary, both declared as data.
The pack format is documented in `languages/English/English.comment.md`.

## `source.json`

Section heading anchors allow attributes before `id`, which the REST rendering writes there.
No varieties are declared, so every reading is the Seoul standard one.
Segoe UI has no hangul glyphs, so the headword and example blocks name Malgun Gothic first.
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

## `respelling[0]` Seoul

The first row drops five combining marks wherever they stand.
They are the no-audible-release mark, the rounding, retraction and lowering marks, and the tie bar.
So p̚ becomes p, ʌ̹ becomes ʌ, a̠ becomes a, and t͡ɕ becomes tɕ.
The stress mark ˈ is dropped, since Korean has no lexical stress.
The middle rows undo the allophones Wiktionary's ko-pron module writes.
The module swallows a j in several of them, so a row not before i restores it.
The ㅋ allophones cç and kx become kʰ.
The ㅎ allophones ç, ʝ, ɸʷ, ɸ, x, ɣ, β and ɦ become h.
The ㅅ and ㅆ allophones ɕʰ, ɕ͈ and ʃ become sʰ, s͈ and s.
The lookbehind on t keeps the ɕ of ㅈ, ㅊ and ㅉ.
The module writes ㅟ in the second half of a word as y, which becomes ɥi.
The ㄴ before j written ɲ becomes nj, and the ɲ before ㅈ or ㅊ becomes n.
The ㄹ allophones ɭ and ʎ become l, while the flap ɾ stays.
The ㅚ vowel ø becomes we.
Lenis voicing stays, so an intervocalic ɡ, b, d or dʑ is kept.
Both mid central vowels ɘ and ə become ʌ, and the open-mid ɛ becomes e.
The optional length mark (ː) goes before the bare ː, so no parentheses are left behind.
Worked through, 먹다 [mʌ̹k̚t͈a̠] gives mʌkt͈a and 물 [muɭ] gives mul.

## `transcription[0]` Romanization

This is Revised Romanization as Wiktionary prints it after the headword, inside the headword-tr span.
The match anchors on the Korean section heading so a Jeju section listed before it is skipped.
The first span after the heading belongs to the first part of speech.

## `transcription[1]` Phonetic

This is the headword respelled as it is pronounced.
Krdict prints it between brackets before the recording link of each entry, marking a long vowel with ː.
A word with two free pronunciations prints each with its own link, and only the first is taken.
A loanword such as 컴퓨터 prints none, so the row falls to Wiktionary.
Wiktionary prints its own inside the ko-pron__ph item, one span or bold per syllable.
The article page writes the underscores of that class as entities, so the pattern accepts both.
The `span` strategy strips the syllable tags and `normalize` the brackets.
A long vowel is marked as (ː) after the syllable, and two free variants are split on the slash.

## `glyph` Hanja

It gives one transcription row per hanja spelling under the scheme named here.
It also gives the language each character opens an entry in.
Batang is a serif face with hanja glyphs and Malgun Gothic the fallback.
Krdict prints the origin of a headword in parentheses right after it.
The origin of a loanword is its Latin spelling, so the pattern requires a first CJK character.
A Sino-Korean verb keeps its hangul ending, as 謝過하다.
Every exact homonym is taken, since each may carry a different hanja.
Wiktionary prints the hanja in the headword line of a noun, inside a link, but a verb's line omits it.
So the Krdict source comes first.

## `pronunciation[0]` Wiktionary

The ko-IPA template derives the IPA from the hangul, so the wikitext carries none and the rendered HTML is read.
The match anchors on the key link of the pronunciation line after the Korean section heading.
So the Jeju section of a page such as 물 is skipped.
The article page writes the space after the colon as an entity, so a short gap is allowed.
The reading is phonetic and stands between brackets, which `normalize` strips.
A long vowel is marked as (ː) after the vowel.

## `audio[0]` Krdict

Krdict links a studio recording of each entry as an mp3 on its media host.
The link sits in the fnSoundPlay call after the phonetic hangul.
A loanword prints no phonetic hangul and so no recording.

## `audio[1]` Naver

Naver's text-to-speech answers every headword, since a synthesizer has no missing entries.
The address is the recording, so the `link` strategy answers it once the fetch succeeded.
The speaker is `ko_kr` and the service `krdic`, and any other speaker code answers 400.

## `audio[2]` Wiktionary

Wiktionary embeds Commons recordings as protocol-relative addresses, so `prefix` supplies the scheme.
Only the transcoded mp3 rendition is taken, because the original is Ogg or WAV.
The first attempt takes a `Ko-` filename after the Korean section heading.
The second takes a Lingua Libre `LL-Q9176_%28kor%29-` one, Q9176 being Korean.

## `frequency[0]` Krdict

Krdict grades its learner vocabulary with one to three stars after the phonetic hangul.
Three stars is 초급, the beginner grade, two 중급, intermediate, and one 고급, advanced.
A grade is an editor's rank with no count behind it, so pattern bands label it instead of an interval.
The star span carries no text, so the pattern reads the grade's name off the legend table below the results.
Each alternative counts the stars of the first exact entry and captures the matching legend cell.
The alternatives share one named group, so `group` 1 is the grade whichever alternative fired.
The bands then label 초급 core, 중급 everyday and 고급 advanced.
An entry outside the learner vocabulary carries no stars, so the attempt yields nothing.

## `morphology`

The list stands empty until a source is pinned.

## `vocabulary.json`

The file holds only the order of the two frame fields an example row shows.
It has no `registers`, `parts`, `features`, `values`, `paradigms` or `retired` section.
Each loader reads a missing section as empty, so Korean offers no register or part-of-speech presets.
A user may still type any register or part of speech, which is stored as typed.

## `exampleOrder`

Korean lists `dependence` first and `particle` second, so the dependence is written before the particle.
Without both names the default order applies.

## Stamp

Commit: `c2f335c331fe`
- `source.json` `c541c3ef69665f2d`
- `vocabulary.json` `c520c8e788f90369`

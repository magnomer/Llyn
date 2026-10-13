# Hindi

The Hindi language pack, declared as data in `source.json`.
The pack format is documented in `languages/English/English.comment.md`.

## `source.json`

Section heading anchors allow attributes before `id`, which the REST rendering writes there.
No varieties are declared, so every reading is the standard one.
No respelling groups are declared, so every reading shows as its source wrote it.
Segoe UI has no Devanagari glyphs, so the headword and example blocks name Nirmala UI first.
Nirmala UI ships with every Windows install since 8.
Georgia has no Devanagari either, so the example block does not name it.
The gloss block keeps Georgia, since the translations under an example are not Devanagari.

## `spelling`

Windows keyboards may type a nukta consonant as one precomposed letter, U+0958 to U+095F.
Wiktionary titles and the Leipzig corpus write the base consonant followed by the nukta sign U+093C.
Leipzig keeps the precomposed spelling as a separate, far rarer entry.
Live, decomposed लड़का gave class 12 and the precomposed one class 15.
So each row turns one precomposed letter into its base and nukta.

## `transcription[0]` Romanization

This is Wiktionary's own Hindi transliteration, close to IAST.
The match anchors on the Hindi section heading so an Angika or Awadhi section listed before it is skipped.
The first span after the heading belongs to the first part of speech.

## `pronunciation[0]` Wiktionary

The hi-IPA template derives the IPA from the Devanagari, so the wikitext carries none and the rendered HTML is read.
The first IPA span after the Hindi section heading is the headword's own reading between slashes.
A Delhi or Urdu-style variant that follows is left alone.

## `audio[0]` Naver

Naver's text-to-speech answers every headword, since a synthesizer has no missing entries.
The address is the recording, so the `link` strategy answers it once the fetch succeeded.
The speaker is `hi_in` and the service `hidic`, and any other speaker code answers 400.

## `audio[1]` Wiktionary

Wiktionary embeds Commons recordings as protocol-relative addresses, so `prefix` supplies the scheme.
Only the transcoded mp3 rendition is taken, because the original is Ogg or WAV.
The first attempt takes a `Hi-` filename after the Hindi section heading.
The second takes a Lingua Libre `LL-Q1568_%28hin%29-` one, Q1568 being Hindi.
Most Hindi recordings on Wiktionary are Lingua Libre ones, so the second attempt answers far more often.

## `frequency[0]` Leipzig

The source is the Leipzig Hindi news corpus of 2011 with one million sentences.
The answer is the word's frequency class.
Class 0 is the most frequent word, and each halving of frequency adds one.
The corpus holds 19,177,172 tokens and its most frequent word के occurs 817,791 times.
So the once rule doubles per class from a factor of 23, the interval of that word.
The ladder rungs are fixed in the engine at one word in 10,000, 100,000 and 1,000,000.
With factor 23, class 8 is one word in 5,888, so classes up to 8 are core.
Class 12 is one in 94,208, so classes 9 to 12 are everyday.
Class 15 is one in 753,664, so classes 13 to 15 are advanced and 16 and up rare.
An unknown spelling answers 404, so the attempt yields nothing.

## `morphology`

The list stands empty until a source is pinned.

## Stamp

Commit: `c2f335c331fe`
- `source.json` `55f99013bc187d73`

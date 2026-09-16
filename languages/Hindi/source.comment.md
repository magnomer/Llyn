# source.json

## pack

Language pack for Hindi.
The pack format is documented in `languages/English/source.comment.md`.
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
Pinned live, decomposed लड़का gave class 12 and the precomposed one class 15.
So each row turns one precomposed letter into its base and nukta.

## `transcription[0]` Romanization

Wiktionary's own Hindi transliteration, close to IAST.
Each headword line prints it in a `<span class="headword-tr">`.
The match anchors on the Hindi section heading so an Angika or Awadhi section listed before it is skipped.
The first span after the heading belongs to the first part of speech.
Pinned live, किताब gave kitāb, पानी pānī, लड़का laṛkā and अच्छा acchā.

## `pronunciation[0]` Wiktionary

The IPA is derived from the Devanagari by the hi-IPA template, so the wikitext carries none.
The rendered HTML is therefore read.
The first IPA span after the Hindi section heading is the headword's own reading between slashes.
A Delhi or Urdu-style variant that follows is left alone.
Pinned live, किताब gave /kɪ.t̪ɑːb/, जाना /d͡ʒɑː.nɑː/, लड़का /ləɽ.kɑː/ and अच्छा /ət̪.t͡ʃʰɑː/.

## `audio[0]` Naver

Naver's Hindi dictionary text-to-speech, one mp3 per request.
The address is the recording, so the `link` strategy answers it once the fetch succeeded.
The speaker is `hi_in` and the service `hidic`, and any other speaker code answers 400.
Every headword answers, since a synthesizer has no missing entries.

## `audio[1]` Wiktionary

Wiktionary embeds Commons recordings as protocol-relative addresses, so `prefix` supplies the scheme.
Only the transcoded mp3 rendition is taken, because the original is Ogg or WAV.
The first attempt takes a `Hi-` filename after the Hindi section heading.
The second takes a Lingua Libre `LL-Q1568_%28hin%29-` one, Q1568 being Hindi.
Most Hindi recordings on Wiktionary are Lingua Libre ones, so the second attempt answers far more often.
Pinned live, पानी and अच्छा gave a Lingua Libre recording and किताब none.

## `frequency[0]` Leipzig

Leipzig Corpora Collection, Hindi news corpus of 2011 with one million sentences.
The answer carries the word's frequency class.
It is 0 for the most frequent word and one higher each time the frequency halves.
The corpus holds 19,177,172 tokens and its most frequent word के occurs 817,791 times.
So the once rule doubles per class from a factor of 23, the interval of that word.
The ladder rungs are fixed in the engine at one word in 10,000, 100,000 and 1,000,000.
With factor 23, class 8 is one word in 5,888, so it and every lower class are core.
Class 12 is one in 94,208, so classes 9 to 12 are everyday.
Class 15 is one in 753,664, so classes 13 to 15 are advanced and 16 and up rare.
An unknown spelling answers 404, so the attempt yields nothing.
Pinned live, पानी gave class 7, किताब class 9 and लड़का class 12.

## `morphology`

The list stands empty until a source is pinned.

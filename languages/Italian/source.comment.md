# source.json

## pack

Language pack for Italian.
The pack format is documented in `languages/English/source.comment.md`.
No varieties are declared, so every reading is the standard one.
No transcription schemes are listed, because Italian is written in Latin script.
No respelling groups are declared, so every reading shows as its source wrote it.

## `pronunciation[0]` Wiktionary

The IPA is derived from the spelling by the it-IPA template, so the wikitext carries none.
The rendered HTML is therefore read.
The match anchors on the Italian section heading, so a Corsican or Catalan section listed before it is skipped.
The first IPA span after the heading is the headword's own phonemic reading between slashes.
The rhyme and any regional reading that follow it are left alone.
Pinned live, casa gave /ˈka.sa/, andare /anˈda.re/, bello /ˈbɛl.lo/ and mangiare /manˈd͡ʒa.re/.

## `pronunciation[1]` Wikizionario

The Italian Wiktionary wikitext.
Each entry carries `{{IPA|…}}` templates under its `{{-pron-}}` heading.
So the first such template after the `{{-it-}}` heading is taken.
Wikizionario writes no syllable dots and marks vowel length, so cane gave /ˈkaːne/ against Wiktionary's /ˈka.ne/.
A page listing a regional reading first gives that one.
So casa gave the northern /ˈkaza/ ahead of the standard /ˈkasa/.
A page without an Italian section yields nothing.
Pinned live, andare gave /anˈda.re/, bello /ˈbɛllo/ and mangiare /manˈdʒare/.

## `audio[0]` Naver

Naver's Italian dictionary text-to-speech, one mp3 per request.
The address is the recording, so the `link` strategy answers it once the fetch succeeded.
The speaker is `it_it` and the service `itdic`, and any other speaker code answers 400.
Every headword answers, since a synthesizer has no missing entries.
The Naver Italian-Korean dictionary search API is not listed, because its entries carry no recording.
Pinned live, casa answered with `pronunFileCount` 0 and an empty `symbolFile`.

## `audio[1]` Wiktionary

Wiktionary embeds Commons recordings as protocol-relative addresses, so `prefix` supplies the scheme.
Only the transcoded mp3 rendition is taken, because the original is Ogg or WAV.
The first attempt takes an `It-` filename after the Italian section heading.
The filename may carry an article, as It-un_gatto.ogg under gatto.
The second takes a Lingua Libre `LL-Q652_%28ita%29-` one, Q652 being Italian.
Pinned live, casa, andare, cane, bello, acqua and mangiare each gave an `It-` recording and ragazzo a Lingua Libre one.

## `frequency[0]` Leipzig

Leipzig Corpora Collection, Italian news corpus of 2012 with one million sentences.
The answer carries the word's frequency class.
It is 0 for the most frequent word and one higher each time the frequency halves.
The corpus holds 19,895,912 tokens and its most frequent word di occurs 741,109 times.
So the once rule doubles per class from a factor of 27, the interval of that word.
The ladder rungs are fixed in the engine at one word in 10,000, 100,000 and 1,000,000.
With factor 27, class 8 is one word in 6,912, so it and every lower class are core.
Class 11 is one in 55,296, so classes 9 to 11 are everyday.
Class 15 is one in 884,736, so classes 12 to 15 are advanced and 16 and up rare.
An unknown spelling answers 404, so the attempt yields nothing.
Pinned live, casa gave class 6, andare class 7, bello class 9, cane class 11 and gatto class 12.

## `morphology`

The list stands empty until a source is pinned.

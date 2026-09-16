# source.json

## pack

Language pack for German.
The pack format is documented in `languages/English/source.comment.md`.
No varieties are declared, so every reading stands for the German of Germany.
The audio sources pass over recordings tagged Austrian or Swiss.
No transcription schemes are listed, because German keeps no romanization beside its IPA.
No respelling groups are declared, so every reading shows as its source wrote it.

## `pronunciation[0]` Wiktionary

The English Wiktionary rendered page.
Most German entries derive their transcription with `{{de-IPA}}`, which the wikitext does not carry.
So the rendered HTML is read.
The match anchors on the German section heading, so an Alemannic or Bavarian section listed before it is skipped.
The first IPA span after the heading is the headword's own.
The rhyme and the regional readings that follow it are left alone.
The REST rendering is asked first and the article page second, so a 429 from the REST host still answers.
Pinned live, Hund gave /hʊnt/, gehen /ˈɡeːən/, schön /ʃøːn/ and Haus [haʊ̯s].

## `pronunciation[1]` Wiktionary (de)

The German Wiktionary wikitext.
Every entry carries a `{{Lautschrift|…}}` template under its Aussprache heading.
So the first such template after the `{{Sprache|Deutsch}}` heading is the headword's own transcription.
An empty template is skipped by the pattern.
A page without a German section yields nothing.
Pinned live, Hund gave hʊnt, gehen ˈɡeːən, Mädchen ˈmɛːtçən and Haus haʊ̯s.

## `audio[0]` Naver

Naver German-Korean dictionary search API.
Each entry carries an opaque, token-bearing `symbolFile` audio URL, empty on an entry without a recording.
The pattern takes the first non-empty one whose entry's `handleEntry` is the headword itself.
A compound listed after it, such as Seehund under Hund, is so passed over.
The API returns an empty body without a Referer header, so one is declared here.

## `audio[1]` Wiktionary

Wiktionary embeds Commons recordings as protocol-relative addresses, so `prefix` supplies the scheme.
Only the transcoded mp3 rendition is taken, because the original is Ogg Vorbis or WAV.
The first attempt takes a `De-` filename after the German section heading.
A `De-at-` or `De-ch-` filename is an Austrian or Swiss recording and is skipped.
The second takes a Lingua Libre `LL-Q188_%28deu%29-` one, Q188 being German.
Pinned live, Hund gave De-Hund.ogg.mp3 ahead of De-at-Hund.ogg.mp3.

## `frequency[0]` Leipzig

Leipzig Corpora Collection, German news corpus of 2012 with one million sentences.
The answer carries the word's frequency class.
It is 0 for the most frequent word and one higher each time the frequency halves.
The corpus holds 16,898,449 tokens and its most frequent word der occurs 480,349 times.
So the once rule doubles per class from a factor of 35, the interval of that word.
The ladder rungs are fixed in the engine at one word in 10,000, 100,000 and 1,000,000.
With factor 35, class 8 is one word in 8,960, so it and every lower class are core.
Class 11 is one in 71,680, so classes 9 to 11 are everyday.
Class 14 is one in 573,440, so classes 12 to 14 are advanced and 15 and up rare.
An unknown spelling answers 404, so the attempt yields nothing.
Pinned live, Haus gave class 8 and Hund class 11.

## `morphology`

The list stands empty until a source is pinned.
The paradigms in `vocabulary.json` then say which forms an entry expects.

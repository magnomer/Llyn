# German

The German language pack and its display vocabulary, both declared as data.
The pack format is documented in `languages/English/English.comment.md`.
The vocabulary file format is documented in `languages/French/French.comment.md`.

## `source.json`

Section heading anchors allow attributes before `id`, which the REST rendering writes there.
No varieties are declared, so every reading stands for the German of Germany.
The first Wiktionary audio attempt passes over recordings tagged Austrian or Swiss.
No transcription schemes are listed, because German keeps no romanization beside its IPA.
No respelling groups are declared, so every reading shows as its source wrote it.

## `pronunciation[0]` Wiktionary

Most German entries derive their transcription with `{{de-IPA}}`, which the wikitext does not carry.
So the rendered HTML is read.
The match anchors on the German section heading, so an Alemannic or Bavarian section listed before it is skipped.
The first IPA span after the heading is the headword's own.
The rhyme and the regional readings that follow it are left alone.
The REST rendering is asked first and the article page second, so a 429 from the REST host still answers.

## `pronunciation[1]` Wiktionary (de)

Every entry carries a `{{Lautschrift|…}}` template under its Aussprache heading.
So the first such template after the `{{Sprache|Deutsch}}` heading is the headword's own transcription.
An empty template is skipped by the pattern.
A page without a German section yields nothing.

## `audio[0]` Naver

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

## `frequency[0]` Leipzig

The source is the Leipzig German news corpus of 2012 with one million sentences.
The answer is the word's frequency class.
Class 0 is the most frequent word, and each halving of frequency adds one.
The corpus holds 16,898,449 tokens and its most frequent word der occurs 480,349 times.
So the once rule doubles per class from a factor of 35, the interval of that word.
The ladder rungs are fixed in the engine at one word in 10,000, 100,000 and 1,000,000.
With factor 35, class 8 is one word in 8,960, so classes up to 8 are core.
Class 11 is one in 71,680, so classes 9 to 11 are everyday.
Class 14 is one in 573,440, so classes 12 to 14 are advanced and 15 and up rare.
An unknown spelling answers 404, so the attempt yields nothing.

## `morphology`

The list stands empty until a source is pinned.
The paradigms in `vocabulary.json` then say which forms an entry expects.

## `vocabulary.json`

Every id is an integer that never changes once published.
Valency and other subtypes belong to the dependence field and the morphology, not to the parts.
Postposition stands beside preposition, for entlang and zufolge.
No partitive article is listed, since German has none.

## `retired`

It lists the parts once published and since removed, so no id is ever reused.
The import applies every row on each start, and a row whose part is already gone does nothing.
`into` names the part that takes over the removed part's links.
`unit` names the lexical unit set on a linking entry that has none yet.
A row with neither drops the links.

## `features`

A noun takes gender and number.
An adjective takes form, the degree of comparison.
A verb takes form, class and auxiliary, and class is weak, strong or mixed.
A preposition takes case, with two-way for one governing accusative or dative.

## `paradigms`

The noun paradigm predicts the plural.
A headword ending in in doubles the n and takes en.
One ending in ung, heit, keit, schaft, tät, ion, ie or ei takes en.
One ending in e takes n.
One ending in el, er, en, chen or lein stays.
One ending in a vowel other than e takes s, and any other takes e.
An umlaut plural such as Häuser is so irregular and carries a note.
The adjective paradigm predicts the comparative and the attributive superlative stem.
The comparative adds er, or r after a final e.
The superlative adds st, or est after d, t, s, ß, x, z or sch.
An umlaut such as älter is irregular.
The verb paradigm predicts the third-person singular present, the preterite and the past participle.
The present adds t to the stem, or et after d, t or a consonant plus m or n.
The preterite adds te or ete the same way.
The participle wraps the stem in ge and t, or et after d or t.
A stem ending in ier takes no ge.
Neither does one opening with be, emp, ent, er, ge, miss, ver or zer.
An eln or ern infinitive loses only its n.
A strong verb such as gehen is so irregular in every form and carries a note.

## Stamp

Commit: `c2f335c331fe`
- `source.json` `530896c8c389c111`
- `vocabulary.json` `3e5c923996378a2f`

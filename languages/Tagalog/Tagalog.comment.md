# Tagalog

The Tagalog language pack and its display vocabulary, both declared as data.
The pack format is documented in `languages/English/English.comment.md`.

## `source.json`

Section heading anchors allow attributes before `id`, which the REST rendering writes there.
No varieties are declared, so every reading is the standard Manila one.
No transcription schemes are listed, because Tagalog is written in Latin script.
No respelling groups are declared, so every reading shows as its source wrote it.
No Naver source is listed, because its Tagalog-Korean dictionary carries no recordings.
Every entry there answers an empty `symbolFile`, and its speech service answers 400 for the `tl_ph` speaker code.

## `pronunciation[0]` Wiktionary

The tl-pr template derives the IPA from the spelling, so the wikitext carries none and the rendered HTML is read.
The match anchors on the Tagalog section heading, so a Cebuano or Ilocano section listed before it is skipped.
The match also refuses to cross another section heading.
A Tausug or Waray-Waray section listed after an entry without IPA would otherwise answer.
The match takes only a bare `<span class="IPA nowrap">`, one without attributes.
An etymology line may quote a sound in an IPAchar span before the pronunciation, as bahay does with /l/.
That span carries template attributes, so the bare form passes it over.
The first bare span after the heading is the headword's own phonemic reading between slashes.
The phonetic reading in brackets, the rhyme, and a second etymology's reading are left alone.

## `audio[0]` Wiktionary

Wiktionary embeds Commons recordings as protocol-relative addresses, so `prefix` supplies the scheme.
Only the transcoded mp3 rendition is taken, because the original is Ogg Vorbis or WAV.
The first attempt takes a `Tl-` filename after the Tagalog section heading, without crossing another heading.
The Commons files are named `Tl-ph-` or `Tl-PH-`, and the pattern accepts both.
The second takes a Lingua Libre `LL-Q34057_%28tgl%29-` one, Q34057 being Tagalog.
Few Tagalog entries carry any recording.

## `frequency[0]` Leipzig

The source is the Leipzig Tagalog news crawl corpus of 2013 with 300,000 sentences.
The answer is the word's frequency class.
Class 0 is the most frequent word, and each halving of frequency adds one.
The corpus holds 5,748,008 tokens and its most frequent word sa occurs 347,416 times.
So the once rule doubles per class from a factor of 17, the interval of that word.
The ladder rungs are fixed in the engine at one word in 10,000, 100,000 and 1,000,000.
With factor 17, class 9 is one word in 8,704, so classes up to 9 are core.
Class 12 is one in 69,632, so classes 10 to 12 are everyday.
Class 15 is one in 557,056, so classes 13 to 15 are advanced and 16 and up rare.
An unknown spelling answers 404, so the attempt yields nothing.

## `morphology`

The list stands empty until a source is pinned.

## `vocabulary.json`

The presets supply part of speech labels for entries.
People can still type labels absent from the lists.
Each id is permanent after publication, and renaming a preset keeps its id.
The shell shows the names in list order.
No inflection features or values are preset yet.

## `registers`

These are the registers the shell offers on a card.
People can still write a register absent from the list.

## `retired`

It lists the parts once published and since removed, so no id is ever reused.
The import applies every row on each start, and a row whose part is already gone does nothing.
`into` names the part that takes over the removed part's links.
`unit` names the lexical unit set on a linking entry that has none yet.
A row with neither drops the links.

## Stamp

Commit: `c2f335c331fe`
- `source.json` `7e41521682c73c9b`
- `vocabulary.json` `7260772236f60bb1`

# Thai

The Thai language pack and its display vocabulary, both declared as data.
The pack format is documented in `languages/English/English.comment.md`.

## `source.json`

No varieties are declared, so every reading is the standard one.
No respelling groups are declared, so every reading shows as its source wrote it.
Segoe UI has no Thai glyphs, so the headword and example blocks name Leelawadee UI first.
Georgia has no Thai either, so the example block does not name it.
The gloss block keeps Georgia, since the translations under an example are not Thai.
No Leipzig source is listed, because the Leipzig API exposes no Thai corpus.

## `transcription[0]` Paiboon

Paiboon romanization marks tone and vowel length.
The th-pron table prints it in a `<span class="tr">` in the first cell after the Paiboon row header.
The match anchors on the Thai section heading, so a Northern Thai or Isan section listed before it is skipped.
The match also refuses to cross another section heading.
The first cell belongs to the headword's own reading, and bound forms in later cells are left alone.

## `transcription[1]` RTGS

The Royal Thai General System marks neither tone nor length.
The same table prints it after the Royal Institute row header.
The match anchors and stops the same way as the Paiboon one.

## `pronunciation[0]` Wiktionary

The th-pron template derives the IPA from the Thai, so the wikitext carries none and the rendered HTML is read.
The match anchors on the Thai section heading and refuses to cross another.
A Northern Thai or Nyaw section before it carries its own IPA span, which น้ำ showed.
The first IPA span after the heading is the headword's own phonemic reading between slashes.
The tone is written in Chao letters after each syllable.
A bound form or rhyme span that follows is left alone.

## `audio[0]` Naver

Naver's text-to-speech answers every headword, since a synthesizer has no missing entries.
The address is the recording, so the `link` strategy answers it once the fetch succeeded.
The speaker is `th_th` and the service `thdic`, and any other speaker code answers 400.

## `audio[1]` Wiktionary

Wiktionary embeds Commons recordings as protocol-relative addresses, so `prefix` supplies the scheme.
Only the transcoded mp3 rendition is taken, because the original is Ogg.
The first attempt takes a `Th-` filename after the Thai section heading, without crossing another heading.
The second takes a Lingua Libre `LL-Q9217_%28tha%29-` one, Q9217 being Thai.
Few Thai entries carry a recording of either kind.

## `frequency[0]` Wiktionary

The source is the Chulalongkorn University list of the two thousand most frequent Thai words, kept on Wiktionary.
The page has two sections, `== 0-1000 ==` and `== 1001-2000 ==`, each one line of bracketed words.
The match captures the section's first rank and finds the bracketed headword without crossing into the next section.
The rank is no figure, so pattern bands label the first thousand core and the second everyday.
A word outside the list yields nothing.

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
- `source.json` `680d79744abe1c29`
- `vocabulary.json` `3308f72f1660f354`

# Swahili

The Swahili language pack and its display vocabulary, both declared as data.
The pack format is documented in `languages/English/English.comment.md`.

## `source.json`

Section heading anchors allow attributes before `id`, which the REST rendering writes there.
No varieties are declared, so every reading is the standard one.
No transcription schemes are listed, because Swahili is written in Latin script.
No respelling groups are declared, so every reading shows as its source wrote it.
The Leipzig API exposes no Swahili corpus, so the frequency source is a Wiktionary rank list instead.
No Naver source is listed, because Naver has no Swahili dictionary.
Its speech service answers 400 for the `sw_ke` speaker code.

## `pronunciation[0]` Wiktionary

The sw-IPA template derives the IPA from the spelling, so the wikitext carries none and the rendered HTML is read.
The match anchors on the Swahili section heading, so a Chichewa or Mwani section listed before it is skipped.
The match also refuses to cross another section heading.
Many Swahili entries carry no IPA, and a Tooro or Yola section listed after them may.
Without that guard kitabu answered the Tooro reading.
The first IPA span after the heading is the headword's own phonemic reading between slashes.
A second span, an aspirated variant as under mtu, is left alone.

## `audio[0]` Wiktionary

Wiktionary embeds Commons recordings as protocol-relative addresses, so `prefix` supplies the scheme.
Only the transcoded mp3 rendition is taken, because the original is FLAC.
The first attempt takes a `Sw-` filename after the Swahili section heading, without crossing another heading.
Nearly every Swahili recording on Wiktionary is a `Sw-ke-` one, a Kenyan speaker.
The second takes a Lingua Libre `LL-Q7838_%28swa%29-` one, Q7838 being Swahili.

## `frequency[0]` Wiktionary

Wiktionary keeps a Swahili frequency list drawn from the 2011 Swahili Wikipedia, a 30,000 sentence Leipzig corpus.
The raw wikitext of its main page is fetched, one address without the headword.
The page lists the 10,000 most frequent words under headings of a thousand ranks each, in rank order.
The match finds the heading whose band holds the headword link and captures the band's first rank.
The tempered scan refuses to cross the next heading, so a word lands in its own band alone.
The links are case-sensitive, so kwa and Kwa are separate rows.
The captured figure is the band floor, so the tooltip reads the band's most frequent word.
The once factor of 12 is the same Zipf estimate Logeion uses, since a rank alone gives no count.
Rank 1 so grades core, 1,001 through 8,001 everyday, and 9,001 advanced.
Two subpages hold ranks 10,000 through 60,000 alphabetically without headings, so no figure can be read from them.
They are left out, and a word past rank 10,000 gets no row.
Spoken words such as asante and jambo fall past rank 10,000, because the corpus is encyclopedic.

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
- `source.json` `867cf0ce2725206e`
- `vocabulary.json` `21f5375b741dd344`

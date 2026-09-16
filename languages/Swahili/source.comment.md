# source.json

## pack

Language pack for Swahili.
The pack format is documented in `languages/English/source.comment.md`.
No varieties are declared, so every reading is the standard one.
No transcription schemes are listed, because Swahili is written in Latin script.
No respelling groups are declared, so every reading shows as its source wrote it.
The Leipzig API exposes no Swahili corpus, so the frequency source is a Wiktionary rank list instead.
No Naver source is listed, because Naver has no Swahili dictionary.
Its speech service answers 400 for the `sw_ke` speaker code.

## `pronunciation[0]` Wiktionary

The IPA is derived from the spelling by the sw-IPA template, so the wikitext carries none.
The rendered HTML is therefore read.
The match anchors on the Swahili section heading, so a Chichewa or Mwani section listed before it is skipped.
The match also refuses to cross another section heading.
Many Swahili entries carry no IPA, and a Tooro or Yola section listed after them may.
Without that guard kitabu answered the Tooro reading.
The first IPA span after the heading is the headword's own phonemic reading between slashes.
A second span, an aspirated variant as under mtu, is left alone.
Pinned live, nyumba gave /ˈɲu.ᵐbɑ/, maji /ˈmɑ.ʄi/, kwenda /ˈkʷɛ.ⁿdɑ/ and chakula /tʃɑˈku.lɑ/.
Kitabu, mtoto and rafiki gave none.

## `audio[0]` Wiktionary

Wiktionary embeds Commons recordings as protocol-relative addresses, so `prefix` supplies the scheme.
Only the transcoded mp3 rendition is taken, because the original is FLAC.
The first attempt takes a `Sw-` filename after the Swahili section heading, without crossing another heading.
Nearly every Swahili recording on Wiktionary is a `Sw-ke-` one, a Kenyan speaker.
The second takes a Lingua Libre `LL-Q7838_%28swa%29-` one, Q7838 being Swahili.
Pinned live, nyumba, maji, kitabu, kwenda, mtoto, mtu, chakula, shule and rafiki each gave a `Sw-ke-` recording.
So did asante, jambo, habari, karibu and ndiyo.
None of them carried a Lingua Libre one.

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
Pinned live, nyumba, mtu, kwenda, mji, kitabu, mtoto, chakula and shule answered 1, rafiki 1,001, kikombe 2,001.
Asante, jambo, habari, karibu and ndiyo are spoken words the encyclopedia corpus leaves past rank 10,000.
The scan takes under 15 ms on the 280 KB page, well inside the two second patience.

## `morphology`

The list stands empty until a source is pinned.

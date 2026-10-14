# Mandarin

The Mandarin language pack and its display vocabulary, both declared as data.
Every URL and extraction rule is data, so an edit here needs no recompile.

## `source.json`

The `pronunciation` and `audio` lists are independent and each is shown in the order written.
The pronunciation sources publish IPA, while Pinyin and Zhuyin are the transcription schemes above.
No morphology sources are listed, because Mandarin words do not inflect.
Wiktionary readings come from ordinary article pages, except the wikitext lookups of Pinyin and Traditional.
The audio sources use the dictionary and Commons APIs.
The article hosts impose no request ceiling, while the REST endpoints reject bursts with 429.
The reader would report such a rejection as an unreachable source.
A simplified-form page only points at its traditional form, so every attempt follows the see link of the rendered page.

## `respelling[0]` Standard

Standard Mandarin is written in a plain phonemic convention, so `phonemic` is on and the respelling stands between slashes.
Wiktionary writes a tie bar over every affricate, and the first row drops it.
The retroflex stop ʈ becomes plain t, so ʈ͡ʂ prints as tʂ.
The velar fricative x becomes h and the alveolo-palatal fricative ɕ becomes s.
The syllabic z̩, the close-mid back unrounded ɤ and the uɤ sequence all become ə.
The uɤ rule comes before ɤ so the longer match wins.
The near-close vowels ɪ and ʊ become i and u.
The open-mid vowels ɛ and ɔ become e and o.
The centralized ä and the open back ɑ become a.
The composed ä matches because the reading is composed first.
The last row keeps only the realized tone of a sandhi pair, so ²¹⁴⁻²¹ prints as ²¹.
It matches superscript digits joined by a superscript or plain hyphen, and keeps the digits after the joiner.
The stored reading keeps the whole pair, and an unrespelled view still shows it.
The pack declares no varieties, so the group runs on every reading.

## `transcription[0]` Pinyin

The wikitext attempt reads the `|m=` parameter of every `{{zh-pron}}` template on the page.
Wiktionary keeps that parameter as the canonical Mandarin reading.
A comma separates alternative readings and each is shown.
`normalize` is on so the alternatives are split, and Pinyin carries no delimiter it could strip.
The parameter also carries markers after a comma, such as `1nb=` for a note and `er=y` for an erhua form.
So the pattern stops at the first piece holding an equals sign.
The HTML attempt is the fallback.
It reads the Latn span of every Hanyu Pinyin line of the rendered pronunciation box with the span strategy.
The line nests a link and a phonetic note the flat pattern would take too.
The article page is asked before the REST rendering, because the REST host rejects bursts with 429.
A simplified-form or variant-form page only points at its main form through `{{zh-see}}`.
The wikitext attempt follows that pointer and the later attempts ask for the form it named.

## `transcription[1]` Bopomofo

Wiktionary derives Zhuyin from the Pinyin at render time, so only the rendered page carries it.
It sits in the Bopo span of the Zhuyin line.
Every line of the box is read, one per alternative.
The box also opens with a summary line joining every alternative with a comma.
That line closes its label with a bracket before the small tag.
So the match anchors on the label without the bracket and keeps the summary out.
The article page is asked before the REST rendering, because the REST host rejects bursts with 429.
A simplified-form page only points at its main form, so the attempt follows the see link.

## `glyph` Traditional

It gives one transcription row under the scheme named here, and the language each character opens an entry in.
The first wikitext attempt reads the `{{zh-see}}` pointer, which names the traditional main form.
The pointer's later parameters are cut at the first bar.
A page with a second pointer so yields a second candidate.
A page that is already traditional carries no pointer, so the first attempt finds nothing there.
The second attempt then answers the page's own title, so Edit's search and the reading view agree.
Its confirm pattern asks for a Chinese section and no pointer, so a page of another language yields nothing.
A word with no page answers an error body without a Chinese section, so the search still finds nothing.
The second attempt asks the same address again, and only when the first finds nothing.

## `pronunciation[0]` Wiktionary

Only the Mandarin row links its IPA key to the Mandarin pronunciation appendix.
That link anchors the reader to the right row and keeps Cantonese, Hakka and Min readings out.
Every such row on the page is read, one per alternative reading and per etymology.
The see link is followed even when the page has readings of its own.
A character can be both a word and the simplified form of another.
The transcription nests tone-note spans inside the IPA span, which a flat pattern would truncate.
The span strategy is required for that reason.
An entry with no Mandarin section carries no anchor, so the attempt yields nothing.

## `pronunciation[1]` Chinese Wiktionary

There the key of the Mandarin row links to the Wiktionary:漢語發音表記 page and the label reads 幫助.
The regional Mandarin rows and every other variety link their key to an encyclopedia article instead.
So the anchor keeps them out.
Its coverage of Mandarin headwords is thinner than the English one, so it stands second.
A simplified-form page only points at its traditional form through a 請見 box, so the attempt follows that.

## `audio[0]` Naver

Each entry of the Chinese-Korean search API carries a `symbolFile`.
It holds a female and a male recording, joined with a bar.
Each is an opaque token-bearing mp3 address that the file host rejects without its token.
The first entry matched exactly is taken, whether as an entry or as a sub-entry.
A traditional headword lands on the simplified entry as a sub-entry.
The anchor at the start of the body keeps every later entry out.
An idiom entry ahead of it carries a bare identifier rather than an address and is passed over.
Two untagged readings take the female and the male address from that one match.
The API returns an empty body without a Referer header, so one is declared here.

## `audio[1]` Daum

The search page answers in one of two shapes.
A headword with one exact entry gets no list.
It gets only a meta refresh pointing at the entry page by its wordid.
So the first attempt captures that wordid as its follow value.
The second attempt asks the entry page, where the recording address stands in the `wordPronFileUrl` script variable.
The hop the follow spends re-searching the wordid itself finds nothing and costs one fetch.
A headword with several entries, or none, gets a list.
Each entry there prints its headword in a `txt_cleansch` or `txt_searchword` link, with the matched part in an emphasis span.
The recording is a data-url on the listen button.
The pattern takes every entry whose whole headword is exactly `{word}`.
So a list of related words for an unknown headword yields nothing.
The addresses are printed with a plain http scheme.
So only the host and path are captured, and `prefix` supplies https.
The entry page refuses a bare Mozilla/5.0 user agent, but accepts the engine's own.
A traditional headword finds no entry, because the dictionary is keyed on simplified forms.

## `audio[2]` Wiktionary

Wiktionary embeds Commons recordings as protocol-relative addresses, so `prefix` supplies the scheme.
Only the transcoded mp3 rendition is taken, because the original is OGG or WAV.
A recording is either a Lingua Libre `LL-Q9192_%28cmn%29-` file or a `Zh-` file.
The `Zh-` files of a page are also written `Zh_` with an underscore, so both separators are taken.
The `Zh-` prefix must not continue as `Zh-yue-` or `Zh-nan-`, which mark Cantonese and Min Nan recordings.
One attempt reads both, so the page is fetched once.
A simplified-form page only points at its traditional form, so the attempt follows the see link.

## `audio[3]` Wikimedia Commons

Most Mandarin recordings on Commons are never embedded on a Wiktionary page, so the file store is searched directly.
Every Lingua Libre `LL-Q9192 (cmn)-` file carries the cmn tag the search asks for.
Search matches by character, so a title holding the headword inside a longer phrase is returned too.
The pattern therefore takes only a file whose title ends in exactly `{word}`.
Only the mp3 rendition is taken, because the original is WAV.
`utf8=1` keeps the title readable so the headword can be matched.
`formatversion=2` keeps the addresses unescaped.

## `frequency[0]` Purple Culture

The dictionary page marks a word on the HSK vocabulary lists with a green badge.
The badge holds HSK 1 to HSK 6.
An HSK level is a syllabus grade with no count behind it, so pattern bands label it, not an interval.
The bands carry the ladder names: HSK 1 and 2 core, 3 and 4 everyday, 5 and 6 advanced.
The headword badge comes first when compounds below carry their own.
The page title names the word looked up, so the confirm guard fails for an unknown spelling.
A word outside the HSK lists has no badge, so the attempt yields nothing.
A traditional spelling resolves to the same page as its simplified form.

## `vocabulary.json`

The parts of speech an entry may carry are presets in dropdowns.
A user may still type a value no preset names, which is stored as typed.
Every id is an integer that never changes once published.
Renaming a preset keeps its id, so every entry linking it follows the rename.
Order is the order of the lists.
A language is added by creating `languages/<Name>/vocabulary.json`.

## `exampleOrder`

It states which of the two frame fields an example row writes first, `particle` or `dependence`.
It states nothing either may hold.

## `parts`

Each part is a bare role.
Valency and other subtypes belong to the dependence field and the morphology, not here.
A part may carry `parent`, the id of the part it specialises.
A paradigm declared on the parent applies to it.

## `retired`

It lists the parts once published and since removed, so no id is ever reused.
The import applies every row on each start, and a row whose part is already gone does nothing.
`into` names the part that takes over the removed part's links.
`unit` names the lexical unit set on a linking entry that has none yet.
A row with neither drops the links.

## Stamp

Commit: `aae5349dbb94`
- `source.json` `9c6a693f7140b2cc`
- `vocabulary.json` `f75daa1c13010703`

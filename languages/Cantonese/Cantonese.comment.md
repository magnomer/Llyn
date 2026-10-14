# Cantonese

The Cantonese language pack and its display vocabulary, both declared as data.
Every URL and extraction rule is data, so an edit here needs no recompile.

## `source.json`

The `pronunciation` and `audio` lists are independent and each is shown in the order written.
The pronunciation sources publish IPA, while Jyutping and Yale are the transcription schemes above.
No morphology sources are listed, because Cantonese words do not inflect.
The Jyutping, glyph and Commons sources call the MediaWiki API.
The other sources read an ordinary article page.
The article hosts impose no request ceiling, while the REST endpoints reject bursts with 429.
The reader would report such a rejection as an unreachable source.

## `transcription[0]` Jyutping

The wikitext attempt reads the `|c=` parameter of every `{{zh-pron}}` template on the page.
Wiktionary keeps that parameter as the canonical Cantonese reading.
A comma separates alternative readings and each is shown.
`normalize` is on so the alternatives are split, and Jyutping carries no delimiter it could strip.
The parameter also carries markers after a comma, such as `1nb=` for a note on the first reading.
So the pattern stops at the first piece holding an equals sign.
A simplified-form or variant-form page only points at its main form through `{{zh-see}}`.
The attempt follows that pointer and reads the form it named.

## `transcription[1]` Yale

Wiktionary derives Yale from the Jyutping at render time, so only the rendered page carries it.
`normalize` is on so the alternatives one line separates with a slash are split.
The Mandarin box prints a Yale line too, so the match anchors on the link to the Cantonese Yale article.
A tone mark with an alternative is wrapped in a help span, which a flat pattern would truncate.
The span strategy is required for that reason.
The article page is asked before the REST rendering, because the REST host rejects bursts with 429.

## `glyph` Traditional

It gives one transcription row under the scheme named here.
It also gives the language each character opens an entry in.
The first wikitext attempt reads the `{{zh-see}}` pointer, which names the traditional main form.
The pointer's later parameters are cut at the first bar.
A page with a second pointer so yields a second candidate.
A page that is already traditional carries no pointer, so the first attempt finds nothing there.
The second attempt then answers the page's own title, so Edit's search and the reading view agree.
Its confirm pattern asks for a Chinese section and no pointer, so a page of another language yields nothing.
A word with no page answers an error body without a Chinese section, so the search still finds nothing.
The second attempt asks the same address again, and only when the first finds nothing.

## `respelling[0]` Standard

Standard Cantonese vowels are written without length marks and with plain letters.
The result is phonemic, so `phonemic` is on and the respelling stands between slashes.
The sources anchor on the Standard Cantonese row alone, so no other variety reaches these rules.
The vowel ɐ stays as written.

## `pronunciation[0]` Wiktionary

Only the Cantonese row links its IPA key to the Cantonese phonology article.
That link anchors the reader to the right row and keeps Mandarin, Hakka and Min readings out.
Every such row on the page is read, one per etymology, and a row listing two readings yields both.
The see link is followed even when the page has readings of its own.
A character can be both a word and the simplified form of another.
The transcription nests tone-note spans inside the IPA span, which a flat pattern would truncate.
The span strategy is required for that reason.
An entry with no Cantonese section carries no anchor, so the attempt yields nothing.

## `pronunciation[1]` Chinese Wiktionary

The Chinese Wiktionary prints the same transcription behind a Chinese anchor.
Its coverage of colloquial Cantonese headwords is thinner than the English one, so it stands second.
A simplified-form page only points at its traditional form through a 請見 box, so the attempt follows that.

## `audio[0]` Wiktionary

Wiktionary embeds Commons recordings as protocol-relative addresses, so `prefix` supplies the scheme.
Only the transcoded mp3 rendition is taken, because the original is WAV or Opus.
A recording is either a Lingua Libre `LL-Q9186_%28yue%29-` file or a `Zh-yue-` file.
Both prefixes keep the Mandarin `Zh-` and Min Nan `Zh-nan-` recordings of the same page out.
One attempt reads both, so the page is fetched once.
A simplified-form page only points at its traditional form, so the attempt follows the see link.

## `audio[1]` Wikimedia Commons

Most Cantonese recordings on Commons are never embedded on a Wiktionary page, so the file store is searched directly.
Search matches by character, so a title holding the headword inside a longer word is returned too.
The pattern therefore takes only a file whose title ends in exactly `{word}`.
Only the mp3 rendition is taken, because the original is WAV or Opus.
`utf8=1` keeps the title readable so the headword can be matched.
`formatversion=2` keeps the addresses unescaped.

## `frequency[0]` CantoDict

CantoDict serves a word page and a character page, both reached straight from the hanzi.
Each carries an editor-assigned difficulty Level from 1 upward.
The pattern keeps the number alone, and the unit puts the word Level back in the tooltip.
A Level is an editor's grade with no count behind it, so pattern bands label it instead of an interval.
The bands carry the four ladder names, Level 1 as core, 2 as everyday, 3 as advanced, higher as rare.
The bands also accept the older `Level: n` text a workspace fetched before the pattern was narrowed.
A single character has no word page, so the first attempt yields nothing there.
The second attempt reads the character page instead.
An unknown spelling returns a bare detail view without a Level, so the attempt yields nothing.

## `vocabulary.json`

This is the display vocabulary for Cantonese.
Its presets fill the dropdowns, and a user may still type a value no preset names, stored as typed.
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

## `features` and `values`

Features are the grammatical features each part takes, and values are those each feature takes.

## Stamp

Commit: `aae5349dbb94`
- `source.json` `0c2e4b3e1c98c912`
- `vocabulary.json` `e267aab5c680e576`

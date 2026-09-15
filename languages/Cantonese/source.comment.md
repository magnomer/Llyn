# source.json

## pack

Language pack for Cantonese.
Every URL and extraction rule is data, so an edit here needs no recompile.
The `pronunciation` and `audio` lists are independent and each is shown in the order written here.
The pronunciation sources publish IPA, while Jyutping and Yale are the transcription schemes above.
No morphology sources are listed, because Cantonese words do not inflect.
Every source is read from an ordinary article page rather than a machine API.
The article hosts impose no request ceiling, while the REST endpoints reject bursts with 429.
The reader would report such a rejection as an unreachable source.

## `transcription[0]` Jyutping

The wikitext attempt reads the `|c=` parameter of every `{{zh-pron}}` template on the page.
Wiktionary keeps that parameter as the canonical Cantonese reading.
A comma separates alternative readings and each is shown.
`normalize` is on so the alternatives are split, and Jyutping carries no delimiter it could strip.
The same parameter also carries markers after a comma, such as `1nb=` for a note on the first reading.
So the pattern stops at the first piece holding an equals sign.
A simplified-form or variant-form page only points at its main form through `{{zh-see}}`.
The wikitext attempt follows that pointer and the later attempts ask for the form it named.

## `transcription[1]` Yale

Cantonese Yale.
Wiktionary derives it from the Jyutping at render time, so only the rendered page carries it.
Every Yale line on the page is read.
`normalize` is on so the alternatives one line separates with a slash are split.
The Mandarin box prints a Yale line too, so the match anchors on the link to the Cantonese Yale article.
A tone mark with an alternative is wrapped in a help span, which a flat pattern would truncate.
The span strategy is required for that reason.
The article page is asked before the REST rendering, because the REST host rejects bursts with 429.

## `glyph` Traditional

The traditional form the entry is written in.
It gives one transcription row under the scheme named here.
It also gives the language each character opens an entry in.
The wikitext attempt reads the `{{zh-see}}` pointer a simplified-form or variant-form page carries.
That pointer names the traditional main form.
A page that is already traditional carries none, so the lookup finds nothing.
The reading view then shows the headword's own characters.
The pointer's later parameters are cut at the first bar.
A page with a second pointer so yields a second candidate.

## `respelling[0]` Standard

Standard Cantonese vowels are written without length marks and with plain letters.
The result is phonemic, so `phonemic` is on and the respelling stands between slashes.
The open-mid front vowel becomes e and the close-mid central rounded vowel becomes œ.
The open-mid back vowel becomes o and the near-close front vowel becomes i.
ɐ stays as written.
The sources anchor on the Standard Cantonese row alone, so no other variety reaches these rules.

## `pronunciation[0]` Wiktionary

The Chinese pronunciation table of an entry carries one row per variety.
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
There the key of the Cantonese row links to the 粵語音系 article and the label reads 幫助.
Its coverage of colloquial Cantonese headwords is thinner than the English one, so it stands second.
A simplified-form page carries no reading of its own.
It only points at its traditional form through a 請見 box, so the attempt follows that pointer.

## `audio[0]` Wiktionary

Wiktionary embeds Commons recordings as protocol-relative addresses, so `prefix` supplies the scheme.
Only the transcoded mp3 rendition is taken, because the original is WAV or Opus.
A Cantonese recording is either a Lingua Libre `LL-Q9186_%28yue%29-` file or a `Zh-yue-` file.
Both prefixes keep the Mandarin `Zh-` and Min Nan `Zh-nan-` recordings of the same page out.
One attempt reads both, so the page is fetched once.
A simplified-form page only points at its traditional form, so the attempt follows the see link.

## `audio[1]` Wikimedia Commons

Most Cantonese recordings on Commons are never embedded on a Wiktionary page.
So the file store is searched directly.
The search asks for files whose title carries the headword and the yue tag.
The API answers with the transcoded renditions of each file found.
Search matches by character, so a title holding the headword inside a longer word is returned too.
The pattern therefore takes only a file whose title ends in exactly `{word}`.
A Lingua Libre `LL-Q9186 (yue)-` file or a `Zh-yue-` file does so.
Only the mp3 rendition is taken, because the original is WAV or Opus.
`utf8=1` keeps the title readable so the headword can be matched.
`formatversion=2` keeps the addresses unescaped.

## `frequency[0]` CantoDict

CantoDict serves a word page at `/dictionary/words/` and a character page at `/dictionary/characters/`.
Both are reached straight from the hanzi, each carrying an editor-assigned difficulty Level from 1 upward.
A single character has no word page, so the first attempt yields nothing there.
The second attempt reads the character page instead.
An unknown spelling returns a bare detail view without a Level, so the attempt yields nothing.

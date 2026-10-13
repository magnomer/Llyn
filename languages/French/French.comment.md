# French

The French language pack and its display vocabulary, both declared as data.
Every URL and extraction rule is data, so an edit here needs no recompile.

## `source.json`

The `pronunciation` and `audio` lists are independent and each is shown in the order written.
No varieties are declared, so every reading stays untagged and stands for the French of France.
The pronunciation sources take the headword's own transcription, never a regional or historical variant.
The audio sources take only recordings tagged France or Paris, or carrying no region at all.
Naver's own recordings are untagged.
No transcription schemes are listed, because French keeps no romanization beside its IPA.
Section heading anchors allow attributes before `id`, which the REST rendering writes there.

## `respelling[0]` Parisian

The transcription is recast as a Parisian speaker says it.
The nasal œ̃ merges into ɛ̃, and the back ɑ merges into the front a.
The nasal ɑ̃ stays as written, so the ɑ row skips an ɑ carried by a combining tilde.
The length mark is dropped.

## `pronunciation[0]` Wiktionnaire

The headword line of every entry carries a `{{pron|…|fr}}` template.
So the first such template after the `{{langue|fr}}` heading is the headword's own transcription.
The `|fr}}` tail keeps the templates of other languages out.
A template with an empty transcription is skipped by the pattern.
A page without a French section yields nothing.
`normalize` is on so the syllable dots the site writes are stripped.

## `pronunciation[1]` Wiktionary

Most French entries derive their transcription with `{{fr-IPA}}`, which the wikitext does not carry.
So the rendered HTML is read, and the first IPA span after the French heading is the headword's own.
The regional and historical variants that follow it are left alone.
The REST rendering is asked first and the article page second, so a 429 from the REST host still answers.

## `audio[0]` Naver

Each entry carries an opaque, token-bearing `symbolFile` audio URL, empty on an entry without a recording.
The pattern takes the first non-empty one whose entry's `handleEntry` is the headword itself.
A neighbouring headword listed first is so passed over.
The API returns an empty body without a Referer header, so one is declared here.

## `audio[1]` Wiktionary

Wiktionary embeds Commons recordings as protocol-relative addresses, so `prefix` supplies the scheme.
Only the transcoded mp3 rendition is taken, because the original is Ogg Vorbis or WAV.
Each audio template is described in a data-mw attribute before its player, and the pattern anchors there.
The file must be a `Fr-` recording or a Lingua Libre `LL-Q150 (fra)-` one.
Its name must end in exactly the headword, so a recording of an inflected form is left out.
Its accent parameter must be absent or read France or Paris, so Canadian, Swiss and Belgian recordings are left out.
Every recording that passes is taken.

## `audio[2]` Le Robert

The pattern anchors on the headword in the definition heading, then on the player beside it.
A page that answered a search with a neighbouring word so yields nothing.
The player's source is site-relative, so `prefix` makes it absolute.
An accented address is redirected to its unaccented spelling, which the reader follows.

## `frequency[0]` Leipzig

The source is the Leipzig French news corpus of 2011 with one million sentences.
The answer is the word's frequency class.
Class 0 is the most frequent word, and each halving of frequency adds one.
The once rule doubles per class from a factor of 22, the interval of the most frequent French word.
The ladder rungs are fixed in the engine at one word in 10,000, 100,000 and 1,000,000.
With factor 22, class 8 is one word in 5,632, so classes up to 8 are core.
Class 12 is one in 90,112, so classes 9 to 12 are everyday.
Class 15 is one in 720,896, so classes 13 to 15 are advanced and 16 and up rare.
An unknown spelling answers 404, so the attempt yields nothing.

## `morphology[0]` Wiktionary

Each inflected form sits in an element whose class reads `Latn form-of lang-fr <kind>-form-of`, wrapping an `<a>` with the form.
The headword line comes before the conjugation tables, so the first match of a kind is the headline form.
The kinds are `p` for a noun plural, `f|s`, `m|p` and `f|p` for adjectives, and `ppr` and `pp` for participles.
The page carries every part of speech the spelling has.
So a reading answers the first of its kind and the paradigm decides which readings apply.
Live, cheval gave chevaux, and beau gave belle, beaux and belles.
Manger gave mangeant and mangé.

## `vocabulary.json`

The parts of speech and morphology an entry may carry are presets in dropdowns.
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

## `paradigms`

Each paradigm lists the forms a part is expected to inflect into, as morphology value ids in display order.
`except` lists part ids on the parent chain the paradigm must not reach.
The optional `regular` is a list of `[pattern, replacement]` pairs.
Each pair whose pattern matches the headword predicts one regular spelling by replacing the first match.
A stored form equal to any prediction is regular and needs no note.
A form no pair predicts, such as yeux from œil, is irregular and carries a note.
The noun paradigm predicts the plural.
A headword ending in s, x or z stays.
One ending in eau, au or eu takes x, one ending in al takes aux, and any other takes s.
The adjective paradigm predicts the feminine singular, the masculine plural and the feminine plural.
The feminine keeps a final e.
It turns f into ve, x into se, er into ère, et into ète or ette, and eau into elle.
It turns teur into trice, eur into euse, c into che or que, and g into gue.
It doubles the consonant of a final el, en, il or on, and otherwise adds e.
The masculine plural follows the noun rules, and the feminine plural is the feminine with s.
The verb paradigm predicts the present and past participles from the infinitive ending.
The ending er gives ant and é, with ger giving geant and cer giving çant.
The ending ir gives issant and i, and re gives ant and u.

## Stamp

Commit: `c2f335c331fe`
- `source.json` `a9ee16d26ba37961`
- `vocabulary.json` `a2f95682ec34aa54`

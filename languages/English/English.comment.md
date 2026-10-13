# English

The English language pack and its display vocabulary, both declared as data.
Every URL and extraction rule is data, so an edit here needs no recompile.
A language is added by creating `languages/<Name>/source.json`.
This file also documents the pack format every other pack shares.

## `pack format`

The `pronunciation` and `audio` lists are independent, and either may stand empty.
A site listed for transcriptions need not be listed for recordings.
Each list is shown in the order it is written, whatever order the answers arrive in.
Reordering a list therefore reorders the menu.
The `font`, `example` and `gloss` blocks give the typography of headwords, example sentences and their translations.
A `style` may be `italic` or `oblique`.

## Strategies

The `regex` strategy runs a capture group on the body.
The `json` strategy follows a dot `path`, then an optional `match`.
The `span` strategy is a tolerant nested-span IPA reader.
`confirm` is a regex with `{word}` the body must match.
`{word}` inside a `match` is the headword itself, escaped to match literally.
A search answer can so be narrowed to one file.
`normalize` strips IPA delimiters and entities.
`prefix` makes a captured site-relative address absolute.
The `link` strategy reads nothing and answers the address the attempt fetched, once the fetch succeeded.
It serves a speech service whose address is itself the recording.
`{headword}` inside a URL is the headword as typed, before any `spelling` rule ran.

## `urls`

An attempt's `urls` are asked in order, and only the first page that answers is read.
A pattern that misses on that page never falls through to the next URL.
So every pattern must fit each rendering its attempt lists.
Wiktionary's REST rendering writes other attributes before `id` on a section heading.
So a section anchor reads `<h2[^>]* id="Name"` and never `<h2 id="Name"`.

## Follow

It is an optional reading-shaped object on an attempt, with strategy, match, group and path.
When the attempt's readings all come up empty, its captured value is taken as another headword.
The attempt is then fetched again for that headword, at most twice.
The later attempts of that source ask for the headword it landed on.
It serves pages that only point at a main form, such as a simplified Chinese page.

## `varieties`

It declares the regional varieties the language distinguishes.
Each `list` row carries a `name` and an ISO 3166-1 alpha-2 `flag`.
`shown` is `flag` or `text`, default `text`, for how the UI labels each reading.
A pronunciation attempt may replace its flat extraction keys with a `readings` list.
Each row carries the same keys and adds `variety`, a name from the list, or omitted for untagged.
It adds `skip`, how many earlier matches to pass over, default 0.
It adds `every`, taking every match from there on instead of the first alone, default off.
The later matches on a dictionary page belong to inflections and neighbouring headwords, hence the default.
All readings run on the same fetched body, and a flat attempt without `readings` is one untagged reading.
An audio attempt takes the same `readings` list, each row capturing one address tagged with its `variety`.
When the row opening the audio menu carries no variety, an untagged recording shows once per declared variety.
An untagged transcription shows the same way.
When that row carries a variety, the menu narrows to that variety alone.
An untagged recording then collapses to one row under it.

## `respelling`

It declares ordered regex rewrite groups recasting a transcription into the pack's preferred symbols.
They run on every reading a lookup fills or the user types, and the result is stored beside the original.
The respelling switch then picks which of the two forms every surface shows and edits.
It lives in the Settings panel and is off by default.
Each group carries a `name`, a `varieties` list naming which tagged readings it serves, and `rules`.
The rules are `[pattern, replacement]` pairs run in written order, each on the previous rule's output.
`cleanup` has the same shape but always runs, for source quirks the built-in removal cannot know.
In a pack that declares varieties a group without a non-empty `varieties` list is skipped.
An untagged reading from a source is shown once per declared variety, under each flag.
In a pack without varieties `varieties` may be omitted and untagged readings stay untagged.
`phonemic` set to `true` says the groups collapse allophones, and a shown respelling then stands between slashes.

## Spelling

It declares ordered `[pattern, replacement]` regex pairs recasting the headword into the spelling sources key on.
They run once before the headword fills `{word}` in any URL or match pattern of any lookup kind.
Dropping the macrons a Latin headword is typed with is one use.
A pack without it sends the headword as typed.

## Transcription

It declares the spelling schemes the language keeps beside its IPA, such as Pinyin or Jyutping.
They stand in the order the form shows them.
A pack without it shows no transcription rows.
Each entry is a bare scheme name or an object with a `name` and a `sources` list.
The `sources` list has the shape of `pronunciation` and runs when the user looks a transcription row up.
A scheme without sources is typed by hand.
A transcription lookup keeps what the source wrote, spaces included, and runs no cleanup or respelling.

## `frequency`

It has the shape of `pronunciation`.
Every source is asked in written order, and each answer is stored as its own row.
`normalize` stays off there because the answer is a figure or a code rather than IPA.
A source carries `once`, the figures that turn its raw answer into a word interval for the tooltip.
The interval grades the answer on the ladder every language shares, one rung per decade of words.
Core is one occurrence within 10,000 words, Everyday within 100,000, Advanced within 1,000,000, Rare beyond.
`total` divides by the raw count.
`factor` with `base` scales the base raised to the raw class.
`factor` alone scales the raw rank.
A source without `once` prints its raw answer in the tooltip instead.
A source whose answer is a code rather than a figure carries `bands` instead.
A band row carries a `name` and a `match` regex.
The name must be one of the four ladder names.
A pack declares bands only when no interval can be read, never to grade a figure on its own scale.
Rows are tried in written order and the first that fits wins.
The raw answer is shown when nothing labels it.

## `morphology`

It has the shape of `frequency`, one reading per inflected form.
Its `variety` is the morphology value code from `vocabulary.json` written as a decimal string.
`normalize` stays off so the form keeps its spelling.
Irregularity is a fact looked up here, not a rule.

## `respelling[0]` British

Digraphs come before their parts so a longer match wins.
So ɛ becomes e before eə becomes eː.
The bare weak i and u that Cambridge and Oxford write without a length mark come last.
The bare i and u rows skip what the long-vowel rows already turned into ij and uw.

## `respelling[1]` American

The same glide and open-mid rows as the British group come first.
Then every spelling of the rhotic vowel becomes ər.
Cambridge writes it ɝː and ɚ, Longman and Oxford ɜːr, Dictionary.com ɜr.
The bare i and u most American sources write for FLEECE and GOOSE come last.
They skip what earlier rows turned into ij and uw.

## `pronunciation[0]` Longman

The class is upper-case PRON and the reader is case-sensitive.
An unknown word returns a results page with no PRON span, so the attempt yields nothing.

## `pronunciation[1]` Wiktionary

The wikitext attempt reads the `{{IPA|en|...}}` templates and keys each reading on the accent qualifier.
The qualifier `RP`, `UK` or `SSB` is British, and `US`, `GA` or `GenAm` is American.
The qualifier may instead sit on an `{{enPR}}` template earlier on the same line, as in break.
So each pattern also accepts an accented enPR followed by an IPA template on that line.
The first matching line is taken.
The HTML attempt is the untagged fallback.

## `pronunciation[2]` Cambridge

The British transcription sits inside the `uk dpron-i` block and the American one inside `us dpron-i`.
Each reading's match runs from its block's opening tag to the first ipa span, never crossing blocks.

## `pronunciation[3]` Oxford

Each reading anchors on its phons_br or phons_n_am block and takes the first span under it, the headword's own.
The inflections listed after it are left alone.
The page 404s on an unknown word, so no confirm guard is needed.

## `pronunciation[4]` Dictionary.com

It prints one IPA span per variety, a few hundred characters after a heading that says American or British.
A comma inside one span separates variants of that same variety.
Each reading anchors on its heading, allows no other span in between, and takes only the first variant.
A headword without a British section therefore yields American only.

## `audio[0]` Naver

Each phonetic symbol carries an opaque, token-bearing `symbolFile` audio URL.
The first American and British recordings in the body belong to the first entry.
The API returns an empty body without a Referer header, so one is declared here.

## `audio[1]` Longman

Both recordings are absolute data-src-mp3 addresses, British under `breProns` and American under `ameProns`.
Longman appends a cache-busting query, which the workspace ignores when it names the saved file.

## `audio[2]` Cambridge

The mp3 sources are site-relative, so `prefix` makes them absolute.
The first source of each variety is the headword pronunciation.

## `audio[3]` Oxford

Oxford exposes absolute mp3 addresses in data-src-mp3, British under `uk_pron` and American under `us_pron`.

## `audio[4]` Wiktionary

Wiktionary embeds Commons recordings as protocol-relative addresses, so `prefix` supplies the scheme.
Only the transcoded mp3 rendition is taken, because the original is Ogg Vorbis or WAV.
The first attempt takes an `En-` filename, the second a Lingua Libre `LL-Q1860_%28eng%29-` one.

## `audio[5]` Dictionary.com

The page marks no variety on its mp3 addresses, so the attempt takes the first one untagged.
An untagged recording shows once per declared variety, unless the menu opens on a tagged row.

## `frequency[0]` Datamuse

The `f:` tag is the word's frequency in occurrences per million words of text.
The once total is one million, so the tooltip reads the interval straight off the figure.
The ladder rungs are fixed in the engine at one word in 10,000, 100,000 and 1,000,000.
So 100 per million and over is core, 10 and over everyday, 1 and over advanced.
A spelling the API does not know returns an empty list.
So the confirm guard fails and the attempt yields nothing.

## `frequency[1]` Longman

Longman marks its top words with a FREQ span holding S1, S2, S3, W1, W2 or W3.
The code is no figure, so pattern bands label the top thousand core and the next two thousand everyday.
The spoken code comes first when a word carries both, and the class is upper-case FREQ.
A word outside those lists, or an unknown word, has no such span, so the attempt yields nothing.

## `morphology[0]` Wiktionary

Each inflected form sits in an element whose class reads `Latn form-of lang-en <kind>-form-of`, wrapping an `<a>` with the form.
The headword line comes before the conjugation tables, so the first match of a kind is the headline form.
The kinds include `spast` for a simple past and `past|part` for a past participle.
The kind `ed-form` is a regular past doubling as participle, and `p` is a plural.
The page carries every part of speech and every etymology the spelling has.
So a reading answers the first of its kind and the paradigm decides which readings apply.
A spelling whose first verb is not the common one, such as lie, may answer that verb's forms.
Live, go gave went, gone and goes, the last being the noun's plural.
Child gave childed, childed and children.
Mouse gave moused, moused and mice.

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

## Stamp

Commit: `c2f335c331fe`
- `source.json` `864be66a326318a4`
- `vocabulary.json` `5d7f24c89eb30a87`

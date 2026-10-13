# Classical Greek

The Classical Greek language pack and its display vocabulary, both declared as data.
Every URL and extraction rule is data, so an edit here needs no recompile.

## `source.json`

Headwords are polytonic.
Wiktionary folds the oxia forms into their tonos equivalents, so either spelling reaches the same page.
No transcription scheme is declared, because a Greek headword is read from its own letters.
No respelling is declared, because the Attic reading already writes pitch accent with an acute and length with a colon.
No morphology sources are listed.
The example typography names Palatino Linotype first.
Georgia carries no Greek Extended block and would drop the breathings and circumflexes.
The varieties are the reconstructed periods Wiktionary prints for every Ancient Greek entry, three of five kept.
They are Attic of the 5th century BCE, Koine of the 1st century CE and Byzantine of the 10th.
The 4th century CE Koine and 15th century CE Constantinopolitan rows can be added with the same anchor shape.
They are labelled by name, since no flag stands for a period.

## `pronunciation[0]` Wiktionary

The Ancient Greek pronunciation list carries one line per period.
Each line is opened by an accent label ending in the period's name and closed by the IPA span.
Each reading anchors on the end of its label and takes the first IPA span after it.
The Modern Greek section carries no such labels, so it stays out.
The span strategy is used because the transcription may nest a note span.
A collapsed summary line above the list repeats the readings without labels and is skipped for the same reason.
The article page is asked before the mobile page, because the REST host rejects bursts with 429.

## `audio[0]` Wikimedia Commons

Recordings are Lingua Libre `LL-Q35497 (grc)-` files on Commons, and Wiktionary never embeds them.
So the file store is searched directly.
Search matches by word, so the pattern takes only a file whose title ends in exactly `{word}`.
Only the mp3 rendition is taken, because the original is WAV.
`utf8=1` keeps the title readable so the headword can be matched.
`formatversion=2` keeps the addresses unescaped.
The recording carries no period, so it is shown once under every variety.

## `frequency[0]` Logeion

Logeion's info API answers with the headword's rank in the Perseus and PhiloLogic Greek corpus as a sentence.
The sentence gives the rank, or unranked when the word appears fewer than 50 times.
The pattern keeps the figure or the word unranked, so a pattern band can still label the latter.
The once factor of 12 is a Zipf estimate, since a rank alone gives no count.
The ladder rungs are fixed in the engine at one word in 10,000, 100,000 and 1,000,000.
At twelve words per rank, rank 833 and under is core, 8,333 and under everyday, 83,333 and under advanced.
A spelling Logeion does not know is unranked too, so a typo is shown as rare rather than as nothing.
Unranked is the one band the pack still declares, since no interval can grade a word.

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
The noun paradigm asks for the genitive, which fixes the declension and stem, and a proper noun is excepted.
The verb paradigm asks for the five further principal parts after the present, in dictionary order.
They are the future, aorist, perfect active, perfect middle and aorist passive.
A slot the verb lacks is left empty.
The adjective paradigm asks for the feminine and neuter forms, which a two-termination adjective repeats.
No `regular` predictions are declared.
A Greek form is a fact of its stem and its accent rather than of its spelling.

## Stamp

Commit: `c2f335c331fe`
- `source.json` `c7e28292475825db`
- `vocabulary.json` `0cdc8ee7b52fffdf`

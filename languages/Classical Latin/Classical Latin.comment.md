# Classical Latin

The Classical Latin language pack and its display vocabulary, both declared as data.
Every URL and extraction rule is data, so an edit here needs no recompile.

## `source.json`

No transcription scheme is declared, because a Latin headword is read from its own letters.
No respelling is declared, because the Classical reading is already written in plain IPA.
No morphology sources are listed.
The varieties are the two readings Wiktionary prints for every Latin entry.
One is the reconstructed Classical reading and the other the modern Italianate Ecclesiastical one.
They are labelled by name, since no flag stands for a period.

## `spelling`

Headwords are typed with their macrons, as a learner writes them.
Every source keys its entries on the bare spelling.
The ordered regex rewrite pairs recast the headword into that spelling.
They run before the headword fills `{word}` in any URL or match pattern, and every source list shares them.
Here they drop the macron and the breve from each vowel, since the stored headword keeps them.

## `pronunciation[0]` Wiktionary

The Latin pronunciation list carries one line per reading.
Each line is opened by an accent label linking to the Classical Latin article or ending in Ecclesiastical.
Each reading anchors on its label and takes the first IPA span after it.
The other languages sharing the spelling carry no such labels, so they stay out.
A page listing several Latin forms, such as rosa and rosā, prints the headword's own list first.
So the first match is the headword's.
The span strategy is used because the transcription may nest a note span.
The article page is asked before the mobile page, because the REST host rejects bursts with 429.

## `audio[0]` Wikimedia Commons

Latin recordings on Commons come in three families, and Wiktionary never embeds them.
`La-cls-` files are in the Classical reading and `La-ecc-` files in the Ecclesiastical reading.
Lingua Libre `LL-Q397 (lat)-` files are in whichever reading the speaker chose.
The search only joins terms with and, so each family is its own attempt.
The source keeps asking until both varieties are filled.
Search matches by word, so each pattern takes only a file whose title ends in exactly `{word}`.
The part-of-speech suffix some `La-cls-` files carry, such as corium-noun, is allowed.
Only the mp3 rendition is taken, because the original is Ogg Vorbis or WAV.
`utf8=1` keeps the title readable so the headword can be matched.
`formatversion=2` keeps the addresses unescaped.
The Lingua Libre recording carries no reading, so it is shown once under every variety.

## `frequency[0]` Logeion

Logeion's info API answers with the headword's rank in the PhiloLogic Latin corpus as a sentence.
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
The noun paradigm asks for the genitive singular, which fixes the declension and stem, and a proper noun is excepted.
The verb paradigm asks for the three further principal parts, infinitive, perfect and supine, in dictionary order.
A deponent verb writes its perfect participle in the supine slot.
The adjective paradigm asks for the feminine and neuter nominative.
A third-declension adjective of one termination repeats it.
No `regular` predictions are declared.
A Latin form is a fact of its stem rather than of its spelling.

## Stamp

Commit: `c2f335c331fe`
- `source.json` `92c4f46f73bc0543`
- `vocabulary.json` `59154178fa3f9d0d`

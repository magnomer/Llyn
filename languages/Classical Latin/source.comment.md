# source.json

## pack

Language pack for Classical Latin.
Every URL and extraction rule is data, so an edit here needs no recompile.
No transcription scheme is declared, because a Latin headword is read from its own letters.
Headwords are typed with their macrons, as a learner writes them.
Every source keys its entries on the bare spelling.
`spelling` declares ordered regex rewrite pairs recasting the headword into the spelling the sources key on.
They run before the headword fills `{word}` in any URL or match pattern.
Every source list of the pack shares them.
Here they drop the macron and the breve from each vowel, since the stored headword keeps them.
The varieties are the two readings Wiktionary prints for every Latin entry.
One is the reconstructed Classical reading and the other the modern Italianate Ecclesiastical one.
They are labelled by name, since no flag stands for a period.
No respelling is declared, because the Classical reading is already written in plain IPA.
No morphology sources are listed, because no `vocabulary.json` declares the paradigms yet.

## `pronunciation[0]` Wiktionary

The Latin pronunciation list of an entry carries one line per reading.
Each line is opened by an accent label linking to the Classical Latin article or ending in Ecclesiastical.
Each line is closed by the IPA span.
Each reading anchors on its label and takes the first IPA span after it.
The other languages sharing the spelling carry no such labels, so they stay out.
A page listing several Latin forms, such as rosa and rosā, prints the headword's own list first.
So the first match is the headword's.
The span strategy is used because the transcription may nest a note span.
The article page is asked before the mobile page, because the REST host rejects bursts with 429.

## `audio[0]` Wikimedia Commons

Latin recordings on Commons come in three families.
`La-cls-` files are in the Classical reading and `La-ecc-` files in the Ecclesiastical reading.
Lingua Libre `LL-Q397 (lat)-` files are in whichever reading the speaker chose.
Wiktionary never embeds them, so the file store is searched directly.
The search only joins terms with and, so each family is its own attempt.
The source keeps asking until both varieties are filled.
Each search asks for files whose title carries the headword and the family tag.
The API answers with the transcoded renditions of each file found.
Search matches by word, so each pattern takes only a file whose title ends in exactly `{word}`.
The part-of-speech suffix some `La-cls-` files carry, such as corium-noun, is allowed.
Only the mp3 rendition is taken, because the original is Ogg Vorbis or WAV.
`utf8=1` keeps the title readable so the headword can be matched.
`formatversion=2` keeps the addresses unescaped.
The Lingua Libre recording carries no reading, so it is shown once under every variety.

## `frequency[0]` Logeion

Logeion's info API is the same call its sidebar makes.
It answers with the headword's rank in the Latin corpus of the PhiloLogic texts as a sentence.
The sentence reads the 304th most frequent word, or unranked when it appears fewer than 50 times.
The pattern keeps the figure or the word unranked, so a pattern band can still label the latter.
The once factor of 12 is a Zipf estimate, since a rank alone gives no count.
The ladder rungs are fixed in the engine at one word in 10,000, 100,000 and 1,000,000.
At twelve words per rank, rank 833 and under is core, 8,333 and under everyday, 83,333 and under advanced.
A spelling Logeion does not know is unranked too, so a typo is shown as rare rather than as nothing.
Unranked is the one band the pack still declares, since no interval can grade a word.

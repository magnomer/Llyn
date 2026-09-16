# source.json

## pack

Language pack for Classical Greek.
Every URL and extraction rule is data, so an edit here needs no recompile.
Headwords are polytonic.
Wiktionary folds the oxia forms into their tonos equivalents, so either spelling reaches the same page.
No transcription scheme is declared, because a Greek headword is read from its own letters.
The example typography names Palatino Linotype first.
Georgia carries no Greek Extended block and would drop the breathings and circumflexes.
The varieties are the reconstructed periods Wiktionary prints for every Ancient Greek entry, three of five kept.
Attic is the 5th century BCE reading.
Koine is the 1st century CE Egyptian reading of the New Testament era.
Byzantine is the 10th century CE reading.
The 4th century CE Koine and 15th century CE Constantinopolitan rows can be added with the same anchor shape.
They are labelled by name, since no flag stands for a period.
No respelling is declared.
The Attic reading already writes the pitch accent with an acute and vowel length with a colon.
No morphology sources are listed, because no `vocabulary.json` declares the paradigms yet.

## `pronunciation[0]` Wiktionary

The Ancient Greek pronunciation list of an entry carries one line per period.
Each line is opened by an accent label ending in the period's name, such as 5th BCE Attic.
Each line is closed by the IPA span.
Each reading anchors on the end of its label and takes the first IPA span after it.
The Modern Greek section of the same page carries no such labels, so it stays out.
The span strategy is used because the transcription may nest a note span.
A collapsed summary line above the list repeats the readings without labels and is skipped for the same reason.
The article page is asked before the mobile page, because the REST host rejects bursts with 429.

## `audio[0]` Wikimedia Commons

Ancient Greek recordings on Commons are Lingua Libre `LL-Q35497 (grc)-` files, and Wiktionary never embeds them.
So the file store is searched directly.
The search asks for files whose title carries the headword and the Lingua Libre tag.
The API answers with the transcoded renditions of each file found.
Search matches by word, so the pattern takes only a file whose title ends in exactly `{word}`.
Only the mp3 rendition is taken, because the original is WAV.
`utf8=1` keeps the title readable so the headword can be matched.
`formatversion=2` keeps the addresses unescaped.
The recording carries no period, so it is shown once under every variety.

## `frequency[0]` Logeion

Logeion's info API is the same call its sidebar makes.
It answers with the headword's rank in the Greek corpus of the Perseus and PhiloLogic texts as a sentence.
The sentence reads the 57th most frequent word, or unranked when it appears fewer than 50 times.
The pattern keeps the figure or the word unranked, so a pattern band can still label the latter.
The once factor of 12 is a Zipf estimate, since a rank alone gives no count.
The ladder rungs are fixed in the engine at one word in 10,000, 100,000 and 1,000,000.
At twelve words per rank, rank 833 and under is core, 8,333 and under everyday, 83,333 and under advanced.
A spelling Logeion does not know is unranked too, so a typo is shown as rare rather than as nothing.
Unranked is the one band the pack still declares, since no interval can grade a word.

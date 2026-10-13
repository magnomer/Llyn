# Portuguese

The Portuguese language pack and its display vocabulary, both declared as data.
The pack format is documented in `languages/English/English.comment.md`.

## `source.json`

Section heading anchors allow attributes before `id`, which the REST rendering writes there.
Two varieties are declared, Brazilian under the Brazilian flag and European under the Portuguese one.
Every Wiktionary page is asked as the REST rendering before the article page.
A Wiktionary page such as casa carries twenty other languages, so every pattern stays inside the Portuguese section.
The section runs from its heading to the next `<h2 `, and no pattern may cross that boundary.

## `cleanup[0]` Optional

Wiktionary writes an optional final sound in parentheses, as /faˈla(ʁ)/ for the Brazilian falar.
The built-in removal trims delimiters at both ends and so eats the closing parenthesis.
The rule puts it back when an opening parenthesis stands unmatched at the end.

## `pronunciation[0]` Wiktionary

The pt-IPA template derives the IPA from the spelling, so the wikitext carries none and the rendered HTML is read.
Each variety's line opens with a qualifier linking to its Wikipedia article, Brazil or Portugal.
The reading anchors on that link and its closing span and takes the first IPA span after it.
The first span is the phonemic reading, and a phonetic one in brackets may follow it, which is left alone.
An audio label names the variety too, but no span closes its link, so it anchors nothing.
A collapsed switcher may print the Portugal line twice, and the first copy is taken.
Regional readings such as São Paulo or Northern Portugal follow the national ones and are left alone.

## `pronunciation[1]` Wikcionário

The Portuguese Wiktionary is read as wikitext.
The Portuguese section runs from `={{-pt-}}=` to the next language heading.
A pronúncia section may list Brasil and Portugal subsections, each with a city subsection under it.
An AFI line may be a bare link or an `{{AFI}}` template, so the pattern accepts both.
The Brazilian reading takes the first AFI line under a Brasil heading, the European one under Portugal.
A page with one AFI line straight under the pronúncia heading answers it untagged.
Some pages write the stress as an apostrophe, which is left as written.
The host answers bursts with 429, so a lookup right after another may yield nothing.

## `audio[0]` Naver

Naver's text-to-speech answers every headword, since a synthesizer has no missing entries.
The address is the recording, so the `link` strategy answers it once the fetch succeeded.
The speaker is `pt_pt` and the service `ptdic`, and any other speaker code answers 400.
The voice is European, so the reading is tagged so and the Brazilian flag never lists it.

## `audio[1]` Wiktionary

Wiktionary embeds Commons recordings as protocol-relative addresses, so `prefix` supplies the scheme.
Only the transcoded mp3 rendition is taken, because the original is Ogg or WAV.
Each audio row opens with a label cell naming the accent, such as Brazil, Central-West Brazil or Portugal (Porto).
The Brazilian reading takes the first row whose label says Brazil and the European one Portugal.
The untagged reading takes the first row whose label says neither, so an unlabeled recording shows under both flags.
Every reading stays within its own row, so a label never claims the file of the next row.
Most Portuguese recordings are Lingua Libre ones, `LL-Q5146_%28por%29-`, Q5146 being Portuguese.

## `frequency[0]` Leipzig

The source is the Leipzig Portuguese news corpus of 2013 with one million sentences.
The answer is the word's frequency class.
Class 0 is the most frequent word, and each halving of frequency adds one.
The corpus holds 19,054,351 tokens and its most frequent word de occurs 811,565 times.
So the once rule doubles per class from a factor of 23, the interval of that word.
The ladder rungs are fixed in the engine at one word in 10,000, 100,000 and 1,000,000.
With factor 23, class 8 is one word in 5,888, so classes up to 8 are core.
Class 12 is one in 94,208, so classes 9 to 12 are everyday.
Class 15 is one in 753,664, so classes 13 to 15 are advanced and 16 and up rare.
An unknown spelling answers 404, so the attempt yields nothing.

## `morphology`

The list stands empty until a source is pinned.

## `vocabulary.json`

The file format is documented in `languages/French/French.comment.md`.
Every id is an integer that never changes once published.
Feature codes 3 to 8 and value codes 5 to 21 are retired and must not be reissued.
They once listed adjective gender and number and verb mood, tense, person and number.
Those are inflection dimensions, not facts a dictionary entry states, so the features were replaced.

## `registers`

Literary covers the second-person plural and the mesoclitic future, which speech has dropped.
Slang covers gíria such as cara and fixe.

## `parts`

Determiner stands beside article, for meu and este before a noun.
No partitive article is listed, since Portuguese has none.

## `retired`

It lists the parts once published and since removed, so no id is ever reused.
The import applies every row on each start, and a row whose part is already gone does nothing.
`into` names the part that takes over the removed part's links.
`unit` names the lexical unit set on a linking entry that has none yet.
A row with neither drops the links.

## `features`

A noun takes gender and number, and number marks a plurale tantum such as óculos.
An adjective takes form, the four gender and number spellings.
A verb takes form and conjugation.
Form lists the infinitive, the gerund and the past participle.
Conjugation is first, second or third, the ar, er and ir verbs.
No auxiliary feature is listed, since every compound tense takes ter.

## `paradigms`

The list stands empty until the regular rules are written.

## Stamp

Commit: `c2f335c331fe`
- `source.json` `5743e89728f410e1`
- `vocabulary.json` `58acdd0eae49c1f5`

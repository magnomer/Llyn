# source.json
Hash: `e934d07b2e32cf6a`

## `language`

The pack identifies Spanish independently of the interface language.

## `flag`

The selector uses the Spain flag identifier.

## `font`

Headwords use Segoe UI at size 40.

## `example`

Examples prefer Georgia, falling back to Segoe UI, at size 15.

## `pronunciation[0]` Wiktionary

The IPA is derived from the spelling by the es-pr template, so the wikitext carries none.
The rendered HTML is therefore read.
The match anchors on the Spanish section heading, so a Galician or Asturian section before it is skipped.
The first IPA span after the heading is the phonemic reading between slashes.
The phonetic reading in brackets, the rhyme and the regional readings that follow are left alone.
A word that splits by region, such as ciudad, so gives the distinción reading first.
Pinned live, agua gave /ˈaɡwa/, casa /ˈkasa/, hablar /aˈblaɾ/, perro /ˈpero/ and ciudad /θjuˈdad/.

## `audio[0]` Naver

Naver's Spanish dictionary text-to-speech, one mp3 per request.
The address is the recording, so the `link` strategy answers it once the fetch succeeded.
The speaker is `es_es` and the service `esdic`.
Every headword answers, since a synthesizer has no missing entries.

## `audio[1]` Wiktionary

Wiktionary embeds Commons recordings as protocol-relative addresses, so `prefix` supplies the scheme.
Only the transcoded mp3 rendition is taken, because the original is Ogg, Oga or WAV.
The first attempt takes an `Es-` filename after the Spanish section heading.
The filename carries a region code, as Es-am-lat-agua.ogg or Es-bo-casa.oga.
The second takes a Lingua Libre `LL-Q1321_%28spa%29-` one, Q1321 being Spanish.
Pinned live, agua and casa gave an `Es-` recording and hablar, perro and ciudad a Lingua Libre one.

## `inflection`

The inflection key names the file beside this one holding the verb rule book, `inflection.json`.
The book predicts each regular form and marks the letters where a stored form departs from it.
It is kept apart because its rules and layout are long and change on their own.

## `morphology[0]`

One Wiktionary REST HTML attempt declares 64 cell-keyed readings.
Normalization is disabled so conjugated spelling survives extraction.
The attempt requires `lang-es` on the page, so a page without a Spanish section gives no reading.
The Spanish conjugation sits in one `roa-inflection-table`.
Each finite row opens with a `<th>` holding `<span title="T">`, where `T` is the Spanish row name.
Six `<td>` cells follow, one per person and number, first singular to third plural.
A cell's form is the text of its first `<a>`.
Indicative and subjunctive 2sg cells carry the tú form first and the vos form second.
So the first `<a>` keeps the tú form and drops vos.
The imperative rows come last and keep an empty first `<td>` for the missing first singular.
Every negative imperative cell reads `no` and the form as two `<a>` in one span.
Its reading skips the `no` link and captures the next one.
The two imperfect subjunctive rows share one title and differ by `<br/>(ra)` or `<br/>(se)` after it.
The future subjunctive header ends in a footnote `<sup>`, so a header may carry markup before `</th>`.
Rows after the present indicative first require its title, avoiding earlier tables with reused imperative titles.
These anchors do not explicitly enforce table boundaries.
Each reading takes column k by skipping k cells, and its `variety` is the cell key.
Patterns require exact row-title spellings, including `copréterito`.
They read `presente de indicativo` and `pretérito imperfecto (copréterito)`.
They read `pretérito perfecto simple (pretérito indefinido)` and `futuro simple (futuro imperfecto)`.
They read `condicional simple (pospretérito de modo indicativo)` and `presente de subjuntivo`.
They read `pretérito imperfecto de subjuntivo` and `futuro simple de subjuntivo (futuro de subjuntivo)`.
They read `imperativo afirmativo` and `imperativo negativo`.
Embedded recibir fixtures verify alternate tú forms, negative-particle skipping, and earlier Galician decoys.

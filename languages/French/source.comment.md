# source.json

## pack

Language pack for French.
Every URL and extraction rule is data, so an edit here needs no recompile.
The `pronunciation` and `audio` lists are independent and each is shown in the order written here.
No varieties are declared, so every reading stays untagged and stands for the French of France.
The pronunciation sources take the headword's own transcription, never a regional or historical variant.
The audio sources take only recordings tagged France or Paris, or carrying no region at all.
Naver's own recordings are untagged.
No transcription schemes are listed, because French keeps no romanization beside its IPA.

## `respelling[0]` Parisian

The transcription as a Parisian speaker says it.
The nasal œ̃ merges into ɛ̃.
The back ɑ merges into the front a, while the nasal ɑ̃ stays as written.
The length mark is dropped.
The ɑ row skips an ɑ carried by a combining tilde so the nasal vowel keeps its letter.

## `pronunciation[0]` Wiktionnaire

The French Wiktionary wikitext.
The headword line of every entry carries a `{{pron|…|fr}}` template.
So the first such template after the `{{langue|fr}}` heading is the headword's own transcription.
The `|fr}}` tail keeps the templates of other languages out.
A template with an empty transcription is skipped by the pattern.
A page without a French section yields nothing.
`normalize` is on so the syllable dots the site writes are stripped.

## `pronunciation[1]` Wiktionary

The English Wiktionary rendered page.
Most French entries derive their transcription with `{{fr-IPA}}`, which the wikitext does not carry.
So the rendered HTML is read.
The first IPA span after the French heading is the headword's own.
The regional and historical variants that follow it are left alone.
The REST rendering is asked first and the article page second, so a 429 from the REST host still answers.

## `audio[0]` Naver

Naver French-Korean dictionary search API.
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
Its accent parameter must be absent or read France or Paris.
A Canadian, Swiss or Belgian recording is so left out.
Every recording that passes is taken.

## `audio[2]` Le Robert

Dico en ligne Le Robert.
The definition heading reads Définition de followed by the headword.
A `d_sound_cont` span beside it holds the player.
The player's source is a site-relative mp3 under `/medias/SOUNDS/`, and `prefix` makes it absolute.
The pattern anchors on the headword in that heading.
A page that answered a search with a neighbouring word so yields nothing.
An accented address is redirected to its unaccented spelling, which the reader follows.

## `frequency[0]` Leipzig

Leipzig Corpora Collection, French news corpus of 2011 with one million sentences.
The answer carries the word's frequency class.
It is 0 for the most frequent word and one higher each time the frequency halves.
The once rule doubles per class from a factor of 22, the interval of the most frequent French word.
The ladder rungs are fixed in the engine at one word in 10,000, 100,000 and 1,000,000.
With factor 22, class 8 is one word in 5,632, so it and every lower class are core.
Class 12 is one in 90,112, so classes 9 to 12 are everyday.
Class 15 is one in 720,896, so classes 13 to 15 are advanced and 16 and up rare.
An unknown spelling answers 404, so the attempt yields nothing.

## `morphology[0]` Wiktionary

Wiktionary REST HTML.
Each inflected form sits in a `<b>` or `<span>` whose class reads `Latn form-of lang-fr <kind>-form-of`.
That element wraps an `<a>` with the form as its text.
The headword line comes before the conjugation tables, so the first match of a kind is the headline form.
`p` is a noun plural.
`f|s` is an adjective's feminine singular, `m|p` its masculine plural, `f|p` its feminine plural.
`ppr` is a present participle and `pp` a past participle.
The page carries every part of speech the spelling has.
So a reading answers the first of its kind on the page and the paradigm decides which readings apply.
Pinned live, cheval gave chevaux.
Beau gave belle, beaux and belles.
Heureux gave heureuse, heureux and heureuses.
Manger gave mangeant and mangé.
Finir gave finissant and fini.
Vendre gave vendant and vendu.

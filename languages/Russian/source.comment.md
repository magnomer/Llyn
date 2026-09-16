# source.json

## pack

Language pack for Russian.
The pack format is documented in `languages/English/source.comment.md`.
No varieties are declared, so every reading is the standard Moscow one.
No respelling groups are declared, so every reading shows as its source wrote it.
Segoe UI and Georgia both carry Cyrillic, so the typography blocks stay as the other packs write them.

## `spelling`

A headword may be typed with the combining acute U+0301 that marks its stress.
Wiktionary titles, Naver and the Leipzig corpus all key on the bare spelling.
So the rule strips the mark before the headword fills any URL or pattern.

## `transcription[0]` Stress

The headword with its stress marked, as Wiktionary prints it on the headword line.
Russian writing leaves stress unmarked, so the marked form is the learner's reading of the word.
Each headword line prints it in a `<strong class="Cyrl headword" lang="ru">`.
The match anchors on the Russian section heading, so an Old East Slavic section listed before it is skipped.
The `lang="ru"` attribute keeps a Ukrainian headword after it from answering.
The first headword after the heading belongs to the first part of speech.
Pinned live, собака gave соба́ка, идти идти́, человек челове́к and дом unmarked, being one syllable.

## `transcription[1]` Romanization

Wiktionary's own Russian transliteration, scientific with the stress carried over.
Each headword line prints it in a `<span class="headword-tr">`.
The match anchors on the Russian section heading for the same reason as the stress scheme.
The first span after the heading belongs to the first part of speech.
Pinned live, собака gave sobáka, идти idtí, хорошо xorošó and человек čelovék.

## `pronunciation[0]` Wiktionary

The English Wiktionary rendered page.
Every Russian entry derives its transcription with `{{ru-IPA}}`, which the wikitext does not carry.
So the rendered HTML is read.
The match anchors on the Russian section heading, so an Old East Slavic section listed before it is skipped.
The first IPA span after the heading is the headword's own, written between brackets.
A dated or dialectal reading that follows it is left alone.
The REST rendering is asked first and the article page second, so a 429 from the REST host still answers.
Pinned live, собака gave [sɐˈbakə], идти [ɪˈtʲːi], хорошо [xərɐˈʂo] and человек [t͡ɕɪɫɐˈvʲek].

## `pronunciation[1]` Wiktionary (ru)

The Russian Wiktionary rendered page.
Its `{{transcription-ru}}` template carries the stressed headword and derives the IPA, so the wikitext holds none.
The Russian section is a level-one heading whose id is the language name in Cyrillic.
The first IPA span after it is the headword's own, a noun's singular or a verb's infinitive.
The plural or the past tense that follows is left alone.
Pinned live, собака gave sɐˈbakə, идти ɪˈtʲːi, книга ˈknʲiɡə and Москва mɐˈskva.

## `audio[0]` Naver

Naver Russian-Korean dictionary search API.
Each entry carries an opaque, token-bearing `symbolFile` audio URL, empty on an entry without a recording.
The pattern takes the first non-empty one whose entry's `handleEntry` is the headword itself.
A phrase listed after it, such as заброшенная собака under собака, is so passed over.
The API returns an empty body without a Referer header, so one is declared here.

## `audio[1]` Wiktionary

Wiktionary embeds Commons recordings as protocol-relative addresses, so `prefix` supplies the scheme.
Only the transcoded mp3 rendition is taken, because the original is Ogg Vorbis or WAV.
The first attempt takes a `Ru-` filename after the Russian section heading.
A `Be-` or `Uk-` filename on the same page is a Belarusian or Ukrainian recording and does not match.
The second takes a Lingua Libre `LL-Q7737_%28rus%29-` one, Q7737 being Russian.
Pinned live, собака gave Ru-собака.ogg.mp3 and дом both a Ru- and a Lingua Libre recording.

## `frequency[0]` Leipzig

Leipzig Corpora Collection, Russian news corpus of 2013 with one million sentences.
The answer carries the word's frequency class.
It is 0 for the most frequent word and one higher each time the frequency halves.
The corpus holds 17,325,431 tokens and its most frequent word в occurs 685,850 times.
So the once rule doubles per class from a factor of 25, the interval of that word.
The ladder rungs are fixed in the engine at one word in 10,000, 100,000 and 1,000,000.
With factor 25, class 8 is one word in 6,400, so it and every lower class are core.
Class 11 is one in 51,200, so classes 9 to 11 are everyday.
Class 15 is one in 819,200, so classes 12 to 15 are advanced and 16 and up rare.
An unknown spelling answers 404, so the attempt yields nothing.
Pinned live, человек gave class 5, дом class 8, книга class 11 and собака class 12.

## `morphology`

The list stands empty until a source is pinned.

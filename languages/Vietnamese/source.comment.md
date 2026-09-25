# source.json

## pack

Language pack for Vietnamese.
The pack format is documented in `languages/English/source.comment.md`.
Three varieties are declared, Northern, Central and Southern, shown by name because all three share one flag.
Wiktionary labels them by their reference cities, Hà Nội, Huế and Saigon.
`tonal` is on, so the UI draws a contour under each reading from the tone letters it carries.
No transcription schemes are listed, because Vietnamese is written in Latin script.
No respelling groups are declared, so every reading shows as its source wrote it.
Every Wiktionary page is asked as the REST rendering before the article page.
A headword with a space, such as cảm ơn, redirects to its underscored page.
The reader follows that redirect.
The Vietnamese section runs from its heading to the next `<h2 `, and no pattern may cross that boundary.

## `pronunciation[0]` Wiktionary

The IPA is derived from the spelling by the vi-IPA template, so the wikitext carries none.
The rendered HTML is therefore read.
The template prints one line per variety, each opening with a qualifier linking to its city's Wikipedia article.
The reading anchors on that link, its closing spans and the IPA label after them.
It takes the first IPA span after that label.
An audio label links the same city, but a colon follows it rather than the IPA label.
So it anchors nothing.
The readings are phonetic ones between brackets, and `normalize` strips the brackets.
Pinned live, nhà gave [ɲaː˨˩], [ɲaː˦˩] and [ɲaː˨˩], sách [sajk̟̚˧˦], [ʂat̚˦˧˥] and [ʂat̚˦˥].
Người gave [ŋɨəj˨˩], [ŋɨj˦˩] and [ŋɨj˨˩], and cảm ơn [kaːm˧˩ ʔəːn˧˧], [kaːm˧˨ ʔəːŋ˧˧] and [kaːm˨˩˦ ʔəːŋ˧˧].

## `audio[0]` Naver

Naver Vietnamese-Korean dictionary search API, then Naver text-to-speech.
An entry with a recording carries an opaque, token-bearing `symbolFile` audio URL.
An entry without one carries no such key.
The pattern takes the first address whose entry's `handleEntry` is the headword itself.
It refuses to cross a `rank` key, so an address never claims the headword of the next entry.
A tone-mate listed after it, such as nhã under nha, is so passed over.
The API returns an empty body without a Referer header, so one is declared here.
Pinned live, nhà, người, sách, học, đẹp, tôi, yêu and cảm ơn each gave a recording and xin chào none.
The second attempt is the text-to-speech, one mp3 per request.
The address is the recording, so the `link` strategy answers it once the fetch succeeded.
The speaker is `vi_vn` and the service `vidic`, and any other speaker code answers 400.
Every headword answers there, since a synthesizer has no missing entries.
Neither recording names a region, so both show once under every variety.

## `audio[1]` Wiktionary

Wiktionary embeds Commons recordings as protocol-relative addresses, so `prefix` supplies the scheme.
Only the transcoded mp3 rendition is taken, because the original is WAV.
Each audio row opens with a label cell linking the speaker's city, Hà Nội or Saigon.
The Northern reading takes the first row whose label links Hanoi, the Central one Huế and the Southern one Saigon.
The untagged reading takes the first row whose label links no city, so an unlabeled recording shows under every variety.
Every reading stays within its own row, so a label never claims the file of the next row.
Nearly every Vietnamese recording is a Lingua Libre `LL-Q9199_%28vie%29-` one, Q9199 being Vietnamese.
Pinned live, nhà and học each gave a Hà Nội and a Saigon one.
Sách and cảm ơn gave a Saigon one.
Người, ăn, nước, tôi and yêu gave a Hà Nội one.
Việt Nam gave a Hà Nội and an unlabeled one, and đẹp none.
No headword pinned carried a Huế one.

## `frequency[0]` Leipzig

Leipzig Corpora Collection, Vietnamese news corpus of 2013 with one million sentences.
The answer carries the word's frequency class.
It is 0 for the most frequent word and one higher each time the frequency halves.
The corpus holds 18,037,085 tokens and its most frequent word và occurs 301,729 times.
So the once rule doubles per class from a factor of 60, the interval of that word.
The ladder rungs are fixed in the engine at one word in 10,000, 100,000 and 1,000,000.
With factor 60, class 7 is one word in 7,680, so it and every lower class are core.
Class 10 is one in 61,440, so classes 8 to 10 are everyday.
Class 14 is one in 983,040, so classes 11 to 14 are advanced and 15 and up rare.
A compound of several syllables is found when the corpus holds it as one token, as it holds cảm ơn.
An unknown spelling answers 404, so the attempt yields nothing.
Pinned live, người gave class 2, nước class 3, nhà class 4 and ăn class 5.
Sách gave class 7, cảm ơn class 8 and xin chào nothing.

## `morphology`

The list stands empty, because Vietnamese words do not inflect.
Tense and aspect are marked by separate particles such as đã and sẽ, which are entries of their own.

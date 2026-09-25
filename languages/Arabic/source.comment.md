# source.json

## pack

Language pack for Modern Standard Arabic.
The pack format is documented in `languages/English/source.comment.md`.
No varieties are declared, so every reading is the standard one.
Wiktionary also prints regional readings after the standard one, and a later pack may tag them.
Segoe UI carries the Arabic script on every Windows install, so no Naskh face is named.
Georgia has no Arabic glyphs, so the example block names Segoe UI alone.

## `spelling`

A headword is typed with its vowel marks so the card shows them.
Wiktionary titles carry no marks, so every mark is dropped before the headword fills `{word}`.
The first row drops the harakat, tanwin, shadda, sukun, dagger alif and the tatweel.
The second row turns alif wasla into plain alif.

## `transcription[0]` Romanization

Wiktionary's own Arabic transliteration, close to DIN 31635.
Each headword line prints it in a `<span class="headword-tr">`.
The match anchors on the Arabic section heading so a dialect section listed before it is skipped.
The first span after the heading belongs to the first part of speech.
An inflected-form page prints no transliteration, so the lookup yields nothing there.

## `pronunciation[0]` Wiktionary

The IPA is computed from the vowelled headword by the ar-pr template, so the wikitext carries none.
The first IPA span after the Arabic section heading is the standard reading between slashes.
Regional readings follow in brackets and are left alone.
Pinned live, كتاب gave /ki.taːb/, مدرسة /mad.ra.sa/, شمس /ʃams/ and يكتب /jak.tu.bu/.

## `audio[0]` Naver

Naver's Arabic dictionary text-to-speech, one mp3 per request.
The address is the recording, so the `link` strategy answers it once the fetch succeeded.
The speech reads the marks, so the URL carries `{headword}` as typed rather than the bare-spelled `{word}`.
Every headword answers, since a synthesizer has no missing entries.

## `audio[1]` Wiktionary

Wiktionary embeds Commons recordings as protocol-relative addresses, so `prefix` supplies the scheme.
Only the transcoded mp3 rendition is taken, because the original is Ogg or WAV.
The first attempt takes an `Ar-` filename after the Arabic section heading.
The second takes a Lingua Libre `LL-Q13955_%28ara%29-` one, Q13955 being Arabic.
Dialect recordings carry other language codes such as `ajp` and are skipped.

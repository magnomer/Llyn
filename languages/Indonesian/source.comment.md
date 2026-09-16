# source.json

## pack

Language pack for Indonesian.
The pack format is documented in `languages/English/source.comment.md`.
No varieties are declared, so every reading is the standard one.
No transcription schemes are listed, because Indonesian is written in Latin script.
No respelling groups are declared, so every reading shows as its source wrote it.
No frequency source is listed, because the Leipzig API exposes no Indonesian corpus.

## `pronunciation[0]` Wiktionary

The IPA is derived from the spelling by the id-IPA template, so the wikitext carries none.
The rendered HTML is therefore read.
The match anchors on the Indonesian section heading, so an Ilocano or Hausa section listed before it is skipped.
The first IPA span after the heading is the headword's own phonemic reading between slashes.
The phonetic reading in brackets and the rhyme that follow it are left alone.
Pinned live, rumah gave /ˈrumah/, pergi /pərˈɡi/, tidak /ˈtidaʔ/ and sekolah /səˈkolah/.

## `audio[0]` Naver

Naver Indonesian-Korean dictionary search API, then Naver text-to-speech.
Each entry carries an opaque, token-bearing `symbolFile` audio URL, empty on an entry without a recording.
An entry with a recording lists a second, empty `symbolFile` after the first.
So the pattern crosses empty ones and only refuses to cross another non-empty one.
It takes the first non-empty one whose entry's `handleEntry` is the headword itself.
A compound listed after it, such as rumah sakit under rumah, is so passed over.
The API returns an empty body without a Referer header, so one is declared here.
Pinned live, rumah, makan, pergi, buku and sedang each gave a recording.
The second attempt is the text-to-speech, one mp3 per request.
The address is the recording, so the `link` strategy answers it once the fetch succeeded.
The speaker is `id_id` and the service `iddic`, and any other speaker code answers 400.
Every headword answers there, since a synthesizer has no missing entries.

## `audio[1]` Wiktionary

Wiktionary embeds Commons recordings as protocol-relative addresses, so `prefix` supplies the scheme.
Only the transcoded mp3 rendition is taken, because the original is Ogg or WAV.
The first attempt takes an `Id-` filename after the Indonesian section heading.
The second takes a Lingua Libre `LL-Q9240_%28ind%29-` one, Q9240 being Indonesian.
Nearly every Indonesian recording on Wiktionary is a Lingua Libre one, so the second attempt answers far more often.
Pinned live, rumah, pergi, air, orang and saya each gave a Lingua Libre recording and makan none.

## `morphology`

The list stands empty until a source is pinned.

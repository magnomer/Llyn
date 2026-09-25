# source.json

## pack

Language pack for Thai.
The pack format is documented in `languages/English/source.comment.md`.
No varieties are declared, so every reading is the standard one.
No respelling groups are declared, so every reading shows as its source wrote it.
Segoe UI has no Thai glyphs, so the headword and example blocks name Leelawadee UI first.
Leelawadee UI ships with every Windows install since 8.1.
Georgia has no Thai either, so the example block does not name it.
The gloss block keeps Georgia, since the translations under an example are not Thai.
No Leipzig source is listed, because the Leipzig API exposes no Thai corpus.

## `transcription[0]` Paiboon

The Paiboon romanization, which marks tone and vowel length.
The th-pron table prints it in a `<span class="tr">` in the first cell after the Paiboon row header.
The match anchors on the Thai section heading, so a Northern Thai or Isan section listed before it is skipped.
The match also refuses to cross another section heading.
The first cell belongs to the headword's own reading, and bound forms in later cells are left alone.
Pinned live, น้ำ gave náam, บ้าน bâan, กิน gin, หนังสือ nǎng-sʉ̌ʉ and ประเทศ bprà-têet.

## `transcription[1]` RTGS

The Royal Thai General System, which marks neither tone nor length.
The same table prints it after the Royal Institute row header.
The match anchors and stops the same way as the Paiboon one.
Pinned live, น้ำ gave nam, บ้าน ban, กิน kin, หนังสือ nang-sue and ประเทศ pra-thet.

## `pronunciation[0]` Wiktionary

The IPA is derived from the Thai by the th-pron template, so the wikitext carries none.
The rendered HTML is therefore read.
The match anchors on the Thai section heading and refuses to cross another.
A Northern Thai or Nyaw section before it carries its own IPA span, which น้ำ showed.
The first IPA span after the heading is the headword's own phonemic reading between slashes.
The tone is written in Chao letters after each syllable.
A bound form or rhyme span that follows is left alone.
Pinned live, น้ำ gave /naːm˦˥/, บ้าน /baːn˥˩/, กิน /kin˧/, หนังสือ /naŋ˩˩˦.sɯː˩˩˦/ and ขอบคุณ /kʰɔːp̚˨˩.kʰun˧/.

## `audio[0]` Naver

Naver's Thai dictionary text-to-speech, one mp3 per request.
The address is the recording, so the `link` strategy answers it once the fetch succeeded.
The speaker is `th_th` and the service `thdic`, and any other speaker code answers 400.
Every headword answers, since a synthesizer has no missing entries.

## `audio[1]` Wiktionary

Wiktionary embeds Commons recordings as protocol-relative addresses, so `prefix` supplies the scheme.
Only the transcoded mp3 rendition is taken, because the original is Ogg.
The first attempt takes a `Th-` filename after the Thai section heading, without crossing another heading.
The second takes a Lingua Libre `LL-Q9217_%28tha%29-` one, Q9217 being Thai.
Pinned live, น้ำ, รัก, แมว and ภาษา gave a `Th-` recording.
บ้าน, กิน, หนังสือ, ไป, ขอบคุณ, ที่ and ประเทศ gave none.
None of them carried a Lingua Libre one.

## `frequency[0]` Wiktionary

The Chulalongkorn University list of the two thousand most frequent Thai words, kept on Wiktionary.
The page has two sections, `== 0-1000 ==` and `== 1001-2000 ==`, each one line of bracketed words.
The match captures the section's first rank and finds the bracketed headword without crossing into the next section.
The rank is no figure, so pattern bands label the first thousand core and the second everyday.
A word outside the list yields nothing.
Pinned live, น้ำ, บ้าน, กิน, หนังสือ, ไป, ที่ and ประเทศ were core, ขอบคุณ everyday and แมว absent.

## `morphology`

The list stands empty until a source is pinned.

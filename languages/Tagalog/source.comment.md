# source.json

## pack

Language pack for Tagalog.
The pack format is documented in `languages/English/source.comment.md`.
No varieties are declared, so every reading is the standard Manila one.
No transcription schemes are listed, because Tagalog is written in Latin script.
No respelling groups are declared, so every reading shows as its source wrote it.
No Naver source is listed, because its Tagalog-Korean dictionary carries no recordings.
Every entry there answers an empty `symbolFile`, and its speech service answers 400 for the `tl_ph` speaker code.

## `pronunciation[0]` Wiktionary

The IPA is derived from the spelling by the tl-pr template, so the wikitext carries none.
The rendered HTML is therefore read.
The match anchors on the Tagalog section heading, so a Cebuano or Ilocano section listed before it is skipped.
The match also refuses to cross another section heading.
A Tausug or Waray-Waray section listed after an entry without IPA would otherwise answer.
The match takes only a bare `<span class="IPA nowrap">`, one without attributes.
An etymology line may quote a sound in an IPAchar span before the pronunciation, as bahay does with /l/.
That span carries template attributes, so the bare form passes it over.
The first bare span after the heading is the headword's own phonemic reading between slashes.
The phonetic reading in brackets and the rhyme that follow it are left alone.
A second etymology's reading, as bahay's /baˈhaj/, is left alone too.
Pinned live, bahay gave /ˈbahaj/, salamat /saˈlamat/, kumain /kuˈmaʔin/, tubig /ˈtubiɡ/ and maganda /maɡanˈda/.
Ako gave /ʔaˈko/, araw /ˈʔaɾaw/, hindi /hinˈdiʔ/, oo /ˈʔoʔo/, aso /ˈʔaso/ and pusa /ˈpusaʔ/.

## `audio[0]` Wiktionary

Wiktionary embeds Commons recordings as protocol-relative addresses, so `prefix` supplies the scheme.
Only the transcoded mp3 rendition is taken, because the original is Ogg Vorbis or WAV.
The first attempt takes a `Tl-` filename after the Tagalog section heading, without crossing another heading.
The Commons files are named `Tl-ph-` or `Tl-PH-`, and the pattern accepts both.
The second takes a Lingua Libre `LL-Q34057_%28tgl%29-` one, Q34057 being Tagalog.
Few Tagalog entries carry any recording.
Pinned live, mahal gave Tl-ph-mahal.ogg.mp3 and pusa Tl-PH-pusa.ogg.mp3.
Bahay, salamat, kumain, tubig, maganda, ako, araw, hindi, oo and aso gave none.
None of them carried a Lingua Libre one.

## `frequency[0]` Leipzig

Leipzig Corpora Collection, Tagalog news crawl corpus of 2013 with 300,000 sentences.
The answer carries the word's frequency class.
It is 0 for the most frequent word and one higher each time the frequency halves.
The corpus holds 5,748,008 tokens and its most frequent word sa occurs 347,416 times.
So the once rule doubles per class from a factor of 17, the interval of that word.
The ladder rungs are fixed in the engine at one word in 10,000, 100,000 and 1,000,000.
With factor 17, class 9 is one word in 8,704, so it and every lower class are core.
Class 12 is one in 69,632, so classes 10 to 12 are everyday.
Class 15 is one in 557,056, so classes 13 to 15 are advanced and 16 and up rare.
An unknown spelling answers 404, so the attempt yields nothing.
Pinned live, ako gave class 5, bahay class 6, tubig class 8, maganda class 8 and salamat class 10.

## `morphology`

The list stands empty until a source is pinned.

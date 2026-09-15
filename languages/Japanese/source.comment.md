# source.json

## pack

Language selector pack for Japanese.
Pronunciation and audio sources are listed separately.
Either list can be filled without changing the application.

## `transcription[0]` Romaji

Hepburn romaji as Wiktionary prints it after the Japanese headword, inside the headword-tr span.
The lang attribute keeps other languages' transliterations out.
The span strategy strips the link a romaji with its own entry is wrapped in.
The article page is asked before the REST rendering, because the REST host rejects bursts with 429.

## `glyph` Kanji

The kanji of the headword.
It gives one transcription row under the scheme named here.
It also gives the language each character opens an entry in.
No source is listed.
A Japanese headword already carries its kanji and the reading view drops the kana itself.
The row exists so a kyūjitai form can be typed in by hand.

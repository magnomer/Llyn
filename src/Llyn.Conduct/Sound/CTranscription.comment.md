# CTranscription.cs

## `public sealed class CTranscription`

The transcription rows of the entry an editor holds, as the editor shows and edits them.
It was split from `CTimbre` by role, since the rows are edited rather than read from the pack.
The editor builds it over its desk, so it keeps no copy.

## `public CTranscriptionSheet CTranscriptionRead()`

The held draft's transcription block, ready to paint, with every row outside the glyph scheme.
Each row carries its scheme dropdown, marked by the engine's one-scheme rule.
The editor reads it on every draft repaint and for the add command's verdict.
An empty desk answers the hidden block with no rows, so nothing can be added.

## `public void CTranscriptionSet(long transcription, string text)`

The user typed `text` into the transcription row `transcription`, the glyph row among them.
The engine defers it, and a filling desk writes nothing.

## `public void CTranscriptionSchemeSet(long transcription, string scheme)`

The user picked `scheme` in the dropdown of the transcription row `transcription`.
The engine applies it at once, and a filling desk writes nothing.

## `public void CTranscriptionAdd(long transcription)`

The user pressed plus on the transcription row `transcription`, or on the block for id zero.
The engine adds the row under the first free scheme, and a filling desk writes nothing.

## `public void CTranscriptionRemove(long transcription)`

The user pressed minus on the transcription row `transcription`.

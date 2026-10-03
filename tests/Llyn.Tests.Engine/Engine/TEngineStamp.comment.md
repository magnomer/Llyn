# TEngineStamp.cs
Hash: `b0c187aec5a7b435`

## `public sealed class TEngineStamp`

Covers the reading view's stamp times, which the engine words for Conduct.

## `public void EngineStampRead_StoredEntry_WordsBothTimes()`

A stored entry answers both of its times, already worded.

## `public void EngineStampRead_UnknownEntry_AnswersNothing()`

An id no entry has answers not stored with empty times, so the view hides its stamp row.

## `public void EngineStampFormat_UnreadableText_ReturnsEmpty()`

A stamp that does not parse, or none at all, shows as nothing.

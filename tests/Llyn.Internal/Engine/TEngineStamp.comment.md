# TEngineStamp.cs

## `public sealed class TEngineStamp`

Covers the reading view's facts the engine words for Conduct: the stamp times and the narrative check.

## `public void EngineStampRead_StoredEntry_WordsBothTimes()`

A stored entry answers both of its times, already worded.

## `public void EngineStampRead_UnknownEntry_AnswersNothing()`

An id no entry has answers not stored with empty times, so the view hides its stamp row.

## `public void EngineStampFormat_UnreadableText_ReturnsEmpty()`

A stamp that does not parse, or none at all, shows as nothing.

## `public void EngineNarrativeCheck_Text_HoldsWordsOnlyWhenNotBlank(string text, bool expected)`

Only text with a character besides white space counts as a narrative, by the etymology draft's rule.

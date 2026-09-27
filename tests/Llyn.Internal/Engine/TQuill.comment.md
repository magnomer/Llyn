# TQuill.cs

## `public sealed class TQuill`

The quill's text edits, one fact per member.
Each fact starts a tenure on a fresh workspace with no delay, so a deferred edit lands at once.

## `public void AuthorSet_FreshAuthor_WritesName()`

A typed name lands on a fresh Author and marks the draft changed.

## `public void ExampleSet_TextTypedOverUnknown_WritesKnownText()`

Typed text lands as known even over an unknown value, and marks the draft changed.

## `public void SpeakerSet_LanguagePicked_WritesLanguage()`

A picked language lands on the held Example.

## `public void ReferenceSet_ReferencePicked_CitesReference()`

A picked Source becomes the held Example's citation.

## `public void GlossAdd_LanguageGiven_AddsGlossInLanguage()`

An added Gloss takes the given language.

## `public void GlossRemove_AddedGloss_DropsIt()`

An added Gloss is dropped by its id.

## `public void GlossSet_TextThenLanguage_WritesBoth()`

A typed text and then a chosen language both land on the same Gloss.

## `public void MentionAdd_SpanOverEntry_LinksWithoutSense()`

A span links to the Entry with its offset and length, and no sense yet.

## `public void MentionRemove_AddedMention_DropsIt()`

An added Mention is dropped by its id.

## `public void MentionSet_SensePicked_PointsMentionAtSense()`

A picked sense lands on the Mention.

## `public void SituationSet_UnknownKindLeftEmpty_KeepsKindUnknown()`

A field left empty whose held value is unknown stays unknown, while the typed title lands.

## `private static LTenure TQuillExampleStart(LEngine engine, LStateValue text)`

Stores an English Example with the given text and starts a tenure on it with no delay.

## `private static LEntry TQuillEntrySave(LEngine engine)`

Stores an English Entry with one sense, for a Mention to link to.

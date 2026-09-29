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

## `public void GlossInsert_PlaceGiven_AddsGlossInTheGlossLanguageAtThePlace()`

An inserted Gloss takes the language the engine resolves for a new Gloss.

## `public void GlossRemove_AddedGloss_DropsIt()`

An added Gloss is dropped by its id.

## `public void GlossSet_TextThenLanguage_WritesBoth()`

A typed text and then a chosen language both land on the same Gloss.

## `public void SituationSet_UnknownKindLeftEmpty_KeepsKindUnknown()`

A field left empty whose held value is unknown stays unknown, while the typed title lands.

## `public void EtymologySet_TextTyped_WritesNarrative()`

A typed narrative lands on the held entry's etymology.

## `public void EtymonAdd_TwoSources_KeepsTheGivenOrder()`

A source added at position zero goes ahead of one appended at the end.

## `public void EtymonRemove_AddedSource_DropsIt()`

An added source is dropped by its entry id.

## `public void MentionSave_SpanOverEntry_LinksNarrativeSpan()`

A span of the narrative links to the given entry at the given offset.

## `public void CitationSet_SourcePicked_CitesSentence()`

A picked Source becomes the citation of a card's sentence.

## `private static LTenure TQuillEntryStart(LEngine engine)`

Starts a tenure on a fresh entry with no delay.

## `private static LTenure TQuillExampleStart(LEngine engine, LStateValue text)`

Stores an English Example with the given text and starts a tenure on it with no delay.

## `private static LEntry TQuillEntrySave(LEngine engine)`

Stores an English Entry with one sense, for a Mention to link to.

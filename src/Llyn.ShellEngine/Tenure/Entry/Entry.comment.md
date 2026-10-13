# Entry

This folder holds entry draft field edits, row edits and the reads those edits need.
The quills and easel turn typed values and deliberate picks into tenure requests.

## `LEasel`

Image and video edits share the tenure's request path rather than writing the draft themselves.
Additions read their end position from the held draft, so callers need not carry row counts.
Removals find the owning card from the draft instead of trusting a caller's card id.
Location edits share `LEaselRequestRun`, so images and videos honour the same deferral flag.

## `LQuillCard`

Card placement and removal guards belong to `LDraftClerkCard`, keeping list decisions outside the request builder.
Structural edits apply immediately, while typed title, expression and meaning values defer.

## `LQuillChip`

Chip edits and searches share the draft port used to resolve stored records and selected spans.
List parsing can build several requests from one typed value, rather than one request per edit.
Completed parts advance the requested insertion position before the remaining text is searched.

Refused tag, situation, register and reference searches answer empty offers.
Other failures are not caught here.
Translation and mention searches requested directly do not catch failures here.

Citation resolution precedes its request in one method, so callers cannot reverse those operations.
Card sentences and the held Example use different reference requests.

Translation resolution passes the draft's stored identity to the draft port.
An unresolved translation keeps its typed text instead of clearing the field.
A new translation court inherits the owner draft's origin before its target is linked.
Removal unlinks first, then deletes any matching court and its target draft.

Translation target reads combine meaning and collocation links into one distinct-id engine read.
A refused target read supplies an empty result to the card facade.
Other target read failures reach the caller.

Sense changes and span-based mention removals persist pending typing before finding the Mention.
Both share `LQuillMention.LQuillMentionFind`, avoiding separate interpretations of the selected span.
An unmatched span produces no request.

## `LQuillEntry`

Note normalization and comparison share the entry clerk, so editing and repaint checks use the same rule.
Unit and freedom choices compare against the current draft rather than a caller's cached selection.
Choosing the held value requests the empty value, allowing the same operation to clear it.

## `LQuillEtymology`

Narrative typing defers, while etymon and mention links apply immediately.
A missing draft or refused etymon read answers empty, allowing the caller to display without resolved targets.
Other etymon read failures reach the caller.

## `LQuillPronunciation`

The engine's respelling setting selects the request type for both the primary reading and accent text.

Accent loading returns a fresh sheet only when its language still matches the sheet loaded.
This avoids returning flags for a language the draft has left during the await.

Audio resolution clears the row's audio when the engine cannot resolve its address.
Variety edits read the primary identity at application time instead of retaining an earlier primary row id.

## `LQuillReflex`

The reflex port decides whether row text is written as respelling.
The language edit returns replacement rows immediately, while its request remains deferred.

Visibility depends on available reflex rules or the draft's reflected flag, not directly on its row count.
Lookup starts only for a stored draft whose reflected flag is false.

Anchor scans use the stored entry, draft language, reflex language and row tone together.
A missing stored identity, missing row or refused scan answers empty.
Other scan failures reach the caller.

## `LQuillSentence`

Gloss language choices apply immediately, while gloss text defers.
A supplied language takes precedence over text in `LSentenceGlossSet`.

Gloss insertion reads its language through the engine rather than embedding a language in the request builder.
Preparation delegates the empty-gloss decision to the gloss clerk before requesting a first row.

## `LQuillSpeech`

The quill keeps pending speech-chip state alongside the tenure, rather than making callers reconstruct it.
Chip settlement, parsing and offer matching belong to the speech clerk.

Speech creation uses the draft language's vocabulary before sending the updated chips.
A refused vocabulary read supplies an empty catalog to offer matching.
A refused vocabulary write answers no catalog value.
Other vocabulary failures reach the caller.

## `LQuillTranscription`

The sheet combines engine schemes and the glyph split through the reading clerk.
Addition takes its scheme from that sheet instead of duplicating scheme selection in the request builder.
Typed text defers, while scheme picks apply immediately.

## Stamp

Commit: `c2f335c331fe`
- `LEasel.cs` `d253cf368264ecad`
- `LQuillCard.cs` `53194206db46ddde`
- `LQuillChip.cs` `ccf49aacacdd2de8`
- `LQuillEntry.cs` `923cb6b552ca3f64`
- `LQuillEtymology.cs` `d06d61da2598794e`
- `LQuillPronunciation.cs` `705a829d2b1a3d36`
- `LQuillReflex.cs` `0a80a427f2224a5c`
- `LQuillSentence.cs` `0015d1ed21b5f7f1`
- `LQuillSpeech.cs` `1dd744211b040b7c`
- `LQuillTranscription.cs` `a15074db3fbc86b6`

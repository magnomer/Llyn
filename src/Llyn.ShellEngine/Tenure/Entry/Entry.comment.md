# Entry

This folder holds entry draft field edits, row edits and the reads those edits need.
The quills and easel turn typed values and deliberate picks into tenure requests.

## `LEasel`

Image and video edits share the tenure's request path rather than writing the draft themselves.
Additions read their end position from the held draft, so callers need not carry row counts.
Every card edit names the card by its kind and id, since the two lists number their cards apart.
Removals take the owning card from the caller and apply nothing once no draft is held.
Location edits share `LEaselRequestRun`, so images and videos honour the same deferral flag.

## `LQuillAncestor`

The edits of the etymology map boxes of the draft the tenure holds.
`LQuillLineageRead` answers an empty tree when no draft is held, and otherwise reads the Lineage of the held boxes.
A draft of a new entry has no stored id, so the read passes zero.
No entry is then on the path.
`LQuillTextSet` is deferred while typing, like other text.
It applies at once for a linked box, since the first keystroke must drop the link before anything else shows.
`LQuillGlossSet` is deferred the same way.
`LQuillTargetSet` applies at once, and answers true when the box had sources that are now set aside.
The caller shows the one Undo only on true, and a refused link answers false.
`LQuillLanguageSet` trims the name and matches it, ignoring case, to the language list.
A user tongue is chosen by id, and any other name is stored as text.
Relation, insertion, addition, removal and restoration all apply at once, since each changes the shape of the map.

## `LQuillCard`

Card placement and removal guards belong to `LDraftClerkCard`, keeping list decisions outside the request builder.
Each edit names its card by kind and id.
Structural edits apply immediately, while typed title, expression and meaning values defer.

## `LQuillChip`

Chip edits and searches share the draft port used to resolve stored records and selected spans.
List parsing can build several requests from one typed value, rather than one request per edit.
Completed parts advance the requested insertion position before the remaining text is searched.
The tag, situation and register adds answer the kept text, and each find is a separate call.

Tag, situation, register, reference, translation and mention searches catch nothing here.
A stale draft refuses the search, and the refusal reaches the Conduct caller that shows it.

Citation resolution precedes its request in one method, so callers cannot reverse those operations.
Card sentences and the held Example use different reference requests.
The Mention add, sense set and removal take a card sentence by kind, card and sentence.
The same edits take an example site internally, so a card sentence and the held Example share one path.

Translation resolution passes the draft's stored identity to the draft port.
An unresolved translation keeps its typed text instead of clearing the field.
A new translation court inherits the owner draft's origin before its target is linked.
Removal unlinks first, then deletes any matching court and its target draft.

Translation target reads combine the links of every meaning and collocation, nested cards included, into one distinct-id engine read.
The tenure reads through the same resolution as the facade, so both key their answers alike.
A target read failure, a refusal included, reaches the caller.

Sense changes and span-based mention removals persist pending typing before finding the Mention.
Both share `LQuillMention.LQuillMentionFind`, avoiding separate interpretations of the selected span.
An unmatched span produces no request.

## `LQuillEntry`

Note normalization and comparison share the entry clerk, so editing and repaint checks use the same rule.
Unit and freedom choices compare against the current draft rather than a caller's cached selection.
Choosing the held value requests the empty value, allowing the same operation to clear it.

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
A missing stored identity or missing row answers empty.
Every scan failure, a refusal included, reaches the caller.

## `LQuillSentence`

Gloss language choices apply immediately, while gloss text defers.
Gloss edits and the first-row preparation address a card sentence by kind, card and sentence.
Without them they address the draft's own Example.
Both forms share one private path over an example site.
A supplied language takes precedence over text in `LSentenceGlossSet`.

Gloss insertion reads its language through the engine rather than embedding a language in the request builder.
Preparation delegates the empty-gloss decision to the gloss clerk before requesting a first row.

## `LQuillSpeech`

The quill keeps pending speech-chip state alongside the tenure, rather than making callers reconstruct it.
Chip settlement, parsing and offer matching belong to the speech clerk.

Speech creation uses the draft language's vocabulary before sending the updated chips.
The chip edits read the held draft, so a stale draft's refusal reaches the caller.
The repaint read and the offer read use the present draft, which a stale or ended draft leaves empty.
Then the repaint shows no chips and the offer is empty.
Every vocabulary failure, a refusal included, reaches the caller.

## `LQuillTranscription`

The sheet combines engine schemes and the glyph split through the reading clerk.
Addition takes its scheme from that sheet instead of duplicating scheme selection in the request builder.
Typed text defers, while scheme picks apply immediately.

## Stamp

Commit: `36e31eb71c98`
- `LEasel.cs` `5262f25cced57f5a`
- `LQuillAncestor.cs` `9a383e8186246a3b`
- `LQuillCard.cs` `01ba2db86c718ee7`
- `LQuillChip.cs` `304eddb966bcceff`
- `LQuillEntry.cs` `923cb6b552ca3f64`
- `LQuillPronunciation.cs` `705a829d2b1a3d36`
- `LQuillReflex.cs` `0f96dbb24c7d79be`
- `LQuillSentence.cs` `dd417f90b4200ff2`
- `LQuillSpeech.cs` `ea93b024c490dca1`
- `LQuillTranscription.cs` `a15074db3fbc86b6`

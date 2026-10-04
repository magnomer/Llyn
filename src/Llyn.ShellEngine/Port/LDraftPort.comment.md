# LDraftPort.cs
Hash: `557db94c11347858`

## `public interface LDraftPort`

The slice of the engine a deportment sees when it holds or links a draft.
It starts a tenure, drops a draft, attaches an observer, and answers the link lookups a card edit asks.
It also divides a sentence around its Mentions, converts text offsets, and toggles or matches anchors.
`LDraftOutlet` implements it today, and the draft clerk takes it over when the parts are dismantled.
Member names keep the engine's `LEngine*` form until that hand-over renames them once.

## `LTenure LEngineTenureStart(LVista vista, long? id);`

Starts a tenure over the entry the vista chose, or a fresh entry when `id` is null.

## `LTenure LEngineTenureStart(string origin, LSubject subject, long? id);`

Starts a tenure for a non-entry subject from a named origin, as the corpus and repertoire holds do.

## `LTenure LEngineOccurrenceStart(LVista vista, long? situation);`

Starts a fresh entry already linked to the Situation, as a new occurrence in the repertoire opens.

## `LTenure LEngineQuotationStart(LVista vista, long? example);`

Starts a fresh entry already citing the Example, as a new quotation in the corpus opens.

## `LTenure LEngineFootnoteStart(LVista vista, long? reference);`

Starts a fresh entry already citing the Source, as a new footnote on the shelf opens.

## `LTenure LEngineMembershipStart(LVista vista, long? tag);`

Starts a fresh entry already carrying the Tag, as a new member of the taxonomy opens.

## `LTenure LEngineCohortStart(LVista vista, long? register);`

Starts a fresh entry already carrying the Register, as a new member of the tenor panel's cohort opens.

## `void LEngineDraftDelete(long id);`

Drops one held draft and its claim, after the links the draft owns are settled.
A draft bulletin with id zero follows, since the held set the status bar counts shrank.

## `void LEngineObserverAttach(Action<LBulletin> observer);`

Subscribes `observer` to future bulletins.
Attaching the same delegate twice still calls it once.

## `void LEngineLeftoverSweep();`

Cancels every unheld draft that says nothing its origin does not.
A swept draft can settle a court into a held draft, so no tenure may keep its state across it.

## `LCourt LEngineCourtStart(long ownerId, string origin, string headword, string language);`

Starts a tentative target and records the court row naming it, as one call.
A failure after the start cancels the target, so the workspace holds both or neither.

## `IReadOnlyList<LBylineRow> LEngineBylineFind(long draft, string query);`

The Authors a typed credit may already name, left out those the draft credits.
Each name comes split around the typed word, ready to paint.

## `LTagOffer LEngineTagFind(LTenure held, long card, string text);`

The stored Tags a card's tag field offers for the text it keeps, ready to show.
It sits beside the other finds a chip field makes as the user types.

## `LRegisterOffer LEngineRegisterFind(LTenure held, long card, string text);`

The stored Registers a card's register field offers for the text it keeps, ready to show.
The held draft names the language, so the caller passes none.

## `LSituationOffer LEngineSituationFind(LTenure held, long card, string text);`

The stored Situations a card's situation field offers for the text it keeps, ready to show.
The held draft names the card's links, so those are left out.

## `LReferenceOffer LEngineReferenceFind(LTenure held, long card, long sentence, string text);`

The stored Sources a card sentence's citation field offers for the typed text, ready to show.
The held draft names the Source the sentence cites, so typing its byline offers nothing.

## `long LEngineCitationResolve(LTenure held, long card, long sentence, string title);`

The id of the Source a typed title names, minting it when none matches.
The held draft names the Source the sentence cites now, so an unchanged title keeps it.

## `IReadOnlyDictionary<long, IReadOnlyList<LMentionLabel>> LEngineMentionResolve(LTenure held);`

The chip line of every sentence row on every card of the held draft, keyed by the row.
Each chip carries the words it covers, the linked headword and the chosen sense.

## `IReadOnlyList<LMentionLabel> LEngineMentionResolve(LTenure held, long card, long sentence);`

The chip line of one Example of the held draft, addressed by card and sentence.
Card 0 and sentence 0 address the draft's own Example.

## `IReadOnlyList<LMentionLabel> LEngineEtymologyResolve(LTenure held);`

The chip line of the held draft's etymology.

## `static IReadOnlyList<LMentionPiece> LEngineMentionDivide(string text, IReadOnlyList<LMention> mentions)`

The pieces a sentence falls into around its Mentions.
It is static, since Conduct divides a row's text while it maps the row and no port is at hand.

## `int LEngineUnitRead(string text, int offset);`

The UTF-16 index in `text` of a code-point offset.
Mention spans count code points while a text box counts UTF-16 units, so the engine converts.

## `LMentionDraft LEngineSpanRead(string text, int start, int length);`

A field's selection as the span a Mention request carries, measured in code points by the engine.

## `bool LEngineSpanCheck(string text, int start, int length);`

Whether a field's selection covers any code point, so a link command may run.

## `IReadOnlyList<(long LMeaningId, string LMeaningName, int LMeaningDepth)`

The Meanings the sense menu offers for the linked Mention under a sentence field's selection.
Pending typing is persisted before the find, and no linked Mention answers null.

## `int LEngineOffsetRead(string text, int unit);`

The code-point offset of a UTF-16 index in `text`, the inverse of `LEngineUnitRead`.

## `bool LEngineAnchorCheck(long entryId, string headword);`

Whether the reflex rows of `headword` may carry anchors, judged on the entry's own rime-book rows.

## `string LEngineAnchorFormat(long entryId, IReadOnlyList<long> anchors, string headword, string separator);`

The readings of the entry's anchored rows joined with `separator`, empty when the rows cannot be anchored.

## `string LEngineBylineRead(string? text);`

The word a typed credit searches and highlights the byline with.

## `LTranslationOffer LEngineTranslationFind(LTenure held, string text, string word, bool chosen);`

The offer a typed translation opens in the held draft.
It carries one row per match and the languages a new Entry is offered in.

## `IReadOnlyList<LVistaRow> LEngineProspectFind(LTenure held, string word);`

The Entries a mention of `word` in the held draft may name, in the draft's own language.

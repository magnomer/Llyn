# LDraftPort.cs

## `public interface LDraftPort`

The slice of the engine a deportment sees when it holds or links a draft.
It starts a tenure, drops a draft, attaches an observer, and answers the link lookups a card edit asks.
It also divides a sentence around its Mentions, converts text offsets, and toggles or matches anchors.
`LEngine` implements it today, and the draft clerk takes it over when the parts are dismantled.
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

## `IReadOnlyList<LAuthor> LEngineBylineFind(long draft, string query);`

The Authors a typed credit may already name, left out those the draft credits.

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

## `string LEngineBylineRead(string? text);`

The word a typed credit searches and highlights the byline with.

## `IReadOnlyList<LVistaRow> LEngineProspectFind(string query);`

The entries a typed translation may link to, one row per match plus one per language for a new entry.

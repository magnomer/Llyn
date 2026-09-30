# LEntryPort.cs

## `public interface LEntryPort`

The slice of the engine a deportment sees when it reads stored records.
It finds and reads entries, the catalog rows of every tab, and the per-entry marks a reader shows.
The marks are favourite, grasp, frequency, epithet, incoming links, mentions and glyphs.
The catalog side finds and creates tags, registers, situations, examples, references and authors.
The pure facts a panel draws with travel here too: the grasp step and wording, twin names and markdown blocks.
Everything here reads or writes a committed record, never a held draft.
`LEngine` implements it today, and the entry clerk takes it over when the parts are dismantled.

## `IReadOnlyList<LVistaRow> LEngineEntryFind(LVista? parent, LVista? child);`

The entry rows of a child list narrowed by the parent catalog's choice, as the footnote and cohort lists read.

## `IReadOnlyList<LGlyphCell> LEngineGlyphDivide(LEntryDraft draft);`

The cells of the draft's glyph row, empty when its language declares no glyph section.

## `LGlyph? LEngineGlyphRead(LEntryDraft draft);`

The glyph section of the draft's language, so a reader hands the draft unread.

## `long LEngineGlyphResolve(string character, string language);`

The id of the entry a glyph cell opens, made when none exists yet.

## `IReadOnlyList<LTranscriptionDraft> LEngineTranscriptionRead(LEntryDraft draft);`

The filled transcription rows a reading view lists, without the glyph row.

## `IReadOnlyList<LReflexDraft> LEngineReflexRead(LEntryDraft draft);`

The written reflex rows of the draft, which a reading view lists.

## `IReadOnlyList<LCatalogAuthor> LEngineRollFind(LVista? vista);`

The Authors of the authors panel's roll, headed by the uncredited row, and none while the vista is missing.

## `IReadOnlyList<LCatalogAuthor> LEngineUnionFind(LVista? roll, string typed);`

The Authors the stored Author of the roll may be folded into, matched by `typed` and capped.

## `(string LUnionDropped, string LUnionKept) LEngineUnionRead(LTenure? held, long kept);`

The held draft's written Author name and the kept Author's name, as the union question shows them.

## `LVita LEngineVitaRead(LVista? roll);`

The read sheet of the Author the roll stands on, or the sheet of nobody.

## `IReadOnlyList<LCatalogReference> LEngineOeuvreFind(LVista? roll, LVista? oeuvre);`

The Sources of the authors panel's oeuvre, and none while either vista is missing.

## `string LEngineWorkFormat(int count);`

An Author's work count as the roll shows it.

## `LColophon LEngineColophonRead(LDraft draft);`

The read sheet of a Source draft, with the citation line of the Source it holds.

## `LImprint LEngineImprintRead(LDraft? draft);`

The edit sheet of a Source draft, or of a blank Source with no draft.

## `IReadOnlyList<LAuthorRow> LEngineCreditRead(LTenure? held);`

The credit rows of the held Source draft, after its deferred requests are applied.

## `string LEngineTallyRead(long? reference);`

The citation line of one Source, worded as none when no Source is given.

## `IReadOnlyList<LCatalogReference> LEngineReferenceFind();`

Every Source as the whole shelf lists it, ordered by author.

## `static bool LEngineTextMatch(string field, string shown)`

Whether a field showing `field` already shows the text `shown`, a blank field reading as nothing recorded.
It is static, since it reads no record and a Conduct verdict holds no port for it.

## `(bool, string, string) LEngineStampRead(long entryId);`

Whether an entry is stored, with its worded creation and update times, empty once it is gone.
The engine parses and words the stored stamps, so Conduct only copies them.

## `static IReadOnlyList<(string LReferenceKindTag, string LReferenceKindKey)> LEngineKindRead()`

The kind menu of the source editor, each option's tag and localization key, in menu order.
It is static, since the menu is the same for every Source and needs no port.

## `static bool LEngineNarrativeCheck(string text)`

Whether an etymology narrative holds words, by the etymology draft's own rule.
It is static, since the reading view asks it of text a control holds and no port is at hand.

## `static long? LEngineLinkRead(long? link)`

The entry a clicked link names, or null when it names none.
A link names its entry only through a stored id, so an empty id names nothing.
It is static, since it reads only what the click hands over.

## `static (string, string, string) LEngineLineRead(LSentenceDraft sentence, LSentenceOrder order, string mark, IReadOnlyDictionary<long, string> citations)`

The frame, the sentence and the Source line a reading card shows for one sentence row.
The clerk composes them, so the display and the portrait share one rule.
It is static, since it reads only the row, the order and the lines the caller holds.

## `IReadOnlyDictionary<long, IReadOnlyList<LTranslationTarget>> LEngineTranslationRead(LEntryDraft shown);`

The link targets of every meaning and collocation of the shown entry, keyed by card id.
Every card of the entry has a key, so a reader never checks for a missing card.

## `static (LOwner, int)? LEngineCardFind(LEntryDraft draft, long id)`

Which card list of `draft` holds the card `id`, and at which place, or null when neither does.
It is static, since it reads only the draft the caller holds.

## `IReadOnlyDictionary<long, string> LEngineCitationRead(LEntryDraft shown);`

The citation line of every Source the shown entry cites, child cards included.
A Source that is gone reads as its bare id.

## `string LEngineCitationRead(LDraft? draft);`

The citation line of the Source the draft's own Example cites, and empty for no draft or no citation.

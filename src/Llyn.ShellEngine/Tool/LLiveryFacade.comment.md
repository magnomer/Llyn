# LLiveryFacade.cs
Hash: `ada8bba382f1d148`

## `internal sealed class LLiveryFacade`

`LEngine` builds it once and holds it as `LEngineLivery`.
`LCourierFacade.LEngineCourierSend` hands both its reads to the Joplin push.

## `public LLiveryFacade(LEngineHearth hearth, LCardFacade card, LCatalogFacade catalog, LEntryFacade entry, LFanqieFacade fanqie, LLanguageFacade language, LReferenceFacade reference, LReflexFacade reflex, LScriptFacade script, LSettingsFacade settings, LVocabularyFacade vocabulary)`

Stores the hearth and the sibling facades it calls, all built by `LEngine` before this one.
The gate, the staff and the shared state are read through the hearth.
It takes its siblings rather than the engine, so it names only the facades it uses.

## `private LEngineStaff LLiveryFacadeStaff`

Reads `LEngineHearth.LEngineStaffHeld`, so a call after a workspace switch reads the new workspace.

## `public LLiveryPage? LEngineLiveryRead(long entryId, bool pending)`

Holds `LEngineGate` for the whole read, so every part comes from one moment.
It loads the draft through `LEngineEntryLoad` and answers null when no entry is stored.
It then gathers the remaining members through engine reads and clerk reads.
It hands every answer to `LLiveryClerk.LLiveryClerkBuild` and returns the page that builder makes.
It calls no read that starts a fetch.
The frequency rows come from `LFrequencyClerkRead` with fetch off.
The guise list reads the draft's reflex rows in draft order, each guise carrying its shown text.
The folded reflex languages come from `LReflexFoldedRead` for the draft's language.
The inflection table comes from `LVocabularyFacade.LEngineInflectionRead`, the path the display reads.
It asks with `held` off, since a note never holds a fill.
Its `enabled` is `LEngineMorphologyCheck`, the setting the display reads.
The caller's `pending` chooses whether the page reads `LEngineInflectionCheck`, the in-memory lacuna mark the display reads.
The export passes true, so the page shows each row and table as the view shows them at that moment.
The Joplin note passes false, since the lacuna mark lives only in memory and a note keeps stored data.
An in-flight fill would freeze as pending in Joplin, since nothing rewrites the note when the fill ends.
The same pending state and `enabled` pass to the builder, which checks each paradigm row with them.
The headword and glyph fonts come from `LEngineFontRead` for the draft's language.
The opened boxes are each `LFoldBox` that `LEngineBoxCheck` finds open.
The reading line comes from `LFanqieClerkFormat`, the rule `LEngineReadingRead` applies.
It skips that read's fanqie start, since starting would launch a fetch.
The folded card ids come from `LFoldClerkRead`, the path `LEngineFoldRead` reads.
They are read inside the same lock as the rest.
The accent sheet is read once and passed both to the page and to `LVarietyFlagScan`.
It hands `LLanguageFlagFind` itself to the builder, which asks it once per named language.
The flags come from `LLanguageFlagFind` and `LVarietyFlagScan`, which read stored files only.

## `public LLiveryLanguage LEngineLiveryRead(string language, Func<string, string?> localize)`

Holds `LEngineGate` for the whole read, so every list comes from one moment.
It hands the stem clerk only while `LShengfuRuleRead` answers a rule for the language.
It hands the diwei clerk only while `LEngineBookCheck` finds a fanqie book for the language.
The respelling check and `LSettingsTally` are read as `LFanqieFacade.LEngineDiweiResolve` reads them.
It hands `LSettingsRespelled` too, so the series members get the guises the entry note gets.
It hands everything to `LLiveryClerk.LLiveryClerkBuild` and returns the model that builder makes.
It never fetches and never starts background work.

# LLiveryFacade.cs
Hash: `cdf3779ad6ae7205`

## `internal sealed class LLiveryFacade`

`LEngine` builds it once and holds it as `LEngineLivery`.
`LCourierFacade.LEngineCourierSend` hands both its reads to the Joplin push.

## `public LLiveryFacade(LEngineHearth hearth, LCardFacade card, LCatalogFacade catalog, LEntryFacade entry, LFanqieFacade fanqie, LLanguageFacade language, LReferenceFacade reference, LReflexFacade reflex, LScriptFacade script, LSettingsFacade settings, LVocabularyFacade vocabulary)`

Stores the hearth and the sibling facades it calls, all built by `LEngine` before this one.
The gate, the staff and the shared state are read through the hearth.
It takes its siblings rather than the engine, so it names only the facades it uses.
Reads `LEngineHearth.LEngineStaffHeld` on each call, so a call after a workspace switch reads the new workspace.

## `private LEngineStaff LLiveryFacadeStaff`

Reads `LEngineHearth.LEngineStaffHeld`, so a call after a workspace switch reads the new workspace.

## `public LLiveryPage? LEngineLiveryRead(long entryId)`

Holds `LEngineGate` for the whole read, so every part comes from one moment.
It loads the draft through `LEngineEntryLoad` and answers null when no entry is stored.
It then gathers the remaining members through engine reads and clerk reads.
It hands every answer to `LLiveryClerk.LLiveryClerkBuild` and returns the page that builder makes.
It calls no read that starts a fetch.
The frequency rows come from `LFrequencyClerkRead` with fetch off.
The guise list follows the draft's reflexes in draft order.
The accent sheet is read once and passed both to the page and to `LVarietyFlagScan`.
It hands `LLanguageFlagFind` itself to the builder, which asks it once per named language.
The flags come from `LLanguageFlagFind` and `LVarietyFlagScan`, which read stored files only.

## `public LLiveryLanguage LEngineLiveryRead(string language, Func<string, string?> localize)`

Holds `LEngineGate` for the whole read, so every list comes from one moment.
The pronunciation rows come from `LPronunciationClerkFind` with an empty query in name order.
It hands the stem clerk only while `LShengfuRuleRead` answers a rule for the language.
It hands the diwei clerk only while `LEngineBookCheck` finds a fanqie book for the language.
The respelling check and `LSettingsTally` are read as `LFanqieFacade.LEngineDiweiResolve` reads them.
It hands everything to `LLiveryClerk.LLiveryClerkBuild` and returns the model that builder makes.
It never fetches and never starts background work.

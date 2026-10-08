# LLiveryFacade.cs
Hash: `634375f6a5a022a5`

## `internal sealed class LLiveryFacade`

`LEngine` builds it once and holds it as `LEngineLivery`.
`LCourierFacade.LEngineCourierSend` hands both its reads to the Joplin push.

## `public LLiveryFacade(LEngine engine)`

Stores the engine, whose facades and staff it reads on each call.

## `private LEngineStaff LLiveryFacadeStaff`

Reads `LEngine.LEngineStaffHeld`, so a call after a workspace switch reads the new workspace.

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

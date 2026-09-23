# LEngineStaff.cs

## `internal sealed record LEngineStaff(...)`

The record keeps every clerk and engine helper that belongs to the current rig.
The engine swaps one record when the workspace changes.
Each facade reads its concern from this record.

**Parameters**

- `LEngineStaffIdentity` issues temporary ids for the workspace.
- `LEngineStaffCache` holds the loaded language packs.
- `LEngineStaffDraft` manages held drafts.
- `LEngineStaffChronicle` manages draft history.
- `LEngineStaffCourt` manages claims and ownership history.
- `LEngineStaffClaim` manages held claims.
- `LEngineStaffTag` manages tags.
- `LEngineStaffRegister` manages registers.
- `LEngineStaffTranslation` manages translation links.
- `LEngineStaffReference` manages references.
- `LEngineStaffExample` manages examples.
- `LEngineStaffSituation` manages situations.
- `LEngineStaffCard` manages meaning cards.
- `LEngineStaffMeaning` manages meaning operations.
- `LEngineStaffMention` manages text mentions.
- `LEngineStaffUsage` manages usage records.
- `LEngineStaffVocabulary` manages vocabulary rows.
- `LEngineStaffParadigm` manages paradigms.
- `LEngineStaffInflection` manages inflections.
- `LEngineStaffPronunciation` manages pronunciations.
- `LEngineStaffTrail` manages workspace trails.
- `LEngineStaffLanguage` manages language data.
- `LEngineStaffRecording` manages recordings.
- `LEngineStaffTranscription` manages transcriptions.
- `LEngineStaffReflex` manages reflexes.
- `LEngineStaffEntry` manages entries.
- `LEngineStaffLacuna` manages missing entries.
- `LEngineStaffFrequency` manages frequency data.
- `LEngineStaffOutcome` manages outcome calculations.
- `LEngineStaffAuthor` manages authors.
- `LEngineStaffFavorite` manages favorites.
- `LEngineStaffCitation` manages citations.
- `LEngineStaffFanqie` manages fanqie data.
- `LEngineStaffShengfu` manages 音韻地位 data.
- `LEngineStaffStem` manages stems.
- `LEngineStaffDiwei` manages 音韻地位 placements.
- `LEngineStaffScript` manages script data.
- `LEngineStaffWorkspace` manages workspace state.
- `LEngineStaffMarkup` manages markup records.
- `LEngineStaffIntake` imports markup.
- `LEngineStaffPortrait` builds portraits.
- `LEngineStaffEnsign` manages language flags.

## `internal static LEngineStaff LEngineStaffBuild(...)`

Builds the staff in dependency order over one rig.
Fetch clerks receive the gate, settings reader and bulletin raiser supplied by the engine.
The identity issuer receives the stale ids, so the new workspace never issues one a tenure still holds.

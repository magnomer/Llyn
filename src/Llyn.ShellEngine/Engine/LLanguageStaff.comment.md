# LLanguageStaff.cs
Hash: `524c68b3f2df2224`

## `internal sealed record LLanguageStaff(...)`

The group of clerks for language data, pronunciations and every background fetch.
All six fetching clerks sit here, so one call cancels them together.

**Parameters**

- `LLanguageStaffVocabulary` manages vocabulary rows.
- `LLanguageStaffParadigm` manages paradigms.
- `LLanguageStaffPronunciation` manages pronunciations.
- `LLanguageStaffTrail` manages workspace trails.
- `LLanguageStaffLanguage` manages language data.
- `LLanguageStaffRecording` manages recordings.
- `LLanguageStaffTranscription` manages transcriptions.
- `LLanguageStaffReflex` manages reflexes.
- `LLanguageStaffLacuna` fetches missing inflected forms and records the lacunae left.
- `LLanguageStaffFrequency` manages frequency data.
- `LLanguageStaffFanqie` manages fanqie data.
- `LLanguageStaffShengfu` manages a character's phonetic series, its 聲符.
- `LLanguageStaffStem` manages stems.
- `LLanguageStaffDiwei` manages 音韻地位 placements.
- `LLanguageStaffScript` manages script data.
- `LLanguageStaffEnsign` manages language flags.

## `internal static LLanguageStaff LLanguageStaffBuild(LRig rig, LLanguageCache cache, object gate, Action<LSubject, long> raise, Func<LSettings> settings, LClaimStaff claim)`

Builds the language clerks over one rig after the claim group.
Fetch clerks receive the gate and the bulletin raiser supplied by the engine.
The frequency and lacuna fetches also receive the settings reader.
The recording, reflex and lacuna clerks read the claim clerk, so they see which drafts this engine holds.

## `internal void LLanguageStaffApply()`

Derives the 音韻地位 categories again from the stored fanqie rows and the hypothesis on disk.
Then it builds again the phonetic series of every pack that declares a series source.
A bound workspace needs both before the first read.

## `internal void LLanguageStaffClear()`

Cancels every pending fetch of the six fetching clerks, on a rig apply and on dispose.
The reflex fetch is reached through the reflex clerk that holds it.

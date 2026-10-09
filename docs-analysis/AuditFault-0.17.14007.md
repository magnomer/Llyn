# Fault audit 0.17.14007

- Generation: 21
- Llyn.Application: 10, ceiling 10
- Llyn.ShellEngine: 19, ceiling 19
- Exempt: 2
- Above ceiling: 0
- Stale ceilings: 0
- Stale exempt rows: 0

A catch clause below Conduct swallows its fault unless its block throws, carries the caught variable out, or invokes a delegate or an event. A catch of a cancellation is exempt by shape.

## Llyn.Application

- `src/Llyn.Application/Pronunciation/Clerk/LFanqieClerk.cs:347` LFanqieClerk.LFanqieClerkRun catch (Exception) swallows the fault
- `src/Llyn.Application/Pronunciation/Clerk/LFrequencyClerk.cs:279` LFrequencyClerk.LBandMatch catch (RegexMatchTimeoutException) swallows the fault
- `src/Llyn.Application/Pronunciation/Clerk/LFrequencyClerk.cs:328` LFrequencyClerk.LFrequencyClerkRun catch (Exception) swallows the fault
- `src/Llyn.Application/Pronunciation/Clerk/LReflexClerkEpithet.cs:73` LReflexClerkEpithet.LReflexEpithetFormat catch (Exception) swallows the fault
- `src/Llyn.Application/Pronunciation/Clerk/LReflexFetch.cs:231` LReflexFetch.LReflexFetchRun catch (Exception) swallows the fault
- `src/Llyn.Application/Pronunciation/Clerk/LScriptClerk.cs:266` LScriptClerk.LScriptClerkRun catch (Exception) swallows the fault
- `src/Llyn.Application/Pronunciation/Clerk/LShengfuClerk.cs:251` LShengfuClerk.LShengfuClerkRun catch (Exception) swallows the fault
- `src/Llyn.Application/Vocabulary/LLacunaClerk.cs:270` LLacunaClerk.LLacunaClerkRun catch (Exception) swallows the fault
- `src/Llyn.Application/Workspace/LTrailClerk.cs:91` LTrailClerk.LTrailFileResolve catch (Exception) swallows the fault
- `src/Llyn.Application/Workspace/LWorkspaceClerk.cs:205` LWorkspaceClerk.LWorkspaceClerkUpdate catch (Exception) swallows the fault

## Llyn.ShellEngine

- `src/Llyn.ShellEngine/Tenure/LErrand.cs:42` LErrand.LErrandRecordingStart catch (Exception) swallows the fault
- `src/Llyn.ShellEngine/Tenure/LErrand.cs:86` LErrand.LErrandTranscriptionStart catch (Exception) swallows the fault
- `src/Llyn.ShellEngine/Tenure/LForay.cs:186` LForay.LForayRun catch (Exception) swallows the fault
- `src/Llyn.ShellEngine/Tenure/LQuillChip.cs:110` LQuillChip.LQuillReferenceFind catch (Exception) swallows the fault
- `src/Llyn.ShellEngine/Tenure/LQuillChip.cs:224` LQuillChip.LQuillTranslationRead catch (Exception) swallows the fault
- `src/Llyn.ShellEngine/Tenure/LQuillChip.cs:290` LQuillChip.LQuillTagFind catch (Exception) swallows the fault
- `src/Llyn.ShellEngine/Tenure/LQuillChip.cs:302` LQuillChip.LQuillSituationFind catch (Exception) swallows the fault
- `src/Llyn.ShellEngine/Tenure/LQuillChip.cs:314` LQuillChip.LQuillRegisterFind catch (Exception) swallows the fault
- `src/Llyn.ShellEngine/Tenure/LQuillChip.cs:331` LQuillChip.LQuillProspectFind catch (Exception) swallows the fault
- `src/Llyn.ShellEngine/Tenure/LQuillEtymology.cs:54` LQuillEtymology.LQuillEtymonRead catch (Exception) swallows the fault
- `src/Llyn.ShellEngine/Tenure/LQuillReflex.cs:147` LQuillReflex.LQuillAnchorScan catch (Exception) swallows the fault
- `src/Llyn.ShellEngine/Tenure/LQuillSpeech.cs:88` LQuillSpeech.LQuillSpeechFind catch (Exception) swallows the fault
- `src/Llyn.ShellEngine/Tenure/LQuillSpeech.cs:102` LQuillSpeech.LQuillSpeechCreate catch (Exception) swallows the fault
- `src/Llyn.ShellEngine/Tenure/LTenure.cs:153` LTenure.LTenureCancel catch (Exception) swallows the fault
- `src/Llyn.ShellEngine/Tenure/LTenure.cs:201` LTenure.LTenureFinish catch (Exception) swallows the fault
- `src/Llyn.ShellEngine/Tenure/LTenure.cs:301` LTenure.LTenureStoredRead catch (Exception) swallows the fault
- `src/Llyn.ShellEngine/Tenure/LTenureHerald.cs:120` LTenureHerald.LTenureKeptRead catch (Exception) swallows the fault
- `src/Llyn.ShellEngine/Tenure/LTenureQueue.cs:111` LTenureQueue.LTenureQueueApply catch (Exception) swallows the fault
- `src/Llyn.ShellEngine/Vista/LAuthorFacade.cs:213` LAuthorFacade.LEngineBylineFind catch (Exception) swallows the fault

## Exempt

- `src/Llyn.Application/Workspace/LWorkspaceClerk.cs:79` LWorkspaceClerk.LWorkspaceFallbackSave catch (LVaultFault) swallows the fault
- `src/Llyn.Application/Workspace/LWorkspaceClerk.cs:167` LWorkspaceClerk.LWorkspacePostureRead catch (LVaultFault) swallows the fault

## Above ceiling

## Stale ceilings

## Stale exempt rows

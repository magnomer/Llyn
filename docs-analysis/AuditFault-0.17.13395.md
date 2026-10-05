# Fault audit 0.17.13395

- Generation: 21
- Llyn.Application: 10, ceiling 10
- Llyn.ShellEngine: 19, ceiling 19
- Exempt: 2
- Above ceiling: 0
- Stale ceilings: 0
- Stale exempt rows: 0

A catch clause below Conduct swallows its fault unless its block throws, carries the caught variable out, or invokes a delegate or an event. A catch of a cancellation is exempt by shape.

## Llyn.Application

- `src/Llyn.Application/Pronunciation/Clerk/LFanqieClerk.cs:346` LFanqieClerk.LFanqieClerkRun catch (Exception) swallows the fault
- `src/Llyn.Application/Pronunciation/Clerk/LFrequencyClerk.cs:276` LFrequencyClerk.LBandMatch catch (RegexMatchTimeoutException) swallows the fault
- `src/Llyn.Application/Pronunciation/Clerk/LFrequencyClerk.cs:325` LFrequencyClerk.LFrequencyClerkRun catch (Exception) swallows the fault
- `src/Llyn.Application/Pronunciation/Clerk/LReflexClerkEpithet.cs:75` LReflexClerkEpithet.LReflexEpithetFormat catch (Exception) swallows the fault
- `src/Llyn.Application/Pronunciation/Clerk/LReflexFetch.cs:231` LReflexFetch.LReflexFetchRun catch (Exception) swallows the fault
- `src/Llyn.Application/Pronunciation/Clerk/LScriptClerk.cs:264` LScriptClerk.LScriptClerkRun catch (Exception) swallows the fault
- `src/Llyn.Application/Pronunciation/Clerk/LShengfuClerk.cs:251` LShengfuClerk.LShengfuClerkRun catch (Exception) swallows the fault
- `src/Llyn.Application/Vocabulary/LLacunaClerk.cs:270` LLacunaClerk.LLacunaClerkRun catch (Exception) swallows the fault
- `src/Llyn.Application/Workspace/LTrailClerk.cs:91` LTrailClerk.LTrailFileResolve catch (Exception) swallows the fault
- `src/Llyn.Application/Workspace/LWorkspaceClerk.cs:200` LWorkspaceClerk.LWorkspaceClerkUpdate catch (Exception) swallows the fault

## Llyn.ShellEngine

- `src/Llyn.ShellEngine/Tenure/LForay.cs:176` LForay.LForayRun catch (Exception) swallows the fault
- `src/Llyn.ShellEngine/Tenure/LQuillChip.cs:106` LQuillChip.LQuillReferenceFind catch (Exception) swallows the fault
- `src/Llyn.ShellEngine/Tenure/LQuillChip.cs:265` LQuillChip.LQuillTagFind catch (Exception) swallows the fault
- `src/Llyn.ShellEngine/Tenure/LQuillChip.cs:277` LQuillChip.LQuillSituationFind catch (Exception) swallows the fault
- `src/Llyn.ShellEngine/Tenure/LQuillChip.cs:289` LQuillChip.LQuillRegisterFind catch (Exception) swallows the fault
- `src/Llyn.ShellEngine/Tenure/LQuillChip.cs:306` LQuillChip.LQuillProspectFind catch (Exception) swallows the fault
- `src/Llyn.ShellEngine/Tenure/LTenure.cs:233` LTenure.LTenureCancel catch (Exception) swallows the fault
- `src/Llyn.ShellEngine/Tenure/LTenure.cs:281` LTenure.LTenureFinish catch (Exception) swallows the fault
- `src/Llyn.ShellEngine/Tenure/LTenureForay.cs:25` LTenure.LTenureRecordingStart catch (Exception) swallows the fault
- `src/Llyn.ShellEngine/Tenure/LTenureForay.cs:69` LTenure.LTenureTranscriptionStart catch (Exception) swallows the fault
- `src/Llyn.ShellEngine/Tenure/LTenureLanguage.cs:70` LTenure.LTenureAnchorScan catch (Exception) swallows the fault
- `src/Llyn.ShellEngine/Tenure/LTenureLanguage.cs:197` LTenure.LTenureEtymonRead catch (Exception) swallows the fault
- `src/Llyn.ShellEngine/Tenure/LTenureLanguage.cs:214` LTenure.LTenureTranslationRead catch (Exception) swallows the fault
- `src/Llyn.ShellEngine/Tenure/LTenureLanguage.cs:310` LTenure.LTenureSpeechFind catch (Exception) swallows the fault
- `src/Llyn.ShellEngine/Tenure/LTenureLanguage.cs:324` LTenure.LTenureSpeechCreate catch (Exception) swallows the fault
- `src/Llyn.ShellEngine/Tenure/LTenureObserver.cs:190` LTenure.LTenureStoredRead catch (Exception) swallows the fault
- `src/Llyn.ShellEngine/Tenure/LTenureObserver.cs:219` LTenure.LTenureKeptRead catch (Exception) swallows the fault
- `src/Llyn.ShellEngine/Tenure/LTenureQueue.cs:111` LTenureQueue.LTenureQueueApply catch (Exception) swallows the fault
- `src/Llyn.ShellEngine/Vista/LAuthorFacade.cs:195` LAuthorFacade.LEngineBylineFind catch (Exception) swallows the fault

## Exempt

- `src/Llyn.Application/Workspace/LWorkspaceClerk.cs:79` LWorkspaceClerk.LWorkspaceFallbackSave catch (LVaultFault) swallows the fault
- `src/Llyn.Application/Workspace/LWorkspaceClerk.cs:162` LWorkspaceClerk.LWorkspacePostureRead catch (LVaultFault) swallows the fault

## Above ceiling

## Stale ceilings

## Stale exempt rows

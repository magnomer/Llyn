# LRig.cs
Hash: `b958a91e698f5736`

## `public sealed record LRig(LVault LRigVault, LDoctorVault LRigDoctor, LSettingsVault LRigSettings, LAuditVault LRigAudit, LPostureVault LRigPosture, LEntryVault LRigEntries, LEtymologyVault LRigEtymologies, LDraftVault LRigDrafts, LClaimVault LRigClaims, LCourtVault LRigCourts, LRevisionVault LRigRevisions, LWorkspaceVault LRigWorkspaces, LTombstoneVault LRigTombstones, LAuthorVault LRigAuthors, LCollocationVault LRigCollocations, LDiweiVault LRigDiwei, LExampleVault LRigExamples, LFanqieVault LRigFanqie, LShengfuVault LRigShengfu, LStemVault LRigStems, LFavoriteVault LRigFavorites, LFrequencyVault LRigFrequencies, LGlossVault LRigGlosses, LImageVault LRigImages, LInflectionVault LRigInflections, LLacunaVault LRigLacunae, LMeaningVault LRigMeanings, LMentionVault LRigMentions, LMorphologyVault LRigMorphologies, LNoteVault LRigNotes, LPronunciationVault LRigPronunciations, LReferenceVault LRigReferences, LReflexVault LRigReflexes, LRegisterVault LRigRegisters, LScriptVault LRigScripts, LSentenceVault LRigSentences, LSituationVault LRigSituations, LSpeechVault LRigSpeeches, LTagVault LRigTags, LTranscriptionVault LRigTranscriptions, LTranslationVault LRigTranslations, LVideoVault LRigVideos, LSourceFactory LRigSources, LFanqieSource LRigFanqieSource, LShengfuSource LRigShengfuSource, LReflexSource LRigReflexSource, LScriptSource LRigScriptSource, LRecordingVault LRigRecordings, LLanguageVault LRigLanguages, LLocalizationVault LRigLocalization, LMarkupVault LRigMarkup, LPortraitVault LRigPortrait, LPress LRigPress, LUsher LRigUsher, LPhonograph LRigPhonograph, LTrail LRigTrail, LClock LRigClock, int LRigProcess, string LRigWorkspace)`

The bundle of ports the engine is built over.
It is one record and one constructor argument.
The composition root assembles it through `LRigFactory` in Infrastructure, and a test assembles it from fakes.
The engine copies each port into a field of its own and never learns which adapter stands behind it.
A workspace change hands the engine a whole new rig, so every port swaps at once.
There is no identity port.
The engine builds `LIdentity` over `LRigWorkspaces` itself, since it is a use case.
`LEnsign` is likewise built engine-side over `LRigUsher`, since it is a cache and not an adapter.
`LRigPress` is the printing surface, handed in by the root beside the usher since both are media.
`LRigPhonograph` is the player, handed in by the root beside them for the same reason.
Every property is named `LRig{Base}`, the base being the port's own, so a port and its slot read alike.
The fetch ports follow the port's whole name, since `LRigFanqie` already names the fanqie vault.
`LRigTrail` and `LRigClock` are the two ambient facts the engine may not read itself, the path rules and the time.
`LRigProcess` is the id of the process the rig was built in.
A claim on a draft is compared against it.
`LRigWorkspace` is the root every root-bound adapter above was built over.
It is kept so the engine can say where it stands.

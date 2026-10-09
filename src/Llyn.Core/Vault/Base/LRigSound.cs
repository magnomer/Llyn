namespace Llyn.Core;

public sealed record LRigSound(
    LDiweiVault LRigSoundDiwei,
    LFanqieVault LRigSoundFanqie,
    LShengfuVault LRigSoundShengfu,
    LStemVault LRigSoundStems,
    LFrequencyVault LRigSoundFrequencies,
    LPronunciationVault LRigSoundPronunciations,
    LReflexVault LRigSoundReflexes,
    LScriptVault LRigSoundScripts,
    LTranscriptionVault LRigSoundTranscriptions);

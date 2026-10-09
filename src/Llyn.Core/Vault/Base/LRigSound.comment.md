# LRigSound.cs
Hash: `8226c2ce0a371822`

## `public sealed record LRigSound(LDiweiVault LRigSoundDiwei, LFanqieVault LRigSoundFanqie, LShengfuVault LRigSoundShengfu, LStemVault LRigSoundStems, LFrequencyVault LRigSoundFrequencies, LPronunciationVault LRigSoundPronunciations, LReflexVault LRigSoundReflexes, LScriptVault LRigSoundScripts, LTranscriptionVault LRigSoundTranscriptions)`

The sound group of the rig, holding the ports that describe how a word is pronounced.
It covers diwei, fanqie, shengfu, stems, frequencies, pronunciations, reflexes, scripts and transcriptions.
`LRig` holds it as `LRigSound`, so the rig stays small while each port keeps its own slot.
Every property is named `LRigSound{Base}`, the base being the port's own, as on the rig.
The composition root builds it through `LRigFactorySound`, and a test builds it from fakes.

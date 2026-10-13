namespace Llyn.Core;

public sealed record LRigSource(
    LSourceFactory LRigSourceFactory,
    LFanqieSource LRigSourceFanqie,
    LShengfuSource LRigSourceShengfu,
    LReflexSource LRigSourceReflex,
    LScriptSource LRigSourceScript,
    LRecordingVault LRigSourceRecordings);

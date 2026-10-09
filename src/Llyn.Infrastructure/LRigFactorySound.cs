using Llyn.Core;

namespace Llyn.Infrastructure;

public static class LRigFactorySound
{
    public static LRigSound LRigSoundBuild(LDatabase database) =>
        new(
            new LDiweiArchive(database),
            new LFanqieArchive(database),
            new LShengfuArchive(database),
            new LStemArchive(database),
            new LFrequencyArchive(database),
            new LPronunciationArchive(database),
            new LReflexArchive(database),
            new LScriptArchive(database),
            new LTranscriptionArchive(database));
}

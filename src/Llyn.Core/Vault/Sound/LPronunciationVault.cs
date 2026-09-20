using System.Collections.Generic;

namespace Llyn.Core;

public interface LPronunciationVault
{
    LPronunciation LPronunciationCreate(LPronunciation pronunciation);

    IReadOnlyList<LPronunciation> LPronunciationRead(long entryId);

    void LPronunciationUpdate(LPronunciation pronunciation);

    void LPronunciationOrderSet(long entryId, IReadOnlyList<long> order);

    void LPronunciationDelete(long id);

    void LPronunciationAudioSave(long pronunciationId, string file, string? source);

    LPronunciationAudio? LPronunciationAudioRead(long pronunciationId);

    IReadOnlyList<string> LPronunciationAudioScan();

    long? LPronunciationHolderRead(long id);
}

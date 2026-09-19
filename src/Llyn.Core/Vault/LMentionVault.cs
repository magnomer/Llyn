using System.Collections.Generic;

namespace Llyn.Core;

public interface LMentionVault
{
    IReadOnlyList<LMention> LMentionExampleRead(long exampleId);

    IReadOnlyList<long> LMentionExampleSave(long exampleId, IReadOnlyList<LMention> mentions);

    IReadOnlyList<long> LMentionSenseClear(long meaningId);

    bool LMentionSenseSet(long mentionId, long senseId);
}

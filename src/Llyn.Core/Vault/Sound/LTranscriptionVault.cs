using System.Collections.Generic;

namespace Llyn.Core;

public interface LTranscriptionVault
{
    IReadOnlyList<LTranscription> LTranscriptionRead(long entryId);

    IReadOnlyList<LTranscription> LTranscriptionSet(long entryId, IReadOnlyList<LTranscription> transcriptions);
}

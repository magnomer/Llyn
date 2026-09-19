using System.Collections.Generic;

namespace Llyn.Core;

public interface LFrequencyVault
{
    IReadOnlyList<LFrequency> LFrequencyRead(long entryId);

    void LFrequencySet(long entryId, IReadOnlyList<LFrequency> rows);

    void LFrequencyClear(long entryId);

    void LFrequencyBandSet(long entryId, string source, string? band);
}

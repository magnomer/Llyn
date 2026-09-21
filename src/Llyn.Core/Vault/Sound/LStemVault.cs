using System.Collections.Generic;

namespace Llyn.Core;

public interface LStemVault
{
    void LStemApply(string language, string character, IReadOnlyList<string> keys);

    void LStemRebuild(string language, string separator);

    IReadOnlyList<LStem> LStemRead(string language);

    LStem? LStemRead(long stemId);

    LStem? LStemFind(string language, string key);

    IReadOnlyList<string> LStemCharacterRead(long stemId);

    IReadOnlyList<long> LStemEntryScan(string language, IReadOnlyList<long> stemIds);
}

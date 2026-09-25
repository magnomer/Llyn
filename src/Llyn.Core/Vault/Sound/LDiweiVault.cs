using System.Collections.Generic;

namespace Llyn.Core;

public interface LDiweiVault
{
    void LDiweiApply(string language, string character, LHypothesis? hypothesis);

    void LDiweiRebuild(string language, LHypothesis? hypothesis);

    IReadOnlyList<LDiwei> LDiweiRead(string language, string kind);

    LDiwei? LDiweiRead(long diweiId);

    LDiwei? LDiweiFind(string language, string kind, string key);

    IReadOnlyList<long> LDiweiEntryScan(string language, IReadOnlyList<long> diweiIds);

    IReadOnlyList<LFanqieRow> LDiweiFanqieRead(long diweiId);
}

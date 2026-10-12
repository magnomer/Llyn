using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LCollocationLayout(
    IReadOnlyDictionary<long, IReadOnlyList<LCardDraft>> LCollocationLayoutHome,
    IReadOnlyDictionary<long, IReadOnlyList<(long LCollocationId, LStateValue LCollocationPhrase, string LMeaningPath)>>
        LCollocationLayoutStub,
    IReadOnlyList<LCardDraft> LCollocationLayoutOther,
    IReadOnlyDictionary<long, LStateValue> LCollocationLayoutPhrase,
    IReadOnlyDictionary<long, IReadOnlyList<string>> LCollocationLayoutFlag,
    IReadOnlyDictionary<long, int> LCollocationLayoutExample,
    IReadOnlyDictionary<long, IReadOnlyList<string>> LCollocationLayoutAlso);

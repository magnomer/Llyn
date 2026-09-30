using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CLecternCard(
    IReadOnlyList<CLeaf> CLecternCardMeanings,
    IReadOnlyList<CLeaf> CLecternCardCollocations,
    bool CLecternCardDefined,
    bool CLecternCardCollocated);

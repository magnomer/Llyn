using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CLecternCard(
    CSentenceOrder CLecternCardOrder,
    IReadOnlyDictionary<long, string> CLecternCardCitations,
    IReadOnlyList<CTranslationTarget> CLecternCardTargets,
    bool CLecternCardDefined,
    bool CLecternCardCollocated);

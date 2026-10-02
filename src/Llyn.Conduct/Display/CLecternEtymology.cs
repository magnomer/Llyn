using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CLecternEtymology(
    string CLecternEtymologyText,
    IReadOnlyList<CTranslationTarget> CLecternEtymologyTargets,
    bool CLecternEtymologyShown,
    bool CLecternEtymologyNarrated,
    bool CLecternEtymologyLinked,
    bool CLecternEtymologyDerived);

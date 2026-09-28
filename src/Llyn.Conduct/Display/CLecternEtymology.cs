using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CLecternEtymology(
    string CLecternEtymologyLanguage,
    string CLecternEtymologyText,
    IReadOnlyList<CTranslationTarget> CLecternEtymologyTargets,
    bool CLecternEtymologyShown,
    bool CLecternEtymologyDerived);

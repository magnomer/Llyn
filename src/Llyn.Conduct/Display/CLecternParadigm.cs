using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CLecternParadigm(
    IReadOnlyList<CParadigmSlot> CLecternParadigmSlots,
    bool CLecternParadigmPending,
    bool CLecternParadigmMorphology,
    CFont CLecternParadigmFont);

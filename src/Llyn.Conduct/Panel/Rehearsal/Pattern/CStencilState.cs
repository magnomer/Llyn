using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CStencilState(
    IReadOnlyList<CStencilSlot> CStencilStateSlots,
    IReadOnlyList<CStencilRow> CStencilStateSaved,
    IReadOnlyList<CStencilSide> CStencilStateSides,
    IReadOnlyList<CStencilField> CStencilStateFields,
    IReadOnlyList<CStencilJoin> CStencilStateJoins,
    int CStencilStatePicture,
    int CStencilStateSound,
    string CStencilStateGrain);

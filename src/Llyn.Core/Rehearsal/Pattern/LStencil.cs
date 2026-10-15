using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LStencil(
    long LStencilId,
    string LStencilName,
    bool LStencilSaved,
    IReadOnlyList<LStencilSlot> LStencilSlots);

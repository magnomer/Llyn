namespace Llyn.Conduct;

public sealed record CStencilSlot(
    int CStencilSlotColumn,
    int CStencilSlotIndex,
    string CStencilSlotBody,
    string CStencilSlotCaption,
    string CStencilSlotNumber);

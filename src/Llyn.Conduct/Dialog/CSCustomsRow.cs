namespace Llyn.Conduct;

public sealed record CSCustomsRow(
    CSCustomsMode CSCustomsRowMode,
    long CSCustomsRowTarget,
    bool CSCustomsRowTargeted,
    long CSCustomsRowLoss);

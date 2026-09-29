namespace Llyn.Conduct;

public sealed record CSCustomsRow(
    CSCustomsMode CSCustomsRowMode,
    long CSCustomsRowTarget,
    bool CSCustomsRowTargeted,
    string? CSCustomsRowLoss,
    int CSCustomsRowMeaning,
    int CSCustomsRowCollocation);

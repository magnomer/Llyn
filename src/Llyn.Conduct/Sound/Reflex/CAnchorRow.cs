namespace Llyn.Conduct;

public sealed record CAnchorRow(
    long CAnchorRowId,
    string CAnchorRowSummary,
    bool CAnchorRowHeld,
    bool CAnchorRowEstimated);

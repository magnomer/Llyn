namespace Llyn.Conduct;

public sealed record CCueRow(
    long CCueRowId,
    int CCueRowPosition,
    bool CCueRowFlag,
    bool CCueRowExcluded,
    bool CCueRowOrphaned,
    string CCueRowLine);

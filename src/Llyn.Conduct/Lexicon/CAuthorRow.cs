namespace Llyn.Conduct;

public sealed record CAuthorRow(
    long CAuthorRowId,
    string CAuthorRowName,
    int CAuthorRowPosition,
    bool CAuthorRowEarlier,
    bool CAuthorRowLater);

namespace Llyn.Conduct;

public sealed record CStudyRow(
    long CStudyRowId,
    long? CStudyRowFolder,
    string CStudyRowName,
    int CStudyRowActive,
    bool CStudyRowChosen);

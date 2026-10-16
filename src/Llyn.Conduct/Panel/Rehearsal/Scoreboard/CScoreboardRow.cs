namespace Llyn.Conduct;

public sealed record CScoreboardRow(
    long? CScoreboardRowId,
    long? CScoreboardRowFolder,
    string CScoreboardRowName,
    bool CScoreboardRowChosen);

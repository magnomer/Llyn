namespace Llyn.Conduct;

public sealed record CCueCell(
    int CCueCellColumn,
    int CCueCellIndex,
    string CCueCellValue,
    string CCueCellCaption,
    string CCueCellNumber,
    bool CCueCellFile);

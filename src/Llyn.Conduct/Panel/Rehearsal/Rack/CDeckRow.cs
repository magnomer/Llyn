namespace Llyn.Conduct;

public sealed record CDeckRow(
    long CDeckRowId,
    long? CDeckRowFolder,
    string CDeckRowName,
    bool CDeckRowChosen,
    int CDeckRowCount,
    string CDeckRowKey);

namespace Llyn.Conduct;

public sealed record CDeckRow(
    long CDeckRowId,
    long? CDeckRowFolder,
    string CDeckRowName,
    int CDeckRowGrain,
    bool CDeckRowChosen);

namespace Llyn.Core;

public sealed record LAncestorTongue(
    long LAncestorTongueId,
    string LAncestorTongueName,
    int LAncestorTongueUsage = 0)
{
    public string LAncestorTongueName { get; init; } = LAncestorTongueName ?? string.Empty;
}

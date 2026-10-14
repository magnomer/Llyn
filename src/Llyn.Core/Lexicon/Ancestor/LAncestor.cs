namespace Llyn.Core;

public sealed record LAncestor(
    long LAncestorId,
    long? LAncestorChildId,
    int LAncestorPosition,
    long? LAncestorTargetId,
    string LAncestorText,
    string LAncestorLanguage = "",
    long? LAncestorTongueId = null,
    string LAncestorGloss = "",
    long? LAncestorRelationId = null)
{
    public string LAncestorText { get; init; } = LAncestorText ?? string.Empty;

    public string LAncestorLanguage { get; init; } = LAncestorLanguage ?? string.Empty;

    public string LAncestorGloss { get; init; } = LAncestorGloss ?? string.Empty;
}

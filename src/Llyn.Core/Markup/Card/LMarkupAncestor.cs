namespace Llyn.Core;

public sealed record LMarkupAncestor(
    int LMarkupAncestorKey,
    int? LMarkupAncestorChild,
    int LMarkupAncestorPosition,
    string LMarkupAncestorHeadword = "",
    string LMarkupAncestorText = "",
    string LMarkupAncestorLanguage = "",
    string LMarkupAncestorGloss = "",
    string LMarkupAncestorRelation = "")
{
    public string LMarkupAncestorHeadword { get; init; } = LMarkupAncestorHeadword ?? string.Empty;

    public string LMarkupAncestorText { get; init; } = LMarkupAncestorText ?? string.Empty;

    public string LMarkupAncestorLanguage { get; init; } = LMarkupAncestorLanguage ?? string.Empty;

    public string LMarkupAncestorGloss { get; init; } = LMarkupAncestorGloss ?? string.Empty;

    public string LMarkupAncestorRelation { get; init; } = LMarkupAncestorRelation ?? string.Empty;
}

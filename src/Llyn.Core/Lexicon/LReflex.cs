namespace Llyn.Core;

public sealed record LReflex(
    long LReflexId,
    long LReflexEntryId,
    int LReflexPosition,
    string LReflexLanguage,
    string LReflexKind,
    string LReflexText,
    bool LReflexMain = false,
    string LReflexNote = "",
    string LReflexRespelling = "",
    string LReflexRegion = "",
    string LReflexRemark = "")
{
    public string LReflexNote { get; init; } = LReflexNote ?? string.Empty;

    public string LReflexRespelling { get; init; } = LReflexRespelling ?? string.Empty;

    public string LReflexRegion { get; init; } = LReflexRegion ?? string.Empty;

    public string LReflexRemark { get; init; } = LReflexRemark ?? string.Empty;
}

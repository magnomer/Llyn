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
    string LReflexRespelling = "")
{
    public string LReflexNote { get; init; } = LReflexNote ?? string.Empty;

    public string LReflexRespelling { get; init; } = LReflexRespelling ?? string.Empty;
}

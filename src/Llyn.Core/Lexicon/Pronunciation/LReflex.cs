using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LReflex(
    long LReflexId,
    long LReflexEntryId,
    int LReflexPosition,
    string LReflexLanguage,
    string LReflexKind,
    string LReflexText,
    bool LReflexMain = false,
    string LReflexRomanization = "",
    string LReflexMeaning = "",
    bool LReflexOwned = false,
    string LReflexNote = "",
    string LReflexRespelling = "",
    string LReflexRegion = "",
    LAnatomy? LReflexAnatomy = null,
    IReadOnlyList<long>? LReflexAnchors = null)
{
    public string LReflexRomanization { get; init; } = LReflexRomanization ?? string.Empty;

    public string LReflexMeaning { get; init; } = LReflexMeaning ?? string.Empty;

    public string LReflexNote { get; init; } = LReflexNote ?? string.Empty;

    public string LReflexRespelling { get; init; } = LReflexRespelling ?? string.Empty;

    public string LReflexRegion { get; init; } = LReflexRegion ?? string.Empty;

    public LAnatomy LReflexAnatomy { get; init; } = LReflexAnatomy ?? LAnatomy.LAnatomyEmpty;

    public IReadOnlyList<long> LReflexAnchors { get; init; } = LAnchor.LAnchorNormalize(LReflexAnchors);
}

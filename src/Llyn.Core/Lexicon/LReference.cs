namespace Llyn.Core;

public sealed record LReference(
    string LReferenceId,
    LStateValue LReferenceTitle,
    LStateValue LReferenceYear,
    LReferenceKind LReferenceKind,
    LStateValue LReferenceNote,
    LStateValue LReferenceUrl,
    LState LReferenceAuthorState)
{
    public LStateValue LReferenceTitle { get; init; } = LReferenceTitle ?? LStateValue.LStateValueUnspecified;

    public LStateValue LReferenceYear { get; init; } = LReferenceYear ?? LStateValue.LStateValueUnspecified;

    public LStateValue LReferenceNote { get; init; } = LReferenceNote ?? LStateValue.LStateValueUnspecified;

    public LStateValue LReferenceUrl { get; init; } = LReferenceUrl ?? LStateValue.LStateValueUnspecified;

    public string LReferenceNameRead()
    {
        return LReferenceTextRead(LReferenceTitle)
            ?? LReferenceTextRead(LReferenceUrl)
            ?? LReferenceId;
    }

    public static string LReferenceKindFormat(LReferenceKind kind)
    {
        return kind switch
        {
            LReferenceKind.LReferenceKindUnknown => "unknown",
            LReferenceKind.LReferenceKindBook => "book",
            LReferenceKind.LReferenceKindJournal => "journal",
            LReferenceKind.LReferenceKindArticle => "article",
            LReferenceKind.LReferenceKindWeb => "web",
            LReferenceKind.LReferenceKindVideo => "video",
            LReferenceKind.LReferenceKindAudio => "audio",
            LReferenceKind.LReferenceKindPicture => "picture",
            LReferenceKind.LReferenceKindOther => "other",
            _ => "unspecified",
        };
    }

    public static LStateValue LReferenceKindShow(LReferenceKind kind)
    {
        return kind switch
        {
            LReferenceKind.LReferenceKindUnspecified => LStateValue.LStateValueUnspecified,
            LReferenceKind.LReferenceKindUnknown => LStateValue.LStateValueUnknown,
            _ => LStateValue.LStateValueCreate(LReferenceKindFormat(kind)),
        };
    }

    public static LReferenceKind LReferenceKindParse(string? text)
    {
        return text switch
        {
            "unknown" => LReferenceKind.LReferenceKindUnknown,
            "book" => LReferenceKind.LReferenceKindBook,
            "journal" => LReferenceKind.LReferenceKindJournal,
            "article" => LReferenceKind.LReferenceKindArticle,
            "web" => LReferenceKind.LReferenceKindWeb,
            "video" => LReferenceKind.LReferenceKindVideo,
            "audio" => LReferenceKind.LReferenceKindAudio,
            "picture" => LReferenceKind.LReferenceKindPicture,
            "other" => LReferenceKind.LReferenceKindOther,
            _ => LReferenceKind.LReferenceKindUnspecified,
        };
    }

    private static string? LReferenceTextRead(LStateValue value)
    {
        return value.LStateValueState == LState.LStateSpecified
            && !string.IsNullOrWhiteSpace(value.LStateValueText)
                ? value.LStateValueText
                : null;
    }
}

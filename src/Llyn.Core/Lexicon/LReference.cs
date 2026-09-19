using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Llyn.Core;

public sealed record LReference(
    long LReferenceId,
    LStateValue LReferenceTitle,
    LStateValue LReferenceYear,
    LReferenceKind LReferenceKind,
    LStateValue LReferenceNote,
    LStateValue LReferenceUrl,
    LStateMark LReferenceAuthorState)
{
    public LStateValue LReferenceTitle { get; init; } = LReferenceTitle ?? LStateValue.LStateValueUnspecified;

    public LStateValue LReferenceYear { get; init; } = LReferenceYear ?? LStateValue.LStateValueUnspecified;

    public LStateValue LReferenceNote { get; init; } = LReferenceNote ?? LStateValue.LStateValueUnspecified;

    public LStateValue LReferenceUrl { get; init; } = LReferenceUrl ?? LStateValue.LStateValueUnspecified;

    public LStateMark LReferenceAuthorState { get; init; } = LReferenceAuthorState ?? LStateMark.LStateMarkUnspecified;

    public string LReferenceKindKey => LReferenceKindResolve(LReferenceKind);

    public string LReferenceKindTag => LReferenceKindFormat(LReferenceKind);

    public string LReferenceTitleHint => LReferenceHintRead(LReferenceTitle, "Source.Untitled");

    public string LReferenceYearHint => LReferenceHintRead(LReferenceYear, "Source.Year");

    public string LReferenceUrlHint => LReferenceHintRead(LReferenceUrl, "Source.Url");

    public string LReferenceNoteHint => LReferenceHintRead(LReferenceNote, "Source.Note");

    private static string LReferenceHintRead(LStateValue value, string key)
    {
        return value.LStateValueUncertain ? "Display.Unknown" : key;
    }

    public LReference LReferenceNormalize()
    {
        return this with
        {
            LReferenceTitle = LReferenceTitle.LStateValueNormalize(),
            LReferenceYear = LReferenceYear.LStateValueNormalize(),
            LReferenceNote = LReferenceNote.LStateValueNormalize(),
            LReferenceUrl = LReferenceUrl.LStateValueNormalize(),
            LReferenceAuthorState = LReferenceAuthorState.LStateMarkNormalize(),
        };
    }

    public bool LReferenceKindMatch(LReferenceKind kind)
    {
        return LReferenceKind == kind;
    }

    public bool LReferenceKindCheck()
    {
        return LReferenceKind != LReferenceKind.LReferenceKindUnspecified;
    }

    public string LReferenceNameRead()
    {
        return LReferenceTextRead(LReferenceTitle)
            ?? LReferenceTextRead(LReferenceUrl)
            ?? LReferenceId.ToString(CultureInfo.InvariantCulture);
    }

    public string? LReferenceCreditRead(IReadOnlyList<LAuthor> credits)
    {
        ArgumentNullException.ThrowIfNull(credits);

        return credits.Count == 0 ? null : string.Join(", ", credits.Select(static author => author.LAuthorName));
    }

    public bool LReferenceAuthorCheck(IReadOnlyList<LAuthor> credits)
    {
        ArgumentNullException.ThrowIfNull(credits);

        return credits.Count > 0 || LReferenceAuthorState.LStateMarkUncertain;
    }

    public string LReferenceBylineRead(IReadOnlyList<LAuthor> credits)
    {
        ArgumentNullException.ThrowIfNull(credits);

        List<string> names = [];
        foreach (LAuthor author in credits)
        {
            string name = author.LAuthorName.Trim();
            if (name.Length > 0)
            {
                names.Add(name);
            }
        }

        string head = names.Count > 0 ? string.Join(", ", names) : LReferenceNameRead();
        string? year = LReferenceTextRead(LReferenceYear);
        return year is null ? head : $"{head} ({year})";
    }

    public LColophon LReferenceColophonRead(
        IReadOnlyList<LAuthor> credits,
        string tally,
        Func<string, string> localize)
    {
        return LColophon.LColophonCreate(this, credits, tally, localize);
    }

    public static string LReferenceUsageFormat(int count, Func<string, string> localize)
    {
        ArgumentNullException.ThrowIfNull(localize);

        return count switch
        {
            0 => localize("Source.UsageNone"),
            1 => localize("Source.UsageOne"),
            _ => $"{count.ToString(CultureInfo.CurrentCulture)} {localize("Source.UsageMany")}",
        };
    }

    public static string LReferenceKindResolve(LReferenceKind kind)
    {
        return kind switch
        {
            LReferenceKind.LReferenceKindUnknown => "Source.KindUnknown",
            LReferenceKind.LReferenceKindBook => "Source.KindBook",
            LReferenceKind.LReferenceKindJournal => "Source.KindJournal",
            LReferenceKind.LReferenceKindArticle => "Source.KindArticle",
            LReferenceKind.LReferenceKindWeb => "Source.KindWeb",
            LReferenceKind.LReferenceKindVideo => "Source.KindVideo",
            LReferenceKind.LReferenceKindAudio => "Source.KindAudio",
            LReferenceKind.LReferenceKindPicture => "Source.KindPicture",
            LReferenceKind.LReferenceKindOther => "Source.KindOther",
            _ => "Source.KindUnspecified",
        };
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

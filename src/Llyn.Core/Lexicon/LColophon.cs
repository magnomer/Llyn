using System;
using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LColophon(
    string LColophonTitle,
    bool LColophonTitleFaint,
    string LColophonKind,
    bool LColophonKindShown,
    string LColophonYear,
    bool LColophonYearFaint,
    bool LColophonYearShown,
    string LColophonUrl,
    bool LColophonUrlFaint,
    bool LColophonUrlShown,
    string LColophonNote,
    bool LColophonNoteFaint,
    bool LColophonNoteShown,
    string LColophonAuthor,
    bool LColophonAuthorFaint,
    bool LColophonAuthorShown,
    string LColophonTally)
{
    public static LColophon LColophonCreate(
        LReference reference,
        IReadOnlyList<LAuthor> credits,
        string tally,
        Func<string, string> localize)
    {
        ArgumentNullException.ThrowIfNull(reference);
        ArgumentNullException.ThrowIfNull(credits);
        ArgumentNullException.ThrowIfNull(tally);
        ArgumentNullException.ThrowIfNull(localize);

        string unknown = localize("Display.Unknown");
        string? credit = reference.LReferenceCreditRead(credits);
        bool authorShown = reference.LReferenceAuthorCheck(credits);

        return new LColophon(
            LColophonTextRead(reference.LReferenceTitle, unknown) ?? localize("Source.Untitled"),
            reference.LReferenceTitle.LStateValueShown is null || reference.LReferenceTitle.LStateValueUncertain,
            reference.LReferenceKindCheck()
                ? localize(LReference.LReferenceKindResolve(reference.LReferenceKind))
                : string.Empty,
            reference.LReferenceKindCheck(),
            LColophonTextRead(reference.LReferenceYear, unknown) ?? string.Empty,
            reference.LReferenceYear.LStateValueUncertain,
            LColophonTextRead(reference.LReferenceYear, unknown) is not null,
            LColophonTextRead(reference.LReferenceUrl, unknown) ?? string.Empty,
            reference.LReferenceUrl.LStateValueUncertain,
            LColophonTextRead(reference.LReferenceUrl, unknown) is not null,
            LColophonTextRead(reference.LReferenceNote, unknown) ?? string.Empty,
            reference.LReferenceNote.LStateValueUncertain,
            LColophonTextRead(reference.LReferenceNote, unknown) is not null,
            authorShown
                ? credit ?? (reference.LReferenceAuthorState.LStateMarkUncertain ? unknown : string.Empty)
                : string.Empty,
            credit is null && reference.LReferenceAuthorState.LStateMarkUncertain,
            authorShown,
            tally);
    }

    private static string? LColophonTextRead(LStateValue value, string unknown)
    {
        return value.LStateValueUncertain ? unknown : value.LStateValueShown;
    }
}

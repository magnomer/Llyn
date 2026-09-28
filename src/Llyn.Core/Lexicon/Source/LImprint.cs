using System;

namespace Llyn.Core;

public sealed record LImprint(
    string LImprintTitle,
    string LImprintTitleHint,
    string LImprintYear,
    string LImprintYearHint,
    string LImprintUrl,
    string LImprintUrlHint,
    string LImprintNote,
    string LImprintNoteHint,
    string LImprintKindKey,
    string LImprintKindTag)
{
    public static LImprint LImprintCreate(LReference reference)
    {
        ArgumentNullException.ThrowIfNull(reference);

        return new LImprint(
            reference.LReferenceTitle.LStateValueShow(),
            reference.LReferenceTitleHint,
            reference.LReferenceYear.LStateValueShow(),
            reference.LReferenceYearHint,
            reference.LReferenceUrl.LStateValueShow(),
            reference.LReferenceUrlHint,
            reference.LReferenceNote.LStateValueShow(),
            reference.LReferenceNoteHint,
            reference.LReferenceKindKey,
            reference.LReferenceKindTag);
    }
}

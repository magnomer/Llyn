namespace Llyn.Conduct;

public sealed record CReference(
    string CReferenceTitle,
    string CReferenceTitleHint,
    string CReferenceYear,
    string CReferenceYearHint,
    string CReferenceUrl,
    string CReferenceUrlHint,
    string CReferenceNote,
    string CReferenceNoteHint,
    string CReferenceKindKey,
    string CReferenceKindTag);

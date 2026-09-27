namespace Llyn.Conduct;

public sealed record CColophon(
    string CColophonTitle,
    bool CColophonTitleFaint,
    string CColophonKind,
    bool CColophonKindShown,
    string CColophonYear,
    bool CColophonYearFaint,
    bool CColophonYearShown,
    string CColophonUrl,
    bool CColophonUrlFaint,
    bool CColophonUrlShown,
    string CColophonNote,
    bool CColophonNoteFaint,
    bool CColophonNoteShown,
    string CColophonAuthor,
    bool CColophonAuthorFaint,
    bool CColophonAuthorShown,
    string CColophonTally);

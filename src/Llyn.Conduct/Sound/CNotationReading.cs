namespace Llyn.Conduct;

public sealed record CNotationReading(
    string CNotationReadingPhonetic,
    string CNotationReadingText,
    CVariety CNotationReadingVariety,
    bool CNotationReadingFlagged,
    CRespellingMark CNotationReadingMark);

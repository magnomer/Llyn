namespace Llyn.Core;

public sealed record LFanqieRow(
    string LFanqieRowCharacter,
    string LFanqieRowBook,
    int LFanqieRowPosition,
    string LFanqieRowText,
    string LFanqieRowInitial = "",
    string LFanqieRowRime = "",
    string LFanqieRowHeading = "",
    string LFanqieRowDivision = "",
    string LFanqieRowTone = "",
    bool LFanqieRowRounded = false,
    string? LFanqieRowSource = null,
    string LFanqieRowSpelling = "",
    string LFanqieRowReading = "",
    string LFanqieRowClass = "",
    long LFanqieRowId = 0)
{
    public string LFanqieRowSource { get; init; } = LFanqieRowSource ?? LFanqieRowBook;

    public string LFanqieRowReading { get; init; } = LFanqieRowReading ?? string.Empty;

    public string LFanqieRowClass { get; init; } = LFanqieRowClass ?? string.Empty;
}

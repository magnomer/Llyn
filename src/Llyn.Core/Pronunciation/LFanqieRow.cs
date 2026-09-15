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
    string LFanqieRowSpelling = "")
{
    public string LFanqieRowSource { get; init; } = LFanqieRowSource ?? LFanqieRowBook;
}

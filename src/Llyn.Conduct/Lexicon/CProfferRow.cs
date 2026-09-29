namespace Llyn.Conduct;

public sealed record CProfferRow(
    long CProfferRowId,
    string CProfferRowLead,
    string CProfferRowMark,
    string CProfferRowTail,
    string CProfferRowCount = "");

namespace Llyn.Conduct;

public sealed record CCompassRow(
    CCompassPart CCompassRowPart,
    int? CCompassRowCard,
    string CCompassRowName,
    string CCompassRowNumber,
    int CCompassRowDepth);

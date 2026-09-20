namespace Llyn.Core;

public sealed record LMarkupOmission(
    int LMarkupOmissionLine,
    string LMarkupOmissionText)
{
    public string LMarkupOmissionText { get; init; } = LMarkupOmissionText ?? string.Empty;
}

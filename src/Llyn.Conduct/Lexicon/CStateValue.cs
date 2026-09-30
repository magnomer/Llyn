namespace Llyn.Conduct;

public sealed record CStateValue(string CStateValueText, bool CStateValueUncertain)
{
    public static CStateValue CStateValueEmpty { get; } = new(string.Empty, false);

    public string? CStateValueShown => CStateValueText.Length > 0 ? CStateValueText : null;
}

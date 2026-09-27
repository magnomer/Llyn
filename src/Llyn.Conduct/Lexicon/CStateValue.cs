namespace Llyn.Conduct;

public sealed record CStateValue(string CStateValueText, bool CStateValueUncertain, bool CStateValueLegible)
{
    public static CStateValue CStateValueEmpty { get; } = new(string.Empty, false, false);

    public string? CStateValueShown => CStateValueText.Length > 0 ? CStateValueText : null;
}

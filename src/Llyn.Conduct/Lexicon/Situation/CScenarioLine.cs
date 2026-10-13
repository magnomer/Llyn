namespace Llyn.Conduct;

public sealed record CScenarioLine(string CScenarioLineText, string CScenarioLineHint)
{
    public bool CScenarioLineVacant => CScenarioLineText.Length == 0;

    public string? CScenarioLineWording => CScenarioLineVacant ? CScenarioLineHint : null;
}

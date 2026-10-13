namespace Llyn.Conduct;

public sealed record CScenario(CSituationDraft CScenarioDraft)
{
    public CScenarioLine CScenarioTitle => LScenarioTitleRead(
        CScenarioDraft.CSituationDraftTitle.CStateValueText, CScenarioDraft.CSituationDraftTitle.CStateValueUncertain);

    public CScenarioLine CScenarioKind => LScenarioKindRead(
        CScenarioDraft.CSituationDraftKind.CStateValueText, CScenarioDraft.CSituationDraftKind.CStateValueUncertain);

    public CScenarioLine CScenarioDescription => LScenarioDescriptionRead(
        CScenarioDraft.CSituationDraftDescription.CStateValueText,
        CScenarioDraft.CSituationDraftDescription.CStateValueUncertain);

    internal static CScenarioLine LScenarioTitleRead(string text, bool uncertain)
    {
        return LScenarioLineRead(text, uncertain, "Situation.Untitled");
    }

    internal static CScenarioLine LScenarioKindRead(string text, bool uncertain)
    {
        return LScenarioLineRead(text, uncertain, "Situation.Kind");
    }

    internal static CScenarioLine LScenarioDescriptionRead(string text, bool uncertain)
    {
        return LScenarioLineRead(text, uncertain, "Situation.DescriptionHint");
    }

    internal static CSituationDraft LScenarioBlankRead()
    {
        return new CSituationDraft(
            0, CStateValue.CStateValueEmpty, CStateValue.CStateValueEmpty, CStateValue.CStateValueEmpty, [], []);
    }

    private static CScenarioLine LScenarioLineRead(string text, bool uncertain, string key)
    {
        return new CScenarioLine(text, uncertain ? "Display.Unknown" : key);
    }
}

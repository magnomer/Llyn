using System;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CPlaywright
{
    private readonly CAtelier _cPlaywrightAtelier;

    private readonly CEnvoy _cPlaywrightEnvoy;

    internal CPlaywright(CAtelier atelier, CEnvoy envoy, Action<Action> marshal)
    {
        ArgumentNullException.ThrowIfNull(atelier);
        ArgumentNullException.ThrowIfNull(envoy);
        ArgumentNullException.ThrowIfNull(marshal);

        _cPlaywrightAtelier = atelier;
        _cPlaywrightEnvoy = envoy;
        LPlaywrightDesk = new CDesk(
            atelier.CAtelierDraftPort,
            atelier.CAtelierSettingsPort,
            "Situation",
            envoy,
            "Repertoire",
            CSubject.CSubjectSituation);
        LPlaywrightDesk.CDeskObserverAttach(marshal, LPlaywrightDraftResonate);
    }

    public event Action<CScenario>? CPlaywrightScenarioChanged;

    public event Action<CScenario>? CPlaywrightDraftChanged;

    internal CDesk LPlaywrightDesk { get; }

    public CImage CPlaywrightImage => new(LPlaywrightDesk);

    public CVideo CPlaywrightVideo => new(LPlaywrightDesk);

    private LQuillSituation? LPlaywrightQuill =>
        !LPlaywrightDesk.CDeskDraft.CDeskDraftFilling && LPlaywrightDesk.CDeskDraft.CDeskDraftTenure is LTenure held
            ? new LQuillSituation(held)
            : null;

    internal CSituationDraft? LPlaywrightScenarioRead()
    {
        try
        {
            return CAtlas.LAtlasDraftRead(
                LPlaywrightDesk.CDeskDraft.CDeskDraftRead(), _cPlaywrightAtelier.CAtelierMediaPort);
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(
                _cPlaywrightEnvoy, _cPlaywrightAtelier.CAtelierSettingsPort, "Situation.HoldFailed", exception);
            return null;
        }
    }

    internal void LPlaywrightHeldResonate()
    {
        CPlaywrightScenarioChanged?.Invoke(
            new CScenario(LPlaywrightScenarioRead() ?? CScenario.LScenarioBlankRead()));
    }

    private void LPlaywrightDraftResonate()
    {
        if (LPlaywrightScenarioRead() is CSituationDraft situation)
        {
            CPlaywrightDraftChanged?.Invoke(new CScenario(situation));
        }
    }

    public CScenarioLine CPlaywrightTitleSet(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        LPlaywrightQuill?.LQuillTitleSet(text);
        return CScenario.LScenarioTitleRead(text, false);
    }

    public CScenarioLine CPlaywrightKindSet(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        LPlaywrightQuill?.LQuillKindSet(text);
        return CScenario.LScenarioKindRead(text, false);
    }

    public CScenarioLine CPlaywrightDescriptionSet(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        LPlaywrightQuill?.LQuillDescriptionSet(text);
        return CScenario.LScenarioDescriptionRead(text, false);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CYunjing
{
    private readonly CAtelier _cYunjingAtelier;

    private readonly LFanqiePort _cYunjingFanqiePort;

    private readonly LDiweiPort _cYunjingDiweiPort;

    private readonly CEnvoy _cYunjingEnvoy;

    private readonly LSettingsPort _cYunjingSettingsPort;

    private readonly Action<Action> _cYunjingMarshal;

    private bool _cYunjingFinal;

    private CYunjing(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy, Action<Action> marshal)
    {
        ArgumentNullException.ThrowIfNull(atelier);
        ArgumentNullException.ThrowIfNull(shownSeam);
        ArgumentNullException.ThrowIfNull(envoy);
        ArgumentNullException.ThrowIfNull(marshal);

        _cYunjingAtelier = atelier;
        _cYunjingFanqiePort = atelier.CAtelierPhonologyBundle.CPhonologyBundleFanqie;
        _cYunjingDiweiPort = atelier.CAtelierPhonologyBundle.CPhonologyBundleDiwei;
        _cYunjingEnvoy = envoy;
        _cYunjingSettingsPort = atelier.CAtelierSettingsPort;
        _cYunjingMarshal = marshal;
        LVistaPort vistas = atelier.CAtelierEntryBundle.CEntryBundleVista;
        CYunjingShengmu = new CAperture(
            envoy,
            _cYunjingSettingsPort,
            vistas,
            "Yunjing.LoadFailed",
            "Yunjing.ShengmuEmpty",
            "Yunjing.ShengmuUnmatched");
        CYunjingYunmu = new CAperture(
            envoy, _cYunjingSettingsPort, vistas, "Yunjing.LoadFailed", "Yunjing.YunmuEmpty", "Yunjing.YunmuUnmatched");
        CYunjingXiaoyun = new CEntryList(
            atelier, shownSeam, envoy, marshal, "Yunjing", "Xiaoyun", () => LYunjingAllowed, LYunjingXiaoyunFind);
        CYunjingXiaoyun.CEntryListChanged += () => CYunjingChanged?.Invoke();
        atelier.CAtelierWorkspace.LWorkspaceVistaAdd(LYunjingVistaRestore);
        atelier.CAtelierNavigation.LNavigationDiweiAttach(LYunjingDiweiOpen);
        LYunjingVistaRestore();
    }

    public static CYunjing CYunjingCreate(
        CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy, Action<Action> marshal)
    {
        return new CYunjing(atelier, shownSeam, envoy, marshal);
    }

    public event Action? CYunjingChanged;

    public event Action? CYunjingWorkspaceChanged;

    public CAperture CYunjingShengmu { get; }

    public CAperture CYunjingYunmu { get; }

    public CEntryList CYunjingXiaoyun { get; }

    public event Action? CYunjingDiweiOpened;

    internal bool LYunjingAllowed => _cYunjingFanqiePort.LEngineBookCheck();

    public bool CYunjingDiweiShown => LYunjingDiweiChosen && !CYunjingXiaoyun.CEntryListPanel.CPanelModeEnabled;

    public bool CYunjingDisplayShown => !CYunjingDiweiShown && !CYunjingXiaoyun.CEntryListEditing;

    public string CYunjingDiweiKey => _cYunjingFinal ? "Yunjing.Yunmu" : "Yunjing.Shengmu";

    public string CYunjingXiaoyunKey => LYunjingDiweiChosen
        ? CYunjingXiaoyun.CEntryListPanel.CPanelAperture.CApertureKey
        : "Yunjing.XiaoyunEmpty";

    private bool LYunjingDiweiChosen => LYunjingSide?.LVistaChosen is not null;

    private LVista? LYunjingSide => _cYunjingFinal ? CYunjingYunmu.CApertureVista : CYunjingShengmu.CApertureVista;

    internal void LYunjingVistaRestore()
    {
        CYunjingShengmu.CApertureRestore(
            _cYunjingAtelier.CAtelierVistaStart("yunjing", null, CCatalogOrder.CCatalogOrderName));
        CYunjingYunmu.CApertureRestore(
            _cYunjingAtelier.CAtelierVistaStart("yunmu", null, CCatalogOrder.CCatalogOrderName));
        CYunjingXiaoyun.LEntryListRestore();
        LYunjingObserverAttach();
    }

    private void LYunjingObserverAttach()
    {
        Action<CBulletin> rows = _ => _cYunjingMarshal(CYunjingXiaoyun.LEntryListResonate);
        CYunjingShengmu.CApertureObserverAttach(CSubject.CSubjectVista, rows);
        CYunjingYunmu.CApertureObserverAttach(CSubject.CSubjectVista, rows);
        CYunjingShengmu.CApertureObserverAttach(
            CSubject.CSubjectWorkspace, _ => _cYunjingMarshal(LYunjingWorkspaceResonate));
        CYunjingShengmu.CApertureObserverAttach(CSubject.CSubjectFanqie, rows);
        CYunjingShengmu.CApertureObserverAttach(CSubject.CSubjectSettings, rows);
        CYunjingShengmu.CApertureObserverAttach(CSubject.CSubjectReflex, rows);
        CYunjingXiaoyun.LEntryListAttach();
    }

    public static IReadOnlyList<CCatalogOrder> CYunjingOrderRead()
    {
        return
        [
            CCatalogOrder.CCatalogOrderName,
            CCatalogOrder.CCatalogOrderReverse,
            CCatalogOrder.CCatalogOrderUsage,
        ];
    }

    public IReadOnlyList<CDiwei> CYunjingShengmuRead()
    {
        IReadOnlyList<CDiwei> rows = LYunjingColumnRead(CYunjingShengmu.CApertureVista, false);
        CYunjingShengmu.CApertureCountSet(rows.Count);
        return rows;
    }

    public IReadOnlyList<CDiwei> CYunjingYunmuRead()
    {
        IReadOnlyList<CDiwei> rows = LYunjingColumnRead(CYunjingYunmu.CApertureVista, true);
        CYunjingYunmu.CApertureCountSet(rows.Count);
        return rows;
    }

    private IReadOnlyList<LVistaRow> LYunjingXiaoyunFind(LVista xiaoyun)
    {
        return CYunjingShengmu.CApertureVista is LVista onset && CYunjingYunmu.CApertureVista is LVista rime
            ? _cYunjingDiweiPort.LEngineXiaoyunFind(LYunjingSide?.LVistaChosen, onset, rime, xiaoyun)
            : [];
    }

    public CDiweiPage CYunjingDiweiRead()
    {
        LDiweiPage page = _cYunjingDiweiPort.LEngineDiweiResolve(
            CYunjingDiweiShown ? LYunjingSide?.LVistaChosen : null, _cYunjingSettingsPort.LEngineTextFind);
        return new CDiweiPage(
            page.LDiweiPageLanguage,
            page.LDiweiPageKey,
            page.LDiweiPageSections.Select(LYunjingSectionRead).ToList(),
            page.LDiweiPageEmpty,
            CFont.CFontRead(_cYunjingSettingsPort, page.LDiweiPageLanguage, CFontRole.CFontRoleHeadword),
            CFont.CFontRead(_cYunjingSettingsPort, page.LDiweiPageLanguage, CFontRole.CFontRoleGlyph));
    }

    private void LYunjingWorkspaceResonate()
    {
        CYunjingShengmu.CApertureVista?.LVistaSelect(null);
        CYunjingYunmu.CApertureVista?.LVistaSelect(null);
        CYunjingXiaoyun.CEntryListPanel.CPanelEntryClose();
        CYunjingChanged?.Invoke();
        CYunjingWorkspaceChanged?.Invoke();
    }

    public void CYunjingDiweiSelect(long? id, bool? final)
    {
        if (id is not long cell)
        {
            return;
        }

        if (final is not bool rime)
        {
            return;
        }

        if (!CYunjingXiaoyun.CEntryListPanel.CPanelLeaveConfirm())
        {
            return;
        }

        LYunjingDiweiToggle(cell, rime);
    }

    private void LYunjingDiweiToggle(long cell, bool rime)
    {
        _cYunjingFinal = rime;
        LYunjingSide?.LVistaToggle(cell);
        CYunjingXiaoyun.CEntryListPanel.CPanelEntryClose();
        CYunjingChanged?.Invoke();
    }

    internal void LYunjingDiweiOpen(string language, string kind, string key)
    {
        ArgumentNullException.ThrowIfNull(language);
        ArgumentNullException.ThrowIfNull(kind);
        ArgumentNullException.ThrowIfNull(key);

        CYunjingShengmu.CApertureQuerySet(string.Empty);
        CYunjingYunmu.CApertureQuerySet(string.Empty);
        CYunjingDiweiOpened?.Invoke();

        if (_cYunjingDiweiPort.LEngineDiweiFind(language, kind, key) is not (long cell, bool rime))
        {
            return;
        }

        CYunjingShengmu.CApertureVista?.LVistaSelect(null);
        CYunjingYunmu.CApertureVista?.LVistaSelect(null);
        LYunjingDiweiToggle(cell, rime);
    }

    public void CYunjingTallyToggle(bool? respelled)
    {
        if (respelled is not bool chosen)
        {
            return;
        }

        try
        {
            _cYunjingSettingsPort.LEngineTallySave(chosen);
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cYunjingEnvoy, _cYunjingSettingsPort, "Settings.SaveFailed", exception);
        }

        CYunjingChanged?.Invoke();
    }

    public void CYunjingGlyphSelect(string? character)
    {
        if (string.IsNullOrEmpty(character))
        {
            return;
        }

        if (!CYunjingDiweiShown)
        {
            return;
        }

        if (CCatalog.LCatalogGlyphOpen(
                _cYunjingEnvoy,
                _cYunjingSettingsPort,
                () => _cYunjingDiweiPort.LEngineDiweiResolve(LYunjingSide?.LVistaChosen, character))
            is long entry)
        {
            _cYunjingAtelier.CAtelierNavigation.CNavigationEntryOpen(entry);
        }
    }

    private IReadOnlyList<CDiwei> LYunjingColumnRead(LVista? vista, bool final)
    {
        return vista is LVista column
            ? _cYunjingDiweiPort.LEngineDiweiFind(column, LYunjingSide?.LVistaChosen, final)
                .Select(static row => new CDiwei(
                    row.LDiweiId, row.LDiweiKey, row.LDiweiCount, row.LDiweiFinal, row.LDiweiChosen))
                .ToList()
            : [];
    }

    private static CDiweiSection LYunjingSectionRead(LDiweiSection section)
    {
        return new CDiweiSection(
            section.LDiweiSectionLabel,
            section.LDiweiSectionLines
                .Select(static line => new CDiweiLine(
                    line.LDiweiLineReading, line.LDiweiLineLabel, line.LDiweiLineRounded, line.LDiweiLineCharacters))
                .ToList(),
            section.LDiweiSectionTallies
                .Select(static row => new CTally(
                    row.LTallyRowLanguage,
                    row.LTallyRowKind,
                    row.LTallyRowMarks
                        .Select(static mark => new CTallyMark(
                            mark.LTallyMarkText, mark.LTallyMarkCount, mark.LTallyMarkCharacters))
                        .ToList()))
                .ToList(),
            section.LDiweiSectionSwitched,
            section.LDiweiSectionRespelled);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CYunjing
{
    private readonly CAtelier _cYunjingAtelier;

    private readonly LPhonologyPort _cYunjingPort;

    private readonly LPortraitPort _cYunjingPortraitPort;

    private readonly CEnvoy _cYunjingEnvoy;

    private readonly LSettingsPort _cYunjingSettingsPort;

    private LVista? _cYunjingShengmu;

    private LVista? _cYunjingYunmu;

    private LVista? _cYunjingXiaoyun;

    private bool _cYunjingFinal;

    private int _cYunjingShengmuCount;

    private int _cYunjingYunmuCount;

    private int _cYunjingXiaoyunCount;

    private CYunjing(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(atelier);
        ArgumentNullException.ThrowIfNull(shownSeam);
        ArgumentNullException.ThrowIfNull(envoy);

        _cYunjingAtelier = atelier;
        _cYunjingPort = atelier.CAtelierPhonologyPort;
        _cYunjingPortraitPort = atelier.CAtelierPortraitPort;
        _cYunjingEnvoy = envoy;
        _cYunjingSettingsPort = atelier.CAtelierSettingsPort;
        CEditor editor = CEditor.CEditorCreate(atelier, envoy);
        CYunjingEditor = editor;
        CYunjingPanel = new CPanel(
            envoy,
            "Yunjing.LoadFailed",
            "Scribe",
            editor.CEditorDesk.LDeskChangeCheck,
            editor.LEditorFinish,
            shownSeam);
        CYunjingPanel.CPanelCleared += () => editor.CEditorEntryOpen(null);
        CYunjingPanel.CPanelEdited += id => editor.CEditorEntryOpen(id);
        atelier.CAtelierNavigation.LNavigationTabAdd(
            "Yunjing",
            CYunjingPanel.CPanelLeaveConfirm,
            CYunjingPanel.LPanelChosenRead,
            CYunjingPanel.LPanelScribeRestore,
            id => CYunjingPanel.CPanelRowOpen(id),
            () => LYunjingAllowed);
        atelier.CAtelierWorkspace.LWorkspaceDraftAdd(CYunjingPanel.LPanelChangeCheck, editor.LEditorFinish);
        atelier.CAtelierNavigation.LNavigationDiweiAttach(LYunjingDiweiOpen);
        CYunjingPanel.LPanelStationAttach(atelier.CAtelierNavigation.LNavigationStationAdd);
    }

    public static CYunjing CYunjingCreate(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy)
    {
        return new CYunjing(atelier, shownSeam, envoy);
    }

    public event Action? CYunjingChanged;

    public CEditor CYunjingEditor { get; }

    public CPanel CYunjingPanel { get; }

    public event Action? CYunjingDiweiOpened;

    internal bool LYunjingAllowed => _cYunjingPort.LEngineBookCheck();

    public bool CYunjingDiweiShown => LYunjingDiweiChosen && !CYunjingPanel.CPanelModeEnabled;

    public bool CYunjingDisplayShown => !CYunjingDiweiShown && !CYunjingPanel.CPanelEditing;

    public bool CYunjingEditorShown => CYunjingPanel.CPanelEditing;

    public string CYunjingDiweiKey => _cYunjingFinal ? "Yunjing.Yunmu" : "Yunjing.Shengmu";

    public bool CYunjingShengmuEmpty => _cYunjingShengmuCount == 0;

    public bool CYunjingYunmuEmpty => _cYunjingYunmuCount == 0;

    public bool CYunjingXiaoyunEmpty => _cYunjingXiaoyunCount == 0;

    public string CYunjingShengmuKey =>
        _cYunjingShengmu?.LVistaQueried ?? false ? "Yunjing.ShengmuUnmatched" : "Yunjing.ShengmuEmpty";

    public string CYunjingYunmuKey =>
        _cYunjingYunmu?.LVistaQueried ?? false ? "Yunjing.YunmuUnmatched" : "Yunjing.YunmuEmpty";

    public string CYunjingXiaoyunKey => LYunjingDiweiChosen ? LYunjingVacantKey : "Yunjing.XiaoyunEmpty";

    public CCatalogOrder CYunjingShengmuOrder => CPanel.CPanelOrderRead(LVista.LVistaOrderRead(_cYunjingShengmu));

    public CCatalogOrder CYunjingYunmuOrder => CPanel.CPanelOrderRead(LVista.LVistaOrderRead(_cYunjingYunmu));

    private string LYunjingVacantKey =>
        _cYunjingXiaoyun?.LVistaQueried ?? false ? "Yunjing.XiaoyunUnmatched" : "Yunjing.XiaoyunVacant";

    private bool LYunjingDiweiChosen => LYunjingSide?.LVistaChosen is not null;

    private LVista? LYunjingSide => _cYunjingFinal ? _cYunjingYunmu : _cYunjingShengmu;

    public void CYunjingVistaRestore()
    {
        LVista shengmu = _cYunjingAtelier.CAtelierVistaStart("yunjing", null, CCatalogOrder.CCatalogOrderName);
        LVista yunmu = _cYunjingAtelier.CAtelierVistaStart("yunmu", null, CCatalogOrder.CCatalogOrderName);
        LVista xiaoyun = _cYunjingAtelier.CAtelierVistaStart(
            "xiaoyun", CSubject.CSubjectEntry, CCatalogOrder.CCatalogOrderHeadword);
        _cYunjingShengmu = shengmu;
        _cYunjingYunmu = yunmu;
        _cYunjingXiaoyun = xiaoyun;
        CYunjingPanel.CPanelVistaRestore(xiaoyun);
        CYunjingEditor.LEditorVistaRestore(xiaoyun);
    }

    public void CYunjingShengmuAttach(CSubject subject, Action<CBulletin> observer)
    {
        LYunjingObserverAttach(_cYunjingShengmu, subject, observer);
    }

    public void CYunjingYunmuAttach(CSubject subject, Action<CBulletin> observer)
    {
        LYunjingObserverAttach(_cYunjingYunmu, subject, observer);
    }

    public IReadOnlyList<CDiwei> CYunjingShengmuRead()
    {
        IReadOnlyList<CDiwei> rows = LYunjingColumnRead(_cYunjingShengmu, false);
        _cYunjingShengmuCount = rows.Count;
        return rows;
    }

    public IReadOnlyList<CDiwei> CYunjingYunmuRead()
    {
        IReadOnlyList<CDiwei> rows = LYunjingColumnRead(_cYunjingYunmu, true);
        _cYunjingYunmuCount = rows.Count;
        return rows;
    }

    public IReadOnlyList<CVistaRow> CYunjingXiaoyunRead()
    {
        IReadOnlyList<CVistaRow> rows =
            _cYunjingShengmu is LVista onset && _cYunjingYunmu is LVista rime && _cYunjingXiaoyun is LVista xiaoyun
                ? _cYunjingPort.LEngineXiaoyunFind(LYunjingSide?.LVistaChosen, onset, rime, xiaoyun)
                    .Select(CPanel.CPanelRowRead)
                    .ToList()
                : [];
        _cYunjingXiaoyunCount = rows.Count;
        return rows;
    }

    public CDiweiPage CYunjingDiweiRead()
    {
        LDiweiPage page = _cYunjingPort.LEngineDiweiResolve(
            CYunjingDiweiShown ? LYunjingSide?.LVistaChosen : null, _cYunjingSettingsPort.LEngineTextFind);
        return new CDiweiPage(
            page.LDiweiPageLanguage,
            page.LDiweiPageKey,
            page.LDiweiPageSections.Select(LYunjingSectionRead).ToList(),
            page.LDiweiPageEmpty);
    }

    public void CYunjingShengmuFind(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        _cYunjingShengmu?.LVistaQuerySet(query);
    }

    public void CYunjingYunmuFind(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        _cYunjingYunmu?.LVistaQuerySet(query);
    }

    public void CYunjingXiaoyunFind(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        _cYunjingXiaoyun?.LVistaQuerySet(query);
    }

    public void CYunjingShengmuSet(CCatalogOrder? order)
    {
        _cYunjingShengmu?.LVistaOrderSet(CPanel.CPanelOrderRead(order));
    }

    public void CYunjingYunmuSet(CCatalogOrder? order)
    {
        _cYunjingYunmu?.LVistaOrderSet(CPanel.CPanelOrderRead(order));
    }

    public void CYunjingDiweiCancel()
    {
        _cYunjingShengmu?.LVistaSelect(null);
        _cYunjingYunmu?.LVistaSelect(null);
        CYunjingPanel.CPanelEntryClose();
        CYunjingChanged?.Invoke();
    }

    public void CYunjingRowsResonate()
    {
        CYunjingChanged?.Invoke();
        CYunjingPanel.CPanelRowsResonate();
    }

    public void CYunjingEntryResonate(CBulletin bulletin)
    {
        CYunjingChanged?.Invoke();
        CYunjingPanel.CPanelEntryResonate(bulletin);
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

        _cYunjingFinal = rime;
        LYunjingSide?.LVistaToggle(cell);
        CYunjingPanel.CPanelEntryClose();
        CYunjingChanged?.Invoke();
    }

    internal void LYunjingDiweiOpen(string language, string kind, string key)
    {
        ArgumentNullException.ThrowIfNull(language);
        ArgumentNullException.ThrowIfNull(kind);
        ArgumentNullException.ThrowIfNull(key);

        _cYunjingShengmu?.LVistaQuerySet(string.Empty);
        _cYunjingYunmu?.LVistaQuerySet(string.Empty);
        CYunjingDiweiOpened?.Invoke();

        if (_cYunjingPort.LEngineDiweiFind(language, kind, key) is not (long cell, bool rime))
        {
            return;
        }

        _cYunjingShengmu?.LVistaSelect(null);
        _cYunjingYunmu?.LVistaSelect(null);
        CYunjingDiweiSelect(cell, rime);
    }

    public void CYunjingTallyToggle(bool? respelled)
    {
        if (respelled is not bool chosen)
        {
            return;
        }

        _cYunjingPort.LEngineTallySave(chosen);
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
                _cYunjingEnvoy, () => _cYunjingPort.LEngineDiweiResolve(LYunjingSide?.LVistaChosen, character))
            is long entry)
        {
            _cYunjingAtelier.CAtelierNavigation.CNavigationEntryOpen(entry);
        }
    }

    internal string LYunjingFileRead()
    {
        return LVista.LVistaFileRead(_cYunjingXiaoyun);
    }

    public Task CYunjingPortraitPrint()
    {
        if (!CYunjingPanel.CPanelPressAllowed)
        {
            return Task.CompletedTask;
        }

        return CPortrait.LPortraitTicketPrint(
            _cYunjingEnvoy,
            chosen => _cYunjingPortraitPort.LEnginePortraitPrint(
                _cYunjingXiaoyun, CPortrait.LPortraitLabelRead(_cYunjingSettingsPort), chosen));
    }

    public Task CYunjingPortraitExport()
    {
        return CPortrait.LPortraitFileExport(
            _cYunjingEnvoy,
            LYunjingFileRead(),
            (file, medium) => _cYunjingPortraitPort.LEnginePortraitExport(
                _cYunjingXiaoyun, file, medium, CPortrait.LPortraitLabelRead(_cYunjingSettingsPort)));
    }

    private IReadOnlyList<CDiwei> LYunjingColumnRead(LVista? vista, bool final)
    {
        return vista is LVista column
            ? _cYunjingPort.LEngineDiweiFind(column, LYunjingSide?.LVistaChosen, final)
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

    private static void LYunjingObserverAttach(LVista? vista, CSubject subject, Action<CBulletin> observer)
    {
        ArgumentNullException.ThrowIfNull(observer);

        vista?.LVistaObserverAttach(
            CPanel.CPanelSubjectRead(subject), bulletin => observer(CAtelier.CAtelierBulletinRead(bulletin)));
    }
}

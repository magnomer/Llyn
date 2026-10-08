using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CXiesheng
{
    private readonly CAtelier _cXieshengAtelier;

    private readonly LStemPort _cXieshengPort;

    private readonly LPortraitPort _cXieshengPortraitPort;

    private readonly LSettingsPort _cXieshengSettingsPort;

    private readonly CEnvoy _cXieshengEnvoy;

    private readonly Action<Action> _cXieshengMarshal;

    private LVista? _cXieshengGrove;

    private LVista? _cXieshengKindred;

    private int _cXieshengGroveCount;

    private int _cXieshengKindredCount;

    private CXiesheng(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy, Action<Action> marshal)
    {
        ArgumentNullException.ThrowIfNull(atelier);
        ArgumentNullException.ThrowIfNull(shownSeam);
        ArgumentNullException.ThrowIfNull(envoy);
        ArgumentNullException.ThrowIfNull(marshal);

        _cXieshengAtelier = atelier;
        _cXieshengPort = atelier.CAtelierPhonologyBundle.CPhonologyBundleStem;
        _cXieshengPortraitPort = atelier.CAtelierPortraitPort;
        _cXieshengSettingsPort = atelier.CAtelierSettingsPort;
        _cXieshengEnvoy = envoy;
        _cXieshengMarshal = marshal;
        CEditor editor = CEditor.CEditorCreate(atelier, envoy);
        CXieshengEditor = editor;
        CXieshengPanel = new CPanel(
            envoy,
            _cXieshengSettingsPort,
            atelier.CAtelierEntryBundle.CEntryBundleVista,
            "Xiesheng.LoadFailed",
            "Scribe",
            editor.CEditorDesk.LDeskChangeCheck,
            editor.LEditorFinish,
            shownSeam);
        CXieshengPanel.CPanelCleared += () => editor.CEditorEntryOpen(null);
        CXieshengPanel.CPanelEdited += id => editor.CEditorEntryOpen(id);
        atelier.CAtelierNavigation.LNavigationTabAdd(
            "Xiesheng",
            CXieshengPanel.CPanelLeaveConfirm,
            CXieshengPanel.LPanelChosenRead,
            CXieshengPanel.LPanelScribeRestore,
            id => CXieshengPanel.CPanelRowOpen(id),
            () => LXieshengAllowed);
        atelier.CAtelierWorkspace.LWorkspaceDraftAdd(CXieshengPanel.LPanelChangeCheck, editor.LEditorFinish);
        atelier.CAtelierWorkspace.LWorkspaceVistaAdd(LXieshengVistaRestore);
        atelier.CAtelierWorkspace.LWorkspaceClosureAdd(LXieshengClose);
        atelier.CAtelierNavigation.LNavigationStemAttach(LXieshengStemOpen);
        CXieshengPanel.LPanelStationAttach(atelier.CAtelierNavigation.LNavigationStationAdd);
        LXieshengVistaRestore();
    }

    public static CXiesheng CXieshengCreate(
        CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy, Action<Action> marshal)
    {
        return new CXiesheng(atelier, shownSeam, envoy, marshal);
    }

    public event Action? CXieshengChanged;

    public event Action? CXieshengWorkspaceChanged;

    public CEditor CXieshengEditor { get; }

    public CPanel CXieshengPanel { get; }

    public event Action? CXieshengStemOpened;

    internal bool LXieshengAllowed => _cXieshengPort.LEngineStemCheck();

    public bool CXieshengStemShown => LXieshengStemChosen && !CXieshengPanel.CPanelModeEnabled;

    public bool CXieshengDisplayShown => !CXieshengStemShown && !CXieshengPanel.CPanelEditing;

    public bool CXieshengEditorShown => CXieshengPanel.CPanelEditing;

    public bool CXieshengGroveEmpty => _cXieshengGroveCount == 0;

    public bool CXieshengKindredEmpty => _cXieshengKindredCount == 0;

    public string CXieshengGroveKey =>
        _cXieshengGrove?.LVistaQueried ?? false ? "Xiesheng.GroveUnmatched" : "Xiesheng.GroveEmpty";

    public string CXieshengKindredKey => LXieshengStemChosen ? LXieshengVacantKey : "Xiesheng.KindredEmpty";

    public CCatalogOrder CXieshengOrder => CCatalog.LCatalogOrderRead(LVista.LVistaOrderRead(_cXieshengGrove));

    private string LXieshengVacantKey =>
        _cXieshengKindred?.LVistaQueried ?? false ? "Xiesheng.KindredUnmatched" : "Xiesheng.KindredVacant";

    private bool LXieshengStemChosen => _cXieshengGrove?.LVistaChosen is not null;

    internal void LXieshengVistaRestore()
    {
        LVista grove = _cXieshengAtelier.CAtelierVistaStart("grove", null, CCatalogOrder.CCatalogOrderName);
        LVista kindred = _cXieshengAtelier.CAtelierVistaStart(
            "kindred", CSubject.CSubjectEntry, CCatalogOrder.CCatalogOrderHeadword);
        grove.LVistaQuerySet(_cXieshengGrove?.LVistaQuery ?? string.Empty);
        kindred.LVistaQuerySet(_cXieshengKindred?.LVistaQuery ?? string.Empty);
        _cXieshengGrove = grove;
        _cXieshengKindred = kindred;
        CXieshengPanel.CPanelVistaRestore(kindred);
        CXieshengEditor.LEditorVistaRestore(kindred);
        LXieshengObserverAttach(grove);
    }

    private void LXieshengObserverAttach(LVista grove)
    {
        CPanel panel = CXieshengPanel;
        Action<CBulletin> rows = _ => _cXieshengMarshal(LXieshengRowsResonate);
        LXieshengGroveAttach(grove, CSubject.CSubjectVista, rows);
        LXieshengGroveAttach(grove, CSubject.CSubjectWorkspace, _ => _cXieshengMarshal(LXieshengWorkspaceResonate));
        LXieshengGroveAttach(grove, CSubject.CSubjectFanqie, rows);
        LXieshengGroveAttach(grove, CSubject.CSubjectSettings, rows);
        panel.CPanelObserverAttach(CSubject.CSubjectVista, _ => _cXieshengMarshal(panel.CPanelRowsResonate));
        panel.CPanelObserverAttach(
            CSubject.CSubjectEntry, bulletin => _cXieshengMarshal(() => LXieshengEntryResonate(bulletin)));
        panel.CPanelChosenAttach(CSubject.CSubjectEntry, _ => _cXieshengMarshal(panel.CPanelDraftResonate));
    }

    public static IReadOnlyList<CCatalogOrder> CXieshengOrderRead()
    {
        return
        [
            CCatalogOrder.CCatalogOrderName,
            CCatalogOrder.CCatalogOrderReverse,
            CCatalogOrder.CCatalogOrderUsage,
        ];
    }

    public IReadOnlyList<CStem> CXieshengGroveRead()
    {
        IReadOnlyList<CStem> rows = _cXieshengGrove is LVista grove
            ? _cXieshengPort.LEngineStemFind(grove)
                .Select(static row => new CStem(row.LStemId, row.LStemKey, row.LStemCount, row.LStemChosen))
                .ToList()
            : [];
        _cXieshengGroveCount = rows.Count;
        return rows;
    }

    public IReadOnlyList<CVistaRow> CXieshengKindredRead()
    {
        IReadOnlyList<CVistaRow> rows;
        try
        {
            rows = _cXieshengGrove is LVista grove && _cXieshengKindred is LVista kindred
                ? _cXieshengPort.LEngineKindredFind(grove, kindred).Select(CCatalog.LCatalogRowRead).ToList()
                : [];
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cXieshengEnvoy, _cXieshengSettingsPort, "Xiesheng.LoadFailed", exception);
            rows = [];
        }

        _cXieshengKindredCount = rows.Count;
        return rows;
    }

    public Task<CEnsignSheet<IReadOnlyList<CVistaRow>>> CXieshengKindredLoad(
        Func<IReadOnlyList<CEnsignRow>, Action<string, Exception>, Action> store) =>
        CCatalog.LCatalogEnsignLoad(
            _cXieshengEnvoy, _cXieshengSettingsPort, "Xiesheng.LoadFailed", store, CXieshengKindredRead);

    public CStemPage CXieshengStemRead()
    {
        LStemPage page = _cXieshengPort.LEngineStemResolve(
            CXieshengStemShown ? _cXieshengGrove?.LVistaChosen : null);
        return new CStemPage(
            page.LStemPageLanguage,
            page.LStemPageKey,
            page.LStemPageCharacters,
            page.LStemPageEmpty,
            CCatalog.LCatalogFontRead(_cXieshengSettingsPort, page.LStemPageLanguage, CFontRole.CFontRoleHeadword),
            CCatalog.LCatalogFontRead(_cXieshengSettingsPort, page.LStemPageLanguage, CFontRole.CFontRoleGlyph));
    }

    public void CXieshengGroveFind(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        _cXieshengGrove?.LVistaQuerySet(query);
    }

    public void CXieshengKindredFind(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        _cXieshengKindred?.LVistaQuerySet(query);
    }

    public void CXieshengGroveSet(CCatalogOrder? order)
    {
        _cXieshengGrove?.LVistaOrderSet(CCatalog.LCatalogOrderRead(order));
    }

    private void LXieshengWorkspaceResonate()
    {
        _cXieshengGrove?.LVistaSelect(null);
        CXieshengPanel.CPanelEntryClose();
        CXieshengChanged?.Invoke();
        CXieshengWorkspaceChanged?.Invoke();
    }

    private void LXieshengRowsResonate()
    {
        CXieshengChanged?.Invoke();
        CXieshengPanel.CPanelRowsResonate();
    }

    private void LXieshengEntryResonate(CBulletin bulletin)
    {
        CXieshengChanged?.Invoke();
        CXieshengPanel.CPanelEntryResonate(bulletin);
    }

    private void LXieshengClose()
    {
        CXieshengEditor.CEditorClose();
        CXieshengEditor.CEditorDisplay.CDisplaySound.CDisplayPlaybackCancel();
    }

    public void CXieshengStemSelect(long? id)
    {
        if (id is not long stem)
        {
            return;
        }

        if (!CXieshengPanel.CPanelLeaveConfirm())
        {
            return;
        }

        LXieshengStemToggle(stem);
    }

    private void LXieshengStemToggle(long stem)
    {
        _cXieshengGrove?.LVistaToggle(stem);
        CXieshengPanel.CPanelEntryClose();
        CXieshengChanged?.Invoke();
    }

    internal void LXieshengStemOpen(string language, string? key)
    {
        ArgumentNullException.ThrowIfNull(language);

        _cXieshengGrove?.LVistaQuerySet(string.Empty);
        CXieshengStemOpened?.Invoke();

        if (_cXieshengPort.LEngineStemFind(language, key) is not long stem)
        {
            return;
        }

        _cXieshengGrove?.LVistaSelect(null);
        LXieshengStemToggle(stem);
    }

    public void CXieshengGlyphSelect(string? character)
    {
        if (string.IsNullOrEmpty(character))
        {
            return;
        }

        if (!CXieshengStemShown)
        {
            return;
        }

        if (CCatalog.LCatalogGlyphOpen(
                _cXieshengEnvoy,
                _cXieshengSettingsPort,
                () => _cXieshengPort.LEngineStemResolve(_cXieshengGrove?.LVistaChosen, character))
            is long entry)
        {
            _cXieshengAtelier.CAtelierNavigation.CNavigationEntryOpen(entry);
        }
    }

    internal string LXieshengFileRead()
    {
        return _cXieshengAtelier.CAtelierEntryBundle.CEntryBundleVista.LEngineFileRead(_cXieshengKindred);
    }

    public Task CXieshengPortraitPrint()
    {
        if (!CXieshengPanel.CPanelPressAllowed)
        {
            return Task.CompletedTask;
        }

        return CPortrait.LPortraitTicketPrint(
            _cXieshengEnvoy,
            _cXieshengSettingsPort,
            chosen => _cXieshengPortraitPort.LEnginePortraitPrint(
                _cXieshengKindred, CPortrait.LPortraitLabelRead(_cXieshengSettingsPort), chosen));
    }

    public Task CXieshengPortraitExport()
    {
        return CPortrait.LPortraitFileExport(
            _cXieshengEnvoy,
            _cXieshengSettingsPort,
            LXieshengFileRead,
            (file, medium) => _cXieshengPortraitPort.LEnginePortraitExport(
                _cXieshengKindred, file, medium, CPortrait.LPortraitLabelRead(_cXieshengSettingsPort)));
    }

    private static void LXieshengGroveAttach(LVista grove, CSubject subject, Action<CBulletin> observer)
    {
        ArgumentNullException.ThrowIfNull(observer);

        grove.LVistaObserverAttach(
            CCatalog.LCatalogSubjectRead(subject), bulletin => observer(CAtelier.CAtelierBulletinRead(bulletin)));
    }
}

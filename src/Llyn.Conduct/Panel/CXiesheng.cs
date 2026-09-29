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

    private readonly LPhonologyPort _cXieshengPort;

    private readonly LPortraitPort _cXieshengPortraitPort;

    private readonly LSettingsPort _cXieshengSettingsPort;

    private readonly CEnvoy _cXieshengEnvoy;

    private LVista? _cXieshengGrove;

    private LVista? _cXieshengKindred;

    private int _cXieshengGroveCount;

    private int _cXieshengKindredCount;

    private CXiesheng(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(atelier);
        ArgumentNullException.ThrowIfNull(shownSeam);
        ArgumentNullException.ThrowIfNull(envoy);

        _cXieshengAtelier = atelier;
        _cXieshengPort = atelier.CAtelierPhonologyPort;
        _cXieshengPortraitPort = atelier.CAtelierPortraitPort;
        _cXieshengSettingsPort = atelier.CAtelierSettingsPort;
        _cXieshengEnvoy = envoy;
        CEditor editor = CEditor.CEditorCreate(atelier, envoy);
        CXieshengEditor = editor;
        CXieshengPanel = new CPanel(
            envoy,
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
        atelier.CAtelierNavigation.LNavigationStemAttach(LXieshengStemOpen);
        CXieshengPanel.LPanelStationAttach(atelier.CAtelierNavigation.LNavigationStationAdd);
    }

    public static CXiesheng CXieshengCreate(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy)
    {
        return new CXiesheng(atelier, shownSeam, envoy);
    }

    public event Action? CXieshengChanged;

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

    public CCatalogOrder CXieshengOrder => CPanel.CPanelOrderRead(LVista.LVistaOrderRead(_cXieshengGrove));

    private string LXieshengVacantKey =>
        _cXieshengKindred?.LVistaQueried ?? false ? "Xiesheng.KindredUnmatched" : "Xiesheng.KindredVacant";

    private bool LXieshengStemChosen => _cXieshengGrove?.LVistaChosen is not null;

    public void CXieshengVistaRestore()
    {
        LVista grove = _cXieshengAtelier.CAtelierVistaStart("grove", null, CCatalogOrder.CCatalogOrderName);
        LVista kindred = _cXieshengAtelier.CAtelierVistaStart(
            "kindred", CSubject.CSubjectEntry, CCatalogOrder.CCatalogOrderHeadword);
        _cXieshengGrove = grove;
        _cXieshengKindred = kindred;
        CXieshengPanel.CPanelVistaRestore(kindred);
        CXieshengEditor.LEditorVistaRestore(kindred);
    }

    public void CXieshengObserverAttach(CSubject subject, Action<CBulletin> observer)
    {
        ArgumentNullException.ThrowIfNull(observer);

        _cXieshengGrove?.LVistaObserverAttach(
            CPanel.CPanelSubjectRead(subject), bulletin => observer(CAtelier.CAtelierBulletinRead(bulletin)));
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
        IReadOnlyList<CVistaRow> rows = _cXieshengGrove is LVista grove && _cXieshengKindred is LVista kindred
            ? _cXieshengPort.LEngineKindredFind(grove, kindred).Select(CPanel.CPanelRowRead).ToList()
            : [];
        _cXieshengKindredCount = rows.Count;
        return rows;
    }

    public CStemPage CXieshengStemRead()
    {
        LStemPage page = _cXieshengPort.LEngineStemResolve(
            CXieshengStemShown ? _cXieshengGrove?.LVistaChosen : null);
        return new CStemPage(
            page.LStemPageLanguage, page.LStemPageKey, page.LStemPageCharacters, page.LStemPageEmpty);
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
        _cXieshengGrove?.LVistaOrderSet(CPanel.CPanelOrderRead(order));
    }

    public void CXieshengStemCancel()
    {
        _cXieshengGrove?.LVistaSelect(null);
        CXieshengPanel.CPanelEntryClose();
        CXieshengChanged?.Invoke();
    }

    public void CXieshengRowsResonate()
    {
        CXieshengChanged?.Invoke();
        CXieshengPanel.CPanelRowsResonate();
    }

    public void CXieshengEntryResonate(CBulletin bulletin)
    {
        CXieshengChanged?.Invoke();
        CXieshengPanel.CPanelEntryResonate(bulletin);
    }

    public void CXieshengStemSelect(long? id)
    {
        if (id is not long stem)
        {
            return;
        }

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
        CXieshengStemSelect(stem);
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
                _cXieshengEnvoy, () => _cXieshengPort.LEngineStemResolve(_cXieshengGrove?.LVistaChosen, character))
            is long entry)
        {
            _cXieshengAtelier.CAtelierNavigation.CNavigationEntryOpen(entry);
        }
    }

    internal string LXieshengFileRead()
    {
        return LVista.LVistaFileRead(_cXieshengKindred);
    }

    public Task CXieshengPortraitPrint()
    {
        if (!CXieshengPanel.CPanelPressAllowed)
        {
            return Task.CompletedTask;
        }

        return CPortrait.LPortraitTicketPrint(
            _cXieshengEnvoy,
            chosen => _cXieshengPortraitPort.LEnginePortraitPrint(
                _cXieshengKindred, CPortrait.LPortraitLabelRead(_cXieshengSettingsPort), chosen));
    }

    public Task CXieshengPortraitExport()
    {
        return CPortrait.LPortraitFileExport(
            _cXieshengEnvoy,
            LXieshengFileRead(),
            (file, medium) => _cXieshengPortraitPort.LEnginePortraitExport(
                _cXieshengKindred, file, medium, CPortrait.LPortraitLabelRead(_cXieshengSettingsPort)));
    }
}

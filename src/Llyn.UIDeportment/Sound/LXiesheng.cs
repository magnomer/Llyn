using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIDeportment;

public sealed class LXiesheng
{
    private readonly LPhonologyPort _lPhonologyPort;

    private readonly LPortraitPort _lPortraitPort;

    private LVista? _lGroveVista;

    private LVista? _lKindredVista;

    private int _lGroveCount;

    private int _lKindredCount;

    internal LXiesheng(
        LPhonologyPort phonology,
        LPortraitPort portraits,
        LEditor editor,
        LLectern lectern,
        Func<bool> shownSeam,
        CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(phonology);
        ArgumentNullException.ThrowIfNull(portraits);
        ArgumentNullException.ThrowIfNull(editor);
        ArgumentNullException.ThrowIfNull(lectern);

        _lPhonologyPort = phonology;
        _lPortraitPort = portraits;
        LXieshengEditor = editor;
        LXieshengPanel = new CPanel(
            envoy, "Xiesheng.LoadFailed", "Scribe",
            editor.LEditorStudio.CEditorDesk.CDeskChangeCheck, editor.LEditorStudio.CEditorFinish,
            shownSeam);
        LXieshengPanel.CPanelCleared += LXieshengEditorClear;
        LXieshengPanel.CPanelEdited += LXieshengEditorOpen;
        LXieshengPanel.CPanelDraftChanged += lectern.LLecternDraftShow;
        LXieshengPanel.CPanelCleared += lectern.LLecternClear;
    }

    private void LXieshengEditorClear()
    {
        LXieshengEditor.LEditorStudio.CEditorEntryOpen(null);
    }

    private void LXieshengEditorOpen(long id)
    {
        LXieshengEditor.LEditorStudio.CEditorEntryOpen(id);
    }

    public event Action? LXieshengChanged;

    public event Action<string, string>? LXieshengGlyphChosen;

    public LEditor LXieshengEditor { get; }

    public CPanel LXieshengPanel { get; }

    public bool LXieshengAllowed => _lPhonologyPort.LEngineStemFind() is not null;

    public bool LXieshengStemShown => LXieshengStemChosen && !LXieshengPanel.CPanelModeEnabled;

    public bool LXieshengDisplayShown => !LXieshengStemShown && !LXieshengPanel.CPanelEditing;

    public bool LXieshengEditorShown => LXieshengPanel.CPanelEditing;

    public bool LXieshengGroveEmpty => _lGroveCount == 0;

    public bool LXieshengKindredEmpty => _lKindredCount == 0;

    public string LXieshengGroveKey => LXieshengLodestarQueried ? "Xiesheng.GroveUnmatched" : "Xiesheng.GroveEmpty";

    public string LXieshengKindredKey => LXieshengStemChosen ? LXieshengVacantKey : "Xiesheng.KindredEmpty";

    public CCatalogOrder LXieshengRung =>
        CPanel.CPanelOrderRead(_lGroveVista?.LVistaOrder ?? LCatalogOrder.LCatalogOrderName);

    private string LXieshengVacantKey =>
        LXieshengSextantQueried ? "Xiesheng.KindredUnmatched" : "Xiesheng.KindredVacant";

    private bool LXieshengLodestarQueried => _lGroveVista?.LVistaQueried ?? false;

    private bool LXieshengSextantQueried => _lKindredVista?.LVistaQueried ?? false;

    private bool LXieshengStemChosen => _lGroveVista?.LVistaChosen is not null;

    private string LXieshengLanguage =>
        _lPhonologyPort.LEngineStemRead(_lGroveVista?.LVistaChosen)?.LStemLanguage
        ?? _lPhonologyPort.LEngineStemFind()
        ?? string.Empty;

    internal void LXieshengVistaRestore(CAtelier atelier)
    {
        ArgumentNullException.ThrowIfNull(atelier);

        LXieshengVistaRestore(
            atelier.CAtelierVistaStart("grove", null, CCatalogOrder.CCatalogOrderName),
            atelier.CAtelierVistaStart(
                "kindred", CSubject.CSubjectEntry, CCatalogOrder.CCatalogOrderHeadword));
    }

    internal void LXieshengVistaRestore(LVista grove, LVista kindred)
    {
        ArgumentNullException.ThrowIfNull(grove);
        ArgumentNullException.ThrowIfNull(kindred);

        _lGroveVista = grove;
        _lKindredVista = kindred;
        LXieshengPanel.CPanelVistaRestore(kindred);
        LXieshengEditor.LEditorStudio.CEditorVistaRestore(kindred);
    }

    public IReadOnlyList<CStem> LXieshengGroveRead()
    {
        IReadOnlyList<CStem> rows = LXieshengGroveBuild(LXieshengStemFind());
        _lGroveCount = rows.Count;
        return rows;
    }

    internal static IReadOnlyList<CStem> LXieshengGroveBuild(IReadOnlyList<LStem> rows)
    {
        return LSplice.LSpliceBuild(
            rows,
            static row => new CStem(row.LStemId, row.LStemKey, row.LStemCount, row.LStemChosen));
    }

    private IReadOnlyList<LStem> LXieshengStemFind()
    {
        if (_lGroveVista is not LVista vista)
        {
            return [];
        }

        return LXieshengAllowed ? _lPhonologyPort.LEngineStemFind(vista, LXieshengLanguage) : [];
    }

    public IReadOnlyList<CVistaRow> LXieshengKindredRead()
    {
        IReadOnlyList<CVistaRow> rows = LSplice.LSpliceBuild(LXieshengKindredFind(), CPanel.CPanelRowRead);
        _lKindredCount = rows.Count;
        return rows;
    }

    private IReadOnlyList<LVistaRow> LXieshengKindredFind()
    {
        if (_lGroveVista is not LVista grove)
        {
            return [];
        }

        if (_lKindredVista is not LVista vista)
        {
            return [];
        }

        return _lPhonologyPort.LEngineKindredFind(LXieshengLanguage, grove, vista);
    }

    public CStemPage LXieshengStemRead()
    {
        return LXieshengPageBuild(LXieshengStemShown
            ? _lPhonologyPort.LEngineStemResolve(_lGroveVista?.LVistaChosen)
            : LStemPage.LStemPageBlank);
    }

    internal static CStemPage LXieshengPageBuild(LStemPage page)
    {
        ArgumentNullException.ThrowIfNull(page);

        return new CStemPage(
            page.LStemPageLanguage, page.LStemPageKey, page.LStemPageCharacters, page.LStemPageEmpty);
    }

    public void LXieshengLodestarSet(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        _lGroveVista?.LVistaQuerySet(query);
    }

    public void LXieshengSextantSet(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        _lKindredVista?.LVistaQuerySet(query);
    }

    public void LXieshengRungSet(CCatalogOrder? order)
    {
        if (_lGroveVista is not LVista vista)
        {
            return;
        }

        vista.LVistaOrderSet(CPanel.CPanelOrderRead(order) ?? vista.LVistaOrder);
    }

    public void LXieshengReset()
    {
        _lGroveVista?.LVistaSelect(null);
        LXieshengPanel.CPanelEntryClose();
        LXieshengChanged?.Invoke();
    }

    public void LXieshengRowsUpdate()
    {
        LXieshengChanged?.Invoke();
        LXieshengPanel.CPanelRowsUpdate();
    }

    public void LXieshengEntryHandle(CBulletin bulletin)
    {
        LXieshengChanged?.Invoke();
        LXieshengPanel.CPanelEntryUpdate(bulletin);
    }

    public void LXieshengStemSelect(long? id)
    {
        if (id is not long stem)
        {
            return;
        }

        _lGroveVista?.LVistaToggle(stem);
        LXieshengPanel.CPanelEntryClose();
        LXieshengChanged?.Invoke();
    }

    public void LXieshengStemShow(string language, string? key)
    {
        ArgumentNullException.ThrowIfNull(language);

        if (string.IsNullOrEmpty(key))
        {
            return;
        }

        if (_lPhonologyPort.LEngineStemFind(language, key) is not LStem found)
        {
            return;
        }

        _lGroveVista?.LVistaSelect(null);
        LXieshengStemSelect(found.LStemId);
    }

    public void LXieshengGlyphSelect(string? character)
    {
        if (string.IsNullOrEmpty(character))
        {
            return;
        }

        if (!LXieshengStemShown)
        {
            return;
        }

        if (!LXieshengPanel.CPanelLeaveConfirm())
        {
            return;
        }

        LXieshengGlyphChosen?.Invoke(character, LXieshengLanguage);
    }

    public void LXieshengGroveAttach(CSubject subject, Action<CBulletin> observer)
    {
        ArgumentNullException.ThrowIfNull(observer);

        _lGroveVista?.LVistaObserverAttach(CPanel.CPanelSubjectRead(subject), LXieshengBulletinSend);

        void LXieshengBulletinSend(LBulletin bulletin)
        {
            observer(new CBulletin(bulletin.LBulletinId, bulletin.LBulletinStored));
        }
    }

    public Task LXieshengPortraitPrint(CPortraitLabel label, CPressTicket ticket)
    {
        if (!LXieshengPanel.CPanelPressAllowed)
        {
            return Task.CompletedTask;
        }

        return _lPortraitPort.LEnginePortraitPrint(
            LXieshengPanel.CPanelVista, QPortrait.QPortraitLabelRead(label), QPortrait.QPortraitTicketRead(ticket));
    }

    public Task LXieshengPortraitExport(string path, CPortraitMedium format, CPortraitLabel label)
    {
        return _lPortraitPort.LEnginePortraitExport(
            LXieshengPanel.CPanelVista,
            path,
            QPortrait.QPortraitMediumRead(format),
            QPortrait.QPortraitLabelRead(label));
    }

    public string LXieshengFileRead()
    {
        return LVista.LVistaFileRead(LXieshengPanel.CPanelVista);
    }
}

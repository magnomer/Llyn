using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIDeportment;

public sealed class LYunjing
{
    private readonly LPhonologyPort _lPhonologyPort;

    private readonly LPortraitPort _lPortraitPort;

    private readonly LSettingsPort _lSettingsPort;

    private LVista? _lShengmuVista;

    private LVista? _lYunmuVista;

    private LVista? _lXiaoyunVista;

    private bool _lYunjingFinalSide;

    private int _lShengmuCount;

    private int _lYunmuCount;

    private int _lXiaoyunCount;

    internal LYunjing(
        LPhonologyPort phonology,
        LPortraitPort portraits,
        LSettingsPort settings,
        LEditor editor,
        LLectern lectern,
        Func<bool> shownSeam,
        Func<bool> leaveSeam,
        Func<bool> deleteSeam)
    {
        ArgumentNullException.ThrowIfNull(phonology);
        ArgumentNullException.ThrowIfNull(portraits);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(editor);
        ArgumentNullException.ThrowIfNull(lectern);

        _lPhonologyPort = phonology;
        _lPortraitPort = portraits;
        _lSettingsPort = settings;
        LYunjingEditor = editor;
        LYunjingPanel = new LPanel(
            "Yunjing.LoadFailed", "Scribe.DeleteFailed",
            editor.LEditorStudio.CEditorDesk.CDeskChangeCheck, shownSeam, leaveSeam, deleteSeam);
        LYunjingPanel.LPanelCleared += LYunjingEditorClear;
        LYunjingPanel.LPanelEdited += LYunjingEditorOpen;
        LYunjingPanel.LPanelDraftChanged += lectern.LLecternDraftShow;
        LYunjingPanel.LPanelCleared += lectern.LLecternClear;
    }

    private void LYunjingEditorClear()
    {
        LYunjingEditor.LEditorStudio.CEditorEntryOpen(null);
    }

    private void LYunjingEditorOpen(long id)
    {
        LYunjingEditor.LEditorStudio.CEditorEntryOpen(id);
    }

    public event Action? LYunjingChanged;

    public event Action<string, string>? LYunjingGlyphChosen;

    public LEditor LYunjingEditor { get; }

    public LPanel LYunjingPanel { get; }

    public bool LYunjingAllowed => _lPhonologyPort.LEngineBookFind() is not null;

    public bool LYunjingDiweiShown => LYunjingDiweiChosen && !LYunjingPanel.LPanelModeEnabled;

    public bool LYunjingDisplayShown => !LYunjingDiweiShown && !LYunjingPanel.LPanelEditing;

    public bool LYunjingEditorShown => LYunjingPanel.LPanelEditing;

    public string LYunjingDiweiKey => _lYunjingFinalSide ? "Yunjing.Yunmu" : "Yunjing.Shengmu";

    public bool LYunjingShengmuEmpty => _lShengmuCount == 0;

    public bool LYunjingYunmuEmpty => _lYunmuCount == 0;

    public bool LYunjingXiaoyunEmpty => _lXiaoyunCount == 0;

    public string LYunjingShengmuKey => LYunjingPlumbQueried ? "Yunjing.ShengmuUnmatched" : "Yunjing.ShengmuEmpty";

    public string LYunjingYunmuKey => LYunjingFathomQueried ? "Yunjing.YunmuUnmatched" : "Yunjing.YunmuEmpty";

    public string LYunjingXiaoyunKey => LYunjingDiweiChosen ? LYunjingVacantKey : "Yunjing.XiaoyunEmpty";

    private string LYunjingVacantKey =>
        LYunjingBeaconQueried ? "Yunjing.XiaoyunUnmatched" : "Yunjing.XiaoyunVacant";

    private bool LYunjingPlumbQueried => _lShengmuVista?.LVistaQueried ?? false;

    private bool LYunjingFathomQueried => _lYunmuVista?.LVistaQueried ?? false;

    private bool LYunjingBeaconQueried => _lXiaoyunVista?.LVistaQueried ?? false;

    private bool LYunjingDiweiChosen => LYunjingSideVista?.LVistaChosen is not null;

    private LVista? LYunjingSideVista => _lYunjingFinalSide ? _lYunmuVista : _lShengmuVista;

    private string LYunjingLanguage =>
        _lPhonologyPort.LEngineDiweiRead(LYunjingSideVista?.LVistaChosen)?.LDiweiLanguage
        ?? _lPhonologyPort.LEngineBookFind()
        ?? string.Empty;

    internal void LYunjingVistaRestore(LVista shengmu, LVista yunmu, LVista xiaoyun)
    {
        ArgumentNullException.ThrowIfNull(shengmu);
        ArgumentNullException.ThrowIfNull(yunmu);
        ArgumentNullException.ThrowIfNull(xiaoyun);

        _lShengmuVista = shengmu;
        _lYunmuVista = yunmu;
        _lXiaoyunVista = xiaoyun;
        LYunjingPanel.LPanelVistaRestore(xiaoyun);
        LYunjingEditor.LEditorStudio.CEditorVistaRestore(xiaoyun);
    }

    public IReadOnlyList<CDiwei> LYunjingShengmuRead()
    {
        IReadOnlyList<CDiwei> rows = LYunjingDiweiBuild(LYunjingDiweiFind(_lShengmuVista, LDiwei.LDiweiInitial));
        _lShengmuCount = rows.Count;
        return rows;
    }

    public IReadOnlyList<CDiwei> LYunjingYunmuRead()
    {
        IReadOnlyList<CDiwei> rows = LYunjingDiweiBuild(LYunjingDiweiFind(_lYunmuVista, LDiwei.LDiweiRime));
        _lYunmuCount = rows.Count;
        return rows;
    }

    internal static IReadOnlyList<CDiwei> LYunjingDiweiBuild(IReadOnlyList<LDiwei> rows)
    {
        return LSplice.LSpliceBuild(
            rows,
            static row => new CDiwei(row.LDiweiId, row.LDiweiKey, row.LDiweiCount, row.LDiweiFinal, row.LDiweiChosen));
    }

    private IReadOnlyList<LDiwei> LYunjingDiweiFind(LVista? vista, string kind)
    {
        if (vista is null)
        {
            return [];
        }

        if (!LYunjingAllowed)
        {
            return [];
        }

        return _lPhonologyPort.LEngineDiweiFind(vista, LYunjingLanguage, kind);
    }

    public IReadOnlyList<CVistaRow> LYunjingXiaoyunRead()
    {
        IReadOnlyList<CVistaRow> rows = LSplice.LSpliceBuild(LYunjingXiaoyunFind(), CPanel.CPanelRowRead);
        _lXiaoyunCount = rows.Count;
        return rows;
    }

    private IReadOnlyList<LVistaRow> LYunjingXiaoyunFind()
    {
        if (_lShengmuVista is not LVista onset)
        {
            return [];
        }

        if (_lYunmuVista is not LVista rime)
        {
            return [];
        }

        if (_lXiaoyunVista is not LVista vista)
        {
            return [];
        }

        return _lPhonologyPort.LEngineXiaoyunFind(LYunjingLanguage, onset, rime, vista);
    }

    public CDiweiPage LYunjingDiweiRead()
    {
        return LYunjingPageBuild(LYunjingDiweiShown
            ? _lPhonologyPort.LEngineDiweiResolve(LYunjingSideVista?.LVistaChosen, _lSettingsPort.LEngineTextFind)
            : LDiweiPage.LDiweiPageBlank);
    }

    internal static CDiweiPage LYunjingPageBuild(LDiweiPage page)
    {
        ArgumentNullException.ThrowIfNull(page);

        return new CDiweiPage(
            page.LDiweiPageLanguage,
            page.LDiweiPageKey,
            LSplice.LSpliceBuild(page.LDiweiPageSections, LYunjingSectionBuild),
            page.LDiweiPageEmpty);
    }

    private static CDiweiSection LYunjingSectionBuild(LDiweiSection section)
    {
        return new CDiweiSection(
            section.LDiweiSectionLabel,
            LSplice.LSpliceBuild(section.LDiweiSectionLines, LYunjingLineBuild),
            LSplice.LSpliceBuild(
                section.LDiweiSectionTallies, line => LYunjingTallyBuild(line, section.LDiweiSectionRespelled)),
            section.LDiweiSectionSwitched,
            section.LDiweiSectionRespelled);
    }

    private static CDiweiLine LYunjingLineBuild(LDiweiLine line)
    {
        return new CDiweiLine(
            line.LDiweiLineReading, line.LDiweiLineLabel, line.LDiweiLineRounded, line.LDiweiLineCharacters);
    }

    private static CTally LYunjingTallyBuild(LTallyLine line, bool respelled)
    {
        return new CTally(
            line.LTallyLineLanguage,
            line.LTallyLineKind,
            LSplice.LSpliceBuild(line.LTallyLineRead(respelled), LYunjingMarkBuild));
    }

    private static CTallyMark LYunjingMarkBuild(LTallyMark mark)
    {
        return new CTallyMark(mark.LTallyMarkText, mark.LTallyMarkCount, mark.LTallyMarkCharacters);
    }

    public void LYunjingPlumbSet(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        _lShengmuVista?.LVistaQuerySet(query);
    }

    public void LYunjingFathomSet(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        _lYunmuVista?.LVistaQuerySet(query);
    }

    public void LYunjingBeaconSet(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        _lXiaoyunVista?.LVistaQuerySet(query);
    }

    public void LYunjingLadderSet(CCatalogOrder? order)
    {
        LYunjingOrderSet(_lShengmuVista, order);
    }

    public void LYunjingStairSet(CCatalogOrder? order)
    {
        LYunjingOrderSet(_lYunmuVista, order);
    }

    private static void LYunjingOrderSet(LVista? vista, CCatalogOrder? order)
    {
        if (vista is null)
        {
            return;
        }

        vista.LVistaOrderSet(LPanel.LPanelOrderRead(order) ?? vista.LVistaOrder);
    }

    public void LYunjingReset()
    {
        _lShengmuVista?.LVistaSelect(null);
        _lYunmuVista?.LVistaSelect(null);
        LYunjingPanel.LPanelClear();
        LYunjingChanged?.Invoke();
    }

    public void LYunjingRowsUpdate()
    {
        LYunjingChanged?.Invoke();
        LYunjingPanel.LPanelRowsUpdate();
    }

    public void LYunjingEntryHandle(CBulletin bulletin)
    {
        LYunjingChanged?.Invoke();
        LYunjingPanel.LPanelEntryHandle(bulletin);
    }

    public void LYunjingDiweiSelect(long? id, bool? final)
    {
        if (id is not long cell)
        {
            return;
        }

        if (final is not bool rime)
        {
            return;
        }

        _lYunjingFinalSide = rime;
        LYunjingSideVista?.LVistaToggle(cell);
        LYunjingPanel.LPanelClear();
        LYunjingChanged?.Invoke();
    }

    public void LYunjingDiweiShow(string language, string kind, string key)
    {
        ArgumentNullException.ThrowIfNull(language);
        ArgumentNullException.ThrowIfNull(kind);
        ArgumentNullException.ThrowIfNull(key);

        LYunjingDiweiOpen(_lPhonologyPort.LEngineDiweiFind(language, kind, key));
    }

    private void LYunjingDiweiOpen(LDiwei? found)
    {
        if (found is null)
        {
            return;
        }

        _lShengmuVista?.LVistaSelect(null);
        _lYunmuVista?.LVistaSelect(null);
        LYunjingDiweiSelect(found.LDiweiId, found.LDiweiFinal);
    }

    public void LYunjingTallySet(bool? respelled)
    {
        if (respelled is not bool chosen)
        {
            return;
        }

        _lPhonologyPort.LEngineTallySave(chosen);
        LYunjingChanged?.Invoke();
    }

    public void LYunjingGlyphSelect(string? character)
    {
        if (string.IsNullOrEmpty(character))
        {
            return;
        }

        if (!LYunjingDiweiShown)
        {
            return;
        }

        if (!LYunjingPanel.LPanelLeaveConfirm())
        {
            return;
        }

        LYunjingGlyphChosen?.Invoke(character, LYunjingLanguage);
    }

    public Task LYunjingPortraitPrint(CPortraitLabel label, CPressTicket ticket)
    {
        if (!LYunjingPanel.LPanelPressAllowed)
        {
            return Task.CompletedTask;
        }

        return _lPortraitPort.LEnginePortraitPrint(
            LYunjingPanel.LPanelVista, QPortrait.QPortraitLabelRead(label), QPortrait.QPortraitTicketRead(ticket));
    }

    internal void LYunjingVistaRestore(CAtelier atelier)
    {
        ArgumentNullException.ThrowIfNull(atelier);

        LYunjingVistaRestore(
            atelier.CAtelierVistaStart("yunjing", null, CCatalogOrder.CCatalogOrderName),
            atelier.CAtelierVistaStart("yunmu", null, CCatalogOrder.CCatalogOrderName),
            atelier.CAtelierVistaStart(
                "xiaoyun", CSubject.CSubjectEntry, CCatalogOrder.CCatalogOrderHeadword));
    }

    public void LYunjingShengmuAttach(CSubject subject, Action<CBulletin> observer)
    {
        LYunjingObserverAttach(_lShengmuVista, subject, observer);
    }

    public void LYunjingYunmuAttach(CSubject subject, Action<CBulletin> observer)
    {
        LYunjingObserverAttach(_lYunmuVista, subject, observer);
    }

    private static void LYunjingObserverAttach(LVista? vista, CSubject subject, Action<CBulletin> observer)
    {
        ArgumentNullException.ThrowIfNull(observer);

        vista?.LVistaObserverAttach(LPanel.LPanelSubjectRead(subject), LYunjingBulletinSend);

        void LYunjingBulletinSend(LBulletin bulletin)
        {
            observer(new CBulletin(bulletin.LBulletinId, bulletin.LBulletinStored));
        }
    }

    public CCatalogOrder LYunjingLadder =>
        LPanel.LPanelOrderRead(_lShengmuVista?.LVistaOrder ?? LCatalogOrder.LCatalogOrderName);

    public CCatalogOrder LYunjingStair =>
        LPanel.LPanelOrderRead(_lYunmuVista?.LVistaOrder ?? LCatalogOrder.LCatalogOrderName);

    public string LYunjingFileRead()
    {
        return LVista.LVistaFileRead(LYunjingPanel.LPanelVista);
    }

    public Task LYunjingPortraitExport(string path, CPortraitMedium format, CPortraitLabel label)
    {
        return _lPortraitPort.LEnginePortraitExport(
            LYunjingPanel.LPanelVista,
            path,
            QPortrait.QPortraitMediumRead(format),
            QPortrait.QPortraitLabelRead(label));
    }
}

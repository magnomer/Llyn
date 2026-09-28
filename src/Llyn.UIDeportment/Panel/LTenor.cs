using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIDeportment;

public sealed class LTenor
{
    private readonly LEntryPort _lEntryPort;

    private readonly LPortraitPort _lPortraitPort;

    private readonly LSettingsPort _lSettingsPort;

    private LVista? _lTenorVista;

    private LVista? _lTenorCohort;

    internal LTenor(
        LEntryPort entries,
        LPortraitPort portraits,
        LSettingsPort settings,
        LEditor editor,
        LLectern lectern,
        Func<bool> shownSeam,
        CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(portraits);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(editor);
        ArgumentNullException.ThrowIfNull(lectern);

        _lEntryPort = entries;
        _lPortraitPort = portraits;
        _lSettingsPort = settings;
        LTenorEditor = editor;

        LTenorPanel = new CPanel(
            envoy, "Register.LoadFailed", "Scribe",
            editor.LEditorStudio.CEditorDesk.CDeskChangeCheck, editor.LEditorStudio.CEditorFinish,
            shownSeam);
        LTenorPanel.CPanelCleared += LTenorEditorClear;
        LTenorPanel.CPanelEdited += LTenorEditorOpen;
        LTenorPanel.CPanelDraftChanged += lectern.LLecternDraftShow;
        LTenorPanel.CPanelCleared += lectern.LLecternClear;
    }

    public LEditor LTenorEditor { get; }

    public CPanel LTenorPanel { get; }

    private void LTenorEditorClear()
    {
        LTenorEditor.LEditorStudio.CEditorEntryOpen(null);
    }

    private void LTenorEditorOpen(long id)
    {
        LTenorEditor.LEditorStudio.CEditorEntryOpen(id);
    }

    public void LTenorEntryCreate()
    {
        LTenorPanel.CPanelFreshOpen();
        if (LTenorChosen is long register)
        {
            LTenorEditor.LEditorStudio.CEditorRegisterAdd(register);
        }
    }

    public bool LTenorCoinageCheck()
    {
        return LTenorChosen is null && !LTenorPanel.CPanelBinEnabled;
    }

    public long? LTenorChosen => _lTenorVista?.LVistaChosen;

    public bool LTenorFiltered => _lTenorVista?.LVistaFiltered ?? false;

    public string LTenorEmptyRead(string query)
    {
        return string.IsNullOrWhiteSpace(query) ? "Register.Vacant" : "Register.Unmatched";
    }

    internal void LTenorVistaRestore(LVista vista, LVista cohort)
    {
        ArgumentNullException.ThrowIfNull(vista);
        ArgumentNullException.ThrowIfNull(cohort);

        _lTenorVista = vista;
        _lTenorCohort = cohort;
        LTenorPanel.CPanelVistaRestore(cohort);
        LTenorEditor.LEditorStudio.CEditorVistaRestore(cohort);
    }

    public void LTenorSoundingSet(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        _lTenorVista?.LVistaQuerySet(query);
    }

    public void LTenorDegreeSet(CCatalogOrder? order)
    {
        if (_lTenorVista is not LVista vista)
        {
            return;
        }

        vista.LVistaOrderSet(CPanel.CPanelOrderRead(order) ?? vista.LVistaOrder);
    }

    public void LTenorGrilleSet(CCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        _lTenorVista?.LVistaFilterSet(filter.CCatalogFilterHidden);
    }

    public void LTenorQuestSet(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        _lTenorCohort?.LVistaQuerySet(query);
    }

    public void LTenorSelect(long? id)
    {
        _lTenorVista?.LVistaSelect(id);
    }

    public IReadOnlyList<CCatalogRegister> LTenorRowsRead()
    {
        return _lTenorVista is LVista vista ? LTenorRegisterRead(_lEntryPort.LEngineRegisterFind(vista)) : [];
    }

    internal static IReadOnlyList<CCatalogRegister> LTenorRegisterRead(IReadOnlyList<LCatalogRegister> rows)
    {
        return LSplice.LSpliceBuild(
            rows,
            static row => new CCatalogRegister(
                new CRegister(
                    row.LCatalogRegisterStored.LRegisterId, row.LCatalogRegisterStored.LRegisterName.LStateValueShow()),
                row.LCatalogRegisterUsage,
                row.LCatalogRegisterChosen));
    }

    public IReadOnlyList<CVistaRow> LTenorCohortRead()
    {
        return LSplice.LSpliceBuild(
            _lEntryPort.LEngineEntryFind(_lTenorVista, _lTenorCohort), CPanel.CPanelRowRead);
    }

    public long LTenorRegisterCreate(string name)
    {
        return _lEntryPort.LEngineRegisterCreate(name).LRegisterId;
    }

    public IReadOnlyList<string> LTenorLanguageRead()
    {
        return _lSettingsPort.LEngineLanguageRead();
    }

    public Task LTenorPortraitPrint(CPortraitLabel label, CPressTicket ticket)
    {
        return _lPortraitPort.LEnginePortraitPrint(
            _lTenorCohort, QPortrait.QPortraitLabelRead(label), QPortrait.QPortraitTicketRead(ticket));
    }

    internal void LTenorVistaRestore(CAtelier atelier)
    {
        ArgumentNullException.ThrowIfNull(atelier);

        LTenorVistaRestore(
            atelier.CAtelierVistaStart(
                "tenor", CSubject.CSubjectRegister, CCatalogOrder.CCatalogOrderName),
            atelier.CAtelierVistaStart(
                "cohort", CSubject.CSubjectEntry, CCatalogOrder.CCatalogOrderHeadword));
    }

    public void LTenorObserverAttach(CSubject subject, Action<CBulletin> observer)
    {
        ArgumentNullException.ThrowIfNull(observer);

        _lTenorVista?.LVistaObserverAttach(CPanel.CPanelSubjectRead(subject), LTenorBulletinSend);

        void LTenorBulletinSend(LBulletin bulletin)
        {
            observer(new CBulletin(bulletin.LBulletinId, bulletin.LBulletinStored));
        }
    }

    public CCatalogOrder LTenorOrder =>
        CPanel.CPanelOrderRead(_lTenorVista?.LVistaOrder ?? LCatalogOrder.LCatalogOrderHeadword);

    public CCatalogFilter LTenorFilter =>
        CPanel.CPanelFilterRead(_lTenorVista?.LVistaFilter ?? LCatalogFilter.LCatalogFilterEmpty);

    public string LTenorFileRead()
    {
        return LVista.LVistaFileRead(_lTenorCohort);
    }

    public Task LTenorPortraitExport(string path, CPortraitMedium format, CPortraitLabel label)
    {
        return _lPortraitPort.LEnginePortraitExport(
            _lTenorCohort, path, QPortrait.QPortraitMediumRead(format), QPortrait.QPortraitLabelRead(label));
    }
}

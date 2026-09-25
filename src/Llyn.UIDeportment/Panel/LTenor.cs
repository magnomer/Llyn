using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

    public LTenor(
        LEntryPort entries,
        LPortraitPort portraits,
        LSettingsPort settings,
        LEditor editor,
        Func<bool> shownSeam,
        Func<bool> leaveSeam,
        Func<bool> deleteSeam)
    {
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(portraits);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(editor);

        _lEntryPort = entries;
        _lPortraitPort = portraits;
        _lSettingsPort = settings;
        LTenorEditor = editor;

        LTenorPanel = new LPanel(
            "Register.LoadFailed", "Scribe.DeleteFailed",
            editor.LEditorDesk.LDeskChangeCheck, shownSeam, leaveSeam, deleteSeam);
        LTenorPanel.LPanelCleared += LTenorEditorClear;
        LTenorPanel.LPanelEdited += LTenorEditorOpen;
    }

    public LEditor LTenorEditor { get; }

    public LPanel LTenorPanel { get; }

    private void LTenorEditorClear()
    {
        LTenorEditor.LEditorOpen(null);
    }

    private void LTenorEditorOpen(long id)
    {
        LTenorEditor.LEditorOpen(id);
    }

    public void LTenorEntryCreate()
    {
        LTenorPanel.LPanelFreshOpen();
        if (LTenorChosen is long register)
        {
            LTenorEditor.LEditorRegisterAdd(register);
        }
    }

    public long? LTenorChosen => _lTenorVista?.LVistaChosen;

    public bool LTenorFiltered => _lTenorVista?.LVistaFiltered ?? false;

    public void LTenorVistaRestore(LVista vista, LVista cohort)
    {
        ArgumentNullException.ThrowIfNull(vista);
        ArgumentNullException.ThrowIfNull(cohort);

        _lTenorVista = vista;
        _lTenorCohort = cohort;
        LTenorPanel.LPanelVistaRestore(cohort);
        LTenorEditor.LEditorVistaRestore(cohort);
    }

    public void LTenorSoundingSet(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        _lTenorVista?.LVistaQuerySet(query);
    }

    public void LTenorDegreeSet(LCatalogOrder? order)
    {
        if (_lTenorVista is not LVista vista)
        {
            return;
        }

        vista.LVistaOrderSet(order ?? vista.LVistaOrder);
    }

    public void LTenorGrilleSet(LCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        _lTenorVista?.LVistaFilterSet(filter);
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

    public IReadOnlyList<LCatalogRegister> LTenorRowsRead()
    {
        return _lTenorVista is LVista vista ? _lEntryPort.LEngineRegisterFind(vista) : [];
    }

    public IReadOnlyList<LVistaRow> LTenorCohortRead()
    {
        return _lEntryPort.LEngineEntryFind(_lTenorVista, _lTenorCohort);
    }

    public LRegister LTenorRegisterCreate(string name)
    {
        return _lEntryPort.LEngineRegisterCreate(name);
    }

    public IReadOnlyList<string> LTenorLanguageRead()
    {
        return _lSettingsPort.LEngineLanguageRead();
    }

    public Task LTenorPortraitPrint(LPortraitLabel label, LPressTicket ticket)
    {
        return _lPortraitPort.LEnginePortraitPrint(_lTenorCohort, label, ticket);
    }

    public void LTenorVistaRestore(LWindow window)
    {
        ArgumentNullException.ThrowIfNull(window);

        LTenorVistaRestore(
            window.LWindowVistaStart("tenor", LSubject.LSubjectRegister, LCatalogOrder.LCatalogOrderName),
            window.LWindowVistaStart("cohort", LSubject.LSubjectEntry, LCatalogOrder.LCatalogOrderHeadword));
    }

    public void LTenorObserverAttach(LSubject subject, Action<LBulletin> observer)
    {
        _lTenorVista?.LVistaObserverAttach(subject, observer);
    }

    public LCatalogOrder LTenorOrder => _lTenorVista?.LVistaOrder ?? LCatalogOrder.LCatalogOrderHeadword;

    public LCatalogFilter LTenorFilter => _lTenorVista?.LVistaFilter ?? LCatalogFilter.LCatalogFilterEmpty;

    public string LTenorFileRead()
    {
        return LVista.LVistaFileRead(_lTenorCohort);
    }

    public Task LTenorPortraitExport(string path, LPortraitFormat format, LPortraitLabel label)
    {
        return _lPortraitPort.LEnginePortraitExport(_lTenorCohort, path, format, label);
    }
}

using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIDeportment;

public sealed class LWing
{
    private readonly LEntryPort _lEntryPort;

    private readonly LSettingsPort _lSettingsPort;

    private LVista? _lWingVista;

    public LWing(LEntryPort entries, LPhonologyPort phonology, LSettingsPort settings)
    {
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(phonology);
        ArgumentNullException.ThrowIfNull(settings);

        _lEntryPort = entries;
        _lSettingsPort = settings;
        LWingDisplay = new LDisplay(entries, phonology, settings);
    }

    public LDisplay LWingDisplay { get; }

    public long? LWingChosen => _lWingVista?.LVistaChosen;

    public bool LWingFiltered => _lWingVista?.LVistaFiltered ?? false;

    public bool LWingQueried => _lWingVista?.LVistaQueried ?? false;

    public void LWingVistaRestore(LVista vista)
    {
        ArgumentNullException.ThrowIfNull(vista);

        _lWingVista = vista;
        LWingDisplay.LDisplayVistaRestore(vista);
    }

    public void LWingQuerySet(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        _lWingVista?.LVistaQuerySet(query);
    }

    public void LWingOrderSet(string? choice)
    {
        if (choice is null)
        {
            return;
        }

        if (_lWingVista is not LVista vista)
        {
            return;
        }

        vista.LVistaOrderSet(LCatalog.LCatalogOrderParse(choice, vista.LVistaOrder));
    }

    public void LWingSieveSet(LCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        _lWingVista?.LVistaFilterSet(filter);
    }

    public void LWingSelect(long? id)
    {
        _lWingVista?.LVistaSelect(id);
    }

    public void LWingVistaRestore(LPosture posture, string tab)
    {
        ArgumentNullException.ThrowIfNull(posture);

        LWingVistaRestore(
            posture.LPostureVistaStart(tab, LSubject.LSubjectEntry, LCatalogOrder.LCatalogOrderHeadword, true));
    }

    public void LWingObserverAttach(LSubject subject, Action<LBulletin> observer)
    {
        _lWingVista?.LVistaObserverAttach(subject, observer);
    }

    public LCatalogOrder LWingOrder => _lWingVista?.LVistaOrder ?? LCatalogOrder.LCatalogOrderHeadword;

    public LCatalogFilter LWingFilter => _lWingVista?.LVistaFilter ?? LCatalogFilter.LCatalogFilterEmpty;

    public IReadOnlyList<LVistaRow> LWingRowsRead()
    {
        return _lWingVista is LVista vista ? _lEntryPort.LEngineEntryFind(vista) : [];
    }

    public LEntryDraft? LWingEntryLoad(long id)
    {
        return _lEntryPort.LEngineEntryLoad(id);
    }

    public IReadOnlyList<string> LWingLanguageRead()
    {
        return _lSettingsPort.LEngineLanguageRead();
    }

    public void LWingEntrySave()
    {
        if (_lWingVista is LVista vista)
        {
            if (vista.LVistaLeft)
            {
                _lSettingsPort.LEngineLeftSave(vista.LVistaChosen);
                return;
            }
        }

        _lSettingsPort.LEngineRightSave(_lWingVista?.LVistaChosen);
    }
}

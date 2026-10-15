using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CAccession
{
    private readonly LDeckPort _cAccessionPort;

    private readonly CEnvoy _cAccessionEnvoy;

    private readonly LSettingsPort _cAccessionSettings;

    private readonly Func<long?> _cAccessionChosen;

    private IReadOnlyList<CAccessionRow> _cAccessionShown = [];

    internal CAccession(CAtelier atelier, CEnvoy envoy, Func<long?> chosen)
    {
        ArgumentNullException.ThrowIfNull(atelier);
        ArgumentNullException.ThrowIfNull(envoy);
        ArgumentNullException.ThrowIfNull(chosen);

        _cAccessionPort = atelier.CAtelierEntryBundle.CEntryBundleDeck;
        _cAccessionEnvoy = envoy;
        _cAccessionSettings = atelier.CAtelierSettingsPort;
        _cAccessionChosen = chosen;
        CAccessionAperture = new CAperture(
            envoy,
            _cAccessionSettings,
            atelier.CAtelierEntryBundle.CEntryBundleVista,
            "Accession.LoadFailed",
            "Accession.Empty",
            "Accession.Unmatched");
        atelier.CAtelierWorkspace.LWorkspaceVistaAdd(() => LAccessionVistaRestore(atelier));
        LAccessionVistaRestore(atelier);
    }

    public event Action? CAccessionChanged;

    public CAperture CAccessionAperture { get; }

    public IReadOnlyList<CAccessionRow> CAccessionRead()
    {
        IReadOnlyList<CAccessionRow> rows = [];
        if (_cAccessionChosen() is long deck && CAccessionAperture.CApertureVista is LVista vista)
        {
            try
            {
                rows = _cAccessionPort.LEngineAccessionFind(vista, deck)
                    .Select(static found => new CAccessionRow(
                        found.LAccessionRow.LVistaRowId,
                        found.LAccessionRow.LVistaRowHeadword,
                        found.LAccessionRow.LVistaRowEpithet,
                        found.LAccessionBound))
                    .ToList();
            }
            catch (Exception exception)
            {
                CLedger.LLedgerFailureShow(
                    _cAccessionEnvoy, _cAccessionSettings, "Accession.LoadFailed", exception);
            }
        }

        _cAccessionShown = rows;
        CAccessionAperture.CApertureCountSet(rows.Count);
        return rows;
    }

    public void CAccessionAdd(long entry)
    {
        LAccessionSave([entry]);
    }

    public void CAccessionShownAdd()
    {
        List<long> entries = _cAccessionShown
            .Where(static row => !row.CAccessionRowBound)
            .Select(static row => row.CAccessionRowEntry)
            .ToList();
        if (entries.Count > 0)
        {
            LAccessionSave(entries);
        }
    }

    private void LAccessionVistaRestore(CAtelier atelier)
    {
        CAccessionAperture.CApertureRestore(atelier.CAtelierVistaStart(
            "accession", CSubject.CSubjectEntry, CCatalogOrder.CCatalogOrderHeadword));
        CAccessionAperture.CApertureObserverAttach(
            CSubject.CSubjectVista, _ => CAccessionAperture.CApertureRowsResonate());
    }

    private void LAccessionSave(IReadOnlyList<long> entries)
    {
        if (_cAccessionChosen() is not long deck)
        {
            return;
        }

        try
        {
            _cAccessionPort.LEngineCueAdd(deck, entries);
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cAccessionEnvoy, _cAccessionSettings, "Deck.SaveFailed", exception);
            return;
        }

        CAccessionChanged?.Invoke();
    }
}

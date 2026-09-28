using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class COeuvre
{
    private readonly LEntryPort _cOeuvreEntryPort;

    private LVista? _cOeuvreRoll;

    private LVista? _cOeuvreVista;

    private int _cOeuvreCount;

    internal COeuvre(LEntryPort entries, CEnvoy envoy, Func<bool> shownSeam)
    {
        ArgumentNullException.ThrowIfNull(entries);

        _cOeuvreEntryPort = entries;
        COeuvrePanel = new CPanel(
            envoy, "Source.LoadFailed", null,
            static () => false, static _ => true, shownSeam);
        COeuvrePanel.CPanelDraftChanged += LOeuvreColophonUpdate;
    }

    public event Action<CColophon>? COeuvreColophonChanged;

    public CPanel COeuvrePanel { get; }

    public bool COeuvreEmpty => _cOeuvreCount == 0;

    public string COeuvreEmptyKey => LOeuvreAuthorChosen ? LOeuvreVacantKey : "Source.Empty";

    private string LOeuvreVacantKey => LOeuvreNarrowed ? "Guild.Unmatched" : "Guild.Vacant";

    private bool LOeuvreAuthorChosen => _cOeuvreRoll?.LVistaChosen is not null;

    private bool LOeuvreNarrowed => (_cOeuvreVista?.LVistaQueried ?? false) || (_cOeuvreRoll?.LVistaFiltered ?? false);

    private bool LOeuvreSourceHeld => _cOeuvreVista?.LVistaChosen is not null;

    internal void LOeuvreVistaRestore(LVista roll, LVista vista)
    {
        ArgumentNullException.ThrowIfNull(roll);
        ArgumentNullException.ThrowIfNull(vista);

        vista.LVistaQuerySet(_cOeuvreVista?.LVistaQuery ?? string.Empty);
        _cOeuvreRoll = roll;
        _cOeuvreVista = vista;
        COeuvrePanel.CPanelVistaRestore(vista);
    }

    public IReadOnlyList<CCatalogReference> COeuvreRowsRead()
    {
        IReadOnlyList<CCatalogReference> rows =
            COeuvreReferenceRead(_cOeuvreEntryPort.LEngineOeuvreFind(_cOeuvreRoll, _cOeuvreVista));
        _cOeuvreCount = rows.Count;
        if (LOeuvreSourceHeld && !rows.Any(static row => row.CCatalogReferenceChosen))
        {
            COeuvrePanel.CPanelEntryClose();
        }

        return rows;
    }

    public void COeuvreQuerySet(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        _cOeuvreVista?.LVistaQuerySet(query);
    }

    public string COeuvreTallyRead()
    {
        return _cOeuvreEntryPort.LEngineTallyRead(_cOeuvreVista?.LVistaChosen);
    }

    private void LOeuvreColophonUpdate(LDraft draft)
    {
        COeuvreColophonChanged?.Invoke(LOeuvreColophonRead(_cOeuvreEntryPort, draft));
    }

    internal static CColophon LOeuvreColophonRead(LEntryPort entries, LDraft draft)
    {
        ArgumentNullException.ThrowIfNull(entries);

        return LOeuvreColophonRead(entries.LEngineColophonRead(draft));
    }

    private static CColophon LOeuvreColophonRead(LColophon sheet)
    {
        return new CColophon(
            sheet.LColophonTitle,
            sheet.LColophonTitleFaint,
            sheet.LColophonKind,
            sheet.LColophonKindShown,
            sheet.LColophonYear,
            sheet.LColophonYearFaint,
            sheet.LColophonYearShown,
            sheet.LColophonUrl,
            sheet.LColophonUrlFaint,
            sheet.LColophonUrlShown,
            sheet.LColophonNote,
            sheet.LColophonNoteFaint,
            sheet.LColophonNoteShown,
            sheet.LColophonAuthor,
            sheet.LColophonAuthorFaint,
            sheet.LColophonAuthorShown,
            sheet.LColophonTally);
    }

    internal IReadOnlyList<CCatalogAuthor> LOeuvreAuthorRead(IReadOnlyList<LCatalogAuthor> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);

        return rows
            .Select(row => new CCatalogAuthor(
                row.LCatalogAuthorStored.LAuthorId,
                row.LCatalogAuthorName,
                _cOeuvreEntryPort.LEngineWorkFormat(row.LCatalogAuthorWork),
                row.LCatalogAuthorUsage,
                row.LCatalogAuthorStored.LAuthorStored,
                row.LCatalogAuthorChosen))
            .ToList();
    }

    internal static CVita LOeuvreVitaRead(LVita vita)
    {
        ArgumentNullException.ThrowIfNull(vita);

        return new CVita(
            vita.LVitaName,
            vita.LVitaNamed,
            vita.LVitaWork,
            vita.LVitaTally,
            vita.LVitaFellows
                .Select(static fellow => new CFellow(fellow.LFellowId, fellow.LFellowName, fellow.LFellowShared))
                .ToList(),
            vita.LVitaUsages.Select(COeuvreUsageRead).ToList());
    }

    internal static CUsage COeuvreUsageRead(LUsage usage)
    {
        ArgumentNullException.ThrowIfNull(usage);

        return new CUsage(
            usage.LUsageId,
            usage.LUsageEntry,
            usage.LUsageName,
            usage.LUsageEpithet,
            usage.LUsageLanguage,
            CFolio.CFolioStateRead(usage.LUsageTitle),
            usage.LUsageQuoted,
            usage.LUsageCollocated);
    }

    internal static IReadOnlyList<CCatalogReference> COeuvreReferenceRead(IReadOnlyList<LCatalogReference> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);

        return rows
            .Select(static row => new CCatalogReference(
                row.LCatalogReferenceStored.LReferenceId,
                row.LCatalogReferenceName,
                row.LCatalogReferenceByline,
                LOeuvreCreditRead(
                    row.LCatalogReferenceWriter,
                    row.LCatalogReferenceStored.LReferenceAuthorState.LStateMarkUncertain),
                CFolio.CFolioStateRead(row.LCatalogReferenceStored.LReferenceYear),
                row.LCatalogReferenceUsage,
                row.LCatalogReferenceChosen))
            .ToList();
    }

    private static CStateValue LOeuvreCreditRead(string? credit, bool uncertain)
    {
        return credit is null ? new CStateValue(string.Empty, uncertain, false) : new CStateValue(credit, false, true);
    }
}

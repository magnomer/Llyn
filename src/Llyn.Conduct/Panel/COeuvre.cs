using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class COeuvre
{
    private readonly LEntryPort _cOeuvreEntryPort;

    private readonly LAuthorPort _cOeuvreAuthorPort;

    private readonly LReferencePort _cOeuvreReferencePort;

    private LVista? _cOeuvreRoll;

    private LVista? _cOeuvreVista;

    private int _cOeuvreCount;

    internal COeuvre(
        LEntryPort entries,
        LAuthorPort authors,
        LReferencePort references,
        LSettingsPort settings,
        LVistaPort vistas,
        CEnvoy envoy,
        Func<bool> shownSeam)
    {
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(authors);
        ArgumentNullException.ThrowIfNull(references);
        ArgumentNullException.ThrowIfNull(settings);

        _cOeuvreEntryPort = entries;
        _cOeuvreAuthorPort = authors;
        _cOeuvreReferencePort = references;
        COeuvrePanel = new CPanel(
            envoy,
            settings,
            vistas,
            "Source.LoadFailed", null,
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

    internal void LOeuvreObserverAttach(Action<Action> marshal)
    {
        ArgumentNullException.ThrowIfNull(marshal);

        COeuvrePanel.CPanelObserverAttach(CSubject.CSubjectVista, _ => marshal(COeuvrePanel.CPanelRowsResonate));
    }

    public IReadOnlyList<CCatalogReference> COeuvreRowsRead()
    {
        IReadOnlyList<CCatalogReference> rows =
            COeuvreReferenceRead(_cOeuvreAuthorPort.LEngineOeuvreFind(_cOeuvreRoll, _cOeuvreVista));
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
        COeuvreColophonChanged?.Invoke(LOeuvreColophonRead(_cOeuvreReferencePort, draft));
    }

    internal static CColophon LOeuvreColophonRead(LReferencePort references, LDraft draft)
    {
        ArgumentNullException.ThrowIfNull(references);

        return LOeuvreColophonRead(references.LEngineColophonRead(draft));
    }

    public IReadOnlyList<CReferenceKind> COeuvreKindRead()
    {
        return CImprint.LImprintKindRead(_cOeuvreReferencePort.LEngineKindRead());
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
                _cOeuvreAuthorPort.LEngineWorkFormat(row.LCatalogAuthorWork),
                row.LCatalogAuthorCount,
                row.LCatalogAuthorStored.LAuthorStored ? "guild" : "unlink",
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
                .Select(static fellow => new CFellow(fellow.LFellowId, fellow.LFellowName, fellow.LFellowCount))
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
                CStateWording.LStateWordingRead(
                    LOeuvreCreditRead(
                        row.LCatalogReferenceWriter,
                        row.LCatalogReferenceStored.LReferenceAuthorState.LStateMarkUncertain),
                    "Source.Unset"),
                CStateWording.LStateWordingRead(
                    CFolio.CFolioStateRead(row.LCatalogReferenceStored.LReferenceYear), "Source.Unset"),
                row.LCatalogReferenceUsage,
                row.LCatalogReferenceChosen))
            .ToList();
    }

    private static CStateValue LOeuvreCreditRead(string? credit, bool uncertain)
    {
        return credit is null ? new CStateValue(string.Empty, uncertain) : new CStateValue(credit, false);
    }
}

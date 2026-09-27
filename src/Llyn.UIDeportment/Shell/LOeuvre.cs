using System;
using System.Collections.Generic;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIDeportment;

public sealed class LOeuvre
{
    private readonly LEntryPort _lEntryPort;

    private readonly LSettingsPort _lSettingsPort;

    private LVista? _lOeuvreRoll;

    private LVista? _lOeuvreVista;

    private int _lOeuvreCount;

    internal LOeuvre(LEntryPort entries, LSettingsPort settings, Func<bool> shownSeam)
    {
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(settings);

        _lEntryPort = entries;
        _lSettingsPort = settings;
        LOeuvrePanel = new LPanel(
            "Source.LoadFailed", "Source.DeleteFailed",
            static () => false, shownSeam, static () => true, static () => false);
        LOeuvrePanel.LPanelDraftChanged += LOeuvreColophonUpdate;
    }

    public event Action<CColophon>? LOeuvreColophonChanged;

    public LPanel LOeuvrePanel { get; }

    public bool LOeuvreEmpty => _lOeuvreCount == 0;

    public string LOeuvreEmptyKey => LOeuvreAuthorChosen ? LOeuvreVacantKey : "Source.Empty";

    private string LOeuvreVacantKey => LOeuvreNarrowed ? "Guild.Unmatched" : "Guild.Vacant";

    private bool LOeuvreAuthorChosen => _lOeuvreRoll?.LVistaChosen is not null;

    private bool LOeuvreNarrowed => LOeuvreQueried || LOeuvreFiltered;

    private bool LOeuvreQueried => _lOeuvreVista?.LVistaQueried ?? false;

    private bool LOeuvreFiltered => _lOeuvreRoll?.LVistaFiltered ?? false;

    internal void LOeuvreVistaRestore(LVista roll, LVista vista)
    {
        ArgumentNullException.ThrowIfNull(roll);
        ArgumentNullException.ThrowIfNull(vista);

        _lOeuvreRoll = roll;
        _lOeuvreVista = vista;
        LOeuvrePanel.LPanelVistaRestore(vista);
    }

    public IReadOnlyList<CCatalogReference> LOeuvreRowsRead()
    {
        return LOeuvreReferenceRead(LOeuvreRowsApply(LOeuvreRowsFind()));
    }

    private IReadOnlyList<LCatalogReference> LOeuvreRowsFind()
    {
        if (_lOeuvreRoll is not LVista roll)
        {
            return [];
        }

        if (_lOeuvreVista is not LVista vista)
        {
            return [];
        }

        return _lEntryPort.LEngineOeuvreFind(roll, vista);
    }

    private IReadOnlyList<LCatalogReference> LOeuvreRowsApply(IReadOnlyList<LCatalogReference> rows)
    {
        _lOeuvreCount = rows.Count;
        if (LOeuvrePanel.LPanelBinEnabled)
        {
            if (!LOeuvreChosenCheck(rows))
            {
                LOeuvrePanel.LPanelClear();
            }
        }

        return rows;
    }

    private static bool LOeuvreChosenCheck(IReadOnlyList<LCatalogReference> rows)
    {
        foreach (LCatalogReference row in rows)
        {
            if (row.LCatalogReferenceChosen)
            {
                return true;
            }
        }

        return false;
    }

    public void LOeuvreQuerySet(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        _lOeuvreVista?.LVistaQuerySet(query);
    }

    public string LOeuvreTallyRead()
    {
        return LReference.LReferenceUsageFormat(
            LOeuvreUsageRead(_lOeuvreVista?.LVistaChosen), _lSettingsPort.LEngineTextRead);
    }

    private int LOeuvreUsageRead(long? id)
    {
        return id is long stored ? _lEntryPort.LEngineUsageRead(LOwner.LOwnerReference).GetValueOrDefault(stored) : 0;
    }

    private void LOeuvreColophonUpdate(LDraft draft)
    {
        LOeuvreColophonChanged?.Invoke(LOeuvreColophonRead(
            draft, LOeuvreTallyRead(), _lSettingsPort.LEngineTextRead, "The oeuvre draft holds no reference."));
    }

    internal static CColophon LOeuvreColophonRead(
        LDraft draft, string tally, Func<string, string> localize, string missing)
    {
        return LOeuvreColophonRead((draft.LDraftReference ?? throw new InvalidOperationException(missing))
            .LReferenceColophonRead(draft.LDraftAuthor, tally, localize));
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

    internal static IReadOnlyList<CCatalogReference> LOeuvreReferenceRead(IReadOnlyList<LCatalogReference> rows)
    {
        return LSplice.LSpliceBuild(
            rows,
            static row => new CCatalogReference(
                row.LCatalogReferenceStored.LReferenceId,
                row.LCatalogReferenceName,
                row.LCatalogReferenceByline,
                LOeuvreCreditRead(
                    row.LCatalogReferenceStored.LReferenceCreditRead(row.LCatalogReferenceCredit),
                    row.LCatalogReferenceStored.LReferenceAuthorState.LStateMarkUncertain),
                LCard.LCardStateRead(row.LCatalogReferenceStored.LReferenceYear),
                row.LCatalogReferenceUsage,
                row.LCatalogReferenceChosen));
    }

    internal static IReadOnlyList<CCatalogAuthor> LOeuvreAuthorRead(
        IReadOnlyList<LCatalogAuthor> rows, Func<string, string> localize)
    {
        return LSplice.LSpliceBuild(
            rows,
            row => new CCatalogAuthor(
                row.LCatalogAuthorStored.LAuthorId,
                row.LCatalogAuthorName,
                LVita.LVitaWorkFormat(row.LCatalogAuthorWork, localize),
                row.LCatalogAuthorUsage,
                row.LCatalogAuthorStored.LAuthorStored,
                row.LCatalogAuthorChosen));
    }

    internal static CVita LOeuvreVitaRead(LVita vita)
    {
        return new CVita(
            vita.LVitaName,
            vita.LVitaNamed,
            vita.LVitaWork,
            vita.LVitaTally,
            LSplice.LSpliceBuild(
                vita.LVitaFellows,
                static fellow => new CFellow(fellow.LFellowId, fellow.LFellowName, fellow.LFellowShared)),
            LSplice.LSpliceBuild(vita.LVitaUsages, LOeuvreUsageRead));
    }

    internal static CUsage LOeuvreUsageRead(LUsage usage)
    {
        return new CUsage(
            usage.LUsageId,
            usage.LUsageEntry,
            usage.LUsageName,
            usage.LUsageEpithet,
            usage.LUsageLanguage,
            LCard.LCardStateRead(usage.LUsageTitle),
            usage.LUsageQuoted,
            usage.LUsageCollocated);
    }

    private static CStateValue LOeuvreCreditRead(string? credit, bool uncertain)
    {
        return credit is null ? new CStateValue(string.Empty, uncertain, false) : new CStateValue(credit, false, true);
    }
}

using System;
using System.Collections.Generic;
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

    public LOeuvre(LEntryPort entries, LSettingsPort settings, Func<bool> shownSeam)
    {
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(settings);

        _lEntryPort = entries;
        _lSettingsPort = settings;
        LOeuvrePanel = new LPanel(
            "Source.LoadFailed", "Source.DeleteFailed",
            static () => false, shownSeam, static () => true, static () => false);
    }

    public LPanel LOeuvrePanel { get; }

    public bool LOeuvreEmpty => _lOeuvreCount == 0;

    public string LOeuvreEmptyKey => LOeuvreAuthorChosen ? LOeuvreVacantKey : "Source.Empty";

    private string LOeuvreVacantKey => LOeuvreNarrowed ? "Guild.Unmatched" : "Guild.Vacant";

    private bool LOeuvreAuthorChosen => _lOeuvreRoll?.LVistaChosen is not null;

    private bool LOeuvreNarrowed => LOeuvreQueried || LOeuvreFiltered;

    private bool LOeuvreQueried => _lOeuvreVista?.LVistaQueried ?? false;

    private bool LOeuvreFiltered => _lOeuvreRoll?.LVistaFiltered ?? false;

    public void LOeuvreVistaRestore(LVista roll, LVista vista)
    {
        ArgumentNullException.ThrowIfNull(roll);
        ArgumentNullException.ThrowIfNull(vista);

        _lOeuvreRoll = roll;
        _lOeuvreVista = vista;
        LOeuvrePanel.LPanelVistaRestore(vista);
    }

    public IReadOnlyList<LCatalogReference> LOeuvreRowsRead()
    {
        return LOeuvreRowsApply(LOeuvreRowsFind());
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

    public LColophon LOeuvreColophonRead(LDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        LReference reference = draft.LDraftReference
            ?? throw new InvalidOperationException("The oeuvre draft holds no reference.");
        return reference.LReferenceColophonRead(
            draft.LDraftAuthor, LOeuvreTallyRead(), _lSettingsPort.LEngineTextRead);
    }
}

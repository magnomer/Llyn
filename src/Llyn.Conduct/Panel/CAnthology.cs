using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CAnthology
{
    private readonly LEntryPort _cAnthologyEntryPort;

    private readonly LPortraitPort _cAnthologyPortraitPort;

    private readonly CDesk _cAnthologyDesk;

    private LVista? _cAnthologyVista;

    internal CAnthology(
        LEntryPort entries,
        LPortraitPort portraits,
        LSettingsPort settings,
        CDesk desk,
        Func<bool> shownSeam,
        CEnvoy envoy,
        Func<bool, bool> finishSeam)
    {
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(portraits);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(desk);

        _cAnthologyEntryPort = entries;
        _cAnthologyPortraitPort = portraits;
        _cAnthologyDesk = desk;
        CAnthologyPanel = new CPanel(
            envoy,
            settings,
            "Example.LoadFailed", "Example", desk.LDeskChangeCheck, finishSeam, shownSeam);
    }

    internal static CAnthology LAnthologyCreate(
        CAtelier atelier, CDesk desk, Func<bool> shownSeam, CEnvoy envoy, Func<bool, bool> finishSeam)
    {
        ArgumentNullException.ThrowIfNull(atelier);

        return new CAnthology(
            atelier.CAtelierEntryPort,
            atelier.CAtelierPortraitPort,
            atelier.CAtelierSettingsPort,
            desk,
            shownSeam,
            envoy,
            finishSeam);
    }

    public CPanel CAnthologyPanel { get; }

    public long? CAnthologyChosen => _cAnthologyVista?.LVistaChosen;

    public bool CAnthologyFiltered => _cAnthologyVista?.LVistaFiltered ?? false;

    internal bool LAnthologyNarrowed => _cAnthologyVista?.LVistaNarrowed ?? false;

    internal void LAnthologyVistaRestore(LVista vista)
    {
        ArgumentNullException.ThrowIfNull(vista);

        _cAnthologyVista = vista;
        CAnthologyPanel.CPanelVistaRestore(vista);
    }

    public void CAnthologyQuerySet(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        _cAnthologyVista?.LVistaQuerySet(query);
    }

    public void CAnthologyOrderSet(CCatalogOrder? order)
    {
        _cAnthologyVista?.LVistaOrderSet(CPanel.CPanelOrderRead(order));
    }

    public void CAnthologyFilterSet(CCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        _cAnthologyVista?.LVistaFilterSet(filter.CCatalogFilterHidden);
    }

    internal IReadOnlyList<CCatalogExample> LAnthologyRowsRead(string unknown, string unwritten)
    {
        return _cAnthologyVista is LVista vista
            ? _cAnthologyEntryPort.LEngineExampleFind(vista, unknown, unwritten).Select(LAnthologyRowRead).ToList()
            : [];
    }

    public IReadOnlyDictionary<long, int> CAnthologyUsageRead()
    {
        return _cAnthologyEntryPort.LEngineUsageRead(LOwner.LOwnerExample);
    }

    public CMentionResult? CAnthologyMentionRead(int offset)
    {
        return _cAnthologyVista?.LVistaChosen is long chosen
            ? CMention.CMentionResultRead(_cAnthologyEntryPort.LEngineMentionFind(chosen, offset))
            : null;
    }

    internal Task LAnthologyPortraitPrint(CEnvoy envoy, LSettingsPort settings)
    {
        return CPortrait.LPortraitTicketPrint(
            envoy,
            settings,
            chosen => _cAnthologyPortraitPort.LEnginePortraitPrint(
                _cAnthologyVista, CPortrait.LPortraitLegendRead(settings, "Example"), chosen));
    }

    public IReadOnlyList<CCatalogReference> CAnthologyReferenceRead()
    {
        return COeuvre.COeuvreReferenceRead(_cAnthologyEntryPort.LEngineReferenceFind());
    }

    public IReadOnlyList<CCitationRow> CAnthologyCitationRead(string word)
    {
        ArgumentNullException.ThrowIfNull(word);

        return CCitationRow.CCitationRowFind(
            COeuvre.COeuvreReferenceRead(
                _cAnthologyEntryPort.LEngineCitationFind(_cAnthologyDesk.CDeskId, word)),
            word);
    }

    public void CAnthologyCitationSet(string title)
    {
        if (_cAnthologyDesk.CDeskQuill is not LQuill quill)
        {
            return;
        }

        quill.LQuillReferenceSet(
            _cAnthologyEntryPort.LEngineCitationResolve(_cAnthologyDesk.CDeskId, 0, 0, title));
    }

    public static bool CAnthologyTextCheck(string text, CStateValue value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return LEntryPort.LEngineTextMatch(text, value.CStateValueText);
    }

    internal static CExample? LAnthologyDraftRead(LDraft? draft)
    {
        return LAnthologyExampleRead(draft?.LDraftExample);
    }

    internal static CExample? LAnthologyExampleRead(LExample? example)
    {
        return example is null
            ? null
            : new CExample(
                example.LExampleLanguage,
                CFolio.CFolioStateRead(example.LExampleText),
                example.LExampleSource.LStateAnchorShown,
                example.LExampleGloss.Select(LAnthologyGlossRead).ToList(),
                example.LExampleMention.Select(LAnthologyMentionRead).ToList(),
                CMention.CMentionRead(example.LExampleExcerpt));
    }

    private static CCatalogExample LAnthologyRowRead(LCatalogExample row)
    {
        return new CCatalogExample(
            row.LCatalogExampleStored.LExampleId,
            row.LCatalogExampleText,
            row.LCatalogExampleName,
            row.LCatalogExampleStored.LExampleLanguage,
            row.LCatalogExampleUsage,
            row.LCatalogExampleChosen);
    }

    private static CGlossDraft LAnthologyGlossRead(LGloss gloss)
    {
        return new CGlossDraft(gloss.LGlossId, gloss.LGlossLanguage, CFolio.CFolioStateRead(gloss.LGlossText));
    }

    private static CMentionDraft LAnthologyMentionRead(LMention mention)
    {
        return new CMentionDraft(
            mention.LMentionId,
            mention.LMentionEntryId,
            mention.LMentionOffset,
            mention.LMentionLength,
            mention.LMentionSenseId);
    }
}

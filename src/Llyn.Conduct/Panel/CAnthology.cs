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

    private readonly CEnvoy _cAnthologyEnvoy;

    private readonly LSettingsPort _cAnthologySettingsPort;

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
        ArgumentNullException.ThrowIfNull(envoy);

        _cAnthologyEntryPort = entries;
        _cAnthologyPortraitPort = portraits;
        _cAnthologyDesk = desk;
        _cAnthologyEnvoy = envoy;
        _cAnthologySettingsPort = settings;
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

    public CProffer CAnthologyCitationRead(string word)
    {
        ArgumentNullException.ThrowIfNull(word);

        return _cAnthologyDesk.CDeskChip?.LQuillReferenceFind(0, 0, word) is LReferenceOffer offer
            ? CCard.LCardProfferRead(offer)
            : new CProffer(word, [], false);
    }

    public void CAnthologyCitationSet(long referenceId)
    {
        _cAnthologyDesk.CDeskQuill?.LQuillReferenceSet(referenceId);
    }

    public void CAnthologyCitationSet(string title)
    {
        ArgumentNullException.ThrowIfNull(title);

        try
        {
            _cAnthologyDesk.CDeskChip?.LQuillReferenceResolve(title);
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cAnthologyEnvoy, _cAnthologySettingsPort, "Reference.CreateFailed", exception);
        }
    }

    private LTenure? CAnthologyTenure => _cAnthologyDesk.CDeskFilling ? null : _cAnthologyDesk.CDeskTenure;

    public void CAnthologyGlossSet(long glossId, string text)
    {
        _cAnthologyDesk.CDeskQuill?.LQuillGlossSet(0, 0, glossId, null, text);
    }

    public void CAnthologyLanguageSet(long glossId, string language)
    {
        _cAnthologyDesk.CDeskQuill?.LQuillGlossSet(0, 0, glossId, language, null);
    }

    public void CAnthologyGlossAdd(int below)
    {
        CAnthologyTenure?.LTenureGlossInsert(0, 0, below + 1);
    }

    public bool CAnthologyGlossPrepare()
    {
        return CAnthologyTenure?.LTenureGlossPrepare(0, 0) == true;
    }

    public void CAnthologyGlossRemove(long glossId)
    {
        _cAnthologyDesk.CDeskQuill?.LQuillGlossRemove(0, 0, glossId);
    }

    public static bool CAnthologyTextCheck(string text, CStateValue value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return LEntryPort.LEngineTextMatch(text, value.CStateValueText);
    }

    internal CExample? LAnthologyDraftRead(LDraft? draft)
    {
        return draft?.LDraftExample is LExample example
            ? LAnthologyExampleRead(example, LAnthologyCitationRead(draft))
            : null;
    }

    private string LAnthologyCitationRead(LDraft draft)
    {
        try
        {
            return _cAnthologyEntryPort.LEngineCitationRead(draft);
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cAnthologyEnvoy, _cAnthologySettingsPort, "Reference.LoadFailed", exception);
            return string.Empty;
        }
    }

    internal static CExample? LAnthologyExampleRead(LExample? example, string citation)
    {
        return example is null
            ? null
            : new CExample(
                example.LExampleLanguage,
                CFolio.CFolioStateRead(example.LExampleText),
                example.LExampleSource.LStateAnchorShown,
                citation,
                example.LExampleGloss.Select(LAnthologyGlossRead).ToList(),
                CMention.CMentionRead(example.LExampleExcerpt));
    }

    private static CCatalogExample LAnthologyRowRead(LCatalogExample row)
    {
        return new CCatalogExample(
            row.LCatalogExampleStored.LExampleId,
            row.LCatalogExampleText,
            row.LCatalogExampleName,
            row.LCatalogExampleStored.LExampleLanguage,
            row.LCatalogExampleCount,
            row.LCatalogExampleChosen);
    }

    private static CGlossDraft LAnthologyGlossRead(LGloss gloss)
    {
        return new CGlossDraft(
            gloss.LGlossId, gloss.LGlossLanguage, CFolio.CFolioStateRead(gloss.LGlossText), gloss.LGlossNamed);
    }
}

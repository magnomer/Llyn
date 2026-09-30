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

    private readonly CMention _cAnthologyMention;

    private LVista? _cAnthologyVista;

    internal CAnthology(
        LEntryPort entries,
        LPortraitPort portraits,
        LSettingsPort settings,
        CDesk desk,
        Func<bool> shownSeam,
        CEnvoy envoy,
        Func<bool, bool> finishSeam,
        CMention mention)
    {
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(portraits);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(desk);
        ArgumentNullException.ThrowIfNull(envoy);
        ArgumentNullException.ThrowIfNull(mention);

        _cAnthologyEntryPort = entries;
        _cAnthologyPortraitPort = portraits;
        _cAnthologyDesk = desk;
        _cAnthologyEnvoy = envoy;
        _cAnthologySettingsPort = settings;
        _cAnthologyMention = mention;
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
            finishSeam,
            atelier.CAtelierMention);
    }

    public CPanel CAnthologyPanel { get; }

    public long? CAnthologyChosen => _cAnthologyVista?.LVistaChosen;

    public bool CAnthologyFiltered => _cAnthologyVista?.LVistaFiltered ?? false;

    internal bool LAnthologyNarrowed => _cAnthologyVista?.LVistaNarrowed ?? false;

    internal void LAnthologyVistaRestore(LVista vista)
    {
        ArgumentNullException.ThrowIfNull(vista);

        vista.LVistaQuerySet(_cAnthologyVista?.LVistaQuery ?? string.Empty);
        _cAnthologyVista = vista;
        CAnthologyPanel.CPanelVistaRestore(vista);
    }

    internal void LAnthologyObserverAttach(Action<Action> marshal, Action workspace)
    {
        ArgumentNullException.ThrowIfNull(marshal);
        ArgumentNullException.ThrowIfNull(workspace);

        Action<CBulletin> rows = _ => marshal(CAnthologyPanel.CPanelRowsResonate);
        CAnthologyPanel.CPanelObserverAttach(CSubject.CSubjectVista, rows);
        CAnthologyPanel.CPanelObserverAttach(CSubject.CSubjectWorkspace, _ => marshal(workspace));
        CAnthologyPanel.CPanelObserverAttach(CSubject.CSubjectExample, rows);
        CAnthologyPanel.CPanelObserverAttach(CSubject.CSubjectReference, rows);
        CAnthologyPanel.CPanelObserverAttach(CSubject.CSubjectReflex, rows);
        CAnthologyPanel.CPanelObserverAttach(CSubject.CSubjectSettings, rows);
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

    public static IReadOnlyList<CCatalogOrder> CAnthologyOrderRead()
    {
        return
        [
            CCatalogOrder.CCatalogOrderText,
            CCatalogOrder.CCatalogOrderLanguage,
            CCatalogOrder.CCatalogOrderSource,
            CCatalogOrder.CCatalogOrderUsage,
        ];
    }

    public void CAnthologyFilterSet(CCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        _cAnthologyVista?.LVistaFilterSet(filter.CCatalogFilterHidden);
    }

    internal IReadOnlyList<CCatalogExample>? LAnthologyRowsRead()
    {
        if (_cAnthologyVista is not LVista vista)
        {
            return [];
        }

        try
        {
            return _cAnthologyEntryPort
                .LEngineExampleFind(
                    vista,
                    _cAnthologySettingsPort.LEngineTextRead("Display.Unknown"),
                    _cAnthologySettingsPort.LEngineTextRead("Example.Unwritten"))
                .Select(LAnthologyRowRead)
                .ToList();
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cAnthologyEnvoy, _cAnthologySettingsPort, "Example.LoadFailed", exception);
            return null;
        }
    }

    internal CMentionOffer? LAnthologyMentionFind(int offset)
    {
        try
        {
            if (_cAnthologyVista?.LVistaChosen is not long chosen)
            {
                return null;
            }

            return _cAnthologyMention.LMentionResultOpen(
                CMention.CMentionResultRead(_cAnthologyEntryPort.LEngineMentionFind(chosen, offset)));
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cAnthologyEnvoy, _cAnthologySettingsPort, "Mention.FindFailed", exception);
            return null;
        }
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

    public string CAnthologyTextSet(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        _cAnthologyDesk.CDeskQuill?.LQuillExampleSet(text);
        return CExample.LExampleHintRead(false);
    }

    public void CAnthologySpeakerSet(string language)
    {
        ArgumentNullException.ThrowIfNull(language);

        _cAnthologyDesk.CDeskQuill?.LQuillSpeakerSet(language);
    }

    public static bool CAnthologyTextCheck(string text, CStateValue value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return LEntryPort.LEngineTextMatch(text, value.CStateValueText);
    }

    internal CExample? LAnthologyDraftRead(LDraft? draft, string tally)
    {
        return draft?.LDraftExample is LExample example
            ? LAnthologyExampleRead(example, LAnthologyCitationRead(draft), tally)
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

    internal static CExample? LAnthologyExampleRead(LExample? example, string citation, string tally)
    {
        return example is null
            ? null
            : new CExample(
                example.LExampleLanguage,
                CFolio.CFolioStateRead(example.LExampleText),
                example.LExampleSource.LStateAnchorShown,
                citation,
                tally,
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CAnthology
{
    private readonly LExamplePort _cAnthologyExamplePort;

    private readonly LMentionPort _cAnthologyMentionPort;

    private readonly LReferencePort _cAnthologyReferencePort;

    private readonly LPortraitPort _cAnthologyPortraitPort;

    private readonly CDesk _cAnthologyDesk;

    private readonly CEnvoy _cAnthologyEnvoy;

    private readonly LSettingsPort _cAnthologySettingsPort;

    private readonly CMention _cAnthologyMention;

    private LVista? _cAnthologyVista;

    private string _cAnthologyTextShown = string.Empty;

    internal CAnthology(
        LExamplePort examples,
        LMentionPort mentions,
        LReferencePort references,
        LPortraitPort portraits,
        LSettingsPort settings,
        LVistaPort vistas,
        CDesk desk,
        Func<bool> shownSeam,
        CEnvoy envoy,
        Func<bool, bool> finishSeam,
        CMention mention)
    {
        ArgumentNullException.ThrowIfNull(examples);
        ArgumentNullException.ThrowIfNull(mentions);
        ArgumentNullException.ThrowIfNull(references);
        ArgumentNullException.ThrowIfNull(portraits);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(desk);
        ArgumentNullException.ThrowIfNull(envoy);
        ArgumentNullException.ThrowIfNull(mention);

        _cAnthologyExamplePort = examples;
        _cAnthologyMentionPort = mentions;
        _cAnthologyReferencePort = references;
        _cAnthologyPortraitPort = portraits;
        _cAnthologyDesk = desk;
        _cAnthologyEnvoy = envoy;
        _cAnthologySettingsPort = settings;
        _cAnthologyMention = mention;
        CAnthologyPanel = new CPanel(
            envoy,
            settings,
            vistas,
            "Example.LoadFailed", "Example", desk.LDeskChangeCheck, finishSeam, shownSeam);
    }

    internal static CAnthology LAnthologyCreate(
        CAtelier atelier, CDesk desk, Func<bool> shownSeam, CEnvoy envoy, Func<bool, bool> finishSeam)
    {
        ArgumentNullException.ThrowIfNull(atelier);

        return new CAnthology(
            atelier.CAtelierEntryBundle.CEntryBundleExample,
            atelier.CAtelierEntryBundle.CEntryBundleMention,
            atelier.CAtelierEntryBundle.CEntryBundleReference,
            atelier.CAtelierPortraitPort,
            atelier.CAtelierSettingsPort,
            atelier.CAtelierEntryBundle.CEntryBundleVista,
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
        _cAnthologyVista?.LVistaOrderSet(CCatalog.LCatalogOrderRead(order));
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
            return _cAnthologyExamplePort
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

    internal CMentionOffer? LAnthologyMentionFind(string text, int unit)
    {
        try
        {
            if (_cAnthologyVista?.LVistaChosen is not long chosen)
            {
                return null;
            }

            int offset = _cAnthologyMention.LMentionOffsetRead(text, unit);
            CMentionResult found = CMention.CMentionResultRead(
                _cAnthologyMentionPort.LEngineMentionFind(chosen, offset));
            return _cAnthologyMention.LMentionResultOpen(found, text);
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
        _cAnthologyDesk.CDeskExample?.LExampleReferenceSet(referenceId);
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

    public void CAnthologyGlossSet(long glossId, string text)
    {
        _cAnthologyDesk.CDeskSentence?.LSentenceGlossSet(0, 0, glossId, null, text);
    }

    public void CAnthologyLanguageSet(long glossId, string language)
    {
        _cAnthologyDesk.CDeskSentence?.LSentenceGlossSet(0, 0, glossId, language, null);
    }

    public void CAnthologyGlossAdd(int below)
    {
        _cAnthologyDesk.CDeskSentence?.LQuillGlossInsert(0, 0, below + 1);
    }

    public bool CAnthologyGlossPrepare()
    {
        return _cAnthologyDesk.CDeskSentence?.LQuillGlossPrepare(0, 0) == true;
    }

    public void CAnthologyGlossRemove(long glossId)
    {
        _cAnthologyDesk.CDeskSentence?.LSentenceGlossRemove(0, 0, glossId);
    }

    public string CAnthologyTextSet(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        _cAnthologyDesk.CDeskExample?.LQuillExampleSet(text);
        _cAnthologyTextShown = text;
        return CExample.LExampleHintRead(false);
    }

    public void CAnthologySpeakerSet(string language)
    {
        ArgumentNullException.ThrowIfNull(language);

        _cAnthologyDesk.CDeskExample?.LExampleSpeakerSet(language);
    }

    internal bool LAnthologyTextCheck(string text, CStateValue value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return _cAnthologyExamplePort.LEngineTextMatch(text, value.CStateValueText);
    }

    internal CExample LAnthologyTextShow(CExample example)
    {
        ArgumentNullException.ThrowIfNull(example);

        _cAnthologyTextShown = example.CExampleText.CStateValueText;
        return example;
    }

    internal CExample LAnthologyTranscriptRead(CExample example)
    {
        ArgumentNullException.ThrowIfNull(example);

        return LAnthologyTextCheck(_cAnthologyTextShown, example.CExampleText)
            ? example with { CExampleTextKept = true }
            : LAnthologyTextShow(example);
    }

    internal CExample? LAnthologyDraftRead(LDraft? draft)
    {
        return draft?.LDraftExample is LExample example
            ? LAnthologyExampleRead(example, LAnthologyCitationRead(draft), CAnthologyPanel.CPanelTallyRead())
            : null;
    }

    private string LAnthologyCitationRead(LDraft draft)
    {
        try
        {
            return _cAnthologyReferencePort.LEngineCitationRead(draft);
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
                CMention.CMentionDivide(example.LExampleText.LStateValuePlain, example.LExampleExcerpt));
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

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Llyn.Application;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIDeportment;

public sealed class LAnthology
{
    private readonly LEntryPort _lEntryPort;

    private readonly LPortraitPort _lPortraitPort;

    private readonly Func<int, bool> _lAnthologyRemovalSeam;

    private readonly LDesk _lAnthologyDesk;

    private LVista? _lAnthologyVista;

    internal LAnthology(
        LEntryPort entries,
        LPortraitPort portraits,
        LDesk desk,
        Func<bool> shownSeam,
        Func<bool> leaveSeam,
        Func<int, bool> removalSeam)
    {
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(portraits);
        ArgumentNullException.ThrowIfNull(desk);
        ArgumentNullException.ThrowIfNull(removalSeam);

        _lEntryPort = entries;
        _lPortraitPort = portraits;
        _lAnthologyDesk = desk;
        _lAnthologyRemovalSeam = removalSeam;
        LAnthologyPanel = new LPanel(
            "Example.LoadFailed", "Example.DeleteFailed",
            desk.LDeskChangeCheck, shownSeam, leaveSeam, LAnthologyDeleteConfirm);
    }

    public LPanel LAnthologyPanel { get; }

    public long? LAnthologyChosen => _lAnthologyVista?.LVistaChosen;

    public bool LAnthologyFiltered => _lAnthologyVista?.LVistaFiltered ?? false;

    public bool LAnthologyNarrowed => _lAnthologyVista?.LVistaNarrowed ?? false;

    internal void LAnthologyVistaRestore(LVista vista)
    {
        ArgumentNullException.ThrowIfNull(vista);

        _lAnthologyVista = vista;
        LAnthologyPanel.LPanelVistaRestore(vista);
    }

    public void LAnthologyQuerySet(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        _lAnthologyVista?.LVistaQuerySet(query);
    }

    public void LAnthologyRankSet(CCatalogOrder? order)
    {
        if (_lAnthologyVista is not LVista vista)
        {
            return;
        }

        vista.LVistaOrderSet(LPanel.LPanelOrderRead(order) ?? vista.LVistaOrder);
    }

    public void LAnthologyGauzeSet(CCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        _lAnthologyVista?.LVistaFilterSet(LPanel.LPanelFilterRead(filter));
    }

    public IReadOnlyList<CCatalogExample> LAnthologyRowsRead(string unknown, string unwritten)
    {
        return _lAnthologyVista is LVista vista
            ? LSplice.LSpliceBuild(
                _lEntryPort.LEngineExampleFind(vista, unknown, unwritten),
                row => LAnthologyRowRead(row, unknown, unwritten))
            : [];
    }

    public IReadOnlyDictionary<long, int> LAnthologyUsageRead()
    {
        return _lEntryPort.LEngineUsageRead(LOwner.LOwnerExample);
    }

    private int LAnthologyUsageRead(long? id)
    {
        return id is long stored ? LAnthologyUsageRead().GetValueOrDefault(stored) : 0;
    }

    private bool LAnthologyDeleteConfirm()
    {
        return _lAnthologyRemovalSeam(LAnthologyUsageRead(LAnthologyChosen));
    }

    public CMentionResult? LAnthologyMentionFind(long? id, int offset)
    {
        return id is long chosen ? LAnthologyMentionRead(_lEntryPort.LEngineMentionFind(chosen, offset)) : null;
    }

    public Task LAnthologyPortraitPrint(CPortraitLegend legend, CPressTicket ticket)
    {
        return _lPortraitPort.LEnginePortraitPrint(
            _lAnthologyVista, LAtlas.LAtlasLegendRead(legend), QPortrait.QPortraitTicketRead(ticket));
    }

    public IReadOnlyList<CCatalogReference> LAnthologyReferenceFind()
    {
        return LAnthologyReferenceFind(string.Empty, LCatalogOrder.LCatalogOrderAuthor);
    }

    private IReadOnlyList<CCatalogReference> LAnthologyReferenceFind(string word, LCatalogOrder order)
    {
        return LOeuvre.LOeuvreReferenceRead(_lEntryPort.LEngineReferenceFind(word, order));
    }

    public IReadOnlyList<CCitationRow> LAnthologyCitationFind(string word, long? source)
    {
        ArgumentNullException.ThrowIfNull(word);

        string text = word.Trim();
        return text.Length == 0 ? [] : CCitationRow.CCitationRowFind(
            LAnthologyReferenceFind(text, LCatalogOrder.LCatalogOrderUsage), text, source);
    }

    public void LAnthologyCitationSet(string title)
    {
        _lAnthologyDesk.LDeskSend(new LRequestExampleReference(
            _lAnthologyDesk.LDeskId, _lEntryPort.LEngineCitationResolve(_lAnthologyDesk.LDeskId, 0, 0, title)));
    }

    public bool LAnthologyTextCheck(string text, CStateValue value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return string.Equals(
            string.IsNullOrWhiteSpace(text) ? string.Empty : text, value.CStateValueText, StringComparison.Ordinal);
    }

    internal static CCatalogExample LAnthologyRowRead(LCatalogExample row, string unknown, string unwritten)
    {
        return new CCatalogExample(
            row.LCatalogExampleStored.LExampleId,
            LAnthologyTextRead(row.LCatalogExampleStored.LExampleText, unknown, unwritten),
            row.LCatalogExampleName,
            row.LCatalogExampleStored.LExampleLanguage,
            row.LCatalogExampleUsage,
            row.LCatalogExampleChosen);
    }

    internal static CExample? LAnthologyExampleRead(LExample? example)
    {
        return example is null
            ? null
            : new CExample(
                example.LExampleLanguage,
                LCard.LCardStateRead(example.LExampleText),
                example.LExampleSource.LStateAnchorShown,
                LSplice.LSpliceBuild(
                    example.LExampleGloss,
                    static gloss => new CGlossDraft(
                        gloss.LGlossId, gloss.LGlossLanguage, LCard.LCardStateRead(gloss.LGlossText))),
                LSplice.LSpliceBuild(
                    example.LExampleMention,
                    static mention => LCard.LCardMentionRead(LMentionDraft.LMentionDraftCreate(mention))),
                LAnthologyExcerptRead(example.LExampleText.LStateValueSound, example.LExampleMention));
    }

    private static string LAnthologyTextRead(LStateValue text, string unknown, string unwritten)
    {
        return (text.LStateValueUncertain ? unknown : text.LStateValueShown) ?? unwritten;
    }

    internal static CMentionResult LAnthologyMentionRead(LMentionResult result)
    {
        return new CMentionResult(
            result.LMentionResultOffset,
            LAnthologyMentionRead(result.LMentionResultStored),
            LCard.LCardTargetRead(result.LMentionResultEntry));
    }

    private static CMentionMark? LAnthologyMentionRead(LMention? mention)
    {
        return mention is null ? null : CMention.CMentionRead([mention])[0];
    }

    private static IReadOnlyList<CMentionMark> LAnthologyExcerptRead(bool sound, IReadOnlyList<LMention> mentions)
    {
        return sound ? CMention.CMentionRead(mentions) : [];
    }
}

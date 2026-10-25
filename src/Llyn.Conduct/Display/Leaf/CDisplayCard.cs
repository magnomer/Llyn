using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CDisplayCard
{
    private readonly CRule _cDisplayRule;

    private readonly LCardPort _cDisplayCardPort;

    private readonly LReferencePort _cDisplayReferencePort;

    private readonly LExamplePort _cDisplayExamplePort;

    private readonly LSentencePort _cDisplaySentence;

    private readonly LAncestorPort _cDisplayAncestorPort;

    private readonly LSettingsPort _cDisplaySettings;

    private readonly CEnvoy _cDisplayEnvoy;

    private readonly Action _cDisplayStored;

    private readonly Action<bool, long> _cDisplaySought;

    internal CDisplayCard(
        CRule display,
        LCardPort cards,
        LReferencePort references,
        LExamplePort examples,
        LSentencePort sentences,
        LAncestorPort ancestors,
        LSettingsPort settings,
        CEnvoy envoy,
        Action stored,
        Action<bool, long> sought)
    {
        ArgumentNullException.ThrowIfNull(display);
        ArgumentNullException.ThrowIfNull(cards);
        ArgumentNullException.ThrowIfNull(references);
        ArgumentNullException.ThrowIfNull(examples);
        ArgumentNullException.ThrowIfNull(sentences);
        ArgumentNullException.ThrowIfNull(ancestors);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(envoy);
        ArgumentNullException.ThrowIfNull(stored);
        ArgumentNullException.ThrowIfNull(sought);

        _cDisplayStored = stored;
        _cDisplaySought = sought;
        _cDisplayRule = display;
        _cDisplayCardPort = cards;
        _cDisplayReferencePort = references;
        _cDisplayExamplePort = examples;
        _cDisplaySentence = sentences;
        _cDisplayAncestorPort = ancestors;
        _cDisplaySettings = settings;
        _cDisplayEnvoy = envoy;
    }

    private LEntryDraft? _cDisplayCardShown;

    private LEntryDraft? LDisplayShown => _cDisplayRule.CVoice.CVoiceShown;

    internal LEntryDraft? CDisplayCardShown => _cDisplayCardShown;

    internal CLecternCard CDisplayCardDrawn { get; private set; } = new([], [], false, false);

    internal IReadOnlyList<CUsage> CDisplayCardUsages { get; private set; } = [];

    internal CEtymologyState CDisplayCardEtymology { get; private set; } = new([], [], []);

    private CLedgerNoticed LDisplayNoticed => _cDisplayRule.CRuleNoticed;

    internal void CDisplayCardReset()
    {
        CDisplayCardDrawn = new CLecternCard([], [], false, false);
        CDisplayCardUsages = [];
        CDisplayCardEtymology = new CEtymologyState([], [], []);
    }

    public CLecternCard CDisplayCardRead()
    {
        _cDisplayCardShown = LDisplayShown;
        if (_cDisplayCardShown is not LEntryDraft shown)
        {
            CDisplayCardDrawn = new CLecternCard([], [], false, false);
            _cDisplayStored();
            return CDisplayCardDrawn;
        }

        LSentenceOrder order = LDisplayOrderRead(shown.LEntryDraftLanguage);
        IReadOnlyDictionary<long, string> citations = LDisplayCitationRead(shown);
        IReadOnlyDictionary<(LCardKind, long), IReadOnlyList<LTranslationTarget>> targets =
            LDisplayTranslationRead(shown);
        string mark = _cDisplaySettings.LEngineTextRead("Display.Unknown");
        string stub = _cDisplaySettings.LEngineTextRead("Display.CollocationStub");
        string example = _cDisplaySettings.LEngineTextRead("Display.CollocationExample");
        string exampleOne = _cDisplaySettings.LEngineTextRead("Display.CollocationExampleOne");
        string also = _cDisplaySettings.LEngineTextRead("Display.CollocationAlso");
        string heading = _cDisplaySettings.LEngineTextRead("Display.CollocationHeading");
        IReadOnlySet<(LCardKind, long)> folds = LDisplayFoldRead();
        LMediaPort media = _cDisplayRule.CRuleMediaPort;
        LCollocationLayout layout = _cDisplayCardPort.LEngineCollocationRead(shown, targets);
        IReadOnlyList<CLeaf> others = CLeaf.LLeafRead(
            shown,
            layout.LCollocationLayoutOther, true, layout, order, mark, stub, example, exampleOne, also, heading,
            citations, targets, folds, media, _cDisplayExamplePort);
        CDisplayCardDrawn = new CLecternCard(
            CLeaf.LLeafRead(
                shown,
                shown.LEntryDraftMeanings, false, layout, order, mark, stub, example, exampleOne, also, heading,
                citations, targets, folds, media, _cDisplayExamplePort),
            others,
            shown.LEntryDraftDefined,
            others.Count > 0);
        _cDisplayStored();
        return CDisplayCardDrawn;
    }

    public bool CDisplayFoldToggle(bool collocated, long cardId, bool folded)
    {
        if (_cDisplayRule.CRuleChosen is not long id)
        {
            return false;
        }

        LCardKind kind = collocated ? LCardKind.LCardKindCollocation : LCardKind.LCardKindMeaning;
        try
        {
            if (folded)
            {
                _cDisplayCardPort.LEngineFoldSave(id, kind, cardId);
            }
            else
            {
                _cDisplayCardPort.LEngineFoldDelete(id, kind, cardId);
            }

            return true;
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cDisplayEnvoy, _cDisplaySettings, "Fold.SaveFailed", exception);
        }

        return false;
    }

    public bool CDisplaySeeOpen(long id)
    {
        if (!CDisplayFoldToggle(true, id, false))
        {
            return false;
        }

        _cDisplaySought(true, id);
        return true;
    }

    private IReadOnlySet<(LCardKind, long)> LDisplayFoldRead()
    {
        if (_cDisplayRule.CRuleChosen is not long id)
        {
            return new HashSet<(LCardKind, long)>();
        }

        try
        {
            return _cDisplayCardPort.LEngineFoldRead(id);
        }
        catch (Exception exception)
        {
            LDisplayNoticed.LLedgerRepaintShow(_cDisplayEnvoy, _cDisplaySettings, "Fold.ReadFailed", exception);
            return new HashSet<(LCardKind, long)>();
        }
    }

    private IReadOnlyDictionary<(LCardKind, long), IReadOnlyList<LTranslationTarget>> LDisplayTranslationRead(
        LEntryDraft shown)
    {
        try
        {
            return _cDisplayCardPort.LEngineTranslationRead(shown);
        }
        catch (Exception exception)
        {
            LDisplayNoticed.LLedgerRepaintShow(
                _cDisplayEnvoy, _cDisplaySettings, "Display.TranslationFailed", exception);
            return new Dictionary<(LCardKind, long), IReadOnlyList<LTranslationTarget>>();
        }
    }

    private LSentenceOrder LDisplayOrderRead(string language)
    {
        try
        {
            return _cDisplaySentence.LEngineOrderRead(language);
        }
        catch (Exception exception)
        {
            LDisplayNoticed.LLedgerRepaintShow(_cDisplayEnvoy, _cDisplaySettings, "Display.OrderFailed", exception);
            return LSentenceOrder.LSentenceOrderDefault;
        }
    }

    private IReadOnlyDictionary<long, string> LDisplayCitationRead(LEntryDraft shown)
    {
        try
        {
            return _cDisplayReferencePort.LEngineCitationRead(shown);
        }
        catch (Exception exception)
        {
            LDisplayNoticed.LLedgerRepaintShow(_cDisplayEnvoy, _cDisplaySettings, "Display.CitationFailed", exception);
            return new Dictionary<long, string>();
        }
    }

    public IReadOnlyList<CUsage> CDisplayIncomingRead()
    {
        CDisplayCardUsages = [];
        if (_cDisplayRule.CRuleChosen is not long id)
        {
            _cDisplayStored();
            return CDisplayCardUsages;
        }

        try
        {
            CDisplayCardUsages = _cDisplayCardPort.LEngineIncomingRead(id).Select(COeuvre.COeuvreUsageRead).ToList();
        }
        catch (Exception exception)
        {
            LDisplayNoticed.LLedgerRepaintShow(_cDisplayEnvoy, _cDisplaySettings, "Display.IncomingFailed", exception);
        }

        _cDisplayStored();
        return CDisplayCardUsages;
    }

    public CEtymologyState CDisplayEtymologyRead()
    {
        CDisplayCardEtymology = new CEtymologyState([], [], []);
        if (LDisplayShown is not LEntryDraft shown || _cDisplayRule.CVoice.CVoiceEntry is not long id)
        {
            _cDisplayStored();
            return CDisplayCardEtymology;
        }

        try
        {
            CDisplayCardEtymology = new CEtymologyState(
                CAncestor.CAncestorRead(
                    _cDisplayAncestorPort.LEngineLineageRead(id, shown.LEntryDraftAncestors), null),
                [.. _cDisplayAncestorPort.LEngineRelationRead().Select(CAncestorChoice.CAncestorChoiceRead)],
                [],
                shown.LEntryDraftHeadword);
        }
        catch (Exception exception)
        {
            LDisplayNoticed.LLedgerRepaintShow(_cDisplayEnvoy, _cDisplaySettings, "Display.AncestryFailed", exception);
        }

        _cDisplayStored();
        return CDisplayCardEtymology;
    }
}

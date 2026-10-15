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

    internal CDisplayCard(
        CRule display,
        LCardPort cards,
        LReferencePort references,
        LExamplePort examples,
        LSentencePort sentences,
        LAncestorPort ancestors,
        LSettingsPort settings,
        CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(display);
        ArgumentNullException.ThrowIfNull(cards);
        ArgumentNullException.ThrowIfNull(references);
        ArgumentNullException.ThrowIfNull(examples);
        ArgumentNullException.ThrowIfNull(sentences);
        ArgumentNullException.ThrowIfNull(ancestors);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(envoy);

        _cDisplayRule = display;
        _cDisplayCardPort = cards;
        _cDisplayReferencePort = references;
        _cDisplayExamplePort = examples;
        _cDisplaySentence = sentences;
        _cDisplayAncestorPort = ancestors;
        _cDisplaySettings = settings;
        _cDisplayEnvoy = envoy;
    }

    private LEntryDraft? LDisplayShown => _cDisplayRule.CVoice.CVoiceShown;

    private CLedgerNoticed LDisplayNoticed => _cDisplayRule.CRuleNoticed;

    public CLecternCard CDisplayCardRead()
    {
        if (LDisplayShown is not LEntryDraft shown)
        {
            return new CLecternCard([], [], false, false);
        }

        LSentenceOrder order = LDisplayOrderRead(shown.LEntryDraftLanguage);
        IReadOnlyDictionary<long, string> citations = LDisplayCitationRead(shown);
        IReadOnlyDictionary<long, IReadOnlyList<LTranslationTarget>> targets = LDisplayTranslationRead(shown);
        string mark = _cDisplaySettings.LEngineTextRead("Display.Unknown");
        string stub = _cDisplaySettings.LEngineTextRead("Display.CollocationStub");
        string example = _cDisplaySettings.LEngineTextRead("Display.CollocationExample");
        string exampleOne = _cDisplaySettings.LEngineTextRead("Display.CollocationExampleOne");
        string also = _cDisplaySettings.LEngineTextRead("Display.CollocationAlso");
        string heading = _cDisplaySettings.LEngineTextRead("Display.CollocationHeading");
        IReadOnlySet<long> folds = LDisplayFoldRead();
        LMediaPort media = _cDisplayRule.CRuleMediaPort;
        LCollocationLayout layout = _cDisplayCardPort.LEngineCollocationRead(shown, targets);
        IReadOnlyList<CLeaf> others = CLeaf.LLeafRead(
            layout.LCollocationLayoutOther, layout, order, mark, stub, example, exampleOne, also, heading, citations,
            targets, folds, media, _cDisplayExamplePort);
        return new CLecternCard(
            CLeaf.LLeafRead(
                shown.LEntryDraftMeanings, layout, order, mark, stub, example, exampleOne, also, heading, citations,
                targets, folds, media, _cDisplayExamplePort),
            others,
            shown.LEntryDraftDefined,
            others.Count > 0);
    }

    public bool CDisplayFoldToggle(long cardId, bool folded)
    {
        if (_cDisplayRule.CRuleChosen is not long id)
        {
            return false;
        }

        try
        {
            if (folded)
            {
                _cDisplayCardPort.LEngineFoldSave(id, cardId);
            }
            else
            {
                _cDisplayCardPort.LEngineFoldDelete(id, cardId);
            }

            return true;
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cDisplayEnvoy, _cDisplaySettings, "Fold.SaveFailed", exception);
        }

        return false;
    }

    private IReadOnlySet<long> LDisplayFoldRead()
    {
        if (_cDisplayRule.CRuleChosen is not long id)
        {
            return new HashSet<long>();
        }

        try
        {
            return _cDisplayCardPort.LEngineFoldRead(id);
        }
        catch (Exception exception)
        {
            LDisplayNoticed.LLedgerRepaintShow(_cDisplayEnvoy, _cDisplaySettings, "Fold.ReadFailed", exception);
            return new HashSet<long>();
        }
    }

    private IReadOnlyDictionary<long, IReadOnlyList<LTranslationTarget>> LDisplayTranslationRead(LEntryDraft shown)
    {
        try
        {
            return _cDisplayCardPort.LEngineTranslationRead(shown);
        }
        catch (Exception exception)
        {
            LDisplayNoticed.LLedgerRepaintShow(
                _cDisplayEnvoy, _cDisplaySettings, "Display.TranslationFailed", exception);
            return new Dictionary<long, IReadOnlyList<LTranslationTarget>>();
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
        if (_cDisplayRule.CRuleChosen is not long id)
        {
            return [];
        }

        try
        {
            return _cDisplayCardPort.LEngineIncomingRead(id).Select(COeuvre.COeuvreUsageRead).ToList();
        }
        catch (Exception exception)
        {
            LDisplayNoticed.LLedgerRepaintShow(_cDisplayEnvoy, _cDisplaySettings, "Display.IncomingFailed", exception);
            return [];
        }
    }

    public CEtymologyState CDisplayEtymologyRead()
    {
        if (LDisplayShown is not LEntryDraft shown || _cDisplayRule.CVoice.CVoiceEntry is not long id)
        {
            return new CEtymologyState([], [], []);
        }

        try
        {
            return new CEtymologyState(
                CAncestor.CAncestorRead(
                    _cDisplayAncestorPort.LEngineLineageRead(id, shown.LEntryDraftAncestors), null),
                [.. _cDisplayAncestorPort.LEngineRelationRead().Select(CAncestorChoice.CAncestorChoiceRead)],
                [],
                shown.LEntryDraftHeadword);
        }
        catch (Exception exception)
        {
            LDisplayNoticed.LLedgerRepaintShow(_cDisplayEnvoy, _cDisplaySettings, "Display.AncestryFailed", exception);
            return new CEtymologyState([], [], []);
        }
    }
}

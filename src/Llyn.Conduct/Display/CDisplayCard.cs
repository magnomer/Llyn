using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CDisplayCard
{
    private readonly LDisplay _cDisplayRule;

    private readonly LEntryPort _cDisplayPort;

    private readonly LPhonologyPort _cDisplayPhonology;

    private readonly LSettingsPort _cDisplaySettings;

    private readonly CEnvoy _cDisplayEnvoy;

    internal CDisplayCard(
        LDisplay display, LEntryPort entries, LPhonologyPort phonology, LSettingsPort settings, CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(display);
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(phonology);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(envoy);

        _cDisplayRule = display;
        _cDisplayPort = entries;
        _cDisplayPhonology = phonology;
        _cDisplaySettings = settings;
        _cDisplayEnvoy = envoy;
    }

    private LEntryDraft? LDisplayShown => _cDisplayRule.LDisplaySound.LDisplayShown;

    private CLedgerNoticed LDisplayNoticed => _cDisplayRule.LDisplayNoticed;

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
        LMediaPort media = _cDisplayRule.LDisplayMediaPort;
        return new CLecternCard(
            CLeaf.LLeafRead(shown.LEntryDraftMeanings, order, mark, citations, targets, media),
            CLeaf.LLeafRead(shown.LEntryDraftCollocations, order, mark, citations, targets, media),
            shown.LEntryDraftDefined,
            shown.LEntryDraftCollocated);
    }

    private IReadOnlyDictionary<long, IReadOnlyList<LTranslationTarget>> LDisplayTranslationRead(LEntryDraft shown)
    {
        try
        {
            return _cDisplayPort.LEngineTranslationRead(shown);
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
            return _cDisplayPhonology.LEngineOrderRead(language);
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
            return _cDisplayPort.LEngineCitationRead(shown);
        }
        catch (Exception exception)
        {
            LDisplayNoticed.LLedgerRepaintShow(_cDisplayEnvoy, _cDisplaySettings, "Display.CitationFailed", exception);
            return new Dictionary<long, string>();
        }
    }

    public IReadOnlyList<CUsage> CDisplayIncomingRead()
    {
        if (_cDisplayRule.LDisplayChosen is not long id)
        {
            return [];
        }

        try
        {
            return _cDisplayPort.LEngineIncomingRead(id).Select(COeuvre.COeuvreUsageRead).ToList();
        }
        catch (Exception exception)
        {
            LDisplayNoticed.LLedgerRepaintShow(_cDisplayEnvoy, _cDisplaySettings, "Display.IncomingFailed", exception);
            return [];
        }
    }

    public CLecternEtymology CDisplayEtymologyRead()
    {
        if (LDisplayShown is not LEntryDraft shown)
        {
            return new CLecternEtymology(string.Empty, [], false, false, false, false);
        }

        LEtymologyResult etymology;
        try
        {
            etymology = _cDisplayPort.LEngineEtymologyRead(shown);
        }
        catch (Exception exception)
        {
            LDisplayNoticed.LLedgerRepaintShow(_cDisplayEnvoy, _cDisplaySettings, "Display.EtymologyFailed", exception);
            etymology = new LEtymologyResult([], shown.LEntryDraftEtymology.LEtymologyDraftNarrated);
        }

        return new CLecternEtymology(
            shown.LEntryDraftEtymology.LEtymologyDraftText,
            CFolio.CFolioTargetRead(etymology.LEtymologyResultTargets),
            etymology.LEtymologyResultFilled,
            etymology.LEtymologyResultNarrated,
            etymology.LEtymologyResultLinked,
            shown.LEntryDraftDerived);
    }
}

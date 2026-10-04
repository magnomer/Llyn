using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CCompass
{
    private readonly LDisplaySound _cCompassVoice;

    private readonly CLedgerNoticed _cCompassNoticed;

    private readonly LEntryPort _cCompassPort;

    private readonly LSettingsPort _cCompassSettings;

    private readonly CEnvoy _cCompassEnvoy;

    internal CCompass(LDisplay display, LEntryPort entries, LSettingsPort settings, CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(display);
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(envoy);

        _cCompassVoice = display.LDisplaySound;
        _cCompassNoticed = display.LDisplayNoticed;
        _cCompassPort = entries;
        _cCompassSettings = settings;
        _cCompassEnvoy = envoy;
    }

    public (CCompassPart, int)? CCompassCardFind(long id)
    {
        if (_cCompassVoice.LDisplayShown is not LEntryDraft shown
            || LEntryPort.LEngineCardFind(shown, id) is not (LOwner owner, _))
        {
            return null;
        }

        (CCompassPart part, IReadOnlyList<LCardDraft> cards) = owner switch
        {
            LOwner.LOwnerMeaning => (CCompassPart.CCompassPartMeaning, shown.LEntryDraftMeanings),
            LOwner.LOwnerCollocation => (CCompassPart.CCompassPartCollocation, shown.LEntryDraftCollocations),
            _ => throw new ArgumentOutOfRangeException(nameof(id), owner, null),
        };
        IReadOnlyList<LCardDraft> ordered = CFolio.CFolioOrderRead(cards);
        for (int index = 0; index < ordered.Count; index++)
        {
            if (ordered[index].LCardDraftId == id)
            {
                return (part, index);
            }
        }

        return null;
    }

    public IReadOnlyList<CCompassRow> CCompassRead(IReadOnlyList<CCompassPart> parts, Func<string, string> lookup)
    {
        ArgumentNullException.ThrowIfNull(parts);
        ArgumentNullException.ThrowIfNull(lookup);

        LEntryDraft? shown = _cCompassVoice.LDisplayShown;
        List<CCompassRow> rows = [];
        List<string> labels = [];
        foreach (CCompassPart part in parts)
        {
            rows.Add(new CCompassRow(part, null, string.Empty, string.Empty, 0));
            labels.Add(lookup(LCompassKeyRead(part)));
            if (shown is null
                || part is not (CCompassPart.CCompassPartMeaning or CCompassPart.CCompassPartCollocation))
            {
                continue;
            }

            bool collocated = part == CCompassPart.CCompassPartCollocation;
            IReadOnlyList<LCardDraft> cards = CFolio.CFolioOrderRead(
                collocated ? shown.LEntryDraftCollocations : shown.LEntryDraftMeanings);
            string kind = lookup(collocated ? "Display.CollocationSingle" : "Display.MeaningSingle");
            string unknown = lookup("Display.Unknown");
            for (int index = 0; index < cards.Count; index++)
            {
                CStateValue title = CFolio.CFolioStateRead(cards[index].LCardDraftTitle);
                rows.Add(new CCompassRow(
                    part,
                    index,
                    string.Empty,
                    cards[index].LCardDraftPosition.ToString(CultureInfo.CurrentCulture),
                    1));
                labels.Add(title.CStateValueUncertain ? unknown : title.CStateValueShown ?? kind);
            }
        }

        IReadOnlyList<string> names = LCompassNameResolve(labels);
        return rows.Select((row, index) => row with { CCompassRowName = names[index] }).ToList();
    }

    private static string LCompassKeyRead(CCompassPart part)
    {
        return part switch
        {
            CCompassPart.CCompassPartSpeech => "Speech.Title",
            CCompassPart.CCompassPartFrequency => "Frequency.Title",
            CCompassPart.CCompassPartMeaning => "Display.MeaningPlural",
            CCompassPart.CCompassPartCollocation => "Display.Collocation",
            CCompassPart.CCompassPartIncoming => "Display.Translated",
            CCompassPart.CCompassPartNote => "Display.Note",
            _ => throw new ArgumentOutOfRangeException(nameof(part), part, null),
        };
    }

    private IReadOnlyList<string> LCompassNameResolve(IReadOnlyList<string> labels)
    {
        try
        {
            return _cCompassPort.LEngineNameResolve(labels);
        }
        catch (Exception exception)
        {
            _cCompassNoticed.LLedgerRepaintShow(_cCompassEnvoy, _cCompassSettings, "Display.NameFailed", exception);
            return labels;
        }
    }
}

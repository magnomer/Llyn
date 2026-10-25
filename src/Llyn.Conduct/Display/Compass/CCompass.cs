using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CCompass
{
    private readonly CVoice _cCompassVoice;

    private readonly CLedgerNoticed _cCompassNoticed;

    private readonly LVistaPort _cCompassVistaPort;

    private readonly LCardPort _cCompassCardPort;

    private readonly LSettingsPort _cCompassSettings;

    private readonly CEnvoy _cCompassEnvoy;

    internal CCompass(
        CRule display,
        LVistaPort vistas,
        LCardPort cards,
        LSettingsPort settings,
        CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(display);
        ArgumentNullException.ThrowIfNull(vistas);
        ArgumentNullException.ThrowIfNull(cards);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(envoy);

        _cCompassVoice = display.CVoice;
        _cCompassNoticed = display.CRuleNoticed;
        _cCompassVistaPort = vistas;
        _cCompassCardPort = cards;
        _cCompassSettings = settings;
        _cCompassEnvoy = envoy;
    }

    public (CCompassPart, int)? CCompassCardFind(bool collocated, long id)
    {
        LCardKind kind = collocated ? LCardKind.LCardKindCollocation : LCardKind.LCardKindMeaning;
        if (_cCompassVoice.CVoiceShown is not LEntryDraft shown
            || _cCompassCardPort.LEngineCardFind(shown, kind, id) is not (LOwner owner, _))
        {
            return null;
        }

        IReadOnlyList<LCardDraft> meanings = shown.LEntryDraftMeanings;
        LCollocationLayout layout = _cCompassCardPort.LEngineCollocationRead(
            shown, new Dictionary<long, IReadOnlyList<LTranslationTarget>>());
        long? home = layout.LCollocationLayoutHome
            .Where(pair => pair.Value.Any(card => card.LCardDraftId == id))
            .Select(static pair => (long?)pair.Key)
            .FirstOrDefault();
        (CCompassPart part, IReadOnlyList<LCardDraft> cards, long target) = owner switch
        {
            LOwner.LOwnerMeaning => (CCompassPart.CCompassPartMeaning, meanings, id),
            LOwner.LOwnerCollocation when home is long homed => (CCompassPart.CCompassPartMeaning, meanings, homed),
            LOwner.LOwnerCollocation => (CCompassPart.CCompassPartCollocation, layout.LCollocationLayoutOther, id),
            _ => throw new ArgumentOutOfRangeException(nameof(id), owner, null),
        };
        IReadOnlyList<LCardDraft> ordered = CFolio.CFolioOrderRead(cards);
        for (int index = 0; index < ordered.Count; index++)
        {
            if (ordered[index].LCardDraftId == target)
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

        LEntryDraft? shown = _cCompassVoice.CVoiceShown;
        LCollocationLayout? layout = shown is null
            ? null
            : _cCompassCardPort.LEngineCollocationRead(
                shown, new Dictionary<long, IReadOnlyList<LTranslationTarget>>());
        List<CCompassRow> rows = [];
        foreach (CCompassPart part in parts)
        {
            if (part == CCompassPart.CCompassPartCollocation && layout?.LCollocationLayoutOther.Count == 0)
            {
                continue;
            }

            rows.Add(new CCompassRow(part, null, lookup(LCompassKeyRead(part)), string.Empty, 0, false));
            if (shown is null
                || layout is null
                || part is not (CCompassPart.CCompassPartMeaning or CCompassPart.CCompassPartCollocation))
            {
                continue;
            }

            bool collocated = part == CCompassPart.CCompassPartCollocation;
            IReadOnlyList<LCardDraft> cards = CFolio.CFolioOrderRead(
                collocated ? layout.LCollocationLayoutOther : shown.LEntryDraftMeanings);
            string kind = lookup(collocated ? "Display.CollocationSingle" : "Display.MeaningSingle");
            string unknown = lookup("Display.Unknown");
            string heading = lookup("Display.CollocationHeading");
            Func<LCardDraft, string, string> label = (card, fallback) =>
            {
                CStateValue title = CFolio.CFolioStateRead(card.LCardDraftTitle);
                return title.CStateValueUncertain ? unknown : title.CStateValueShown ?? fallback;
            };
            for (int index = 0; index < cards.Count; index++)
            {
                rows.Add(new CCompassRow(
                    part,
                    index,
                    label(cards[index], kind),
                    cards[index].LCardDraftPosition.ToString(CultureInfo.CurrentCulture),
                    1,
                    false));
                if (!collocated
                    && layout.LCollocationLayoutHome.GetValueOrDefault(cards[index].LCardDraftId, []).Count > 0)
                {
                    rows.Add(new CCompassRow(
                        part,
                        index,
                        heading,
                        string.Empty,
                        2,
                        true));
                }
            }
        }

        IReadOnlyList<string> names = LCompassNameResolve(
            rows.Where(static row => !row.CCompassRowMuted).Select(static row => row.CCompassRowName).ToList());
        List<CCompassRow> named = [];
        int next = 0;
        foreach (CCompassRow row in rows)
        {
            named.Add(row.CCompassRowMuted ? row : row with { CCompassRowName = names[next++] });
        }

        return named;
    }

    private static string LCompassKeyRead(CCompassPart part)
    {
        return part switch
        {
            CCompassPart.CCompassPartSpeech => "Speech.Title",
            CCompassPart.CCompassPartFrequency => "Frequency.Title",
            CCompassPart.CCompassPartMeaning => "Display.MeaningPlural",
            CCompassPart.CCompassPartCollocation => "Display.CollocationOther",
            CCompassPart.CCompassPartIncoming => "Display.Translated",
            CCompassPart.CCompassPartNote => "Display.Note",
            _ => throw new ArgumentOutOfRangeException(nameof(part), part, null),
        };
    }

    private IReadOnlyList<string> LCompassNameResolve(IReadOnlyList<string> labels)
    {
        try
        {
            return _cCompassVistaPort.LEngineNameResolve(labels);
        }
        catch (Exception exception)
        {
            _cCompassNoticed.LLedgerRepaintShow(_cCompassEnvoy, _cCompassSettings, "Display.NameFailed", exception);
            return labels;
        }
    }
}

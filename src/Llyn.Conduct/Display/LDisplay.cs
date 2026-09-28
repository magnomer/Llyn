using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class LDisplay
{
    private readonly LEntryPort _lEntryPort;

    private readonly LPhonologyPort _lPhonologyPort;

    private LVista? _lDisplayVista;

    private (long, int)? _lDisplayGrasp;

    internal LDisplay(
        LDraftPort drafts, LEntryPort entries, LPhonologyPort phonology, LSettingsPort settings, LMediaPort media)
    {
        ArgumentNullException.ThrowIfNull(entries);

        _lEntryPort = entries;
        _lPhonologyPort = phonology;
        LDisplaySound = new LDisplaySound(drafts, entries, phonology, media, settings);
        LDisplaySound.LDisplaySoundFailed += (key, exception) => LDisplayFailed?.Invoke(key, exception);
        CDisplayArea = new CDisplay(this, entries);
    }

    public LDisplaySound LDisplaySound { get; }

    public CDisplay CDisplayArea { get; }

    public event Action<string, Exception>? LDisplayFailed;

    internal long? LDisplayChosen => _lDisplayVista?.LVistaChosen;

    public void LDisplayVistaRestore(LVista vista)
    {
        ArgumentNullException.ThrowIfNull(vista);

        _lDisplayVista = vista;
        CDisplayArea.LDisplayVistaAttach();
    }

    internal void LDisplayChosenAttach(CSubject subject, Action<CBulletin> observer)
    {
        _lDisplayVista?.LVistaChosenAttach(
            CPanel.CPanelSubjectRead(subject), bulletin => observer(CAtelier.CAtelierBulletinRead(bulletin)));
    }

    internal void LDisplayObserverAttach(CSubject subject, Action<CBulletin> observer)
    {
        _lDisplayVista?.LVistaObserverAttach(
            CPanel.CPanelSubjectRead(subject), bulletin => observer(CAtelier.CAtelierBulletinRead(bulletin)));
    }

    internal void LDisplayDraftLoad(Action<LEntryDraft?> show)
    {
        ArgumentNullException.ThrowIfNull(show);

        LEntryDraft? draft;
        try
        {
            draft = _lDisplayVista?.LVistaLoad()?.LDraftContent;
        }
        catch (Exception exception)
        {
            LDisplayFailed?.Invoke("Sound.LoadFailed", exception);
            return;
        }

        show(draft);
    }

    internal void LDisplayEntryLoad(long id)
    {
        LEntryDraft? loaded = _lEntryPort.LEngineEntryLoad(id);
        _lDisplayVista?.LVistaSelect(loaded is null ? null : id);
        CDisplayArea.LDisplayEntryOpen(loaded);
    }

    public bool LDisplayFanqieCheck(long? id) => LDisplaySound.LDisplayFanqieCheck(id);

    public bool LDisplayScriptCheck(long? id) => LDisplaySound.LDisplayScriptCheck(id);

    public bool LDisplayParadigmCheck(long? id) => LDisplaySound.LDisplayParadigmCheck(id);

    internal bool LDisplayFavoriteRead(long? entry)
    {
        if (entry is not long id)
        {
            return false;
        }

        try
        {
            return _lEntryPort.LEngineFavoriteCheck(id);
        }
        catch (Exception)
        {
            return false;
        }
    }

    internal void LDisplayFavoriteSave(long? entry, bool marked)
    {
        if (entry is not long id)
        {
            return;
        }

        try
        {
            if (marked)
            {
                _lEntryPort.LEngineFavoriteSave(id);
            }
            else
            {
                _lEntryPort.LEngineFavoriteDelete(id);
            }
        }
        catch (Exception exception)
        {
            LDisplayFailed?.Invoke("Favorite.MarkFailed", exception);
        }
    }

    internal int LDisplayGraspStep => _lEntryPort.LEngineGraspStep;

    public IReadOnlyList<CCompassRow> CDisplayCompassRead(
        IReadOnlyList<CCompassPart> parts, Func<string, string> lookup)
    {
        ArgumentNullException.ThrowIfNull(parts);
        ArgumentNullException.ThrowIfNull(lookup);

        LEntryDraft? shown = LDisplaySound.LDisplayShown;
        List<CCompassRow> rows = [];
        List<string> labels = [];
        foreach (CCompassPart part in parts)
        {
            rows.Add(new CCompassRow(part, null, string.Empty, string.Empty, 0));
            labels.Add(lookup(LDisplayKeyRead(part)));
            if (shown is null
                || part is not (CCompassPart.CCompassPartMeaning or CCompassPart.CCompassPartCollocation))
            {
                continue;
            }

            bool collocated = part == CCompassPart.CCompassPartCollocation;
            IReadOnlyList<CCardDraft> cards = CFolio.CFolioSheetRead(
                collocated ? shown.LEntryDraftCollocations : shown.LEntryDraftMeanings);
            string kind = lookup(collocated ? "Display.CollocationSingle" : "Display.MeaningSingle");
            string unknown = lookup("Display.Unknown");
            for (int index = 0; index < cards.Count; index++)
            {
                CStateValue title = cards[index].CCardDraftTitle;
                rows.Add(new CCompassRow(
                    part,
                    index,
                    string.Empty,
                    cards[index].CCardDraftPosition.ToString(CultureInfo.CurrentCulture),
                    1));
                labels.Add(title.CStateValueUncertain ? unknown : title.CStateValueShown ?? kind);
            }
        }

        IReadOnlyList<string> names = LDisplayNameResolve(labels);
        return rows.Select((row, index) => row with { CCompassRowName = names[index] }).ToList();
    }

    private static string LDisplayKeyRead(CCompassPart part)
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

    private IReadOnlyList<string> LDisplayNameResolve(IReadOnlyList<string> labels)
    {
        try
        {
            return _lEntryPort.LEngineNameResolve(labels);
        }
        catch (Exception exception)
        {
            LDisplayFailed?.Invoke("Display.NameFailed", exception);
            return labels;
        }
    }

    public static bool LDisplayCardCheck(LCardDraft card, long id)
    {
        ArgumentNullException.ThrowIfNull(card);

        return card.LCardDraftId == id;
    }

    internal string LDisplayGraspFormat(long? entry, int step)
    {
        return entry is null
            ? string.Empty
            : _lEntryPort.LEngineGraspFormat(step);
    }

    internal int LDisplayGraspRead(long? entry)
    {
        if (entry is not long id)
        {
            return 0;
        }

        try
        {
            int step = _lEntryPort.LEngineGraspRead(id);
            _lDisplayGrasp = (id, step);
            return step;
        }
        catch (Exception)
        {
            return 0;
        }
    }

    internal void LDisplayGraspSave(long? entry, int step)
    {
        if (entry is not long id)
        {
            return;
        }

        int kept = _lDisplayGrasp == (id, step) ? 0 : step;
        try
        {
            _lEntryPort.LEngineGraspSave(id, kept);
            _lDisplayGrasp = (id, kept);
        }
        catch (Exception exception)
        {
            LDisplayFailed?.Invoke("Grasp.MarkFailed", exception);
        }
    }

    internal CFrequency? LDisplayFrequencyRead(long? entry, string once)
    {
        if (entry is not long id)
        {
            return null;
        }

        LFrequencyGauge? gauge;
        try
        {
            gauge = _lEntryPort.LEngineFrequencyResolve(id, once);
        }
        catch (Exception)
        {
            return null;
        }

        return gauge is null
            ? null
            : new CFrequency(
                gauge.LFrequencyGaugeBand,
                gauge.LFrequencyGaugeSource,
                gauge.LFrequencyGaugeRank,
                gauge.LFrequencyGaugeSpare,
                gauge.LFrequencyGaugeRanked);
    }

    public IReadOnlyList<CUsage> LDisplayIncomingRead()
    {
        if (LDisplayChosen is not long id)
        {
            return [];
        }

        IReadOnlyList<LUsage> incoming;
        try
        {
            incoming = _lEntryPort.LEngineIncomingRead(id);
        }
        catch (Exception exception)
        {
            LDisplayFailed?.Invoke("Display.IncomingFailed", exception);
            return [];
        }

        Exception? missed = null;
        List<CUsage> usages = new(incoming.Count);
        foreach (LUsage usage in incoming)
        {
            string epithet;
            try
            {
                epithet = _lEntryPort.LEngineEpithetRead(usage.LUsageEntry);
            }
            catch (Exception exception)
            {
                epithet = string.Empty;
                missed ??= exception;
            }

            usages.Add(COeuvre.COeuvreUsageRead(usage with { LUsageEpithet = epithet }));
        }

        if (missed is not null)
        {
            LDisplayFailed?.Invoke("Display.EpithetFailed", missed);
        }

        return usages;
    }

    public IReadOnlyList<LTranslationTarget> LDisplayTargetRead()
    {
        if (LDisplaySound.LDisplayShown is not LEntryDraft draft)
        {
            return [];
        }

        try
        {
            return _lEntryPort.LEngineTargetRead(draft);
        }
        catch (Exception)
        {
            return [];
        }
    }

    public IReadOnlyList<LTranslationTarget> LDisplayEtymonRead()
    {
        return LDisplaySound.LDisplayShown is LEntryDraft draft ? LDisplayEtymonRead(draft) : [];
    }

    public CSentenceOrder LDisplayOrderRead()
    {
        if (LDisplaySound.LDisplayShown is not LEntryDraft draft)
        {
            return CFolio.CFolioOrderRead(LSentenceOrder.LSentenceOrderDefault);
        }

        try
        {
            return CFolio.CFolioOrderRead(_lPhonologyPort.LEngineOrderRead(draft.LEntryDraftLanguage));
        }
        catch (Exception)
        {
            return CFolio.CFolioOrderRead(LSentenceOrder.LSentenceOrderDefault);
        }
    }

    public IReadOnlyList<LTranslationTarget> LDisplayEtymonRead(LEntryDraft draft)
    {
        try
        {
            return _lEntryPort.LEngineEtymonRead(draft);
        }
        catch (Exception)
        {
            return [];
        }
    }

    public static bool LDisplayEtymologyCheck(string text, int count)
    {
        return text.Trim().Length > 0 || count > 0;
    }

    public IReadOnlyDictionary<long, string> LDisplayCitationRead()
    {
        try
        {
            return _lEntryPort.LEngineCitationRead();
        }
        catch (Exception)
        {
            return new Dictionary<long, string>();
        }
    }

    public void LDisplayMentionFind<LDisplayAnchor>(
        LDisplayAnchor anchor,
        string text,
        string language,
        int offset,
        IReadOnlyList<LMention>? mentions,
        Action<LDisplayAnchor, LMentionResult> show)
    {
        ArgumentNullException.ThrowIfNull(show);

        string shown = language.Length > 0
            ? language
            : LDisplaySound.LDisplayShown?.LEntryDraftLanguage ?? string.Empty;
        LMentionResult result;
        try
        {
            result = _lEntryPort.LEngineMentionFind(text, shown, offset, mentions ?? []);
        }
        catch (Exception exception)
        {
            LDisplayFailed?.Invoke("Mention.FindFailed", exception);
            return;
        }

        show(anchor, result);
    }
}

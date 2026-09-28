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

    private LEntryDraft? _lDisplayLoaded;

    internal LDisplay(
        LDraftPort drafts, LEntryPort entries, LPhonologyPort phonology, LSettingsPort settings, LMediaPort media)
    {
        ArgumentNullException.ThrowIfNull(entries);

        _lEntryPort = entries;
        _lPhonologyPort = phonology;
        LDisplaySound = new LDisplaySound(drafts, entries, phonology, media, settings);
        LDisplaySound.LDisplaySoundFailed += (key, exception) => LDisplayFailed?.Invoke(key, exception);
    }

    public LDisplaySound LDisplaySound { get; }

    public event Action<string, Exception>? LDisplayFailed;

    public long? LDisplayChosen => _lDisplayVista?.LVistaChosen;

    public void LDisplayVistaRestore(LVista vista)
    {
        ArgumentNullException.ThrowIfNull(vista);

        _lDisplayVista = vista;
    }

    public void LDisplayChosenAttach(CSubject subject, Action<CBulletin> observer)
    {
        _lDisplayVista?.LVistaChosenAttach(
            (LSubject)subject, bulletin => observer(CAtelier.CAtelierBulletinRead(bulletin)));
    }

    public void LDisplayObserverAttach(CSubject subject, Action<CBulletin> observer)
    {
        _lDisplayVista?.LVistaObserverAttach(
            (LSubject)subject, bulletin => observer(CAtelier.CAtelierBulletinRead(bulletin)));
    }

    public void LDisplayDraftLoad(Action<LEntryDraft?> show)
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

    public LEntryDraft? LDisplayLoaded => _lDisplayLoaded;

    public void LDisplayEntryLoad(long id)
    {
        _lDisplayLoaded = _lEntryPort.LEngineEntryLoad(id);
        _lDisplayVista?.LVistaSelect(_lDisplayLoaded is null ? null : id);
    }

    public string LDisplayLanguageRead()
    {
        return LDisplaySound.LDisplayShown?.LEntryDraftLanguage ?? string.Empty;
    }

    public void LDisplayShow(LEntryDraft draft)
    {
        LDisplaySound.LDisplaySoundShow(LDisplayChosen, draft);
    }

    public void LDisplayClear()
    {
        LDisplaySound.LDisplaySoundClear();
    }

    public bool LDisplayFanqieCheck(long? id) => LDisplaySound.LDisplayFanqieCheck(id);

    public bool LDisplayScriptCheck(long? id) => LDisplaySound.LDisplayScriptCheck(id);

    public bool LDisplayParadigmCheck(long? id) => LDisplaySound.LDisplayParadigmCheck(id);

    public bool LDisplayFavoriteRead(long? entry)
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

    public void LDisplayFavoriteSave(long? entry, bool marked)
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

    public int LDisplayGraspStep => _lEntryPort.LEngineGraspStep;

    public IReadOnlyList<string> LDisplayNameResolve(IReadOnlyList<string> labels)
    {
        ArgumentNullException.ThrowIfNull(labels);

        IReadOnlyList<string> names;
        try
        {
            names = _lEntryPort.LEngineNameResolve(labels);
        }
        catch (Exception exception)
        {
            LDisplayFailed?.Invoke("Display.NameFailed", exception);
            return labels;
        }

        return names.Count >= labels.Count ? names : [.. names, .. labels.Skip(names.Count)];
    }

    public static string LDisplayTitleRead(LCardDraft card, string kind, string unknown)
    {
        ArgumentNullException.ThrowIfNull(card);

        return card.LCardDraftTitle.LStateValueUncertain
            ? unknown
            : card.LCardDraftTitle.LStateValueShown ?? kind;
    }

    public static bool LDisplayCardCheck(LCardDraft card, long id)
    {
        ArgumentNullException.ThrowIfNull(card);

        return card.LCardDraftId == id;
    }

    public string LDisplayGraspFormat(long? entry, int step)
    {
        return entry is null
            ? string.Empty
            : _lEntryPort.LEngineGraspFormat(step);
    }

    public int LDisplayGraspRead(long? entry)
    {
        if (entry is not long id)
        {
            return 0;
        }

        try
        {
            return _lEntryPort.LEngineGraspRead(id);
        }
        catch (Exception)
        {
            return 0;
        }
    }

    public void LDisplayGraspSave(long? entry, int step)
    {
        if (entry is not long id)
        {
            return;
        }

        try
        {
            _lEntryPort.LEngineGraspSave(id, step);
        }
        catch (Exception exception)
        {
            LDisplayFailed?.Invoke("Grasp.MarkFailed", exception);
        }
    }

    public LDisplayStamp LDisplayStampRead()
    {
        LEntry? entry;
        try
        {
            entry = LDisplayChosen is long id ? _lEntryPort.LEngineEntryRead(id) : null;
        }
        catch (Exception)
        {
            entry = null;
        }

        return entry is null
            ? new LDisplayStamp(false, string.Empty, string.Empty)
            : new LDisplayStamp(
                true, LDisplayStampFormat(entry.LEntryAddedUtc), LDisplayStampFormat(entry.LEntryUpdatedUtc));
    }

    public static string LDisplayStampFormat(string? utc)
    {
        if (utc is null
            || !DateTimeOffset.TryParse(
                utc, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTimeOffset parsed))
        {
            return string.Empty;
        }

        return parsed.ToLocalTime().ToString("g", CultureInfo.CurrentCulture);
    }

    public IReadOnlyList<string> LDisplaySpeechRead()
    {
        if (LDisplaySound.LDisplayShown is not LEntryDraft draft)
        {
            return [];
        }

        List<string> named = new(draft.LEntryDraftSpeeches.Count);
        foreach (LSpeechDraft speech in draft.LEntryDraftSpeeches)
        {
            if (speech.LSpeechDraftNamed)
            {
                named.Add(speech.LSpeechDraftName);
            }
        }

        return named;
    }

    public CFrequency? LDisplayFrequencyRead(long? entry, string once)
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

    public static bool LDisplayNarrativeCheck(bool editable, string text)
    {
        return !editable && text.Trim().Length > 0;
    }

    public static bool LDisplayEtymonCheck(bool editable, int count)
    {
        return editable || count > 0;
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

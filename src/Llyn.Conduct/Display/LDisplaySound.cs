using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class LDisplaySound
{
    private readonly LEntryPort _lEntryPort;

    private readonly LPhonologyPort _lPhonologyPort;

    private readonly LMediaPort _lMediaPort;

    private readonly LSettingsPort _lSettingsPort;

    private LEntryDraft? _lDisplaySoundDraft;

    private long? _lDisplaySoundEntry;

    private bool _lDisplaySoundOpened;

    private IReadOnlyList<LParadigmSlot> _lDisplaySoundParadigm = [];

    public LDisplaySound(LEntryPort entries, LPhonologyPort phonology, LMediaPort media, LSettingsPort settings)
    {
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(phonology);
        ArgumentNullException.ThrowIfNull(media);
        ArgumentNullException.ThrowIfNull(settings);

        _lEntryPort = entries;
        _lPhonologyPort = phonology;
        _lMediaPort = media;
        _lSettingsPort = settings;
    }

    public LEntryDraft? LDisplayShown => _lDisplaySoundDraft;

    public long? LDisplayEntry => _lDisplaySoundEntry;

    public bool LDisplayFoldOpened => _lDisplaySoundOpened;

    internal void LDisplaySoundShow(long? id, LEntryDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        _lDisplaySoundEntry = id;
        _lDisplaySoundDraft = draft;
    }

    internal void LDisplaySoundClear()
    {
        _lDisplaySoundEntry = null;
        _lDisplaySoundDraft = null;
        _lDisplaySoundParadigm = [];
        _lMediaPort.LEngineRecordingStop();
    }

    public void LDisplayFoldSet(bool opened)
    {
        _lDisplaySoundOpened = opened;
    }

    public static HashSet<string> LDisplayFoldScan(IReadOnlyList<LReflexRule> rules)
    {
        ArgumentNullException.ThrowIfNull(rules);

        HashSet<string> folded = new(StringComparer.Ordinal);
        foreach (LReflexRule rule in rules)
        {
            if (rule.LReflexRuleFolded)
            {
                folded.Add(rule.LReflexRuleLanguage);
            }
        }

        return folded;
    }

    public void LDisplayReflexLoad()
    {
        if (_lDisplaySoundEntry is not long shown)
        {
            return;
        }

        try
        {
            _lDisplaySoundDraft = _lEntryPort.LEngineEntryLoad(shown) ?? _lDisplaySoundDraft;
        }
        catch (Exception)
        {
        }
    }

    public IReadOnlyList<LReflexDraft> LDisplayReflexRead()
    {
        return _lDisplaySoundDraft?.LEntryDraftReflexes
            .Where(static reflex => reflex.LReflexDraftWritten)
            .ToList()
            ?? [];
    }

    public void LDisplayReflexStart(long? id)
    {
        LDisplayMarkSend(_lPhonologyPort.LEngineReflexStart, id);
    }

    public bool LDisplayReflexCheck(long? id)
    {
        return LDisplayPendingRead(_lPhonologyPort.LEngineReflexCheck, id);
    }

    public void LDisplayReflexRebuild(long? id)
    {
        LDisplayMarkSend(_lPhonologyPort.LEngineReflexRebuild, id);
    }

    public bool LDisplayFanqieCheck(long? id)
    {
        return LDisplayPendingRead(_lPhonologyPort.LEngineFanqieCheck, id);
    }

    public bool LDisplayScriptCheck(long? id)
    {
        return LDisplayPendingRead(_lPhonologyPort.LEngineScriptCheck, id);
    }

    public bool LDisplayParadigmCheck(long? id)
    {
        return LDisplayPendingRead(_lPhonologyPort.LEngineInflectionCheck, id);
    }

    public void LDisplayFanqieStart()
    {
        LDisplayMarkSend(_lPhonologyPort.LEngineFanqieStart, _lDisplaySoundEntry);
    }

    public IReadOnlyList<LFanqieGroup> LDisplayFanqieDivide()
    {
        return LDisplayListRead(_lPhonologyPort.LEngineFanqieDivide);
    }

    public IReadOnlyList<LFanqieRow> LDisplayAnchorRead()
    {
        return LDisplayFanqieDivide().SelectMany(static group => group.LFanqieGroupRows).ToList();
    }

    public string LDisplayReadingRead()
    {
        if (_lDisplaySoundEntry is not long shown || _lDisplaySoundDraft is null)
        {
            return string.Empty;
        }

        try
        {
            return _lPhonologyPort.LEngineReadingRead(shown, _lDisplaySoundDraft.LEntryDraftHeadword);
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }

    public void LDisplayFanqieSet(long fanqieId, int rank)
    {
        LDisplayMarkSend(shown => _lPhonologyPort.LEngineFanqieSet(shown, fanqieId, rank), _lDisplaySoundEntry);
    }

    public void LDisplayScriptStart()
    {
        LDisplayMarkSend(_lPhonologyPort.LEngineScriptStart, _lDisplaySoundEntry);
    }

    public IReadOnlyList<LScriptGroup> LDisplayScriptDivide()
    {
        return LDisplayListRead(_lPhonologyPort.LEngineScriptDivide);
    }

    public IReadOnlyList<LParadigmSlot> LDisplayParadigmRead()
    {
        _lDisplaySoundParadigm = LDisplayListRead(_lPhonologyPort.LEngineParadigmShow);
        return _lDisplaySoundParadigm;
    }

    public void LDisplayInflectionStart()
    {
        if (!LDisplayMorphologyRead())
        {
            return;
        }

        LDisplayMarkSend(_lPhonologyPort.LEngineInflectionStart, _lDisplaySoundEntry);
    }

    public string LDisplayLanguageRead()
    {
        return _lDisplaySoundParadigm.Count == 0
            ? string.Empty
            : _lDisplaySoundParadigm[0].LParadigmSlotSpeech.LSpeechValueLanguage;
    }

    public bool LDisplayMorphologyRead()
    {
        try
        {
            return _lSettingsPort.LEngineSettingsRead().LSettingsMorphology;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool LDisplayTonalCheck()
    {
        return _lDisplaySoundDraft is not null
            && _lPhonologyPort.LEngineTonalCheck(_lDisplaySoundDraft.LEntryDraftLanguage);
    }

    public bool LDisplayFlaggedCheck()
    {
        return _lDisplaySoundDraft is not null && _lPhonologyPort.LEngineFlaggedCheck(_lDisplaySoundDraft);
    }

    public bool LDisplayFlaggedCheck(string language)
    {
        return string.Equals(_lDisplaySoundDraft?.LEntryDraftLanguage, language, StringComparison.Ordinal)
            && LDisplayFlaggedCheck();
    }

    public IReadOnlyList<LPronunciationDraft> LDisplayAccentRead()
    {
        return _lDisplaySoundDraft?.LEntryDraftAccents
            .Where(static spoken => spoken.LPronunciationDraftNotated)
            .ToList()
            ?? [];
    }

    public IReadOnlyList<LTranscriptionDraft> LDisplayTranscriptionRead()
    {
        if (_lDisplaySoundDraft is null)
        {
            return [];
        }

        LGlyph? glyph = LDisplayGlyphRead();
        return _lDisplaySoundDraft.LEntryDraftTranscriptions
            .Where(spelled => !spelled.LTranscriptionDraftEmpty
                && glyph?.LGlyphSchemeCheck(spelled.LTranscriptionDraftScheme) != true)
            .ToList();
    }

    public LGlyph? LDisplayGlyphRead()
    {
        return _lDisplaySoundDraft is null
            ? null
            : _lEntryPort.LEngineGlyphRead(_lDisplaySoundDraft.LEntryDraftLanguage);
    }

    public IReadOnlyList<LGlyphCell> LDisplayGlyphDivide()
    {
        return _lDisplaySoundDraft is null ? [] : _lEntryPort.LEngineGlyphDivide(_lDisplaySoundDraft);
    }

    public bool LDisplayRecordingCheck()
    {
        return _lDisplaySoundDraft is not null
            && _lMediaPort.LEngineRecordingExist(_lDisplaySoundDraft.LEntryDraftAudio);
    }

    public bool LDisplayAudibleCheck()
    {
        return LDisplayRecordingCheck()
            || (_lDisplaySoundDraft?.LEntryDraftAccents.Any(
                static spoken => spoken.LPronunciationDraftNotated && spoken.LPronunciationDraftAudio.Length > 0)
                ?? false);
    }

    public void LDisplayRecordingPlay()
    {
        if (!LDisplayRecordingCheck())
        {
            return;
        }

        _lMediaPort.LEngineRecordingPlay(_lDisplaySoundDraft!.LEntryDraftAudio);
    }

    public void LDisplayRecordingPlay(string? file)
    {
        _lMediaPort.LEngineRecordingPlay(file);
    }

    public void LDisplayPlaybackStop()
    {
        _lMediaPort.LEngineRecordingStop();
    }

    public void LDisplayVolumeSet(double level)
    {
        _lMediaPort.LEngineVolumeSet(level);
    }

    private IReadOnlyList<LDisplayItem> LDisplayListRead<LDisplayItem>(Func<long, IReadOnlyList<LDisplayItem>> read)
    {
        if (_lDisplaySoundEntry is not long shown)
        {
            return [];
        }

        try
        {
            return read(shown);
        }
        catch (Exception)
        {
            return [];
        }
    }

    private static void LDisplayMarkSend(Action<long> mark, long? id)
    {
        if (id is not long shown)
        {
            return;
        }

        try
        {
            mark(shown);
        }
        catch (Exception)
        {
        }
    }

    private static bool LDisplayPendingRead(Func<long, bool> check, long? id)
    {
        if (id is not long shown)
        {
            return false;
        }

        try
        {
            return check(shown);
        }
        catch (Exception)
        {
            return false;
        }
    }
}

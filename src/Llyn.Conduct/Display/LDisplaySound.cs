using System;
using System.Collections.Generic;
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

    private LEntryDraft? _lDisplaySoundReflex;

    internal LDisplaySound(LEntryPort entries, LPhonologyPort phonology, LMediaPort media, LSettingsPort settings)
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

    internal long? LDisplayEntry => _lDisplaySoundEntry;

    internal bool LDisplayFoldOpened => _lDisplaySoundOpened;

    internal int LDisplayTicket { get; set; }

    internal event Action<string, Exception>? LDisplaySoundFailed;

    internal event Action<string, Exception>? LDisplayMarkFailed;

    internal void LDisplaySoundShow(long? id, LEntryDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        _lDisplaySoundEntry = id;
        _lDisplaySoundDraft = draft;
        _lDisplaySoundReflex = null;
        LDisplayMarkSend(_lPhonologyPort.LEngineSoundStart, id, "Sound.StartFailed");
    }

    internal void LDisplaySoundClear()
    {
        _lDisplaySoundEntry = null;
        _lDisplaySoundDraft = null;
        _lDisplaySoundReflex = null;
        _lMediaPort.LEngineRecordingStop(LDisplayTicket);
    }

    internal void LDisplayFoldSet(bool opened)
    {
        _lDisplaySoundOpened = opened;
    }

    internal void LDisplayReflexLoad()
    {
        if (_lDisplaySoundEntry is not long shown)
        {
            return;
        }

        try
        {
            _lDisplaySoundReflex = _lEntryPort.LEngineEntryLoad(shown) ?? _lDisplaySoundReflex;
        }
        catch (Exception exception)
        {
            LDisplaySoundFailed?.Invoke("Sound.LoadFailed", exception);
        }
    }

    internal IReadOnlyList<LReflexDraft> LDisplayReflexRead()
    {
        LEntryDraft? source = _lDisplaySoundReflex ?? _lDisplaySoundDraft;
        return source is null ? [] : _lEntryPort.LEngineReflexRead(source);
    }

    internal bool LDisplayReflexCheck(long? id)
    {
        return LDisplayPendingRead(_lPhonologyPort.LEngineReflexCheck, id);
    }

    internal void LDisplayReflexRebuild(long? id)
    {
        LDisplayMarkSend(_lPhonologyPort.LEngineReflexRebuild, id, "Display.ReflexRebuildFailed");
    }

    internal bool LDisplayFanqieCheck(long? id)
    {
        return LDisplayPendingRead(_lPhonologyPort.LEngineFanqieCheck, id);
    }

    internal bool LDisplayScriptCheck(long? id)
    {
        return LDisplayPendingRead(_lPhonologyPort.LEngineScriptCheck, id);
    }

    internal bool LDisplayParadigmCheck(long? id)
    {
        return LDisplayPendingRead(_lPhonologyPort.LEngineInflectionCheck, id);
    }

    internal bool LDisplayMorphologyRead()
    {
        try
        {
            return _lSettingsPort.LEngineMorphologyCheck();
        }
        catch (Exception exception)
        {
            LDisplaySoundFailed?.Invoke("Sound.MorphologyFailed", exception);
            return false;
        }
    }

    internal void LDisplayPlaybackStop()
    {
        _lMediaPort.LEngineRecordingStop(LDisplayTicket);
    }

    internal IReadOnlyList<LDisplayItem> LDisplayListRead<LDisplayItem>(
        Func<long, IReadOnlyList<LDisplayItem>> read, string key)
    {
        if (_lDisplaySoundEntry is not long shown)
        {
            return [];
        }

        try
        {
            return read(shown);
        }
        catch (Exception exception)
        {
            LDisplaySoundFailed?.Invoke(key, exception);
            return [];
        }
    }

    private void LDisplayMarkSend(Action<long> mark, long? id, string key)
    {
        if (id is not long shown)
        {
            return;
        }

        try
        {
            mark(shown);
        }
        catch (Exception exception)
        {
            LDisplayMarkFailed?.Invoke(key, exception);
        }
    }

    private bool LDisplayPendingRead(Func<long, bool> check, long? id)
    {
        if (id is not long shown)
        {
            return false;
        }

        try
        {
            return check(shown);
        }
        catch (Exception exception)
        {
            LDisplaySoundFailed?.Invoke("Sound.PendingFailed", exception);
            return false;
        }
    }
}

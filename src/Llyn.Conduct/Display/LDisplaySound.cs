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

    public bool LDisplayFoldOpened => _lDisplaySoundOpened;

    internal int LDisplayTicket { get; set; }

    internal event Action<string, Exception>? LDisplaySoundFailed;

    internal void LDisplaySoundShow(long? id, LEntryDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        _lDisplaySoundEntry = id;
        _lDisplaySoundDraft = draft;
        _lDisplaySoundReflex = null;
        LDisplayMarkSend(_lPhonologyPort.LEngineSoundStart, id);
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

    internal void LDisplayReflexStart(long? id)
    {
        LDisplayMarkSend(_lPhonologyPort.LEngineReflexStart, id);
    }

    internal bool LDisplayReflexCheck(long? id)
    {
        return LDisplayPendingRead(_lPhonologyPort.LEngineReflexCheck, id);
    }

    internal void LDisplayReflexRebuild(long? id)
    {
        LDisplayMarkSend(_lPhonologyPort.LEngineReflexRebuild, id);
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

    public bool LDisplayMorphologyRead()
    {
        try
        {
            return _lSettingsPort.LEngineMorphologyCheck();
        }
        catch (Exception)
        {
            return false;
        }
    }

    internal void LDisplayPlaybackStop()
    {
        _lMediaPort.LEngineRecordingStop(LDisplayTicket);
    }

    internal IReadOnlyList<LDisplayItem> LDisplayListRead<LDisplayItem>(Func<long, IReadOnlyList<LDisplayItem>> read)
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

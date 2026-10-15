using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

internal sealed class CVoice
{
    private readonly LEntryPort _cVoiceEntryPort;

    private readonly LLanguagePort _cVoiceLanguagePort;

    private readonly LReflexPort _cVoiceReflexPort;

    private readonly LFanqiePort _cVoiceFanqiePort;

    private readonly LScriptPort _cVoiceScriptPort;

    private readonly LParadigmPort _cVoiceParadigmPort;

    private readonly LMediaPort _cVoiceMediaPort;

    private readonly LSettingsPort _cVoiceSettingsPort;

    private LEntryDraft? _cVoiceDraft;

    private long? _cVoiceEntry;

    private LEntryDraft? _cVoiceReflex;

    internal CVoice(
        LEntryPort entries,
        LLanguagePort languages,
        LReflexPort reflexes,
        LFanqiePort fanqies,
        LScriptPort scripts,
        LParadigmPort paradigms,
        LMediaPort media,
        LSettingsPort settings)
    {
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(languages);
        ArgumentNullException.ThrowIfNull(reflexes);
        ArgumentNullException.ThrowIfNull(fanqies);
        ArgumentNullException.ThrowIfNull(scripts);
        ArgumentNullException.ThrowIfNull(paradigms);
        ArgumentNullException.ThrowIfNull(media);
        ArgumentNullException.ThrowIfNull(settings);

        _cVoiceEntryPort = entries;
        _cVoiceLanguagePort = languages;
        _cVoiceReflexPort = reflexes;
        _cVoiceFanqiePort = fanqies;
        _cVoiceScriptPort = scripts;
        _cVoiceParadigmPort = paradigms;
        _cVoiceMediaPort = media;
        _cVoiceSettingsPort = settings;
    }

    internal LEntryDraft? CVoiceShown => _cVoiceDraft;

    internal long? CVoiceEntry => _cVoiceEntry;

    internal int CVoiceTicket { get; set; }

    internal event Action<string, Exception>? CVoiceFailed;

    internal event Action<string, Exception>? CVoiceMarkFailed;

    internal void CVoiceShow(long? id, LEntryDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        _cVoiceEntry = id;
        _cVoiceDraft = draft;
        _cVoiceReflex = null;
        CVoiceMarkSend(_cVoiceLanguagePort.LEngineSoundStart, id, "Sound.StartFailed");
    }

    internal void CVoiceClear()
    {
        if (_cVoiceDraft is null)
        {
            return;
        }

        _cVoiceEntry = null;
        _cVoiceDraft = null;
        _cVoiceReflex = null;
        _cVoiceMediaPort.LEngineRecordingStop(CVoiceTicket);
    }

    internal bool CVoiceSpreadCheck(long? id)
    {
        if (id is not long shown)
        {
            return false;
        }

        try
        {
            return _cVoiceReflexPort.LEngineSpreadCheck(shown);
        }
        catch (Exception exception)
        {
            CVoiceFailed?.Invoke("Reflex.SpreadReadFailed", exception);
            return false;
        }
    }

    internal void CVoiceReflexLoad()
    {
        if (_cVoiceEntry is not long shown)
        {
            return;
        }

        try
        {
            _cVoiceReflex = _cVoiceEntryPort.LEngineEntryLoad(shown) ?? _cVoiceReflex;
        }
        catch (Exception exception)
        {
            CVoiceFailed?.Invoke("Sound.LoadFailed", exception);
        }
    }

    internal IReadOnlyList<LReflexDraft> CVoiceReflexRead()
    {
        LEntryDraft? source = _cVoiceReflex ?? _cVoiceDraft;
        return source is null ? [] : _cVoiceEntryPort.LEngineReflexRead(source);
    }

    internal bool CVoiceReflexCheck(long? id)
    {
        return CVoicePendingRead(_cVoiceReflexPort.LEngineReflexCheck, id);
    }

    internal void CVoiceReflexRebuild(long? id)
    {
        CVoiceMarkSend(_cVoiceReflexPort.LEngineReflexRebuild, id, "Display.ReflexRebuildFailed");
    }

    internal bool CVoiceFanqieCheck(long? id)
    {
        return CVoicePendingRead(_cVoiceFanqiePort.LEngineFanqieCheck, id);
    }

    internal bool CVoiceScriptCheck(long? id)
    {
        return CVoicePendingRead(_cVoiceScriptPort.LEngineScriptCheck, id);
    }

    internal bool CVoiceParadigmCheck(long? id)
    {
        return CVoicePendingRead(_cVoiceParadigmPort.LEngineInflectionCheck, id);
    }

    internal bool CVoiceMorphologyRead()
    {
        try
        {
            return _cVoiceSettingsPort.LEngineMorphologyCheck();
        }
        catch (Exception exception)
        {
            CVoiceFailed?.Invoke("Sound.MorphologyFailed", exception);
            return false;
        }
    }

    internal void CVoicePlaybackStop()
    {
        _cVoiceMediaPort.LEngineRecordingStop(CVoiceTicket);
    }

    internal IReadOnlyList<CVoiceItem> CVoiceListRead<CVoiceItem>(
        Func<long, IReadOnlyList<CVoiceItem>> read, string key)
    {
        if (_cVoiceEntry is not long shown)
        {
            return [];
        }

        try
        {
            return read(shown);
        }
        catch (Exception exception)
        {
            CVoiceFailed?.Invoke(key, exception);
            return [];
        }
    }

    private void CVoiceMarkSend(Action<long> mark, long? id, string key)
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
            CVoiceMarkFailed?.Invoke(key, exception);
        }
    }

    private bool CVoicePendingRead(Func<long, bool> check, long? id)
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
            CVoiceFailed?.Invoke("Sound.PendingFailed", exception);
            return false;
        }
    }
}

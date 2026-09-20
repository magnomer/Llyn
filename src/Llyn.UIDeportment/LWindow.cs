using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Application;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIDeportment;

public sealed class LWindow
{
    private readonly LDraftPort _lDraftPort;

    private readonly LSettingsPort _lSettingsPort;

    private readonly LPhonologyPort _lPhonologyPort;

    private readonly LMediaPort _lMediaPort;

    private readonly LEntryPort _lEntryPort;

    private readonly LPortraitPort _lPortraitPort;

    public LWindow(
        LDraftPort drafts,
        LEntryPort entries,
        LSettingsPort settings,
        LPhonologyPort phonology,
        LMediaPort media,
        LPortraitPort portraits)
    {
        ArgumentNullException.ThrowIfNull(drafts);
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(phonology);
        ArgumentNullException.ThrowIfNull(media);
        ArgumentNullException.ThrowIfNull(portraits);

        _lDraftPort = drafts;
        _lEntryPort = entries;
        _lSettingsPort = settings;
        _lPhonologyPort = phonology;
        _lMediaPort = media;
        _lPortraitPort = portraits;
    }

    public void LWindowObserverAttach(LObserver observer)
    {
        _lDraftPort.LEngineObserverAttach(observer);
    }

    public void LWindowObserverDetach(LObserver observer)
    {
        _lDraftPort.LEngineObserverDetach(observer);
    }

    public LFont LWindowFontRead(string language, LFontRole role)
    {
        return _lSettingsPort.LEngineFontRead(language, role);
    }

    public Task<IReadOnlyList<LEnsignRow>> LWindowEnsignLoad()
    {
        return _lSettingsPort.LEngineEnsignLoad();
    }

    public Task<IReadOnlyList<LEnsignRow>> LWindowEnsignLoad(string language, IEnumerable<string> varieties)
    {
        return _lSettingsPort.LEngineEnsignLoad(language, varieties);
    }

    public void LWindowEnsignDelete(string path)
    {
        _lSettingsPort.LEngineEnsignDelete(path);
    }

    public bool LWindowRespellingCheck(string language)
    {
        return _lPhonologyPort.LEngineRespellingCheck(language);
    }

    public bool LWindowPhonemicCheck(string language)
    {
        return _lPhonologyPort.LEnginePhonemicCheck(language);
    }

    public IReadOnlyList<LReflexRule> LWindowReflexRead(string language)
    {
        return _lPhonologyPort.LEngineReflexRead(language);
    }

    public Uri? LWindowLocationRead(string? location)
    {
        return _lMediaPort.LEngineLocationRead(location);
    }

    public bool LWindowRecordingExist(string? file)
    {
        return _lMediaPort.LEngineRecordingExist(file);
    }

    public LSentenceOrder LWindowOrderRead(string language)
    {
        return _lPhonologyPort.LEngineOrderRead(language);
    }

    public IReadOnlyList<LMentionLabel> LWindowMentionResolve(string text, IReadOnlyList<LMentionDraft> mentions)
    {
        return _lEntryPort.LEngineMentionResolve(text, mentions);
    }

    public IReadOnlyList<string> LWindowLanguageRead()
    {
        return _lSettingsPort.LEngineLanguageRead();
    }

    public IReadOnlyList<string> LWindowSchemeRead(string language)
    {
        return _lPhonologyPort.LEngineSchemeRead(language);
    }

    public IReadOnlyList<LAnatomyTone> LWindowToneRead(string language)
    {
        return _lPhonologyPort.LEngineToneRead(language);
    }

    public IReadOnlyList<LSpeechValue> LWindowSpeechRead(string language)
    {
        return _lPhonologyPort.LEngineSpeechRead(language);
    }

    public LSpeechValue? LWindowSpeechAdd(string language, string name)
    {
        return _lPhonologyPort.LEngineSpeechAdd(language, name);
    }

    public LGlyph? LWindowGlyphRead(string language)
    {
        return _lEntryPort.LEngineGlyphRead(language);
    }

    public LEntryDraft? LWindowEntryLoad(long id)
    {
        return _lEntryPort.LEngineEntryLoad(id);
    }

    public IReadOnlyList<LEntry> LWindowMarkupFind(LMarkupEntry entry)
    {
        return _lEntryPort.LEngineMarkupFind(entry);
    }

    public Task<string> LWindowRecordingPrepare(LRecording recording, CancellationToken cancellation)
    {
        return _lMediaPort.LEngineRecordingPrepare(recording, cancellation);
    }

    public LSettings LWindowSettingsRead()
    {
        return _lSettingsPort.LEngineSettingsRead();
    }

    public string LWindowWorkspaceRead()
    {
        return _lSettingsPort.LEngineWorkspaceRead();
    }

    public string LWindowWorkspaceFormat()
    {
        return _lSettingsPort.LEngineWorkspaceFormat();
    }

    public LWorkspaceState LWindowStateRead()
    {
        return _lSettingsPort.LEngineStateRead();
    }

    public string LWindowLocalizationRead()
    {
        return LLocalization.LLocalizationNormalize(_lSettingsPort.LEngineSettingsRead().LSettingsLocalization);
    }

    public IReadOnlyDictionary<string, string> LWindowLocalizationLoad(string language)
    {
        return _lSettingsPort.LEngineLocalizationLoad(language);
    }

    public void LWindowLocalizationSave(string language)
    {
        _lSettingsPort.LEngineLocalizationSave(language);
    }

    public void LWindowEpithetSave(bool epithet)
    {
        _lSettingsPort.LEngineEpithetSave(epithet);
    }

    public void LWindowFrequencySave(bool frequency)
    {
        _lSettingsPort.LEngineFrequencySave(frequency);
    }

    public void LWindowMorphologySave(bool morphology)
    {
        _lSettingsPort.LEngineMorphologySave(morphology);
    }

    public void LWindowRespellingSave(bool respelled)
    {
        _lSettingsPort.LEngineRespellingSave(respelled);
    }

    public LEstablishment LWindowEstablishmentRead()
    {
        return _lSettingsPort.LEngineEstablishmentRead();
    }

    public string? LWindowAuditRecord(Exception exception)
    {
        return _lSettingsPort.LEngineAuditRecord(exception);
    }

    public IReadOnlyList<LDraft> LWindowLeftoverRead()
    {
        return _lDraftPort.LEngineLeftoverRead();
    }

    public void LWindowLeftoverSweep()
    {
        _lDraftPort.LEngineLeftoverSweep();
    }

    public void LWindowRecordingSweep()
    {
        _lMediaPort.LEngineRecordingSweep();
    }

    public LEntry LWindowGlyphResolve(string character, string language)
    {
        return _lEntryPort.LEngineGlyphResolve(character, language);
    }

    public IReadOnlyList<LMeaning> LWindowMeaningRead(long entryId)
    {
        return _lEntryPort.LEngineMeaningRead(entryId, LOwner.LOwnerEntry);
    }

    public Task LWindowPortraitExport(LVista? vista, string path, LPortraitFormat format, LPortraitLabel label)
    {
        return _lPortraitPort.LEnginePortraitExport(vista, path, format, label);
    }
}

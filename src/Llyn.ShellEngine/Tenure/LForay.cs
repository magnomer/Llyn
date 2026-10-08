using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed class LForay
{
    private readonly LEngine _lEngine;

    private readonly LTenure _lForayTenure;

    private readonly CancellationTokenSource _lForayCancellation = new();

    private readonly LQuillPronunciation _lForayPronunciation;

    private bool _lForayCancelled;

    internal LForay(
        LEngine engine,
        LTenure tenure,
        LQuillPronunciation pronunciation,
        string word,
        string language,
        long target,
        string scheme)
    {
        _lEngine = engine;
        _lForayTenure = tenure;
        _lForayPronunciation = pronunciation;
        LForayWord = word;
        LForayLanguage = language;
        LForayTarget = target;
        LForayScheme = scheme;
        LForayFlagged = language.Length > 0 && engine.LEngineLanguage.LEngineFlaggedCheck(language);
    }

    public long LForayTarget { get; }

    public string LForayWord { get; }

    public string LForayLanguage { get; }

    public string LForayScheme { get; }

    public bool LForayFlagged { get; }

    public bool LForaySchemed => LForayScheme.Length > 0;

    public bool LForayPrimary => LForayTarget == 0;

    public bool LForayCancelled
    {
        get
        {
            lock (_lForayCancellation)
            {
                return _lForayCancelled;
            }
        }
    }

    public void LForayCancel()
    {
        lock (_lForayCancellation)
        {
            if (_lForayCancelled)
            {
                return;
            }

            _lForayCancelled = true;
        }

        _lForayCancellation.Cancel();
        _lForayCancellation.Dispose();
    }

    public async Task<bool?> LForayRecordingSave(LRecording recording)
    {
        ArgumentNullException.ThrowIfNull(recording);

        if (!LForayDraftCheck())
        {
            return false;
        }

        string? path = await _lEngine
            .LEnginePronunciation.LEngineRecordingSave(recording, LForayWord, LForayLanguage, CancellationToken.None)
            .ConfigureAwait(false);
        if (path is null)
        {
            return null;
        }

        if (!LForayDraftCheck())
        {
            return false;
        }

        long draft = _lForayTenure.LTenureId;
        LRequest request = LForayTarget == 0
            ? new LRequestAudio(draft, path, recording.LRecordingSource)
            : new LRequestPronunciationAudio(draft, LForayTarget, path, recording.LRecordingSource);
        _lForayTenure.LTenureRequestApply(request);
        _lForayPronunciation.LQuillVarietySet(LForayPrimary, LForayTarget, recording.LRecordingVariety);
        return true;
    }

    public Task<string?> LForayRecordingPrepare(LRecording recording)
    {
        ArgumentNullException.ThrowIfNull(recording);

        return _lEngine.LEnginePronunciation.LEngineRecordingPrepare(recording, CancellationToken.None);
    }

    public Task LForayEnsignLoad(Func<IReadOnlyList<LEnsignRow>, Action<string, Exception>, Action> store)
    {
        ArgumentNullException.ThrowIfNull(store);

        return LForayFlagged && !LForaySchemed
            ? _lEngine.LEngineLanguage.LEngineEnsignLoad(
                LForayLanguage,
                _lEngine.LEngineLanguage.LEngineVarietyRead(LForayLanguage)
                    .Select(static variety => variety.LVarietyName),
                store)
            : Task.CompletedTask;
    }

    public (bool, string, string) LForayMarkRead()
    {
        return LForaySchemed
            ? (false, string.Empty, string.Empty)
            : _lEngine.LEngineSettings.LEngineMarkRead(LForayLanguage);
    }

    public string LForayReadingRead(string? phonetic, string? respelling)
    {
        (bool respelled, _, _) = LForayMarkRead();
        return LLanguageClerk.LLanguageCandidateRead(phonetic, respelling, respelled);
    }

    internal static string LForayWordRead(LDraft? draft)
    {
        return draft?.LDraftContent.LEntryDraftHeadword.Trim() ?? string.Empty;
    }

    internal void LForayStart(Func<CancellationToken, Task> search, Action unstarted)
    {
        _ = LForayRun(search, unstarted);
    }

    private async Task LForayRun(Func<CancellationToken, Task> search, Action unstarted)
    {
        CancellationToken token;
        lock (_lForayCancellation)
        {
            if (_lForayCancelled)
            {
                return;
            }

            token = _lForayCancellation.Token;
        }

        try
        {
            await search(token).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception exception)
        {
            _lEngine.LEngineWorkspace.LEngineAuditRecord(exception);
            if (!token.IsCancellationRequested)
            {
                try
                {
                    unstarted();
                }
                catch (Exception ending)
                {
                    _lEngine.LEngineWorkspace.LEngineAuditRecord(ending);
                }
            }
        }
    }

    private bool LForayDraftCheck()
    {
        _lForayTenure.LTenurePersist();
        LDraft? held = _lForayTenure.LTenureRead();
        return held is not null
            && string.Equals(LForayWordRead(held), LForayWord, StringComparison.Ordinal)
            && string.Equals(held.LDraftContent.LEntryDraftLanguage, LForayLanguage, StringComparison.Ordinal);
    }
}

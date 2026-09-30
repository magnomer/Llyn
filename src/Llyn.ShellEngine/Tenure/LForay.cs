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

    private bool _lForayCancelled;

    internal LForay(LEngine engine, LTenure tenure, string word, string language, long target, string scheme)
    {
        _lEngine = engine;
        _lForayTenure = tenure;
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

    public async Task<bool> LForayRecordingSave(LRecording recording)
    {
        ArgumentNullException.ThrowIfNull(recording);

        if (!LForayDraftCheck())
        {
            return false;
        }

        string path = await _lEngine
            .LEnginePronunciation.LEngineRecordingSave(recording, LForayWord, LForayLanguage, CancellationToken.None)
            .ConfigureAwait(false);

        if (!LForayDraftCheck())
        {
            return false;
        }

        long draft = _lForayTenure.LTenureId;
        LRequest request = LForayTarget == 0
            ? new LRequestAudio(draft, path, recording.LRecordingSource)
            : new LRequestPronunciationAudio(draft, LForayTarget, path, recording.LRecordingSource);
        _lForayTenure.LTenureRequestApply(request);
        _lForayTenure.LTenureVarietySet(LForayPrimary, LForayTarget, recording.LRecordingVariety);
        return true;
    }

    public Task<string> LForayRecordingPrepare(LRecording recording)
    {
        ArgumentNullException.ThrowIfNull(recording);

        return _lEngine.LEnginePronunciation.LEngineRecordingPrepare(recording, CancellationToken.None);
    }

    public Task LForayEnsignLoad(Func<IReadOnlyList<LEnsignRow>, Action<string, Exception>, Action> store)
    {
        ArgumentNullException.ThrowIfNull(store);

        return LForayFlagged
            ? _lEngine.LEngineLanguage.LEngineEnsignLoad(
                LForayLanguage,
                _lEngine.LEngineLanguage.LEngineVarietyRead(LForayLanguage)
                    .Select(static variety => variety.LVarietyName),
                store)
            : Task.CompletedTask;
    }

    internal static string LForayWordRead(LDraft? draft)
    {
        return draft?.LDraftContent.LEntryDraftHeadword.Trim() ?? string.Empty;
    }

    internal void LForayStart(Func<CancellationToken, Task> search, Action finish)
    {
        _ = LForayRun(search, finish);
    }

    private async Task LForayRun(Func<CancellationToken, Task> search, Action finish)
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
        catch (Exception)
        {
            finish();
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

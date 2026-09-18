using System;
using System.Threading;
using System.Threading.Tasks;
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
        LForayFlagged = language.Length > 0 && engine.LEngineFlaggedCheck(language);
    }

    public long LForayTarget { get; }

    public string LForayWord { get; }

    public string LForayLanguage { get; }

    public string LForayScheme { get; }

    public bool LForayFlagged { get; }

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
            .LEngineRecordingSave(recording, LForayWord, LForayLanguage, CancellationToken.None)
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
        return true;
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
        LEntryDraft? content = _lForayTenure.LTenureRead()?.LDraftContent;
        return content is not null
            && string.Equals(content.LEntryDraftHeadword.Trim(), LForayWord, StringComparison.Ordinal)
            && string.Equals(content.LEntryDraftLanguage, LForayLanguage, StringComparison.Ordinal);
    }
}

using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIVeneer;

public partial class PEditor
{
    private void PEditorRequestDefer(LRequest request)
    {
        if (_pEditorFill)
        {
            return;
        }

        if (_pEditorTenure is not LTenure held)
        {
            return;
        }

        held.LTenureRequestDefer(request);
    }

    private void PEditorRequestSend(LRequest request)
    {
        if (_pEditorFill)
        {
            return;
        }

        if (_pEditorTenure is not LTenure held)
        {
            return;
        }

        held.LTenureRequestApply(request);
    }

    private void PEditorLanguageSend()
    {
        string language = PSpeakerName.Text ?? string.Empty;
        if (language.Length == 0)
        {
            return;
        }

        PEditorRequestSend(new LRequestLanguage(PEditorDraft, language));
    }

    private void PEditorSpeechSend()
    {
        PEditorRequestSend(new LRequestSpeech(PEditorDraft, PMarkerRead()));
    }

    private void PEditorAudioSend()
    {
        PEditorRequestSend(new LRequestAudio(PEditorDraft, _pRecording ?? string.Empty, _pRecordingSource));
    }
}

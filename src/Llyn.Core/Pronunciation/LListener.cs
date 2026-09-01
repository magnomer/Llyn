namespace Llyn.Core;

public interface LListener
{
    void LListenerSourceStart(string source);

    void LListenerRecordingAdd(LRecording recording);

    void LListenerFinish();
}

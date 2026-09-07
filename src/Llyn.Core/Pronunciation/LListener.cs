namespace Llyn.Core;

public interface LListener
{
    void LListenerSourceStart(string source, int order);

    void LListenerRecordingAdd(LRecording recording);

    void LListenerFinish();
}

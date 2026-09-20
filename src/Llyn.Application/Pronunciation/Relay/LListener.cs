using Llyn.Core;

namespace Llyn.Application;

public interface LListener
{
    void LListenerSourceStart(string source, int order);

    void LListenerRecordingAdd(LRecording recording);

    void LListenerFinish();
}

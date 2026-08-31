namespace Llyn.Core;

/// <summary>
/// Subscriber contract for an audio-recording discovery. Mirrors <see cref="LReceiver"/> for the
/// download feature: each audio source reports when it starts, every recording it finds is streamed
/// as it arrives, and the discovery reports once when all sources have finished. Callbacks may
/// arrive on background threads; an implementation that touches UI is responsible for marshalling.
/// </summary>
public interface LListener
{
    /// <summary>An audio source has begun searching. <paramref name="source"/> is its name.</summary>
    void LListenerSourceStart(string source);

    /// <summary>A downloadable recording has arrived from a source.</summary>
    void LListenerRecordingAdd(LRecording recording);

    /// <summary>Every audio source has finished; no further callbacks follow.</summary>
    void LListenerFinish();
}

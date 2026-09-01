# LListener.cs

## `public interface LListener`

Subscriber contract for an audio-recording discovery. Mirrors `LReceiver` for the download feature: each audio source reports when it starts, every recording it finds is streamed as it arrives, and the discovery reports once when all sources have finished. Callbacks may arrive on background threads; an implementation that touches UI is responsible for marshalling.

## `void LListenerSourceStart(string source);`

An audio source has begun searching. `source` is its name.

## `void LListenerRecordingAdd(LRecording recording);`

A downloadable recording has arrived from a source.

## `void LListenerFinish();`

Every audio source has finished; no further callbacks follow.

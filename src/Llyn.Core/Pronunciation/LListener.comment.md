# LListener.cs

## `public interface LListener`

Subscriber contract for an audio-recording discovery.
Mirrors `LReceiver` for the download feature.
Each audio source reports when it starts.
Every recording it finds is streamed as it arrives.
The discovery reports once when all sources have finished.
Callbacks may arrive on background threads.
An implementation that touches UI is responsible for marshalling.

## `void LListenerSourceStart(string source);`

An audio source has begun searching.
`source` is its name.

## `void LListenerRecordingAdd(LRecording recording);`

A downloadable recording has arrived from a source.

## `void LListenerFinish();`

Every audio source has finished.
No further callbacks follow.

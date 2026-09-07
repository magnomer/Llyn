# LListener.cs

## `public interface LListener`

Subscriber contract for an audio-recording discovery.
Mirrors `LReceiver` for the download feature.
Each audio source reports when it starts, and each reports exactly one recording when it is done.
The discovery reports once when all sources have finished.
Callbacks may arrive on background threads.
An implementation that touches UI is responsible for marshalling.

## `void LListenerSourceStart(string source, int order);`

An audio source has begun searching.
`source` is its name and `order` is its position in the language pack's list.

## `void LListenerRecordingAdd(LRecording recording);`

A source has finished and this is what it had.
It arrives whether the source offered a recording, had none, or could not be reached.

## `void LListenerFinish();`

Every audio source has finished.
No further callbacks follow.

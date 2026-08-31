namespace Llyn.Core;

/// <summary>One downloadable audio recording found for a headword by an audio source.</summary>
/// <param name="LRecordingSource">The name of the source the recording came from, as declared by
/// that source's language pack (for example <c>"Naver"</c>). Config-driven, never an enum.</param>
/// <param name="LRecordingAddress">The absolute URL the audio bytes are retrieved from.</param>
public sealed record LRecording(string LRecordingSource, string LRecordingAddress);

# PMarkerTemplate.xaml.cs

## `public partial class PMarkerTemplate : ResourceDictionary`

This dictionary draws speech markers as editor chips while speech data stays with the editor.

## `private readonly PEditor _pSpeechHost`

The editor resolves marker activation against the speech attached to the active card.

## `internal PMarkerTemplate(PEditor host)`

The host keeps shared marker resources connected to their owning editor.

## `private void PMarkerChipHandle(object sender, RoutedEventArgs e)`

Marker activation delegates to the editor because it owns speech selection and state.

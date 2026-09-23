# PClipTemplate.xaml.cs

## `public partial class PClipTemplate : ResourceDictionary`

This dictionary supplies audio clip controls while the editor owns the clip and its timing.

## `private readonly PEditor _pClipHost`

The editor resolves clip actions against the audio item in the active card.

## `internal PClipTemplate(PEditor host)`

The host routes shared clip controls to their owning editor.

## `private void PClipPreviewHandle(object sender, RoutedEventArgs e)`

Preview delegates to the editor so playback uses the selected clip's context.

## `private void PClipSelectorHandle(object sender, RoutedEventArgs e)`

The editor opens clip selection because it owns attachment and replacement state.

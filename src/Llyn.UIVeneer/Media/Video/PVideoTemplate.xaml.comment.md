# PVideoTemplate.xaml.cs

## `public partial class PVideoTemplate : ResourceDictionary`

This dictionary presents video attachments while the video host manages their lifecycle.

## `private readonly PVideoHost _pVideoHost`

The video host owns the item state needed to open or remove an attachment.

## `internal PVideoTemplate(PVideoHost host)`

The host keeps shared video controls connected to the attachment owner.

## `private void PVideoOpenHandle(object sender, RoutedEventArgs e)`

Opening delegates to the host so it can resolve the selected video resource.

## `private void PVideoRemoveHandle(object sender, RoutedEventArgs e)`

Removal goes through the host to update attachment state with the display.

# PImageTemplate.xaml.cs

## `public partial class PImageTemplate : ResourceDictionary`

This dictionary presents image attachments while the image host manages their lifecycle.

## `private readonly PImageHost _pImageHost`

The image host owns the item state needed to open or remove an attachment.

## `internal PImageTemplate(PImageHost host)`

The host keeps shared image controls connected to the attachment owner.

## `private void PImageOpenHandle(object sender, RoutedEventArgs e)`

Opening delegates to the host so it can resolve the selected image resource.

## `private void PImageRemoveHandle(object sender, RoutedEventArgs e)`

Removal goes through the host to update attachment state with the display.

# PCategoryTemplate.xaml.cs

## `public partial class PCategoryTemplate : ResourceDictionary`

This dictionary presents category choices while the editor retains control of card categorization.

## `private readonly PEditor _pCategoryHost`

The host applies category changes to the card whose editor is active.

## `internal PCategoryTemplate(PEditor host)`

Passing the editor gives shared category resources access to the active editing context.

## `private void PCategoryHandle(object sender, RoutedEventArgs e)`

Category selection goes through the editor so its card state stays authoritative.

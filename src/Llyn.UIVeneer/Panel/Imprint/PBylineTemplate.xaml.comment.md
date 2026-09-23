# PBylineTemplate.xaml.cs

## `public partial class PBylineTemplate : ResourceDictionary`

This dictionary presents imprint bylines while the imprint view owns the selected contributor.

## `private readonly PImprint _pBylineHost`

The imprint host supplies the context needed to resolve a byline click.

## `internal PBylineTemplate(PImprint host)`

The host connects a shared byline resource to its owning imprint view.

## `private void PBylineHandle(object sender, MouseButtonEventArgs e)`

Byline activation delegates to the host so it can navigate to the contributor.

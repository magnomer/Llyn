# CLedgerPage.cs

## `public sealed record CLedgerPage(string CLedgerPageChild, string CLedgerPageTitle, string CLedgerPageMeta);`

One page of the settings ledger, ready to show.

**Parameters**

- `CLedgerPageChild`: the page's name, which a driver maps to the part it shows.
- `CLedgerPageTitle`: the page's title in the stored language.
- `CLedgerPageMeta`: the one-line summary under the title, empty for Layout.

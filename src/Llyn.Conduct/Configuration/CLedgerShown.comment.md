# CLedgerShown.cs
Hash: `8b61a9eaaf6811db`

## `public sealed record CLedgerShown(IReadOnlyList<string> CLedgerShownChildren, bool CLedgerShownEmpty);`

The settings pages a search shows, ready to paint.

**Parameters**

- `CLedgerShownChildren`: the names of the shown pages, in the ledger's page order.
- `CLedgerShownEmpty`: whether no page is shown, so the empty text appears.

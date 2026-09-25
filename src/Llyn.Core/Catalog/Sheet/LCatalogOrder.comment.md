# LCatalogOrder.cs

## `public enum LCatalogOrder`

Every ordering a browsed catalog can be listed in.
A browsing panel picks one of these rather than inventing a word of its own.
One member means one ordering wherever it is asked for, so two panels ordering by usage agree.
A panel offers only the members its own kind of record can be ordered by.
The set is one because the meaning is one, not because every catalog answers to all of it.
The stored form of a member is its text, so a remembered choice survives a restart.

- `LCatalogOrderName` — By the name the record is shown under.
- `LCatalogOrderHeadword` — By the headword of the entry, ascending.
- `LCatalogOrderReverse` — By the headword or the text, descending.
- `LCatalogOrderRecent` — By when the entry was added, newest first.
- `LCatalogOrderEarliest` — By when the entry was added, oldest first.
- `LCatalogOrderYear` — By the year the source states, unstated years first.
- `LCatalogOrderAuthor` — By the first credited author, uncredited sources first.
- `LCatalogOrderUsage` — By how many places cite the record, most cited first.
- `LCatalogOrderLanguage` — By the language of the record.
- `LCatalogOrderMarked` — By when the favorite mark was made, newest first.
- `LCatalogOrderText` — By the sentence the example carries.
- `LCatalogOrderSource` — By the name of the source the example cites.
- `LCatalogOrderKind` — By the kind the situation states.
- `LCatalogOrderSound` — By the stored pronunciation, entries carrying one first.
- `LCatalogOrderPending` — By what is still missing, entries carrying no pronunciation first.
- `LCatalogOrderWork` — By how many sources credit the author, most credited first.
- `LCatalogOrderGrasp` — By how well the user knows the entry, best known first.

# Catalog

This folder holds the relays for the browsing seam and the vista each catalog tab holds.

## TInterfaceCatalog

The relay for the browsing seam, the find call of each browsed kind and the catalog vocabulary.
Each relay delegates to one engine or Core operation, or builds one row.
`TCardChildSet` is one exception, since it builds the card itself.
`TEngineCitationCreate` is the other, which creates the reference, raises a reference bulletin and returns the stored reference.
The citation resolve relay names a Meaning card site.
It becomes the draft site when card and sentence are both zero, through `TExampleSiteCreate`.
The orderings and the matches under test are reached only here, so no test body calls them itself.
The sort relays order rows as the matching catalog does.
So a test pins the tie rule on rows it builds.
The create relays build browsed rows, so a sort test needs no engine.

## `TEngineRegisterFind`

The card overload reads the offer a Meaning card's register field shows for a typed text.
No draft is held.
Without a draft no card already holds a Register.
So every match is offered, and only the order and the limit decide the rows.

## TInterfaceVista

The relays for a vista, the engine's view state for one catalog tab.
The start and the row finds are relayed over the engine, and every change over the vista itself.
Each relay is transparent.
The engine stores no filter or mode.
So the engine start relay hands it an empty filter and the reading mode.
The posture start relay resolves both from what the posture stored, as the window does.
Both pass the blank flag through with the same default the engine gives it.
The press-open relays pass only the vista and its label or legend, since the engine owns its press.
They answer a null relay when the vista has no row chosen, and the caller disposes any other.
The label overload also answers null for a vista whose subject is not Entry.
The xiaoyun relay takes an optional vista, so a test can pin the order the panel sets.

## `TVistaSubjectRead`

Answers the subject a tab lists, so both start relays need only the tab name.
The two Fanqie tabs list no subject, and any tab not named lists entries.

## Stamp

Commit: `8c403d1df768`
- `TInterfaceCatalog.cs` `329dcac662f88b27`
- `TInterfaceVista.cs` `2975d230d7b4d5be`

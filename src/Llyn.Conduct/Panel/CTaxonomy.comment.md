# CTaxonomy.cs

## `public sealed class CTaxonomy`

The taxonomy panel's session: the Tag list, the entries carrying the chosen Tag, and the entry editor.
The tag vista and the membership vista hold the panel's state, and the drivers only follow.
Every gate holds only the interaction and reaches the engine through a vista, the panel or the editor's desk.
It restores both vistas itself, so no driver holds a port or a vista.

## `private CTaxonomy(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy)`

Builds the entry editor and the membership panel over the atelier's ports.
The panel asks the editor's desk before it leaves an entry, and it finishes through the editor.
A cleared panel cancels the editor's draft, and an edited row opens the editor on it.
So the first draft a fresh member shows already carries the Tag.

## `public static CTaxonomy CTaxonomyCreate(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy)`

Builds the taxonomy over the atelier and the panel's own entry editor.
Building it is no user action, so it is no gate on the atelier.
`shownSeam` answers whether the tab is in front, which only the surface knows until `CNavigation` owns it.

## `public CEditor CTaxonomyEditor { get; }`

The entry editor on the membership side, which the driver wraps for its editor page.

## `public CPanel CTaxonomyPanel { get; }`

The shared panel state over the membership vista: the chosen entry, the scribe mode, the bin and the leave guard.

## `public long? CTaxonomyChosen`

The Tag the tag vista has chosen, or null while none is, which the voyage records.

## `public bool CTaxonomyCoinageAllowed`

Whether New should name a new Tag rather than start an entry.
That holds while no Tag is chosen and no entry is shown.
Both drivers ask this one question, so neither decides it on its own.

## `public string CTaxonomyEmptyKey`

The localization key for the empty entry list.
The engine says whether the membership search holds text, so a search reads as unmatched and no search as vacant.

## `public void CTaxonomyVistaRestore()`

Starts the tag vista and the membership vista for this tab.
The membership vista then goes to the panel and the editor.

## `public void CTaxonomyObserverAttach(CSubject subject, Action<CBulletin> observer)`

Attaches a driver's observer to the tag vista for one subject.
Each engine notice reaches the observer through the atelier's one bulletin map.

## `public void CTaxonomyOrderSet(CCatalogOrder? order)`

Orders the Tag list as the user chose.
A null order keeps the current one, which the vista decides.

## `public void CTaxonomyMembershipFind(string query)`

Narrows the entries of the chosen Tag by the text typed in the membership search.

## `public IReadOnlyList<CCatalogTag> CTaxonomyRowsRead()`

The Tag rows the engine lists, chosen mark included, mapped through the card's one Tag map.
It answers nothing before the vistas are restored.

## `public IReadOnlyList<CVistaRow> CTaxonomyMembershipRead()`

The entries under the chosen Tag, or every entry while none is chosen, as the engine narrows them.

## `public long CTaxonomyTagCreate(string name)`

Makes the Tag and answers only its id, which is all the driver browses by.

## `public void CTaxonomyEntryCreate()`

Opens a fresh entry in the editor, in edit mode.
The chosen Tag goes down with the start, so the engine puts it on the first card in one call.
With no Tag chosen the fresh entry starts blank.

## `public Task CTaxonomyPortraitPrint(CPortraitLabel label, CPressTicket ticket)`

Prints the shown entry, and does nothing while none is shown outside edit mode.
`CTaxonomyPortraitExport` exports it under the same condition.

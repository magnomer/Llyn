# CTenor.cs

## `public sealed class CTenor`

The tenor panel's session: the Register list, the entries carrying the chosen Register, and the entry editor.
The register vista and the cohort vista hold the panel's state, and the drivers only follow.
Every gate holds only the interaction and reaches the engine through a vista, the panel or the editor's desk.
It restores both vistas itself, so no driver holds a port or a vista.

## `private CTenor(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy)`

Builds the entry editor and the cohort panel over the atelier's ports.
The panel asks the editor's desk before it leaves an entry, and it finishes through the editor.
A cleared panel cancels the editor's draft, and an edited row opens the editor on it.
So the first draft a fresh member shows already carries the Register.

## `public static CTenor CTenorCreate(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy)`

Builds the tenor panel over the atelier and the panel's own entry editor.
Building it is no user action, so it is no gate on the atelier.
`shownSeam` answers whether the tab is in front, which only the surface knows until `CNavigation` owns it.

## `public CEditor CTenorEditor { get; }`

The entry editor on the cohort side, which the driver wraps for its editor page.

## `public CPanel CTenorPanel { get; }`

The shared panel state over the cohort vista: the chosen entry, the scribe mode, the bin and the leave guard.

## `public event Action? CTenorRegisterOpened;`

Raised when an arrival or a coinage opens a Register, so the driver shows both searches empty and lists again.

## `public void CTenorRegisterToggle(long id)`

A click on a row: records the station it leaves, then toggles the Register.

## `internal void LTenorRegisterOpen(long id)`

The navigation's arrival: chooses the Register and empties both searches, then raises `CTenorRegisterOpened`.
Emptying the searches is the arrival's own, so no driver has to echo it back through the query gates.

## `internal long? LTenorChosen`

The Register the register vista has chosen, or null while none is, which the voyage records.

## `public bool CTenorCoinageAllowed`

Whether New should name a new Register rather than start an entry.
That holds while no Register is chosen and no entry is shown.
Both drivers ask this one question, so neither decides it on its own.

## `public string CTenorEmptyKey`

The localization key for the empty entry list.
The engine says whether the cohort search holds text, so a search reads as unmatched and no search as vacant.

## `public void CTenorVistaRestore()`

Starts the register vista and the cohort vista for this tab.
The cohort vista then goes to the panel and the editor.

## `public void CTenorObserverAttach(CSubject subject, Action<CBulletin> observer)`

Attaches a driver's observer to the register vista for one subject.
Each engine notice reaches the observer through the atelier's one bulletin map.

## `public void CTenorOrderSet(CCatalogOrder? order)`

Orders the Register list as the user chose.
A null order keeps the current one, which the vista decides.

## `public void CTenorCohortFind(string query)`

Narrows the entries of the chosen Register by the text typed in the cohort search.

## `public IReadOnlyList<CCatalogRegister> CTenorRowsRead()`

The Register rows the engine lists, usage count and chosen mark included.
Each Register maps through the card's one Register map.
It answers nothing before the vistas are restored.

## `public IReadOnlyList<CVistaRow> CTenorCohortRead()`

The entries under the chosen Register, or every entry while none is chosen, as the engine narrows them.

## `public void CTenorRegisterCreate(string name)`

Makes the Register from the raw wording and opens it as an arrival does.
The panel is cleared first, so an unsaved fresh entry the leave check already settled does not linger.
A refused Register is shown through `CEnvoy` as `Register.CreateFailed`, and nothing opens.

## `public void CTenorEntryCreate()`

Opens a fresh entry in the editor, in edit mode.
The chosen Register goes down with the start, so the engine puts it on the first card in one call.
With no Register chosen the fresh entry starts blank.

## `public Task CTenorPortraitPrint()`

Prints the shown entry, and does nothing while none is shown outside edit mode.
`CTenorPortraitExport` exports it under the same condition.
The reader is asked for the printer through the panel's envoy, and a decline prints nothing.
`CPortrait` words the page through the engine and shows `Print.Failed` through the panel's envoy.
The reader is asked for the file and format through the panel's envoy, and a decline exports nothing.
`CPortrait` words the page through the engine and shows `Export.Failed` through the panel's envoy.

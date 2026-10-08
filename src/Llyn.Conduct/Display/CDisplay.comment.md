# CDisplay.cs
Hash: `38514aaefa1cd87c`

## `public sealed class CDisplay`

The reading view's header area.
It owns the entry's life on the lectern, the favourite gate, and the change events its drivers answer.
The editor and each wing build one, and it builds its rules and its other areas once.
So every driver over it hears the same events.
The lectern names the entry's lexical unit by the localization key `LEngineUnitFormat` gives.
Drivers hold this area and the areas it hands out, never its rules, so no `L` type crosses into Deportment.
The sound half stands on [CDisplaySound](CDisplaySound.comment.md), which reads the header here.
The pronunciation block stands on [CDisplayAccent](CDisplayAccent.comment.md).
The play button stands on [CDisplayPlayback](CDisplayPlayback.comment.md).
The grasp stars stand on [CDisplayGrasp](CDisplayGrasp.comment.md).
The card reads stand on [CDisplayCard](CDisplayCard.comment.md).
The clicks that open another place stand on [CDisplayRoute](CDisplayRoute.comment.md).
The floating contents stand on [CCompass](CCompass.comment.md).
The rules it shares with the editor's esteem stay on [LDisplay](LDisplay.comment.md), which it builds and calls.
It holds no state beyond the shown header.

## `private static readonly CLectern _cDisplayBlank`

The header while nothing is shown, every field empty.

## `private readonly LEntryPort _cDisplayEntryPort;`

The entry port the stamp and the unit wording go through.

## `private readonly LMarkdownPort _cDisplayMarkdownPort;`

The markdown port the note parse goes through.

## `private readonly CEnvoy _cDisplayEnvoy;`

The envoy a refused stamp read is shown through.

## `private readonly LSettingsPort _cDisplaySettings;`

The settings port the failure notice is read through.

## `internal CDisplay(LDraftPort drafts, CEntryBundle entries, CPhonologyBundle phonology, LSettingsPort settings, LMediaPort media, CEnvoy envoy, CLedgerNoticed noticed)`

Only the editor and the wing build an area, over the ports and the envoy the atelier handed down.
It builds its rules first, then the sound area over those rules and itself.
Then it builds the accent, playback, grasp, card and route areas and the compass over the same rules.
It reads the entry and phonology bundles and hands each part only the ports that part calls.
It takes the atelier's repaint memory, which its rules hold for every repaint read.
A refused read is shown through `envoy`, so every driver over this display shows it once.
`settings` reads the failure notice the envoy shows.

## `public event Action? CDisplayOpened;`

Raised on the calling thread once an entry opens and `CDisplayShown` holds its header.

## `public event Action? CDisplayClosed;`

Raised once the shown entry closed and playback stopped, so every section empties.

## `public event Action<CBulletin>? CDisplayFavoriteChanged;`

Raised on the engine's thread when the chosen entry's favourite changed.
The favourite, frequency, paradigm and reflex events count only for the chosen entry.
A driver marshals each onto its own thread before it reads.

## `public event Action<CBulletin>? CDisplayFrequencyChanged;`

Raised when the chosen entry's frequency changed.

## `public event Action<CBulletin>? CDisplayParadigmChanged;`

Raised when the chosen entry's inflection changed.

## `public event Action<CBulletin>? CDisplayReflexChanged;`

Raised when the chosen entry's reflexes changed.

## `public event Action<CBulletin>? CDisplayScriptChanged;`

Raised for any entry's script bulletin.

## `public event Action<CBulletin>? CDisplayFanqieChanged;`

Raised for any entry's fanqie bulletin.

## `public event Action<CBulletin>? CDisplayEntryChanged;`

Raised when the shown draft may have changed, so a driver hands it to `CDisplayEntryResonate`.
It counts the chosen entry's own bulletins, and any example, situation, reference, author, tag, register or settings bulletin.
Each of those can change what the draft shows.

## `public event Action<CBulletin>? CDisplayWorkspaceChanged;`

Raised when the workspace was swapped, so a driver hands it to `CDisplayWorkspaceResonate`.

## `public CLectern CDisplayShown { get; private set; }`

The header of the shown entry, or the blank header while nothing is shown.

## `public CDisplaySound CDisplaySound { get; }`

The lectern's sound area, built once here over the rules' sound half and this header area.
A driver reaches it from this area, so it holds one C object for the whole lectern.

## `public CDisplayAccent CDisplayAccent { get; }`

The lectern's pronunciation area, built once here over the rules and the language port.
The reading view and the editor's contour share its pitch scale.

## `public CDisplayPlayback CDisplayPlayback { get; }`

The lectern's playback area, built once here over the rules and the media port.
Every panel that closes its editor stops this view's play through it.

## `public CDisplayGrasp CDisplayGrasp { get; }`

The lectern's grasp area, built once here over the rules.
The vista restore subscribes its change event, so the stars hear only the chosen entry.

## `public CDisplayCard CDisplayCard { get; }`

The lectern's card area, built once here over the rules.
It reads the cards, the incoming usages and the etymology a driver paints.

## `public CDisplayRoute CDisplayRoute { get; }`

The lectern's route area, built once here over the rules.
It opens what a clicked chip, link or word names.
`LDisplayNavigationAttach` hands it the atelier's navigation and mention area.

## `public CCompass CDisplayCompass { get; }`

The lectern's compass, built once here over the rules.
It reads the floating contents and finds the place of a card.

## `internal LDisplay LDisplayRule { get; }`

The display rules this area builds and calls.
The editor's esteem, sound sheet and sound facts take them from here, so all share one shown draft.
It stays internal, so no driver names an `L` type.

## `private long? LDisplayChosen`

The id of the entry the rules say is chosen, or null.

## `internal void LDisplayNavigationAttach(CNavigation navigation, CMention mention)`

Hands every record, rime cell and series the route area and the sound area raise to the atelier's navigation.
Hands the route area the atelier's mention area too, which opens what a clicked word found.
The composition that owns the atelier calls it once, so an area built alone opens nothing.

## `internal void LDisplayVistaRestore(LVista vista)`

Hands the new vista to the rules, then subscribes the plan of which subjects refresh which part on it.
Each vista is new, so the plan is subscribed once per vista.
It stays internal, since the vista is an engine type no driver may name.
Script, fanqie and workspace bulletins count for any entry, since their sections redraw whole.

## `public void CDisplayPanelAttach(CPanel panel)`

Follows a panel.
Each draft it loads opens here, and its clearing closes the view.
The view's deportment names the panel, since a view may follow a panel or none.

## `private void LDisplayDraftOpen(LDraft draft)`

Opens the content of a draft a followed panel loaded.

## `internal void LDisplayEntryOpen(LEntryDraft? draft)`

Opens a draft in the view, or closes the view for a missing draft.
A vista choosing nothing closes the view too, since the draft then stands on no entry.
The note arrives parsed, so the driver draws its blocks and never parses.
The stamp is read from the stored entry, and a refused read hides it.

## `private (bool, string, string) LDisplayStampRead(long id)`

The stamp of the stored entry.
A refused read shows `Display.StampFailed` once until the user acts, and answers an empty stamp, so the entry still opens.

## `public void CDisplayEntryClose()`

Closes the shown entry, which drops the shown draft and stops this view's play.
A driver calls it when it starts a view afresh, and a followed panel's clearing calls it too.

## `public void CDisplayEntryResonate()`

Reloads the chosen entry's draft on the driver's thread and opens it again.
Nothing chosen reloads nothing, and a refused load leaves the view as it stands.
An entry gone from the store closes the view.

## `public void CDisplayWorkspaceResonate()`

Closes the view on the driver's thread once the workspace was swapped.

## `public bool CDisplayFavoriteRead()`

Whether the chosen entry is a favorite.

## `public bool CDisplayFavoriteToggle(bool marked)`

The gate for the heart: stores `marked` on the chosen entry, then answers the stored value.
A refused mark thus answers the old value, and the display reports the failure.

## `public CFrequency? CDisplayFrequencyRead(Func<string, string> lookup)`

The chosen entry's frequency chip, or null when none stands.
Conduct chooses the key of the one-off wording, and the driver's lookup turns it into text.

